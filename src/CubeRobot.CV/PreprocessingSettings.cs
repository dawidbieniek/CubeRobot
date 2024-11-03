using System.ComponentModel;

namespace CubeRobot.CV;

public class PreprocessingSettings
{
    public enum BlurTypes
    {
        [Description("Brak")]
        None,
        [Description("Średnia")]
        Average,
        [Description("Gaussa")]
        Gaussian,
        [Description("Mediana")]
        Median
    }

    public enum Shape
    {
        [Description("Prostokąt")]
        Rect = 0,
        [Description("Krzyż")]
        Cross = 1,
        [Description("Koło")]
        Ellipse = 2,
    }

    public static PreprocessingSettings Default => new();
    public bool UseDenoising { get; set; } = true;
    public int DenoisingStrength { get; set; } = 7;
    public BlurTypes BlurType { get; set; } = BlurTypes.Average;
    public int BlurKernelSize { get; set; } = 3;
    public bool UseCannyDetection { get; set; } = true;
    public int CannyLowerThreshold { get; set; } = 30;
    public int CannyUpperThreshold { get; set; } = 60;
    public int CannySobelOperatorValue { get; set; } = 3;
    public bool UseDialation { get; set; } = true;
    public Shape DialationShape { get; set; } = Shape.Rect;
    public int DialationSize { get; set; } = 7;
}