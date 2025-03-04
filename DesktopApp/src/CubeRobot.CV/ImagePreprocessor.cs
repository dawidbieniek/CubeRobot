using OpenCvSharp;

namespace CubeRobot.CV;

internal static class ImagePreprocessor
{
    public static Mat PreprocessImage(Mat image, PreprocessingSettings settings)
    {
        image = ConvertToGrayscale(image, settings);

        if (settings.UseDenoising)
            image = DenoiseImage(image, settings);
        if (settings.BlurType != PreprocessingSettings.BlurTypes.None)
            image = BlurImage(image, settings);
        if (settings.UseCannyDetection)
            image = CannyDetection(image, settings);
        if (settings.UseDialation)
            image = Dialate(image, settings);

        return image;
    }

    public static async IAsyncEnumerator<byte[]> PreprocessImageStepByStep(Mat image, PreprocessingSettings settings)
    {
        await Task.Run(() => image = ConvertToGrayscale(image, settings));
        yield return image.ToBytes();

        if (settings.UseDenoising)
            await Task.Run(() => image = DenoiseImage(image, settings));
        yield return image.ToBytes();

        if (settings.BlurType != PreprocessingSettings.BlurTypes.None)
            await Task.Run(() => image = BlurImage(image, settings));
        yield return image.ToBytes();

        if (settings.UseCannyDetection)
            await Task.Run(() => image = CannyDetection(image, settings));
        yield return image.ToBytes();

        if (settings.UseDialation)
            await Task.Run(() => image = Dialate(image, settings));
        yield return image.ToBytes();
    }

    private static Mat ConvertToGrayscale(Mat image, PreprocessingSettings settings)
    {
        using Mat buffer = new();
        Cv2.CvtColor(image, buffer, ColorConversionCodes.BGR2GRAY);
        buffer.CopyTo(image);
        return image;
    }

    private static Mat DenoiseImage(Mat image, PreprocessingSettings settings)
    {
        using Mat buffer = new();
        Cv2.FastNlMeansDenoising(image, buffer, settings.DenoisingStrength, 7, 21);
        buffer.CopyTo(image);
        return image;
    }

    private static Mat BlurImage(Mat image, PreprocessingSettings settings)
    {
        using Mat buffer = new();
        switch (settings.BlurType)
        {
            case PreprocessingSettings.BlurTypes.Average:
                Cv2.Blur(image, buffer, new(settings.BlurKernelSize, settings.BlurKernelSize));
                break;

            case PreprocessingSettings.BlurTypes.Gaussian:
                Cv2.GaussianBlur(image, buffer, new(settings.BlurKernelSize, settings.BlurKernelSize), 0);
                break;

            case PreprocessingSettings.BlurTypes.Median:
                Cv2.MedianBlur(image, buffer, settings.BlurKernelSize);
                break;

            case PreprocessingSettings.BlurTypes.None:
            default:
                return image;
        }

        buffer.CopyTo(image);
        return image;
    }

    private static Mat CannyDetection(Mat image, PreprocessingSettings settings)
    {
        using Mat buffer = new();
        Cv2.Canny(image, buffer, settings.CannyLowerThreshold, settings.CannyUpperThreshold, settings.CannySobelOperatorValue);
        buffer.CopyTo(image);
        return image;
    }

    private static Mat Dialate(Mat image, PreprocessingSettings settings)
    {
        using Mat buffer = new();
        Cv2.Dilate(image, buffer, Cv2.GetStructuringElement((MorphShapes)(int)settings.DialationShape, new(settings.DialationSize, settings.DialationSize)));
        buffer.CopyTo(image);
        return image;
    }
}