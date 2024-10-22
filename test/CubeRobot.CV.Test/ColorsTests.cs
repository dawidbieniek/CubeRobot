using CubeRobot.CV.Color;

namespace CubeRobot.CV.Test;

[TestClass]
public class ColorsTests
{
    private const float XYZColorEpsilon = 1E-3f;
    private const float LABColorEpsilon = 1E-1f;
    private const float LABColorDifferenceEpsilon = 1E-2f;

    [TestMethod]
    [DataRow(-0.1f)]
    [DataRow(1.1f)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SRGBColor_Constructor_ThrowsException_ForValuesOutsideAllowedRange(float value)
    {
        new SRGBColor(value, value, value);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(255, 0, 0)]
    [DataRow(51, 233, 51)]
    public void SRGBColor_FromRGB_CreatesCorrectColor_ForCorrectValues(int r, int g, int b)
    {
        SRGBColor color = SRGBColor.FromRGB(r, g, b);

        Assert.AreEqual(r / 255f, color.R, 1 / 255f);
        Assert.AreEqual(g / 255f, color.G, 1 / 255f);
        Assert.AreEqual(b / 255f, color.B, 1 / 255f);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(256)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void SRGBColor_FromRGB_ThrowsException_ForValuesOutsideAllowedRange(int value)
    {
        SRGBColor.FromRGB(value, value, value);
    }

    // Values taken from https://www.nixsensor.com/free-color-converter/ (D64 2°)
    [TestMethod]
    [DataRow(74 / 255f, 89 / 255f, 128 / 255f, 0.1029f, 0.1016f, 0.2184f)]
    [DataRow(0f, 0f, 0f, 0f, 0f, 0f)]
    [DataRow(1f, 1f, 1f, 0.9505f, 1f, 1.089f)]
    [DataRow(0f, 1f, 1f, 0.5380f, 0.7873f, 1.0695f)]
    public void SRGBColor_ToXYZColorSpace_CreatesCorrectXYZColor_ForCorrectSRGBValues(float r, float g, float b, float x, float y, float z)
    {
        SRGBColor srgbColor = new(r, g, b);

        XYZColor xyzColor = srgbColor.ToXYZColorSpace();

        Assert.AreEqual(x, xyzColor.X, XYZColorEpsilon);
        Assert.AreEqual(y, xyzColor.Y, XYZColorEpsilon);
        Assert.AreEqual(z, xyzColor.Z, XYZColorEpsilon);
    }

    [TestMethod]
    [DataRow(0.1029f, 0.1016f, 0.2184f, 38.13f, 5.01f, -23.75f)]
    [DataRow(0f, 0f, 0f, 0f, 0f, 0f)]
    [DataRow(0.9505f, 1f, 1.089f, 100f, 0f, 0f)]
    [DataRow(0.538f, 0.787f, 1.0695f, 91.11f, -48.09f, -14.13f)]
    public void XYZColor_ToLABColorSpace_CreatesCorrectLABColor_ForCorrectXYZValues(float x, float y, float z, float l, float a, float b)
    {
        XYZColor whitePointReference = new(0.9505f, 1f, 1.0888f);
        XYZColor xyzColor = new(x, y, z);

        LABColor labColor = xyzColor.ToLABColorSpace(whitePointReference);

        Assert.AreEqual(l, labColor.L, LABColorEpsilon);
        Assert.AreEqual(a, labColor.A, LABColorEpsilon);
        Assert.AreEqual(b, labColor.B, LABColorEpsilon);
    }

    // Test values taken from "The CIEDE2000 Color-Difference Formula: Implementation Notes,
    // Supplementary Test Data, and Mathematical Observations"
    [TestMethod]
    [DataRow(50f, 0f, 0f, 50f, -1f, 2f, 2.3669f)]   // 7
    [DataRow(50f, 2.5f, 0f, 61f, -5f, 29f, 22.8977f)]   // 18
    [DataRow(2.0776f, 0.0795f, -1.135f, 0.9033f, -0.0636f, -0.5514f, 0.9082f)]   // 34
    public void LABColor_DifferenceTo_ReturnsCorrectDifference_BetweenAnotherColor(float l1, float a1, float b1, float l2, float a2, float b2, double expectedDifference)
    {
        LABColor first = new(l1, a1, b1);
        LABColor second = new(l2, a2, b2);

        double difference = first.DifferenceTo(second);

        Assert.AreEqual(expectedDifference, difference, LABColorDifferenceEpsilon);
    }
}