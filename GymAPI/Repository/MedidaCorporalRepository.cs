using Microsoft.Data.SqlClient;
using GymAPI.Models;

namespace GymAPI.Repositories
{
    public class MedidaCorporalRepository : IMedidaCorporalRepository
    {
        private readonly string _connectionString;

        public MedidaCorporalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<MedidaCorporal>> GetAllByUsuarioAsync(int usuarioId)
        {
            var medidas = new List<MedidaCorporal>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT MedidaID, UsuarioID, FechaRegistro, Peso, Altura
                               FROM MedidasCorporales
                               WHERE UsuarioID = @UsuarioID
                               ORDER BY FechaRegistro DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            medidas.Add(new MedidaCorporal
                            {
                                MedidaID = reader.GetInt32(0),
                                UsuarioID = reader.GetInt32(1),
                                FechaRegistro = reader.GetDateTime(2),
                                Peso = (float)reader.GetDouble(3),
                                Altura = reader.IsDBNull(4) ? null : (float?)reader.GetDouble(4)
                            });
                        }
                    }
                }
            }
            return medidas;
        }

        public async Task<MedidaCorporal?> GetByIdAsync(int id)
        {
            MedidaCorporal? medida = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT MedidaID, UsuarioID, FechaRegistro, Peso, Altura
                               FROM MedidasCorporales
                               WHERE MedidaID = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            medida = new MedidaCorporal
                            {
                                MedidaID = reader.GetInt32(0),
                                UsuarioID = reader.GetInt32(1),
                                FechaRegistro = reader.GetDateTime(2),
                                Peso = (float)reader.GetDouble(3),
                                Altura = reader.IsDBNull(4) ? null : (float?)reader.GetDouble(4)
                            };
                        }
                    }
                }
            }
            return medida;
        }

        public async Task<MedidaCorporal?> GetLatestByUsuarioAsync(int usuarioId)
        {
            MedidaCorporal? medida = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT TOP 1 MedidaID, UsuarioID, FechaRegistro, Peso, Altura
                               FROM MedidasCorporales
                               WHERE UsuarioID = @UsuarioID
                               ORDER BY FechaRegistro DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            medida = new MedidaCorporal
                            {
                                MedidaID = reader.GetInt32(0),
                                UsuarioID = reader.GetInt32(1),
                                FechaRegistro = reader.GetDateTime(2),
                                Peso = (float)reader.GetDouble(3),
                                Altura = reader.IsDBNull(4) ? null : (float?)reader.GetDouble(4)
                            };
                        }
                    }
                }
            }
            return medida;
        }

        public async Task<int> AddAsync(MedidaCorporal medida)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO MedidasCorporales (UsuarioID, FechaRegistro, Peso, Altura) 
                                VALUES (@UsuarioID, @FechaRegistro, @Peso, @Altura);
                                SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", medida.UsuarioID);
                    command.Parameters.AddWithValue("@FechaRegistro", medida.FechaRegistro);
                    command.Parameters.AddWithValue("@Peso", medida.Peso);
                    command.Parameters.AddWithValue("@Altura", (object?)medida.Altura ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    medida.MedidaID = Convert.ToInt32(result);
                    return medida.MedidaID;
                }
            }
        }

        public async Task UpdateAsync(MedidaCorporal medida)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE MedidasCorporales 
                                SET Peso = @Peso, 
                                    Altura = @Altura 
                                WHERE MedidaID = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", medida.MedidaID);
                    command.Parameters.AddWithValue("@Peso", medida.Peso);
                    command.Parameters.AddWithValue("@Altura", (object?)medida.Altura ?? DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM MedidasCorporales WHERE MedidaID = @Id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}