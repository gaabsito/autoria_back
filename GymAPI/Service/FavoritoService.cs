using GymAPI.Models;
using GymAPI.Repositories;

namespace GymAPI.Services
{
    public class FavoritoService : IFavoritoService
    {
        private readonly IFavoritoRepository _repository;

        public FavoritoService(IFavoritoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Favorito>> GetByUsuarioAsync(int usuarioId)
        {
            return await _repository.GetByUsuarioAsync(usuarioId);
        }

        public async Task<bool> ExistsAsync(int usuarioId, int entrenamientoId)
        {
            return await _repository.ExistsAsync(usuarioId, entrenamientoId);
        }

        public async Task AddAsync(Favorito favorito)
        {
            await _repository.AddAsync(favorito);
        }

        public async Task RemoveAsync(int usuarioId, int entrenamientoId)
        {
            await _repository.RemoveAsync(usuarioId, entrenamientoId);
        }
    }
}