﻿using System.Text.Json;
using System.Text.RegularExpressions;
using CommandLine;
using IronOcr;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Font = SixLabors.Fonts.Font;
using FontCollection = SixLabors.Fonts.FontCollection;
using FontFamily = SixLabors.Fonts.FontFamily;
using FontStyle = SixLabors.Fonts.FontStyle;
using Image = SixLabors.ImageSharp.Image;
using PointF = SixLabors.ImageSharp.PointF;
using Rectangle = SixLabors.ImageSharp.Rectangle;

// ReSharper disable AccessToModifiedClosure

namespace TimetablePlus;

public static class Program
{
    public const string CONFIG_FILE_NAME = "config.json";
    public static readonly Config Configuration;

    static Program()
    {
        TryFetchConfig(CONFIG_FILE_NAME, out Configuration);
    }

    public static async Task Main(string[] args)
    {
        var options = Parser.Default.ParseArguments<Options>(args).Value;

        if (args.Contains("--help"))
            return;

        FontCollection collection = new();
        collection.AddSystemFonts();
        FontFamily family = collection.Get("JetBrains Mono");
        Font font = family.CreateFont(50, FontStyle.Bold);
        Font fontSmall = family.CreateFont(30, FontStyle.Bold);

        Image<Argb32> theBigPicture = await GetTimetable(options.UseLocal, options.SaveTimetable);

        Image<Argb32>[][] rows = new Image<Argb32>[4][];
        const int width = 240;
        const int height = 200;

        for (int y = 0; y < rows.Length; y++)
        {
            var subjects = new Image<Argb32>[8];
            for (int x = 0; x < subjects.Length; x++)
            {
                var img = theBigPicture.Clone();
                int imgX = 250 + x * width;
                int imgY = 317 + y * height + 126 * y;
                img.Mutate(p => p.Crop(new Rectangle(imgX, imgY, width, height)));
                subjects[x] = img;
            }

            rows[y] = subjects;
        }

        var ocr = new IronTesseract();

        string[][] subjectNames = new string[4][];
        for (int i = 0; i < subjectNames.Length; i++)
        {
            subjectNames[i] = new string[8];
        }

        SchoolSubject[][] subjectEnums = new SchoolSubject[4][];
        for (int i = 0; i < subjectEnums.Length; i++)
        {
            subjectEnums[i] = new SchoolSubject[8];
            for (int j = 0; j < subjectEnums[i].Length; j++)
            {
                subjectEnums[i][j] = SchoolSubject.Unknown;
            }
        }

        Duration[] classDurations = new Duration[subjectEnums[0].Length + Configuration.BreakMapping.Count];
        {
            int tSum = 0;
            int mappingOffset = 0;
            for (int i = 0; i < classDurations.Length; i++)
            {
                if (!Configuration.TimeModMapping.TryGetValue(i - mappingOffset, out int pl))
                    pl = Configuration.DefaultPeriodLength;
                var sTime = Configuration.StartTime + TimeSpan.FromMinutes(tSum);
                var eTime = Configuration.StartTime + TimeSpan.FromMinutes(tSum + pl);
                foreach ((int index, int value) in Configuration.BreakMapping)
                {
                    if (i <= index)
                        continue;

                    sTime += TimeSpan.FromMinutes(value);
                    eTime += TimeSpan.FromMinutes(value);
                }

                classDurations[i] = new Duration(sTime, eTime);
                tSum += pl;

                if (!Configuration.BreakMapping.TryGetValue(i - mappingOffset, out int breakLength))
                    continue;

                mappingOffset++;
                classDurations[++i] = new Duration(eTime, eTime + TimeSpan.FromMinutes(breakLength));
            }
        }

        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].Length; x++)
            {
                await using var ms = new MemoryStream();

                await rows[y][x].SaveAsync(ms, JpegFormat.Instance);
                byte[] segmentBytes = ms.ToArray();

                using var input = new OcrInput(segmentBytes);

                var result = await ocr.ReadAsync(input);

                string text = Regex.Replace(result.Text, @"\n.*", "");
                subjectNames[y][x] = text;
                foreach ((string matcher, SchoolSubject value) in Configuration.RegexSubjectMappings)
                {
                    if (text.StartsWith(matcher, true, null))
                        subjectEnums[y][x] = value;
                }
            }
        }

        var clone = theBigPicture.Clone();
        clone.Mutate(x =>
        {
            const int i = 1616;
            x.Crop(new Rectangle(0, i, clone.Width / 2, clone.Height - i));
        });

        await using var ms2 = new MemoryStream();

        await clone.SaveAsync(ms2, JpegFormat.Instance);
        byte[] bytes = ms2.ToArray();

        string resultText = (await ocr.ReadAsync(new OcrInput(bytes))).Text;
        string version = Regex.Replace(resultText, @".+?(\d+)\/(\d+)\/(\d+)",
            x => $"v{x.Groups[3].Value[2..]}.{x.Groups[2]}.{x.Groups[1]}");

        const int genW = 250;
        const int genH = 250;

        int xL = subjectEnums[0].Length;
        int yL = subjectEnums.Length;

        int genWidth =
            genW * xL + Configuration.BreakMapping.Count * genW + genW;
        int genHeight = genH * yL + genH;

        var bgCol = options.WhiteMode ? Color.White : Color.Black;
        var fgCol = options.WhiteMode ? Color.Black : Color.White;

        var genImage = new Image<Argb32>(genWidth, genHeight, bgCol);
        const int borderLength = 12;

        genImage.DrawSquares(1 + xL,
            yL,
            (x, y) =>
            {
                if (x == 0)
                    return fgCol;

                var col = subjectEnums[y][x - 1].GetColor();
                if (options.WhiteMode)
                {
                    // invert color
                    col = new Argb32((byte) (255 - col.R), (byte) (255 - col.G), (byte) (255 - col.B), col.A);
                }

                return col;
            },
            (_, _) => genW,
            (_, _) => genH,
            (x, _) => new Point((
                from KeyValuePair<int, int> i in Configuration.BreakMapping
                where x - 1 > i.Key
                select genW
            ).Sum(), genH),
            null,
            (x, y) =>
            {
                if (x == 0)
                    return (Configuration.WeekdayNames[y].Substring(0, 3), font, fgCol);

                var name = subjectEnums[y][x - 1].GetNickname();

                return (name, font, fgCol);
            });

        genImage.DrawSquares(
            classDurations.Length,
            1,
            (_, _) => fgCol,
            (_, _) => genW,
            (_, _) => genH,
            (_, _) => new Point(genW, 0),
            (_, _) => borderLength,
            (x, _) =>
            {
                var dur = classDurations[x];
                return (
                    $" {dur.Length.TotalMinutes}m\n{dur.StartTime.ToString()[..^3]}\n{dur.EndTime.ToString()[..^3]}",
                    font,
                    fgCol);
            });

        genImage.Mutate(i =>
        {
            i.DrawText(new TextOptions(fontSmall)
            {
                Origin = new PointF(genW * .5f, genH * .5f),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }, version, fgCol);
        });

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/BetterTimetable.png";
        await genImage.SaveAsPngAsync(path);
    }

    public static bool TryFetchConfig(string path, out Config configuration)
    {
        configuration = Config.Default;
        if (!File.Exists(path))
            return false;

        try
        {
            string text = File.ReadAllText(path);
            Config? config = JsonSerializer.Deserialize<Config>(text);

            if (config is null)
                return false;

            configuration = config;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async ValueTask<Image<Argb32>> GetTimetable(bool useLocal, bool save = true)
    {
        string imgPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/12B.jpg";

        if (useLocal)
            return Image.Load<Argb32>(imgPath);

        const string url = @"https://tapschool.ae/timetable/";
        using var client = new HttpClient();

        string response = await client.GetStringAsync(url);

        Regex regex = new(@"<h(\d).*>.*12B.*<\/h\1>\s*<p><a href=""(.+?)"".*>Download Timetable<\/a><\/p> ");
        string link = regex.Match(response).Groups[2].Value;

        byte[] bytes = await client.GetByteArrayAsync(link);
        var img = Image.Load<Argb32>(bytes);

        if (save)
            await img.SaveAsJpegAsync(imgPath);

        return img;
    }
}