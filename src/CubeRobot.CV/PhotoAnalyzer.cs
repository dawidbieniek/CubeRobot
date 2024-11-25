using System.Diagnostics;

using CubeRobot.CV.Color;
using CubeRobot.Models.RubiksCube;

using OpenCvSharp;

namespace CubeRobot.CV;

public class PhotoAnalyzer(PreprocessingSettings? settings = null)
{
    private readonly CubeFaceColorParser _colorParser = new(
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

    public PreprocessingSettings Settings { get; private init; } = settings ?? PreprocessingSettings.Default;

    public CubeFaceColor[,] ExtractColorsFromImage(byte[] imageData)
    {
        using Mat image = ImagePreprocessor.PreprocessImage(Mat.FromImageData(imageData), Settings);
        ImageFragmentExtractor fragmentExtractor = new(Mat.FromImageData(imageData));

        Cv2.FindContours(image, out Point[][] contours, out _, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

        // Filter contours for shape simillar to cubie
        Debug.WriteLine("## Found blocks ##");
        List<Rect> filteredContours = ContourHelper.FilterContours(contours, image.Width / 2);
        List<List<Rect>> sortedContours = ContourHelper.SortContoursByPosition(filteredContours);

        if (sortedContours.Count == 0)
        {
            Debug.WriteLine("Couldn't find any contours!");
            throw new InvalidOperationException("Couldn't find any contours!");
        }

        // Get dominant color from image fragments
        Vec3i[,] dominantFragmentColors = fragmentExtractor.ExtractDominantColorsFromFragments(sortedContours);
        return _colorParser.ParseColors(dominantFragmentColors);
    }

    public FragmentData ExtractFragmentDataFromImage(byte[] imageData)
    {
        using Mat originalImage = Mat.FromImageData(imageData);
        using Mat image = ImagePreprocessor.PreprocessImage(originalImage.Clone(), Settings);
        ImageFragmentExtractor fragmentExtractor = new(Mat.FromImageData(imageData));

        Cv2.FindContours(image, out Point[][] contours, out _, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
        Cv2.DrawContours(image, contours, -1, new(255, 0, 255));

        // Filter contours for shape simillar to cubie
        Debug.WriteLine("## Found blocks ##");
        List<Rect> filteredContours = ContourHelper.FilterContours(contours, image.Width / 2);
        List<List<Rect>> sortedContours = ContourHelper.SortContoursByPosition(filteredContours);

        if (sortedContours.Count == 0)
            return new() { ProcessedImageRawData = image.ToBytes() };

        byte[,][] imageFragments = new byte[sortedContours.Count, sortedContours[0].Count][];
        for (int i = 0; i < sortedContours.Count; i++)
            for (int j = 0; j < sortedContours[i].Count; j++)
                imageFragments[i, j] = originalImage[sortedContours[i][j]].ToBytes();

        // Get dominant color from image fragments
        Vec3i[,] dominantFragmentColors = fragmentExtractor.ExtractDominantColorsFromFragments(sortedContours);
        CubeFaceColor[,] colors = _colorParser.ParseColors(dominantFragmentColors);

        return new()
        {
            ProcessedImageRawData = image.ToBytes(),
            FragmentImageRawData = imageFragments,
            FragmentColors = colors
        };
    }

    public byte[] ProcessCubeImage(byte[] imageData, out byte[,][] imageFragments, out CubeFaceColor[,] colors)
    {
        using Mat image = ImagePreprocessor.PreprocessImage(Mat.FromImageData(imageData), Settings);
        using Mat originalImage = Mat.FromImageData(imageData);
        ImageFragmentExtractor fragmentExtractor = new(Mat.FromImageData(imageData));

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
        Debug.WriteLine("## Found blocks ##");
        List<Rect> blocksBoundingRects = ContourHelper.FilterAndDrawContours(contours, image.Width / 2, image);

        if (blocksBoundingRects.Count < 9)
        {
            imageFragments = new byte[0, 0][];
            colors = new CubeFaceColor[0, 0];
            return image.ToBytes();
        }

        // Sort cubies based on their position
        List<List<Rect>> sortedBoundingRects = ContourHelper.SortContoursByPosition(blocksBoundingRects);

        // Get fragmends of image based on bounding rects
        imageFragments = new byte[sortedBoundingRects.Count, sortedBoundingRects[0].Count][];
        for (int i = 0; i < sortedBoundingRects.Count; i++)
            for (int j = 0; j < sortedBoundingRects[i].Count; j++)
                imageFragments[i, j] = originalImage[sortedBoundingRects[i][j]].ToBytes();

        // Get dominant color from image fragment
        Vec3i[,] dominantFragmentColors = fragmentExtractor.ExtractDominantColorsFromFragments(sortedBoundingRects);
        colors = _colorParser.ParseColors(dominantFragmentColors);

        return image.ToBytes();
    }

    public IAsyncEnumerator<byte[]> GetNextImageProcessingStep(byte[] imageRawData) => ImagePreprocessor.PreprocessImageStepByStep(Mat.FromImageData(imageRawData), Settings);

    public readonly struct FragmentData()
    {
        public byte[,][] FragmentImageRawData { get; init; } = new byte[0, 0][];
        public CubeFaceColor[,] FragmentColors { get; init; } = new CubeFaceColor[0, 0];
        public byte[] ProcessedImageRawData { get; init; } = new byte[0];
    }
}