namespace CubeRobot.CV.Color;

public class BGR2LABColorConverter(XYZColor whitePointReference)
{
    public XYZColor WhitePointReference { get; set; } = whitePointReference;

    public BGR2LABColorConverter() : this(new(0.9505f, 1f, 1.0888f))
    { }

    public LABColor Convert(int b, int g, int r)
    {
        SRGBColor srgbColor = SRGBColor.FromRGB(r, g, b);
        XYZColor xyzColor = srgbColor.ToXYZColorSpace();
        return xyzColor.ToLABColorSpace(WhitePointReference);
    }
}