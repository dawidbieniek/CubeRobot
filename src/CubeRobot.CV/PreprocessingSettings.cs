using OpenCvSharp;

namespace CubeRobot.CV;

public class PreprocessingSettings
{
    public bool UseDenoising { get; set; } = true;
    public int DenoisingStrength { get; set; } = 7;
}