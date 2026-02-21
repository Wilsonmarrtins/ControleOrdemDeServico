using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.OpenServiceOrder;
using OsService.Tests.TestData;
using Xunit;

namespace OsService.Tests.Integration.ServiceOrders;

public sealed class ServiceOrdersControllerTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory = factory;
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_ServiceOrders_WhenCustomerExists_Returns201()
    {
        var ct = TestContext.Current.CancellationToken;

        var customers = _factory.Services.GetRequiredService<ICustomerRepository>();

        var customer = Fakers.CustomerEntity().Generate();
        await customers.InsertAsync(customer, ct);

        var cmd = new OpenServiceOrderCommand(customer.Id, "Trocar óleo");

        var res = await _client.PostAsJsonAsync("/v1/service-orders", cmd, ct);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task Post_ServiceOrders_WhenCustomerNotFound_Returns404Or500()
    {
        var ct = TestContext.Current.CancellationToken;

        var cmd = new OpenServiceOrderCommand(Guid.NewGuid(), "Trocar óleo");

        var res = await _client.PostAsJsonAsync("/v1/service-orders", cmd, ct);

        Assert.True(res.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.InternalServerError);
    }
}