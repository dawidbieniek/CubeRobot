namespace CubeRobot.CV;

internal static class ColorDifference
{
    private const double Pi = Math.PI;
    private const double Pi2 = Math.PI * 2;

    private const double Deg30 = Math.PI / 6.0;
    private const double Deg6 = Math.PI / 30.0;
    private const double Deg63 = 7.0 * Math.PI / 20.0;

    private const double Rad2Deg = 360.0 / Pi2;
    private const double Deg2Rad = Pi2 / 360.0;

    private const double Kl = 1.0;
    private const double Kc = 1.0;
    private const double Kh = 1.0;
    private const double Pow25_7 = 6_103_515_625.0;

    public static double LABDifference(LABColor first, LABColor second)
    {
        // 1. Calculate Cip, hip
        double Cab = (Math.Sqrt((first.A * first.A) + (first.B * first.B)) + Math.Sqrt((second.A * second.A) + (second.B * second.B))) * 0.5;
        double Cab7 = Math.Pow(Cab, 7);
        double G = 0.5 * (1.0 - Math.Sqrt(Cab7 / (Cab7 + Pow25_7)));
        double a1p = (1.0 + G) * first.A;
        double a2p = (1.0 + G) * second.A;
        double C1p = Math.Sqrt((a1p * a1p) + (first.B * first.B));
        double C2p = Math.Sqrt((a2p * a2p) + (second.B * second.B));

        double h1p = AreBothCloseTo0(a1p, first.B) ? 0.0 : Math.Atan2(first.B, a1p);
        h1p = KeepAsPositiveAngle(h1p);

        double h2p = AreBothCloseTo0(a2p, second.B) ? 0.0 : Math.Atan2(second.B, a2p);
        h2p = KeepAsPositiveAngle(h2p);

        // 2. Calculate dLp, dCp, dHp
        double dLp = second.L - first.L;
        double dCp = C2p - C1p;

        double dhp = 0.0;
        double hpDifference = h2p - h1p;
        if (!IsCloseTo0(C1p * C2p))
        {
            if (Math.Abs(hpDifference) <= Pi)
                dhp = hpDifference;
            else if (hpDifference > Pi)
                dhp = hpDifference - Pi2;
            else    // hpDifference < -180
                dhp = hpDifference + Pi2;
        }

        double dHp = 2.0 * Math.Sqrt(C1p * C2p) * Math.Sin(dhp * 0.5);

        // Calculate CIEDE2000 Color-Difference dE00:
        double mLp = (first.L + second.L) * 0.5;
        double mCp = (C1p + C2p) * 0.5;

        double hpSum = h1p + h2p;
        double mhp = hpSum;
        double hpDifferenceAbs = Math.Abs(hpDifference);
        if (!IsCloseTo0(C1p * C2p))
        {
            if (hpDifferenceAbs <= Pi)
                mhp = hpSum * 0.5;
            else if (hpDifferenceAbs > Pi && hpSum < Pi2)
                mhp = (hpSum + Pi2) * 0.5;
            else
                mhp = (hpSum - Pi2) * 0.5;
        }

        double T = 1.0 - (0.17 * Math.Cos(mhp - Deg30)) + (0.24 * Math.Cos(2.0 * mhp)) + (0.32 * Math.Cos((3.0 * mhp) + Deg6)) - (0.2 * Math.Cos((4.0 * mhp) - Deg63));
        double dTheta = 30 * Deg2Rad * Math.Exp(-Math.Pow((mhp * Rad2Deg - 275.0) / 25.0, 2));
        double RC = 2.0 * Math.Sqrt(Math.Pow(mCp, 7) / (Math.Pow(mCp, 7) + Pow25_7));
        double mLpSqr = (mLp - 50.0) * (mLp - 50.0);
        double SL = 1.0 + (0.015 * mLpSqr / Math.Sqrt(20.0 + mLpSqr));
        double SC = 1.0 + (0.045 * mCp);
        double SH = 1.0 + (0.015 * mCp * T);
        double RT = -Math.Sin(2.0 * dTheta) * RC;

        double de00 = Math.Sqrt(Math.Pow(dLp / (Kl * SL), 2) + Math.Pow(dCp / (Kc * SC), 2) + Math.Pow(dHp / (Kh * SH), 2) + (RT * dCp / (Kc * SC) * dHp / (Kh * SH)));
        return de00;
    }

    private static bool IsCloseTo0(double value) => Math.Abs(value) <= double.Epsilon;

    private static bool AreBothCloseTo0(double first, double second) => Math.Abs(first) <= double.Epsilon && Math.Abs(second) <= double.Epsilon;

    private static double KeepAsPositiveAngle(double angle) => angle < 0.0 ? angle + Pi : angle;
}