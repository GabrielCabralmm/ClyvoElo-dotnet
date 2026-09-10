using ClyvoElo.Api.Data;
using ClyvoElo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Api.Services;

public class PetService : IPetService
{
    private readonly ClyvoEloContext _context;

    public PetService(ClyvoEloContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pet>> ListarTodosAsync()
    {
        return await _context.Pets.Include(p => p.Tutor).ToListAsync();
    }

    public async Task<Pet?> BuscarPorIdAsync(int id)
    {
        return await _context.Pets.Include(p => p.Tutor)
            .FirstOrDefaultAsync(p => p.IdPet == id);
    }

    public async Task<Pet> CriarAsync(Pet pet)
    {
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
        return pet;
    }

    public async Task<bool> AtualizarPesoAsync(int id, decimal novoPeso)
    {
        if (novoPeso <= 0)
        {
            throw new ArgumentException("O peso deve ser maior que zero.");
        }

        var pet = await _context.Pets.FindAsync(id);
        if (pet is null)
        {
            return false;
        }

        pet.PesoKg = novoPeso;
        await _context.SaveChangesAsync();
        return true;
    }
}
