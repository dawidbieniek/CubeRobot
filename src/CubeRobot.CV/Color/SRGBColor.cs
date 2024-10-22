namespace CubeRobot.CV.Color;

/// <summary>
/// Normalized RGB color representation. Values are in range (0, 1)
/// </summary>
public readonly struct SRGBColor
{
    private const double LinearizationSmallValueThreshold = 0.04045f;

    public SRGBColor(float r, float g, float b)
    {
        if (r < 0 || r > 1)
            throw new ArgumentOutOfRangeException(nameof(r), $"Red component ({r}) must be between 0 and 1");
        if (g < 0 || g > 1)
            throw new ArgumentOutOfRangeException(nameof(g), $"Green component ({g}) must be between 0 and 1");
        if (b < 0 || b > 1)
            throw new ArgumentOutOfRangeException(nameof(b), $"Blue component ({b}) must be between 0 and 1");

        R = r;
        G = g;
        B = b;
    }

    public float R { get; init; }
    public float G { get; init; }
    public float B { get; init; }

    public override string ToString() => $"RGB({R:F4}, {G:F4}, {B:F4})";

    /// <summary>
    /// Creates <see cref="SRGBColor"/> from 8-bit (0-255) color components
    /// </summary>
    public static SRGBColor FromRGB(int r, int g, int b)
    {
        if (r < 0 || r > 255)
            throw new ArgumentOutOfRangeException(nameof(r), "Red component must be between 0 and 255");
        if (g < 0 || g > 255)
            throw new ArgumentOutOfRangeException(nameof(g), "Green component must be between 0 and 255");
        if (b < 0 || b > 255)
            throw new ArgumentOutOfRangeException(nameof(b), "Blue component must be between 0 and 255");

        return new(
            r / 255f,
            g / 255f,
            b / 255f);
    }

    public XYZColor ToXYZColorSpace()
    {
        // Perform inverse gamma correction -> https://en.wikipedia.org/wiki/SRGB#Transformation
        SRGBColor linearized = Linearize();
        // Multiply by transformation matrix
        return new XYZColor(
            0.4124f * linearized.R + 0.3576f * linearized.G + 0.1805f * linearized.B,
            0.2126f * linearized.R + 0.7152f * linearized.G + 0.0722f * linearized.B,
            0.0193f * linearized.R + 0.1192f * linearized.G + 0.9505f * linearized.B);
    }

    private SRGBColor Linearize() => new(
        LinearizeComponent(R),
        LinearizeComponent(G),
        LinearizeComponent(B));

    private static float LinearizeComponent(float colorComponent)
    {
        return colorComponent <= LinearizationSmallValueThreshold
            ? colorComponent / 12.92f
            : MathF.Pow((colorComponent + 0.055f) / 1.055f, 2.4f);
    }
}