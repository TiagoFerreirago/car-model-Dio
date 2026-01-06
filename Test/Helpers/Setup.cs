namespace Test.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Car_Model;  // Para Startup
using Dominio.Interface;  // Para IAdministradorServico
using Test.Mocks;  // Para AdministradorServicoMock
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

public static class Setup
{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default!;
    public static void ClassInit(TestContext context)
    {

        Setup.testContext = context;
        Setup.http = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServico, AdministradorServicoMock>();
            });
        });
        Setup.client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}