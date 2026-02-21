using Bogus;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Services.V1.ChangeServiceOrderStatus;
using OsService.Services.V1.OpenServiceOrder;
using OsService.Services.V1.CreateCustomer;

namespace OsService.Tests.TestData;

public static class Fakers
{
    public static Faker<ServiceOrderEntity> ServiceOrderEntity(ServiceOrderStatus status)
        => new Faker<ServiceOrderEntity>("pt_BR")
            .CustomInstantiator(f => new ServiceOrderEntity
            {
                Id = Guid.NewGuid(),
                Number = f.Random.Int(1, 9999),
                CustomerId = Guid.NewGuid(),
                Description = f.Lorem.Sentence(),
                Status = status,
                OpenedAt = DateTime.UtcNow,
                Price = null,
                Coin = "BRL",
                UpdatedPriceAt = null,
                StartedAt = null,
                FinishedAt = null
            });

    public static Faker<CustomerEntity> CustomerEntity()
        => new Faker<CustomerEntity>("pt_BR")
            .CustomInstantiator(f => new CustomerEntity
            {
                Id = Guid.NewGuid(),
                Name = f.Person.FullName,
                Phone = f.Phone.PhoneNumber(),
                Email = f.Internet.Email(),
                Document = f.Random.ReplaceNumbers("###########"),
                CreatedAt = DateTime.UtcNow
            });

    public static Faker<CreateCustomerCommand> CreateCustomerCommand()
        => new Faker<CreateCustomerCommand>("pt_BR")
            .CustomInstantiator(f => new CreateCustomerCommand(
                f.Person.FullName,
                f.Phone.PhoneNumber(),
                f.Internet.Email(),
                f.Random.ReplaceNumbers("###########")
            ));

    public static Faker<OpenServiceOrderCommand> OpenServiceOrderCommand(Guid customerId)
        => new Faker<OpenServiceOrderCommand>("pt_BR")
            .CustomInstantiator(f =>
                new OpenServiceOrderCommand(
                    customerId,
                    f.Lorem.Sentence()
                ));

    public static Faker<ChangeServiceOrderStatusCommand> ChangeStatusCommand(Guid id, ServiceOrderStatus status)
        => new Faker<ChangeServiceOrderStatusCommand>()
            .CustomInstantiator(_ =>
                new ChangeServiceOrderStatusCommand(id, status)
            );
}