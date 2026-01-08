using System.Reflection;
using Car_Model.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class VeiculoTeste
{
    [TestMethod]
    public void TesteGetSetPropriedades()
    {
        //arrange: e as variaveis necessarias para o teste
        var veiculo = new Veiculo();

        // act: executar a funcionalidade a ser testada
        veiculo.Id = 1;
        veiculo.Nome = "Modelo Teste";
        veiculo.Marca = "ABC1234";
        veiculo.Ano = 2020;
        // assert: validar se o resultado do act esta correto

        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Modelo Teste", veiculo.Nome);
        Assert.AreEqual("ABC1234", veiculo.Marca);
        Assert.AreEqual(2020, veiculo.Ano);
    }
}