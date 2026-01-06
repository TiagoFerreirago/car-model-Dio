using System.Collections.Generic;

namespace Car_Model.Dominio.ModelViews;

public class ErroDeValidacao
{
    public List<string> Mensagem { get; set; } = new List<string>();
}