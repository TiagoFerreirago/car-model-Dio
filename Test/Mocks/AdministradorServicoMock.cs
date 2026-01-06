using Dominio.Entidades;
using Dominio.Dtos;
using Dominio.Interface;

namespace Test.Mocks;

public class AdministradorServicoMock : IAdministradorServico
{

    private static List<Administrador> administradores = new List<Administrador>()
    {
        new Administrador { Id = 1, Perfil = "adm", Email = "admin1@example.com", Senha = "password1" },
        new Administrador { Id = 2, Perfil = "Editor", Email = "admin2@example.com", Senha = "password2" }
    };
    public Administrador? Login(LoginDto loginDto)
    {
        return administradores.Find(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha);
    }

    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count + 1;
        administradores.Add(administrador);
        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        return administradores;
    }

    public Administrador? BuscarPorId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }
}
