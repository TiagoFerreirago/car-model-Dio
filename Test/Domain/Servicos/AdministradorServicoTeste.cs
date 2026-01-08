using System.Reflection;
using System.IO;
using Dominio.Entidades;
using Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infraestrutura.Servicos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domain.Servicos;

[TestClass]
[DoNotParallelize]
public sealed class AdministradorServicoTeste
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

    [TestMethod]
    public void TesteBuscarTodos()
    {
        // arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");

        var adm1 = new Administrador
        {
            Id = 1,
            Email = "eteste@gmail.com",
            Senha = "senha123",
            Perfil = "Adm"
        };
        var adm2 = new Administrador
        {
            Id = 2,
            Email = "eteste2@gmail.com",
            Senha = "senha123",
            Perfil = "Usuario"
        };
        // act: executar a funcionalidade a ser testada
        var servico = new AdministradorServico(contexto);
        servico.Incluir(adm1);
        servico.Incluir(adm2);
        var todosAdmins = servico.Todos(1);

        // assert: validar se o resultado do act esta correto
        Assert.AreEqual(2, todosAdmins.Count());
        Assert.AreEqual("eteste@gmail.com", todosAdmins[0].Email);
        Assert.AreEqual(1, todosAdmins[0].Id);
        Assert.AreEqual("senha123", todosAdmins[0].Senha);
        Assert.AreEqual("Adm", todosAdmins[0].Perfil);
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