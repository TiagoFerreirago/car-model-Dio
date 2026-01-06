using System.Reflection;
using Car_Model.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class AdministradorTest
{
    [TestMethod]
    public void TesteGetSetPropriedades()
    {
        //arrange: e as variaveis necessarias para o teste
        var adm = new Administrador();

        // act: executar a funcionalidade a ser testada
        adm.Id = 1;
        adm.Email = "eteste@teste.com";
        adm.Senha = "senha123";
        adm.Perfil = "Administrador";
        // assert: validar se o resultado do act esta correto

        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("eteste@teste.com", adm.Email);
        Assert.AreEqual("senha123", adm.Senha);
        Assert.AreEqual("Administrador", adm.Perfil);
    }
}