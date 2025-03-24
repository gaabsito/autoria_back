using Microsoft.Data.SqlClient;
using GymAPI.Models;

namespace GymAPI.Repositories
{
    public class FavoritoRepository : IFavoritoRepository
    {
        private readonly string _connectionString;

        public FavoritoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Favorito>> GetByUsuarioAsync(int usuarioId)
        {
            var favoritos = new List<Favorito>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT ef.UsuarioID, ef.EntrenamientoID, ef.FechaAgregado, 
                               e.Titulo, e.ImagenURL, e.Dificultad 
                               FROM EntrenamientosFavoritos ef
                               JOIN Entrenamientos e ON ef.EntrenamientoID = e.EntrenamientoID
                               WHERE ef.UsuarioID = @UsuarioID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var favorito = new Favorito
                            {
                                UsuarioID = reader.GetInt32(0),
                                EntrenamientoID = reader.GetInt32(1),
                                FechaAgregado = reader.GetDateTime(2),
                                Entrenamiento = new Entrenamiento
                                {
                                    EntrenamientoID = reader.GetInt32(1),
                                    Titulo = reader.GetString(3),
                                    ImagenURL = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    Dificultad = reader.GetString(5)
                                }
                            };
                            favoritos.Add(favorito);
                        }
                    }
                }
            }
            return favoritos;
        }

        public async Task<bool> ExistsAsync(int usuarioId, int entrenamientoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT COUNT(1) FROM EntrenamientosFavoritos 
                               WHERE UsuarioID = @UsuarioID AND EntrenamientoID = @EntrenamientoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                    command.Parameters.AddWithValue("@EntrenamientoID", entrenamientoId);
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public async Task AddAsync(Favorito favorito)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO EntrenamientosFavoritos (UsuarioID, EntrenamientoID, FechaAgregado) 
                               VALUES (@UsuarioID, @EntrenamientoID, @FechaAgregado)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", favorito.UsuarioID);
                    command.Parameters.AddWithValue("@EntrenamientoID", favorito.EntrenamientoID);
                    command.Parameters.AddWithValue("@FechaAgregado", favorito.FechaAgregado);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task RemoveAsync(int usuarioId, int entrenamientoId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"DELETE FROM EntrenamientosFavoritos 
                               WHERE UsuarioID = @UsuarioID AND EntrenamientoID = @EntrenamientoID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                    command.Parameters.AddWithValue("@EntrenamientoID", entrenamientoId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}