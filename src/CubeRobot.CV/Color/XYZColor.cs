namespace CubeRobot.CV.Color;

/// <summary> CIE XYZ color representation, assuming D65 light source at 2°. Color components are
/// normalized to 1 (not 100)
public readonly struct XYZColor(float x, float y, float z)
{
    private const float TransformationFunctionValueThreshold = 0.008856452f;    // (6/29)^3 in precision of float
    public float X { get; } = x;
    public float Y { get; } = y;
    public float Z { get; } = z;

    public override string ToString() => $"XYZ({X:F4}, {Y:F4}, {Z:F4})";

    public LABColor ToLABColorSpace(XYZColor whiteBalanceColor)
    {
        XYZColor normalizedXYZColor = NormalizeToWhitePoint(whiteBalanceColor);
        float transformedX = LABTransformationFunctionValue(normalizedXYZColor.X);
        float transformedY = LABTransformationFunctionValue(normalizedXYZColor.Y);
        float transformedZ = LABTransformationFunctionValue(normalizedXYZColor.Z);

        return new LABColor(
            116 * transformedY - 16,
            500 * (transformedX - transformedY),
            200 * (transformedY - transformedZ));
    }

    private static float LABTransformationFunctionValue(float value) => value > TransformationFunctionValueThreshold
        ? MathF.Cbrt(value)
        : 7.787037f * value + 0.137931f;

    private XYZColor NormalizeToWhitePoint(XYZColor whitePoint) => new(
        X / whitePoint.X,
        Y / whitePoint.Y,
        Z / whitePoint.Z);
}