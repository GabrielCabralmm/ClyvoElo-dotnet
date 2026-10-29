using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class PetRepository : IPetRepository
{
    private readonly ClyvoEloContext _context;

    public PetRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<Pet?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Pets
            .Include(p => p.Tutor)
            .FirstOrDefaultAsync(p => p.IdPet == id, cancellationToken);
    }

    public Task<ResultadoPaginado<Pet>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Pets.AsNoTracking().Include(p => p.Tutor).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToUpper();
            query = query.Where(p => p.Nome.ToUpper().Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Especie))
        {
            var especie = filtro.Especie.Trim().ToUpper();
            query = query.Where(p => p.Especie.ToUpper() == especie);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Raca))
        {
            var raca = filtro.Raca.Trim().ToUpper();
            query = query.Where(p => p.Raca != null && p.Raca.ToUpper().Contains(raca));
        }

        if (filtro.IdTutor.HasValue)
        {
            query = query.Where(p => p.IdTutor == filtro.IdTutor.Value);
        }

        query = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "nome" => query.OrdenarPor(p => p.Nome, filtro.EhDescendente()),
            "especie" => query.OrdenarPor(p => p.Especie, filtro.EhDescendente()),
            "peso" => query.OrdenarPor(p => p.PesoKg, filtro.EhDescendente()),
            "datanascimento" => query.OrdenarPor(p => p.DataNascimento, filtro.EhDescendente()),
            _ => query.OrdenarPor(p => p.IdPet, filtro.EhDescendente()),
        };

        return query.PaginarAsync(filtro, cancellationToken);
    }

    public async Task AdicionarAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(pet).State == EntityState.Detached)
        {
            _context.Pets.Update(pet);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
