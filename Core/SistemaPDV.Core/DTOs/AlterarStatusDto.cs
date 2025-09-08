using System.ComponentModel.DataAnnotations;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.DTOs
{
    public class AlterarStatusDto
    {
        [Required(ErrorMessage = "Status é obrigatório")]
        public StatusPedido NovoStatus { get; set; }
    }
}
