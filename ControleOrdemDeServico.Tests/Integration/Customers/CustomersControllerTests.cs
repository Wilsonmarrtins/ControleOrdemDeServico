using System.Net;
using System.Net.Http.Json;
using OsService.Services.V1.CreateCustomer;
using OsService.Tests.TestData;
using Xunit;

namespace OsService.Tests.Integration.Customers;

public sealed class CustomersControllerTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Customers_WhenValid_Returns201AndId()
    {
        var ct = TestContext.Current.CancellationToken;

        var cmd = Fakers.CreateCustomerCommand().Generate();

        var res = await _client.PostAsJsonAsync("/v1/customers", cmd, ct);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken: ct);
        Assert.NotNull(body);
        Assert.True(body!.ContainsKey("id"));
        Assert.True(Guid.TryParse(body["id"], out _));
    }

    [Fact]
    public async Task Post_Customers_WhenNameEmpty_Returns400Or500DependingOnHandler()
    {
        var ct = TestContext.Current.CancellationToken;
        var baseCmd = Fakers.CreateCustomerCommand().Generate();
        var cmd = new CreateCustomerCommand("   ", baseCmd.Phone, baseCmd.Email, baseCmd.Document);
        var res = await _client.PostAsJsonAsync("/v1/customers", cmd, ct);
        Assert.True(res.StatusCode is HttpStatusCode.InternalServerError or HttpStatusCode.BadRequest);
    }
}