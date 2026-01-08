namespace Test.Request;
using Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Car_Model.Dominio.ModelViews;

[TestClass]
public class AdministradorRequestTeste
{
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Setup.ClassInit(context);
    }
    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TesteAdministradorRequest()
    {

        // Arrange
        var loginDto = new Dominio.Dtos.LoginDto
        {
            Email = "admin1@example.com",
            Senha = "password1"
        };
        
        var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        // Act
        var response = await Setup.client.PostAsync("/login", content);
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admPersistido = JsonSerializer.Deserialize<AdministradorLogin>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Assert.IsNotNull(admPersistido);
        Assert.IsNotNull(admPersistido.Token);
        Assert.IsNotNull(admPersistido.Email);
        Assert.IsNotNull(admPersistido.Perfil);
    }
}