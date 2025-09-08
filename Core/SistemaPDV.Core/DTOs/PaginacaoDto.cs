using System.ComponentModel.DataAnnotations;

namespace SistemaPDV.Core.DTOs
{
    public class PaginacaoParametrosDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que 0")]
        public int Pagina { get; set; } = 1;
        
        [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
        public int TamanhoPagina { get; set; } = 10;
        
        public string? Termo { get; set; }
        
        public string? OrdenarPor { get; set; } = "Nome";
        
        public bool OrdemDecrescente { get; set; } = false;
    }
    
    public class PaginacaoResultadoDto<T>
    {
        public List<T> Dados { get; set; } = new();
        
        public int TotalItens { get; set; }
        
        public int PaginaAtual { get; set; }
        
        public int TamanhoPagina { get; set; }
        
        public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / TamanhoPagina);
        
        public bool TemPaginaAnterior => PaginaAtual > 1;
        
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;
        
        public int PaginaAnterior => TemPaginaAnterior ? PaginaAtual - 1 : 1;
        
        public int ProximaPagina => TemProximaPagina ? PaginaAtual + 1 : TotalPaginas;
    }
}
