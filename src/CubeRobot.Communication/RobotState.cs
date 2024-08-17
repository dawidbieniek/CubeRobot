using System.ComponentModel;

namespace CubeRobot.Robot;

public enum RobotState
{
    [Description("Niepodłączony")]
    Disconnected,
    [Description("Brak kostki")]
    NoCube,
    [Description("Gotowy do analizy")]
    ReadyForPhotos,
    [Description("Prezentuje kostkę (1/6)")]
    Presenting1,
    [Description("Prezentuje kostkę (2/6)")]
    Presenting2,
    [Description("Prezentuje kostkę (3/6)")]
    Presenting3,
    [Description("Prezentuje kostkę (4/6)")]
    Presenting4,
    [Description("Prezentuje kostkę (5/6)")]
    Presenting5,
    [Description("Prezentuje kostkę (6/6)")]
    Presenting6,
    [Description("Układa kostkę")]
    Solving,
    [Description("Gotowy do odbioru kostki")]
    ReadyForRelease,
    [Description("Resetuje")]
    Resetting,
}