using Dominio.ModelViews;

namespace Dominio.Dtos;

public class AdministradorDto
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;
}