using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;
using Font = SixLabors.Fonts.Font;

namespace TimetablePlus;

public static class Utilities
{
    public static void DrawSquares<TPixel>(
        this Image<TPixel> img,
        int xAmount,
        int yAmount,
        TPixel color,
        int width,
        int height,
        Point offset = default,
        int borderLength = 12)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        DrawSquares(
            img,
            xAmount,
            yAmount,
            (_, _) => color,
            (_, _) => width,
            (_, _) => height,
            (_, _) => offset,
            (_, _) => borderLength);
    }

    public static void DrawSquares<TPixel>(
        this Image<TPixel> img,
        int xAmount,
        int yAmount,
        Func<int, int, TPixel> color,
        Func<int, int, int> width,
        Func<int, int, int> height,
        Func<int, int, Point>? offset = null,
        Func<int, int, int>? borderLength = null,
        Func<int, int, (string InnerText, Font TextFont, Color TextColor)>? innerText = null)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        DrawSquares(
            img,
            xAmount,
            yAmount,
            (x, y, _, _) => color(x, y),
            width,
            height,
            offset,
            (x, y, _, _) => borderLength?.Invoke(x, y) ?? 12,
            innerText);
    }

    public static void DrawSquares<TPixel>(
        this Image<TPixel> img,
        int xAmount,
        int yAmount,
        Func<int, int, int, int, TPixel> color,
        Func<int, int, int> width,
        Func<int, int, int> height,
        Func<int, int, Point>? offset = null,
        Func<int, int, int, int, int>? borderLength = null,
        Func<int, int, (string InnerText, Font TextFont, Color TextColor)>? innerText = null)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        offset ??= (_, _) => Point.Empty;
        borderLength ??= (_, _, _, _) => 12;

        for (int y = 0; y < yAmount; y++)
        {
            for (int x = 0; x < xAmount; x++)
            {
                var w = width(x, y);
                var h = height(x, y);
                var o = offset(x, y);

                for (int y2 = 0; y2 < h; y2++)
                {
                    for (int x2 = 0; x2 < w; x2++)
                    {
                        var bl = borderLength(x, y, x2, y2);
                        var c = color(x, y, x2, y2);

                        int xPos = (x * w) + x2 + o.X;
                        int yPos = (y * h) + y2 + o.Y;

                        if (x2 < bl
                            || w - x2 < bl
                            || y2 < bl
                            || h - y2 < bl)
                            img[xPos, yPos] = c;
                    }
                }

                if (innerText is null)
                    continue;

                (string text, Font font, Color col) = innerText(x, y);

                Vector2 origin = new(
                    (x * w) + (w * 0.5f) + o.X,
                    (y * h) + (h * 0.5f) + o.Y);

                img.Mutate(i => i.DrawText(new TextOptions(font)
                    {
                        Origin = origin,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }, text, col)
                );
            }
        }
    }
}