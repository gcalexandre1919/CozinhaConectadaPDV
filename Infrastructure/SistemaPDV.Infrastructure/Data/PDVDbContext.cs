using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.Entities;

namespace SistemaPDV.Infrastructure.Data
{
    public class PDVDbContext : DbContext
    {
        public PDVDbContext(DbContextOptions<PDVDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ClienteEndereco> ClienteEnderecos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<Impressora> Impressoras { get; set; }
        public DbSet<FilaImpressao> FilaImpressao { get; set; }
        public DbSet<ConfiguracaoImpressao> ConfiguracoesImpressao { get; set; }
        public DbSet<RelatorioVenda> RelatoriosVenda { get; set; }
        public DbSet<VendaProduto> VendaProdutos { get; set; }
        public DbSet<Restaurante> Restaurantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações da entidade Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CPF).HasMaxLength(11);
                entity.Property(e => e.CNPJ).HasMaxLength(14);
                entity.Property(e => e.Observacoes).HasMaxLength(500);
                entity.Property(e => e.DataCadastro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Ativo).HasDefaultValue(true);
                
                // Ignorar propriedade que não existe na tabela atual
                entity.Ignore(e => e.DataNascimento);
                
                // Relacionamentos
                entity.HasMany(e => e.Enderecos)
                      .WithOne(e => e.Cliente)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(e => e.Pedidos)
                      .WithOne(e => e.Cliente)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                // Índices para melhor performance
                entity.HasIndex(e => e.Email).IsUnique(false);
                entity.HasIndex(e => e.CPF).IsUnique(false);
                entity.HasIndex(e => e.CNPJ).IsUnique(false);
                entity.HasIndex(e => e.Nome);
            });

            // Configurações da entidade ClienteEndereco
            modelBuilder.Entity<ClienteEndereco>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Endereco).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Numero).HasMaxLength(10);
                entity.Property(e => e.Complemento).HasMaxLength(100);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cidade).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UF).IsRequired().HasMaxLength(2);
                entity.Property(e => e.CEP).HasMaxLength(8);
                entity.Property(e => e.Referencia).HasMaxLength(200);
                
                // Índices
                entity.HasIndex(e => e.ClienteId);
                entity.HasIndex(e => e.CEP);
            });

            // Configurações da entidade Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Tipo).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.SubTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PercentualGarcom).HasColumnType("decimal(5,2)");
                entity.Property(e => e.ValorGarcom).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TaxaEntrega).HasColumnType("decimal(10,2)");
                entity.Property(e => e.ValorTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Observacoes).HasMaxLength(500);
                
                // Relacionamentos
                entity.HasMany(e => e.Itens)
                      .WithOne(e => e.Pedido)
                      .HasForeignKey(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Índices
                entity.HasIndex(e => e.DataCriacao);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Tipo);
            });

            // Configurações da entidade PedidoItem
            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantidade).IsRequired();
                entity.Property(e => e.PrecoUnitario).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Observacoes).HasMaxLength(200);
                
                // Relacionamento com Produto
                entity.HasOne(e => e.Produto)
                      .WithMany()
                      .HasForeignKey(e => e.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurações das outras entidades (mantendo as existentes)
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descricao).HasMaxLength(500);
            });

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descricao).HasMaxLength(500);
                entity.Property(e => e.Preco).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<Impressora>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Caminho).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<RelatorioVenda>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataVenda).IsRequired();
                entity.Property(e => e.TotalVenda).HasColumnType("decimal(10,2)");
            });
        }
    }
}