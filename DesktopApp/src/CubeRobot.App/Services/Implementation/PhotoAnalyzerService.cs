using CubeRobot.CV;
using CubeRobot.Models.RubiksCube;

using static CubeRobot.CV.PhotoAnalyzer;

namespace CubeRobot.App.Services.Implementation;

internal class PhotoAnalyzerService : IPhotoAnalyzerService
{
    private PhotoAnalyzer _photoAnalyzer = new();

    public PreprocessingSettings Settings => _photoAnalyzer.Settings;

    public CubeFaceColor[,] ExtractColorsFromImage(byte[] imageData) => _photoAnalyzer.ExtractColorsFromImage(imageData);

    public IAsyncEnumerator<byte[]> GetNextImageProcessingStep(byte[] imageRawData) => _photoAnalyzer.GetNextImageProcessingStep(imageRawData);

    public FragmentData ExtractFragmentDataFromImage(byte[] imageData) => _photoAnalyzer.ExtractFragmentDataFromImage(imageData);
}