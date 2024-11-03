using CubeRobot.CV;
using CubeRobot.Models.RubiksCube;

using static CubeRobot.CV.PhotoAnalyzer;

namespace CubeRobot.App.Services;

public interface IPhotoAnalyzerService
{
    PreprocessingSettings Settings { get; }

    CubeFaceColor[,] ExtractColorsFromImage(byte[] imageData);

    IAsyncEnumerator<byte[]> GetProcessedImagesStepByStep(byte[] imageRawData);

    FragmentData ExtractFragmentDataFromImage(byte[] imageData);
}