using Dominio.Entidades;
using Dominio.Dtos;

namespace Dominio.Interface;

public interface IAdministradorServico
{

    Administrador? Login(LoginDto loginDto);

    Administrador Incluir(Administrador administrador);

    List<Administrador> Todos(int? pagina);

    Administrador? BuscarPorId(int id);
}