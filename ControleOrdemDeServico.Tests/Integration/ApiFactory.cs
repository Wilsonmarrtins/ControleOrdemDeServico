using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OsService.Infrastructure.Repository;
using OsService.Tests.TestDoubles;
using OsService.ApiService;

namespace OsService.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<ApiAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICustomerRepository>();
            services.RemoveAll<IServiceOrderRepository>();
            services.RemoveAll<IServiceOrderAttachmentRepository>();

            services.AddSingleton<ICustomerRepository, FakeCustomerRepository>();
            services.AddSingleton<IServiceOrderRepository, FakeServiceOrderRepository>();
            services.AddSingleton<IServiceOrderAttachmentRepository, FakeServiceOrderAttachmentRepository>();
        });
    }
}