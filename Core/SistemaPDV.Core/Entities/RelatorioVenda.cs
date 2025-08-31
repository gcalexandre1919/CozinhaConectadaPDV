namespace SistemaPDV.Core.Entities
{
    public class RelatorioVenda
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal TotalVenda { get; set; }
        public string? Observacoes { get; set; }
    }
}