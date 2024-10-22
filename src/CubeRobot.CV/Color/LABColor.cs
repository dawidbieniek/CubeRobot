namespace CubeRobot.CV.Color;

public readonly struct LABColor(float l, float a, float b)
{
    public float L => l;
    public float A => a;
    public float B => b;

    public override string ToString() => $"LAB({L:F4}, {A:F4}, {B:F4})";

    public float DifferenceTo(LABColor other) => (float)ColorDifference.LABDifference(this, other);
}
