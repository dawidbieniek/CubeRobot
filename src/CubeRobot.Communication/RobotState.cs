using System.ComponentModel;

namespace CubeRobot.Robot;

public enum RobotState
{
    [Description("niepodłączony")]
    Disconnected,
    [Description("brak kostki")]
    NoCube,
    [Description("gotowy do analizy")]
    ReadyForPhotos,
    [Description("prezentuje kostkę (1/6)")]
    Presenting1,
    [Description("prezentuje kostkę (2/6)")]
    Presenting2,
    [Description("prezentuje kostkę (3/6)")]
    Presenting3,
    [Description("prezentuje kostkę (4/6)")]
    Presenting4,
    [Description("prezentuje kostkę (5/6)")]
    Presenting5,
    [Description("prezentuje kostkę (6/6)")]
    Presenting6,
    [Description("układa kostkę")]
    Solving,
    [Description("gotowy do odbioru kostki")]
    ReadyForRelease,
    [Description("resetuje")]
    Resetting,
}