namespace Car_Model.Dominio.ModelViews;

public record AdministradorLogin
{
     public string Token { get; set; }
    public string Email { get; set; }

    public string Perfil { get; set; }
}