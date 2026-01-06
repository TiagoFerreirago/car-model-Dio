using System.Reflection;
using System.IO;
using Dominio.Entidades;
using Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infraestrutura.Servicos;

namespace Test.Domain.Servicos;

[TestClass]
[DoNotParallelize]
public sealed class AdministradorServicoTest
{
    [TestMethod]
    public void TesteSalvarAdministrador()
    {
        //arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");


        var adm = new Administrador
        {
            Id = 1,
            Email = "eteste@teste.com",
            Senha = "senha123",
            Perfil = "Adm"
        };
        //act: executar a funcionalidade a ser testada
        var servico = new AdministradorServico(contexto);
        servico.Incluir(adm);

        //assert: validar se o resultado do act esta correto
        Assert.AreEqual(1, servico.Todos(1).Count());
    }

    [TestMethod]
    public void TesteBuscarPorId()
    {
        //arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");


        var adm = new Administrador
        {
            Id = 1,
            Email = "eteste@teste.com",
            Senha = "senha123",
            Perfil = "Adm"
        };
        //act: executar a funcionalidade a ser testada
        var servico = new AdministradorServico(contexto);
        servico.Incluir(adm);
        var admPersistido = servico.BuscarPorId(1);

        //assert: validar se o resultado do act esta correto
        Assert.AreEqual(1, admPersistido.Id);
    }


        private DbContexto CriarConexaoTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
        .SetBasePath(path)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

        var configuration = builder.Build();


        return new DbContexto(configuration);
    }
     
      

}