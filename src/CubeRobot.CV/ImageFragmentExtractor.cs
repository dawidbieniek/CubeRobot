using System.Diagnostics;

using OpenCvSharp;

namespace CubeRobot.CV;

internal class ImageFragmentExtractor(Mat image)
{
    private readonly Mat _originalImage = image;

    public Vec3i[,] ExtractDominantColorsFromFragments(List<List<Rect>> fragmentContours)
    {
        Vec3i[,] colors = new Vec3i[fragmentContours.Count, fragmentContours[0].Count];

        for (int i = 0; i < fragmentContours.Count; i++)
            for (int j = 0; j < fragmentContours[i].Count; j++)
            {
                Vec3i color = DominantColor(Mat.FromImageData(_originalImage[fragmentContours[i][j]].ToBytes())).ToVec3i();
                Debug.WriteLine($"Y: {i} X: {j} C:{color}");
            }

        return colors;
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

        // Criteria: – Stop the algorithm iteration if specified accuracy, epsilon, is reached. –
        // Stop the algorithm after the specified number of iterations, MaxIter.
        var criteria = new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 10, 1.0);

        // Finds centers of clusters and groups input samples around the clusters.
        Cv2.Kmeans(data: points, k: k, bestLabels: labels, criteria: criteria, attempts: 3, flags: KMeansFlags.PpCenters, centers: centers);

        return centers.At<Vec3f>(0, 0);
    }
}