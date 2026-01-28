using Hyperbee.Migrations;
using System.Diagnostics;

namespace {{cookiecutter.assembly_name }}.Migrations;

public class MainService : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<MainService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public const string ActivitySourceName = "Migrations";
    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    public MainService(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, ILogger<MainService> logger)
    {
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
        _logger = logger;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;

        await Task.Yield(); // yield to allow startup logs to write to console

        using var activity = _activitySource.StartActivity("Running Migrations", ActivityKind.Client);

        try
        {
            var runner = provider.GetRequiredService<MigrationRunner>();
            await runner.RunAsync(stoppingToken);

            _logger.LogInformation("Migrations completed successfully.");
            Environment.ExitCode = 0;
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Migrations cancelled due to shutdown.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Migrations encountered an unhandled exception.");
            Environment.ExitCode = 1;
        }
        finally
        {
            _applicationLifetime.StopApplication();
        }
    }
}
