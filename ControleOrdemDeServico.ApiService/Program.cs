using MediatR;
using Microsoft.OpenApi;
using OsService.Infrastructure.Databases;
using OsService.Infrastructure.Repository;
using OsService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.AddServiceDefaults();
}

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(ServicesServiceCollecation).Assembly);
});

builder.Services.AddSingleton<IDefaultSqlConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();
builder.Services.AddScoped<IServiceOrderAttachmentRepository, ServiceOrderAttachmentRepository>();
builder.Services.AddSingleton<DatabaseGenerantor>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OsService.ApiService", Version = "v1" });
    c.CustomSchemaIds(t => t.FullName);
    c.ResolveConflictingActions(apiDescs => apiDescs.First());
});

var app = builder.Build();

app.UseMiddleware<OsService.ApiService.ExceptionHandlingMiddleware>();

app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"), branch =>
{
    branch.Use(async (ctx, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = 500;
            ctx.Response.ContentType = "text/plain; charset=utf-8";
            await ctx.Response.WriteAsync(ex.ToString(), Encoding.UTF8);
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
await app.RunAsync();
public partial class Program
{
    protected Program() { }
}