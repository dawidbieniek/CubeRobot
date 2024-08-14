using CubeRobot.Robot;

namespace CubeRobot.App;

internal static class Dependencies
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IRobot, Robot.Robot>();
    }
}