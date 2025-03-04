using CubeRobot.App.Services;
using CubeRobot.App.Services.Implementation;
using CubeRobot.Robot;

namespace CubeRobot.App;

internal static class Dependencies
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IPhotoAnalyzerService, PhotoAnalyzerService>()
            .AddSingleton<IRobot, Robot.Robot>();
    }
}