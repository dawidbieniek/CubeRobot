using System.Diagnostics;

using CubeRobot.Models.RubiksCube;

using OpenCvSharp;

namespace CubeRobot.CV;

public static class PhotoAnalyzer
{
    private static CubeFaceColorParser _parser = new(
        new Dictionary<LABColor, CubeFaceColor>
        {
            {new(100f, 0f, 0f), CubeFaceColor.White },
            {new(73.81f, 26.54f, 78.22f), CubeFaceColor.Orange },
            {new(70.39f, -71.77f, 69.27f), CubeFaceColor.Green},
            {new(97.14f, -21.55f, 94.48f), CubeFaceColor.Yellow },
            {new(53.24f, 80.09f, 67.2f), CubeFaceColor.Red },
            {new(32.3f, 79.19f, -107.86f), CubeFaceColor.Blue },

        }
     );

    public static byte[] TestPhoto(byte[] imageData, bool gray, bool gaussian, int mTh, int MTh, int ma, int Ma)
    {
        using Mat image = Mat.FromImageData(imageData);

        if(gray)
        {
            using Mat tmp = new();
            Cv2.CvtColor(image, tmp, ColorConversionCodes.BGR2GRAY);
            tmp.CopyTo(image);
        }

        if(gaussian)
        {

            using Mat tmp = new();
            Cv2.GaussianBlur(image, tmp, new(3, 3), 0);
            tmp.CopyTo(image);
        }

        // Parameters tuned to detect only circles
        var circleParams = new SimpleBlobDetector.Params
        {
            MinThreshold = mTh,
            MaxThreshold = MTh,

            FilterByArea = true,
            MinArea = ma,
            MaxArea = Ma,
            FilterByCircularity = true,
            MinCircularity = 0.5f,
            MaxCircularity = 0.8f,
            FilterByColor = false,
            FilterByConvexity = false,
            FilterByInertia = false,
        };

        using Mat circles = new();
        using var circleDetector = SimpleBlobDetector.Create(circleParams);
        var circleKeyPoints = circleDetector.Detect(image);
        Cv2.DrawKeypoints(image, circleKeyPoints, circles, Scalar.HotPink, DrawMatchesFlags.DrawRichKeypoints);

        //Cv2.Sobel(image, circles, MatType.CV_64F, 1, 1, 5);

        return circles.ToBytes();
    }

    public static byte[] Threshold(byte[] imageData, int threshold)
    {
        using Mat image = Mat.FromImageData(imageData);
        using Mat dst = new();
        Cv2.Threshold(image, dst, threshold, 1, ThresholdTypes.Tozero);

        return dst.ToBytes();
    }

    public static byte[] DetectRedColor(byte[] imageData, int min, int max)
    {
        using Mat image = Mat.FromImageData(imageData);
        using Mat swap = new();
        Cv2.CvtColor(image, swap, ColorConversionCodes.BGR2HSV);
        Cv2.InRange(swap, new(min, 0, 0), new(max, 255, 255), image);

        return image.ToBytes();
    }

    public static byte[] ProcessCube(byte[] imageData, out byte[,][] imageFragments, out CubeFaceColor[,] colors)
    {
        using Mat image = Mat.FromImageData(imageData);
        using Mat originalImage = Mat.FromImageData(imageData);

        // Process image
        using (Mat buffer = new())
        {
            Cv2.CvtColor(image, buffer, ColorConversionCodes.BGR2GRAY);
            buffer.CopyTo(image);
        }
        using (Mat buffer = new())
        {
            //Cv2.FastNlMeansDenoising(image, buffer);
            Cv2.FastNlMeansDenoising(image, buffer, 7, 7, 20);
            buffer.CopyTo(image);
        }
        using (Mat buffer = new())
        {
            //Cv2.Blur(image, buffer, new(blur, blur));
            Cv2.Blur(image, buffer, new(3, 3));
            buffer.CopyTo(image);
        }
        using (Mat buffer = new())
        {
            //Cv2.Canny(image, buffer, threshold1, threshold2);
            Cv2.Canny(image, buffer, 30, 60, 3);
            buffer.CopyTo(image);
        }
        using (Mat buffer = new())
        {
            //Cv2.Dilate(image, buffer, new Mat());
            Cv2.Dilate(image, buffer, Cv2.GetStructuringElement(MorphShapes.Rect, new(7, 7)));
            buffer.CopyTo(image);

            
        }

        // Convert image back to BRG format so that contours can be draw on it
        Cv2.FindContours(image, out Point[][] contours, out _, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
        using (Mat buffer = new())
        {
            Cv2.CvtColor(image, buffer, ColorConversionCodes.GRAY2BGR);
            buffer.CopyTo(image);
        }
        // Draw contours
        Cv2.DrawContours(image, contours, -1, new(255, 0, 255));

        // Filter contours for shape simillar to cubie
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
                    Cv2.DrawContours(image, [approx], -1, new(0, 255, 255));
                }
            }
        }

        // Sort cubies based on their position
        List<List<Rect>> sortedBoundingRects = [];
        int yEps = blocksBoundingRects[0].Height / 2;

        blocksBoundingRects = [.. blocksBoundingRects.OrderBy(rect => rect.Y)];

        int relativeY = blocksBoundingRects[0].Y;
        List<Rect> row = [];
        foreach (var boundingRect in blocksBoundingRects)
        {
            if (Math.Abs(boundingRect.Y - relativeY) < yEps)
                row.Add(boundingRect);
            else
            {
                sortedBoundingRects.Add(row);
                relativeY = boundingRect.Y;
                row = [boundingRect];
            }
        }
        sortedBoundingRects.Add(row);

        for (int i = 0; i < sortedBoundingRects.Count; i++)
            sortedBoundingRects[i] = [.. sortedBoundingRects[i].OrderBy(rect => rect.X)];

        // Get fragmends of image based on bounding rects
        imageFragments = new byte[sortedBoundingRects.Count,sortedBoundingRects[0].Count][];
        for (int i = 0; i < sortedBoundingRects.Count; i++)
            for (int j = 0; j < sortedBoundingRects[i].Count; j++)
                imageFragments[i, j] = originalImage[sortedBoundingRects[i][j]].ToBytes();

        // Get dominant color from image fragment
        colors = new CubeFaceColor[sortedBoundingRects.Count,sortedBoundingRects[0].Count];
        for (int i = 0; i < sortedBoundingRects.Count; i++)
            for (int j = 0; j < sortedBoundingRects[i].Count; j++)
            {
                Vec3i color = DominantColor(Mat.FromImageData(originalImage[sortedBoundingRects[i][j]].ToBytes())).ToVec3i();
                colors[i, j] = _parser.ParseColor(color.Item0, color.Item1, color.Item2);

                Debug.WriteLine($"Y: {i} X: {j} C:{color}");
            }

        return image.ToBytes();
    }

    private static Vec3f DominantColor(Mat input)
    {
        int k = 1;

        using Mat points = new();
        using Mat labels = new();
        using Mat centers = new();
        int width = input.Cols;
        int height = input.Rows;

        points.Create(width * height, 1, MatType.CV_32FC3);
        centers.Create(k, 1, points.Type());

        // Input Image Data
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                Vec3f vec3f = new()
                {
                    Item0 = input.At<Vec3b>(y, x).Item0,
                    Item1 = input.At<Vec3b>(y, x).Item1,
                    Item2 = input.At<Vec3b>(y, x).Item2
                };

                points.Set(i, vec3f);
            }
        }

        // Criteria:
        // – Stop the algorithm iteration if specified accuracy, epsilon, is reached.
        // – Stop the algorithm after the specified number of iterations, MaxIter.
        var criteria = new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 10, 1.0);

        // Finds centers of clusters and groups input samples around the clusters.
        Cv2.Kmeans(data: points, k: k, bestLabels: labels, criteria: criteria, attempts: 3, flags: KMeansFlags.PpCenters, centers: centers);

        return centers.At<Vec3f>(0, 0);

    }
}
