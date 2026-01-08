using System.Reflection;
using System.IO;
using Dominio.Entidades;
using Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Infraestrutura.Servicos;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Domain.Servicos;

[TestClass]
[DoNotParallelize]
public sealed class VeiculoServicoTeste
{
    [TestMethod]
    public void TesteApagarVeiculo()
    {
        //arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos;");


        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Carro Teste",
            Marca = "Marca Teste",
            Ano = 2020,
        };

        //act: executar a funcionalidade a ser testada
        var servico = new VeiculoServico(contexto);
        servico.Incluir(veiculo);
        var veiculoParaApagar = servico.BuscarPorId(1);
        servico.Apagar(veiculoParaApagar);

        //assert: validar se o resultado do act esta correto
        var veiculoPersistido = servico.BuscarPorId(1);
        Assert.IsNull(veiculoPersistido);
        Assert.AreEqual(0, servico.Todos(1).Count());
    }

    [TestMethod]
    public void TesteAtualizar()
    {
        //arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos;");

        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Carro Teste",
            Marca = "Marca Teste",
            Ano = 2020,
        };
        //act: executar a funcionalidade a ser testada
        var servico = new VeiculoServico(contexto);
        servico.Incluir(veiculo);
        // Modify the same object
        veiculo.Nome = "Carro Teste Atualizado";
        veiculo.Marca = "Marca Teste Atualizado";
        veiculo.Ano = 2021;
        servico.Atualizar(veiculo);
        var veiculoPersistido = servico.BuscarPorId(1);

        //assert: validar se o resultado do act esta correto
        Assert.AreEqual("Carro Teste Atualizado", veiculoPersistido.Nome);
        Assert.AreEqual("Marca Teste Atualizado", veiculoPersistido.Marca);
        Assert.AreEqual(2021, veiculoPersistido.Ano);

    }

    [TestMethod]
    public void TesteBuscarPorId()
    {
        // arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos;");

        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Carro Teste",
            Marca = "Marca Teste",
            Ano = 2020,
        };

        // act: executar a funcionalidade a ser testada
        var servico = new VeiculoServico(contexto);
        servico.Incluir(veiculo);
        var veiculoPersistido = servico.BuscarPorId(1);

        // assert: validar se o resultado do act esta correto
        Assert.AreEqual(1, veiculoPersistido.Id);
        Assert.AreEqual("Carro Teste", veiculoPersistido.Nome);
        Assert.AreEqual("Marca Teste", veiculoPersistido.Marca);
        Assert.AreEqual(2020, veiculoPersistido.Ano);
    }
    [TestMethod]
    public void TesteIncluir()
    {
        // arrange: e as variaveis necessarias para o teste
        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos;");

        var veiculo = new Veiculo
        {
            Id = 1,
            Nome = "Carro Teste",
            Marca = "Marca Teste",
            Ano = 2020,
        };

        var servico = new VeiculoServico(contexto);
        servico.Incluir(veiculo);
        var veiculoPersistido = servico.BuscarPorId(1);

        Assert.AreEqual(1, veiculoPersistido.Id);
        Assert.AreEqual("Carro Teste", veiculoPersistido.Nome);
        Assert.AreEqual("Marca Teste", veiculoPersistido.Marca);
        Assert.AreEqual(2020, veiculoPersistido.Ano);
    }

    [TestMethod]
    public void TesteTodos()
    {

        var contexto = CriarConexaoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;");
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE veiculos;");

        var veiculo1 = new Veiculo
        {
            Id = 1,
            Nome = "Carro Teste 1",
            Marca = "Marca Teste 1",
            Ano = 2020,
        };
        var veiculo2 = new Veiculo
        {
            Id = 2,
            Nome = "Carro Teste 2",
            Marca = "Marca Teste 2",
            Ano = 2021,
        };

        var servico = new VeiculoServico(contexto);
        servico.Incluir(veiculo1);
        servico.Incluir(veiculo2);
        var todosVeiculos = servico.Todos(1);

        Assert.AreEqual(2, todosVeiculos.Count());
        Assert.AreEqual("Carro Teste 1", todosVeiculos[0].Nome);
        Assert.AreEqual("Marca Teste 1", todosVeiculos[0].Marca); 
        Assert.AreEqual(2020, todosVeiculos[0].Ano);   
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