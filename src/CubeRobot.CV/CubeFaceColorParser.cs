using System.Diagnostics;

using CubeRobot.CV.Color;
using CubeRobot.Models.RubiksCube;

using OpenCvSharp;

namespace CubeRobot.CV;

public class CubeFaceColorParser(Dictionary<LABColor, CubeFaceColor> colorFaceMap, XYZColor? whiteBalanceColor = null)
{
    private static readonly XYZColor DefaultWhiteColor = new(0.5062f, 0.5368f, 0.5527f);//new (0.9505f, 1f, 1.0888f);

    private readonly Dictionary<LABColor, CubeFaceColor> _colorFaceMap = colorFaceMap;
    private readonly XYZColor _whiteBalanceColor = whiteBalanceColor ?? DefaultWhiteColor;

    public CubeFaceColor ParseColor(int b, int g, int r)
    {
        LABColor currentColor = SRGBColor.FromRGB(r, g, b).ToXYZColorSpace().ToLABColorSpace(_whiteBalanceColor);

        LABColor? closestColor = null;
        float minDifference = float.MaxValue;

        Debug.Write('\n');
        Debug.WriteLine("## Block color distance ##");
        foreach (var kvp in _colorFaceMap)
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
            : _colorFaceMap[closestColor.Value];
    }

    public CubeFaceColor[,] ParseColors(Vec3i[,] colors)
    {
        CubeFaceColor[,] faceColors = new CubeFaceColor[colors.GetLength(0), colors.GetLength(1)];

        for (int i = 0; i < faceColors.GetLength(0); i++)
            for (int j = 0; j < faceColors.GetLength(1); j++)
            {
                faceColors[i, j] = ParseColor(colors[i, j].Item0, colors[i, j].Item1, colors[i, j].Item2);
            }

        return faceColors;
    }
}