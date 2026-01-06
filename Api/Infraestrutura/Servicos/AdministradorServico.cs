using Dominio.Interface;
using Infraestrutura.Db;
using Dominio.Dtos;
using Dominio.Entidades;

namespace Infraestrutura.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? Login(LoginDto loginDto)
    {
       var adm = _contexto.Administradores.Where(a => a.Email == loginDto.Email && a.Senha == loginDto.Senha).FirstOrDefault();
    
        return adm;        
    }

    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();


        int tamanhoPagina = 10;


        if(pagina != null)
        {
            query = query.Skip(((int)pagina - 1) * tamanhoPagina).Take(tamanhoPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscarPorId(int id)
    {
        var adm = _contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
        return adm;
    }
}