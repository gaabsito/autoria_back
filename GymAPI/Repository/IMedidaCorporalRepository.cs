using GymAPI.Models;

namespace GymAPI.Repositories
{
    public interface IMedidaCorporalRepository
    {
        Task<List<MedidaCorporal>> GetAllByUsuarioAsync(int usuarioId);
        Task<MedidaCorporal?> GetByIdAsync(int id);
        Task<int> AddAsync(MedidaCorporal medida);
        Task UpdateAsync(MedidaCorporal medida);
        Task DeleteAsync(int id);
        Task<MedidaCorporal?> GetLatestByUsuarioAsync(int usuarioId);
    }
}