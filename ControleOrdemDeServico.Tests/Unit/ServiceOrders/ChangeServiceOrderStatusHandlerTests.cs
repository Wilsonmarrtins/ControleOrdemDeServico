using OsService.Domain.Enums;
using OsService.Services.V1.ChangeServiceOrderStatus;
using OsService.Tests.TestDoubles;
using OsService.Tests.TestData;
using Xunit;

namespace OsService.Tests.Unit.ServiceOrders;

public sealed class ChangeServiceOrderStatusHandlerTests
{
    [Fact]
    public async Task Handle_WhenNotFound_ReturnsFalse()
    {
        var repo = new FakeServiceOrderRepository();
        var sut = new ChangeServiceOrderStatusHandler(repo);

        var cmd = Fakers.ChangeStatusCommand(Guid.NewGuid(), ServiceOrderStatus.InProgress).Generate();

        var ok = await sut.Handle(cmd, CancellationToken.None);

        Assert.False(ok);
    }

    [Fact]
    public async Task Handle_OpenToInProgress_ReturnsTrue()
    {
        var repo = new FakeServiceOrderRepository();
        var so = Fakers.ServiceOrderEntity(ServiceOrderStatus.Open).Generate();
        repo.Seed(so);

        var sut = new ChangeServiceOrderStatusHandler(repo);

        var cmd = new ChangeServiceOrderStatusCommand(so.Id, ServiceOrderStatus.InProgress);

        var ok = await sut.Handle(cmd, CancellationToken.None);

        Assert.True(ok);
    }

    [Fact]
    public async Task Handle_InvalidTransition_Throws()
    {
        var repo = new FakeServiceOrderRepository();
        var so = Fakers.ServiceOrderEntity(ServiceOrderStatus.Open).Generate();
        repo.Seed(so);

        var sut = new ChangeServiceOrderStatusHandler(repo);

        var cmd = new ChangeServiceOrderStatusCommand(so.Id, ServiceOrderStatus.Finished);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Handle(cmd, CancellationToken.None));
    }
}