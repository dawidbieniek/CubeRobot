using System.Diagnostics;

using CubeRobot.CV.Color;
using CubeRobot.Models.RubiksCube;

namespace CubeRobot.CV;
public class CubeFaceColorParser(Dictionary<LABColor, CubeFaceColor> color2FaceMap, XYZColor? whiteBalanceColor = null)
{
    private static readonly XYZColor DefaultWhiteColor = new(0.5062f, 0.5368f, 0.5527f);//new (0.9505f, 1f, 1.0888f);

    private readonly Dictionary<LABColor, CubeFaceColor> _color2FaceMap = color2FaceMap;
    private readonly XYZColor _whiteBalanceColor = whiteBalanceColor ?? DefaultWhiteColor;

    public CubeFaceColor ParseColor(int b, int g, int r)
    {
        LABColor currentColor = SRGBColor.FromRGB(r, g, b).ToXYZColorSpace().ToLABColorSpace(_whiteBalanceColor);

        LABColor? closestColor = null;
        float minDifference = float.MaxValue;

        Debug.Write('\n');
        Debug.WriteLine("## Block color distance ##");
        foreach (var kvp in _color2FaceMap)
        {
            float currentDifference = currentColor.DifferenceTo(kvp.Key);
            Debug.WriteLine($"{currentColor}\t{kvp.Key}({kvp.Value})\t{currentDifference}");
            if (currentDifference < minDifference)
            {
                minDifference = currentDifference;
                closestColor = kvp.Key;
            }
        }

        return closestColor == null 
            ? throw new Exception("Couldn't find closest color") 
            : _color2FaceMap[closestColor.Value];
    }
}
