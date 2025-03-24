using GymAPI.Models;

namespace GymAPI.Repositories
{
    public interface IFavoritoRepository
    {
        Task<List<Favorito>> GetByUsuarioAsync(int usuarioId);
        Task<bool> ExistsAsync(int usuarioId, int entrenamientoId);
        Task AddAsync(Favorito favorito);
        Task RemoveAsync(int usuarioId, int entrenamientoId);
    }
}