using OsService.Services.V1.OpenServiceOrder;
using OsService.Tests.TestDoubles;
using OsService.Tests.TestData;
using Xunit;

namespace OsService.Tests.Unit.ServiceOrders;

public sealed class OpenServiceOrderHandlerTests
{
    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_Throws()
    {
        var soRepo = new FakeServiceOrderRepository();
        var cRepo = new FakeCustomerRepository();

        var sut = new OpenServiceOrderHandler(cRepo, soRepo);

        var cmd = Fakers.OpenServiceOrderCommand(Guid.NewGuid()).Generate();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenDescriptionEmpty_Throws()
    {
        var soRepo = new FakeServiceOrderRepository();
        var cRepo = new FakeCustomerRepository();

        var customer = Fakers.CustomerEntity().Generate();
        await cRepo.InsertAsync(customer, CancellationToken.None);

        var sut = new OpenServiceOrderHandler(cRepo, soRepo);

        var cmd = new OpenServiceOrderCommand(customer.Id, "   ");

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsIdAndNumber()
    {
        var soRepo = new FakeServiceOrderRepository();
        var cRepo = new FakeCustomerRepository();

        var customer = Fakers.CustomerEntity().Generate();
        await cRepo.InsertAsync(customer, CancellationToken.None);

        var sut = new OpenServiceOrderHandler(cRepo, soRepo);

        var cmd = Fakers.OpenServiceOrderCommand(customer.Id).Generate();

        var (id, number) = await sut.Handle(cmd, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.True(number > 0);
    }
}