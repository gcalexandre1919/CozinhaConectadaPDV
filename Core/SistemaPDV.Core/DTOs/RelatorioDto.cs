using SistemaPDV.Core.Entities;

namespace SistemaPDV.Core.DTOs
{
    public class RelatorioVendaDto
    {
        public DateTime Data { get; set; }
        public int NumeroPedidos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal TicketMedio { get; set; }
        public Dictionary<string, int> PedidosPorTipo { get; set; } = new();
    }
    
    public class RelatorioProdutoDto
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal PrecoMedio { get; set; }
    }
    
    public class RelatorioClienteDto
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int TotalPedidos { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime? UltimoPedido { get; set; }
        public decimal TicketMedio { get; set; }
    }
    
    public class RelatorioEstatisticasDto
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        
        // Vendas
        public int TotalPedidos { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal TicketMedio { get; set; }
        
        // Por tipo de pedido
        public Dictionary<string, int> PedidosPorTipo { get; set; } = new();
        public Dictionary<string, decimal> VendasPorTipo { get; set; } = new();
        
        // Por período
        public List<RelatorioVendaDto> VendasPorDia { get; set; } = new();
        
        // Produtos mais vendidos
        public List<RelatorioProdutoDto> TopProdutos { get; set; } = new();
        
        // Clientes mais ativos
        public List<RelatorioClienteDto> TopClientes { get; set; } = new();
        
        // Horários de pico
        public Dictionary<int, int> PedidosPorHora { get; set; } = new();
    }
}