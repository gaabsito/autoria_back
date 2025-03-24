using GymAPI.Models;
using GymAPI.Repositories;

namespace GymAPI.Services
{
    public class MedidaCorporalService : IMedidaCorporalService
    {
        private readonly IMedidaCorporalRepository _repository;

        public MedidaCorporalService(IMedidaCorporalRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MedidaCorporal>> GetAllByUsuarioAsync(int usuarioId)
        {
            return await _repository.GetAllByUsuarioAsync(usuarioId);
        }

        public async Task<MedidaCorporal?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MedidaCorporal?> GetLatestByUsuarioAsync(int usuarioId)
        {
            return await _repository.GetLatestByUsuarioAsync(usuarioId);
        }

        public async Task<int> AddAsync(MedidaCorporal medida)
        {
            return await _repository.AddAsync(medida);
        }

        public async Task UpdateAsync(MedidaCorporal medida)
        {
            await _repository.UpdateAsync(medida);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}