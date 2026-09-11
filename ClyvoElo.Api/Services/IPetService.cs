using ClyvoElo.Api.Models;

namespace ClyvoElo.Api.Services;

public interface IPetService
{
    Task<IEnumerable<Pet>> ListarTodosAsync();
    Task<Pet?> BuscarPorIdAsync(int id);
    Task<Pet> CriarAsync(Pet pet);
    Task<bool> AtualizarPesoAsync(int id, decimal novoPeso);
    Task<bool> DeletarAsync(int id);
    Task<IEnumerable<Pet>> ListarPorTutorAsync(int idTutor);
}
