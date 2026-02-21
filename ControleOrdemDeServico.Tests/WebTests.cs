using System.Net;
using Microsoft.Extensions.Logging;

namespace OsService.Tests;

public class WebTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(90);

    [Fact(Skip = "Teste de integração Aspire. Habilitar quando for rodar integration tests com AppHost.")]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.OsService_AppHost>(cancellationToken);

        builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Information);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await builder
            .BuildAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app
            .StartAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        await app.ResourceNotifications
            .WaitForResourceHealthyAsync("webfrontend", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        var client = app.CreateHttpClient("webfrontend");

        var response = await client.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}