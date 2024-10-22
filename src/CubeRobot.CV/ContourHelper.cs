using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace CubeRobot.CV;
internal static class ContourHelper
{

    public static List<Rect> FilterContours(Point[][] contours) => FilterContoursByShape(contours);
    public static List<Rect> FilterAndDrawContours(Point[][] contours, Mat output) => FilterContoursByShape(contours, output);

    public static List<List<Rect> SortContoursByPosition(List<Rect> contours)
    {
        if (contours.Count == 0)
            return [];

        List<List<Rect>> sortedBoundingRects = [];

        // Sort by Y
        contours = [.. contours.OrderBy(rect => rect.Y)];

        // Group rows by X
        int yEps = contours[0].Height / 2;
        int relativeY = contours[0].Y;
        SortedList<int, Rect> row = [];
        foreach (var boundingRect in contours)
        {
            if (Math.Abs(boundingRect.Y - relativeY) < yEps)
                row.Add(boundingRect.X, boundingRect);
            else
            {
                sortedBoundingRects.Add([.. row.Values]);
                relativeY = boundingRect.Y;
                row = new() { { boundingRect.X, boundingRect} };
            }
        }
        sortedBoundingRects.Add([.. row.Values]);

        return sortedBoundingRects;
    }

    private static List<Rect> FilterContoursByShape(Point[][] contours, Mat? drawingOutput = null)
    {
        List<Rect> blocksBoundingRects = [];
        foreach (Point[] contour in contours)
        {
            Point[] approx = Cv2.ApproxPolyDP(contour, 0.1 * Cv2.ArcLength(contour, true), true);

            if (approx.Length == 4)
            {
                Rect boundingRect = Cv2.BoundingRect(approx);
                float ratio = (float)boundingRect.Width / boundingRect.Height;
                double area = Cv2.ContourArea(approx);

                Debug.WriteLine($"R: {ratio} A: {area}");

                if (ratio >= 0.8 && ratio <= 1.21 && boundingRect.Width >= 20 && boundingRect.Width <= 60 && area >= 400)
                {
                    blocksBoundingRects.Add(boundingRect);
                    if(drawingOutput is not null)
                        Cv2.DrawContours(drawingOutput, [approx], -1, new(0, 255, 255));
                }
            }
        }

        return blocksBoundingRects;
    }
}
