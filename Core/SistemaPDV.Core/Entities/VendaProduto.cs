namespace SistemaPDV.Core.Entities
{
    public class VendaProduto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total { get; set; }
        public DateTime DataVenda { get; set; }
    }
}