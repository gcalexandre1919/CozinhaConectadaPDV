using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    public class ClienteEnderecoService : IClienteEnderecoService
    {
        private readonly PDVDbContext _context;

        public ClienteEnderecoService(PDVDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteEndereco>> ObterPorClienteAsync(int clienteId)
        {
            return await _context.ClienteEnderecos
                .Where(e => e.ClienteId == clienteId)
                .OrderByDescending(e => e.EnderecoPrincipal)
                .ThenBy(e => e.DataCriacao)
                .ToListAsync();
        }

        public async Task<ClienteEndereco?> ObterPorIdAsync(int id)
        {
            return await _context.ClienteEnderecos.FindAsync(id);
        }

        public async Task<ClienteEndereco> CriarAsync(ClienteEndereco endereco)
        {
            // Se for o primeiro endereço do cliente, definir como principal
            var existeEndereco = await _context.ClienteEnderecos
                .AnyAsync(e => e.ClienteId == endereco.ClienteId);
            
            if (!existeEndereco)
            {
                endereco.EnderecoPrincipal = true;
            }

            _context.ClienteEnderecos.Add(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<ClienteEndereco> AtualizarAsync(ClienteEndereco endereco)
        {
            _context.ClienteEnderecos.Update(endereco);
            await _context.SaveChangesAsync();
            return endereco;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var endereco = await _context.ClienteEnderecos.FindAsync(id);
            if (endereco == null) return false;

            // Se for o endereço principal, definir outro como principal
            if (endereco.EnderecoPrincipal)
            {
                var outroEndereco = await _context.ClienteEnderecos
                    .Where(e => e.ClienteId == endereco.ClienteId && e.Id != id)
                    .FirstOrDefaultAsync();
                
                if (outroEndereco != null)
                {
                    outroEndereco.EnderecoPrincipal = true;
                }
            }

            _context.ClienteEnderecos.Remove(endereco);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DefinirComoPrincipalAsync(int id, int clienteId)
        {
            // Remover principal de todos os endereços do cliente
            var enderecos = await _context.ClienteEnderecos
                .Where(e => e.ClienteId == clienteId)
                .ToListAsync();

            foreach (var endereco in enderecos)
            {
                endereco.EnderecoPrincipal = endereco.Id == id;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
