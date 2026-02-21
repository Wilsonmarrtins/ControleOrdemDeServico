using OsService.Services.V1.CreateCustomer;
using OsService.Tests.TestDoubles;
using OsService.Tests.TestData;
using Xunit;

namespace OsService.Tests.Unit.Customers;

public sealed class CreateCustomerHandlerTests
{
    [Fact]
    public async Task Handle_WhenNameIsEmpty_Throws()
    {
        var repo = new FakeCustomerRepository();
        var sut = new CreateCustomerHandler(repo);

        var baseCmd = Fakers.CreateCustomerCommand().Generate();
        var cmd = new CreateCustomerCommand("   ", baseCmd.Phone, baseCmd.Email, baseCmd.Document);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenValid_InsertsAndReturnsId()
    {
        var repo = new FakeCustomerRepository();
        var sut = new CreateCustomerHandler(repo);

        var cmd = Fakers.CreateCustomerCommand().Generate();

        var id = await sut.Handle(cmd, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }
}