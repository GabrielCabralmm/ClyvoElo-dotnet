using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly ClyvoEloContext _context;

    public TutorRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<Tutor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Tutores.FirstOrDefaultAsync(t => t.IdTutor == id, cancellationToken);
    }

    public Task<Tutor?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Tutores.FirstOrDefaultAsync(t => t.Email == email, cancellationToken);
    }

    public Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Tutores.AnyAsync(t => t.Email == email, cancellationToken);
    }

    public async Task AdicionarAsync(Tutor tutor, CancellationToken cancellationToken = default)
    {
        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
