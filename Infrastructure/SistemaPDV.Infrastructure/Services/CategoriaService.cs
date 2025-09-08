using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaPDV.Core.DTOs;
using SistemaPDV.Core.Entities;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;

namespace SistemaPDV.Infrastructure.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly PDVDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(PDVDbContext context, ICurrentUserService currentUserService, ILogger<CategoriaService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<(IEnumerable<CategoriaDto> Categorias, int Total)> GetCategoriasAsync(int pagina = 1, int tamanhoPagina = 10)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Buscando categorias para restaurante {RestauranteId} - Página {Pagina}, Tamanho {Tamanho} - Usuário: {UserName}", 
                    restauranteId, pagina, tamanhoPagina, userName);

                var query = _context.Categorias
                    .Where(c => c.RestauranteId == restauranteId)
                    .Include(c => c.Impressora);

                var total = await query.CountAsync();

                var categorias = await query
                    .OrderBy(c => c.Nome)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .Select(c => ConvertToDto(c))
                    .ToListAsync();

                _logger.LogInformation("Encontradas {Count} categorias de {Total} total para restaurante {RestauranteId}", 
                    categorias.Count(), total, restauranteId);

                return (categorias, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias para restaurante {RestauranteId}", _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<CategoriaDto?> GetCategoriaByIdAsync(int id)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Buscando categoria {CategoriaId} para restaurante {RestauranteId} - Usuário: {UserName}", 
                    id, restauranteId, userName);

                var categoria = await _context.Categorias
                    .Where(c => c.Id == id && c.RestauranteId == restauranteId)
                    .Include(c => c.Impressora)
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    _logger.LogWarning("Categoria {CategoriaId} não encontrada para restaurante {RestauranteId}", id, restauranteId);
                    return null;
                }

                return ConvertToDto(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria {CategoriaId} para restaurante {RestauranteId}", 
                    id, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<CategoriaDto> CreateCategoriaAsync(CategoriaCriacaoDto categoriaDto)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Criando categoria '{Nome}' para restaurante {RestauranteId} - Usuário: {UserName}", 
                    categoriaDto.Nome, restauranteId, userName);

                // Verificar se já existe categoria com mesmo nome
                var existeCategoria = await ExisteCategoriaPorNomeAsync(categoriaDto.Nome);
                if (existeCategoria)
                {
                    var mensagem = $"Já existe uma categoria com o nome '{categoriaDto.Nome}' neste restaurante";
                    _logger.LogWarning("Tentativa de criar categoria duplicada: {Nome} - Restaurante: {RestauranteId}", 
                        categoriaDto.Nome, restauranteId);
                    throw new InvalidOperationException(mensagem);
                }

                // Validar impressora se informada
                if (categoriaDto.ImpressoraId.HasValue)
                {
                    var impressoraExiste = await _context.Impressoras
                        .AnyAsync(i => i.Id == categoriaDto.ImpressoraId.Value);
                    
                    if (!impressoraExiste)
                    {
                        var mensagem = $"Impressora com ID {categoriaDto.ImpressoraId} não encontrada";
                        _logger.LogWarning("Tentativa de usar impressora inválida: {ImpressoraId} - Restaurante: {RestauranteId}", 
                            categoriaDto.ImpressoraId, restauranteId);
                        throw new InvalidOperationException(mensagem);
                    }
                }

                var categoria = new Categoria
                {
                    Nome = categoriaDto.Nome.Trim(),
                    Descricao = categoriaDto.Descricao?.Trim(),
                    Ativo = true,
                    DataCriacao = DateTime.Now,
                    RestauranteId = restauranteId,
                    ImpressoraId = categoriaDto.ImpressoraId
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoria criada com sucesso - ID: {CategoriaId}, Nome: '{Nome}', Restaurante: {RestauranteId}, Usuário: {UserName}", 
                    categoria.Id, categoria.Nome, restauranteId, userName);

                // Recarregar com includes para retorno
                var categoriaCompleta = await _context.Categorias
                    .Where(c => c.Id == categoria.Id)
                    .Include(c => c.Impressora)
                    .FirstAsync();

                return ConvertToDto(categoriaCompleta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria '{Nome}' para restaurante {RestauranteId}", 
                    categoriaDto.Nome, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<CategoriaDto?> UpdateCategoriaAsync(int id, CategoriaAtualizacaoDto categoriaDto)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Atualizando categoria {CategoriaId} para restaurante {RestauranteId} - Usuário: {UserName}", 
                    id, restauranteId, userName);

                var categoria = await _context.Categorias
                    .Where(c => c.Id == id && c.RestauranteId == restauranteId)
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    _logger.LogWarning("Categoria {CategoriaId} não encontrada para atualização - Restaurante: {RestauranteId}", 
                        id, restauranteId);
                    return null;
                }

                // Verificar se o novo nome já existe (excluindo a categoria atual)
                var existeCategoria = await ExisteCategoriaPorNomeAsync(categoriaDto.Nome, id);
                if (existeCategoria)
                {
                    var mensagem = $"Já existe uma categoria com o nome '{categoriaDto.Nome}' neste restaurante";
                    _logger.LogWarning("Tentativa de atualizar para nome duplicado: {Nome} - Categoria: {CategoriaId}, Restaurante: {RestauranteId}", 
                        categoriaDto.Nome, id, restauranteId);
                    throw new InvalidOperationException(mensagem);
                }

                // Validar impressora se informada
                if (categoriaDto.ImpressoraId.HasValue)
                {
                    var impressoraExiste = await _context.Impressoras
                        .AnyAsync(i => i.Id == categoriaDto.ImpressoraId.Value);
                    
                    if (!impressoraExiste)
                    {
                        var mensagem = $"Impressora com ID {categoriaDto.ImpressoraId} não encontrada";
                        _logger.LogWarning("Tentativa de usar impressora inválida na atualização: {ImpressoraId} - Categoria: {CategoriaId}, Restaurante: {RestauranteId}", 
                            categoriaDto.ImpressoraId, id, restauranteId);
                        throw new InvalidOperationException(mensagem);
                    }
                }

                // Guardar valores antigos para log
                var nomeAntigo = categoria.Nome;
                var ativoAntigo = categoria.Ativo;

                categoria.Nome = categoriaDto.Nome.Trim();
                categoria.Descricao = categoriaDto.Descricao?.Trim();
                categoria.Ativo = categoriaDto.Ativo;
                categoria.ImpressoraId = categoriaDto.ImpressoraId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoria atualizada - ID: {CategoriaId}, Nome: '{NomeAntigo}' -> '{NomeNovo}', Ativo: {AtivoAntigo} -> {AtivoNovo}, Restaurante: {RestauranteId}, Usuário: {UserName}", 
                    id, nomeAntigo, categoria.Nome, ativoAntigo, categoria.Ativo, restauranteId, userName);

                // Recarregar com includes para retorno
                var categoriaCompleta = await _context.Categorias
                    .Where(c => c.Id == categoria.Id)
                    .Include(c => c.Impressora)
                    .FirstAsync();

                return ConvertToDto(categoriaCompleta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria {CategoriaId} para restaurante {RestauranteId}", 
                    id, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Excluindo categoria {CategoriaId} para restaurante {RestauranteId} - Usuário: {UserName}", 
                    id, restauranteId, userName);

                var categoria = await _context.Categorias
                    .Where(c => c.Id == id && c.RestauranteId == restauranteId)
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    _logger.LogWarning("Categoria {CategoriaId} não encontrada para exclusão - Restaurante: {RestauranteId}", 
                        id, restauranteId);
                    return false;
                }

                // Verificar se há produtos usando esta categoria
                var produtosComCategoria = await _context.Produtos
                    .Where(p => p.CategoriaId == id)
                    .CountAsync();

                if (produtosComCategoria > 0)
                {
                    var mensagem = $"Não é possível excluir a categoria '{categoria.Nome}' pois há {produtosComCategoria} produto(s) associado(s)";
                    _logger.LogWarning("Tentativa de excluir categoria com produtos - ID: {CategoriaId}, Produtos: {Count}, Restaurante: {RestauranteId}", 
                        id, produtosComCategoria, restauranteId);
                    throw new InvalidOperationException(mensagem);
                }

                var nomeCategoria = categoria.Nome;
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Categoria excluída com sucesso - ID: {CategoriaId}, Nome: '{Nome}', Restaurante: {RestauranteId}, Usuário: {UserName}", 
                    id, nomeCategoria, restauranteId, userName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria {CategoriaId} para restaurante {RestauranteId}", 
                    id, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<(IEnumerable<CategoriaDto> Categorias, int Total)> BuscarCategoriasAsync(string? termo, int pagina = 1, int tamanhoPagina = 10)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var userName = _currentUserService.GetUserName();

                _logger.LogInformation("Buscando categorias com termo '{Termo}' para restaurante {RestauranteId} - Página {Pagina}, Tamanho {Tamanho} - Usuário: {UserName}", 
                    termo ?? "vazio", restauranteId, pagina, tamanhoPagina, userName);

                var query = _context.Categorias
                    .Where(c => c.RestauranteId == restauranteId);

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    var termoLower = termo.ToLower();
                    query = query.Where(c => 
                        c.Nome.ToLower().Contains(termoLower) || 
                        (c.Descricao != null && c.Descricao.ToLower().Contains(termoLower)));
                }

                query = query.Include(c => c.Impressora);

                var total = await query.CountAsync();

                var categorias = await query
                    .OrderBy(c => c.Nome)
                    .Skip((pagina - 1) * tamanhoPagina)
                    .Take(tamanhoPagina)
                    .Select(c => ConvertToDto(c))
                    .ToListAsync();

                _logger.LogInformation("Encontradas {Count} categorias de {Total} total com termo '{Termo}' para restaurante {RestauranteId}", 
                    categorias.Count(), total, termo ?? "vazio", restauranteId);

                return (categorias, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias com termo '{Termo}' para restaurante {RestauranteId}", 
                    termo, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<IEnumerable<CategoriaDto>> GetCategoriasAtivasAsync()
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();

                _logger.LogInformation("Buscando categorias ativas para restaurante {RestauranteId}", restauranteId);

                var categorias = await _context.Categorias
                    .Where(c => c.RestauranteId == restauranteId && c.Ativo)
                    .Include(c => c.Impressora)
                    .OrderBy(c => c.Nome)
                    .Select(c => ConvertToDto(c))
                    .ToListAsync();

                _logger.LogInformation("Encontradas {Count} categorias ativas para restaurante {RestauranteId}", 
                    categorias.Count(), restauranteId);

                return categorias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias ativas para restaurante {RestauranteId}", 
                    _currentUserService.GetRestauranteId());
                throw;
            }
        }

        public async Task<bool> ExisteCategoriaPorNomeAsync(string nome, int? idExcluir = null)
        {
            try
            {
                var restauranteId = _currentUserService.GetRestauranteId();
                var nomeNormalizado = nome.Trim().ToLower();

                var query = _context.Categorias
                    .Where(c => c.RestauranteId == restauranteId && c.Nome.ToLower() == nomeNormalizado);

                if (idExcluir.HasValue)
                {
                    query = query.Where(c => c.Id != idExcluir.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência de categoria por nome '{Nome}' para restaurante {RestauranteId}", 
                    nome, _currentUserService.GetRestauranteId());
                throw;
            }
        }

        private static CategoriaDto ConvertToDto(Categoria categoria)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao,
                Ativo = categoria.Ativo,
                DataCriacao = categoria.DataCriacao,
                ImpressoraId = categoria.ImpressoraId,
                ImpressoraNome = categoria.Impressora?.Nome
            };
        }
    }
}