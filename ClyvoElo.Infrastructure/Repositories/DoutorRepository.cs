using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class DoutorRepository : IDoutorRepository
{
    private readonly ClyvoEloContext _context;

    public DoutorRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<Doutor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Doutores
            .Include(d => d.Clinica)
            .FirstOrDefaultAsync(d => d.IdDoutor == id, cancellationToken);
    }

    public Task<Doutor?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Doutores.FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
    }

    public Task<ResultadoPaginado<Doutor>> ListarAsync(DoutorFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Doutores.AsNoTracking().Include(d => d.Clinica).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToUpper();
            query = query.Where(d => d.Nome.ToUpper().Contains(nome));
        }

        if (filtro.IdClinica.HasValue)
        {
            query = query.Where(d => d.IdClinica == filtro.IdClinica.Value);
        }

        query = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "crmv" => query.OrdenarPor(d => d.Crmv, filtro.EhDescendente()),
            "id" => query.OrdenarPor(d => d.IdDoutor, filtro.EhDescendente()),
            _ => query.OrdenarPor(d => d.Nome, filtro.EhDescendente()),
        };

        return query.PaginarAsync(filtro, cancellationToken);
    }
}
