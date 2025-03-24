namespace GymAPI.DTOs
{
    // DTO para obtener un favorito
    public class FavoritoDTO
    {
        public int UsuarioID { get; set; }
        public int EntrenamientoID { get; set; }
        public DateTime FechaAgregado { get; set; }
        
        // Opcional: datos del entrenamiento
        public string? TituloEntrenamiento { get; set; }
        public string? ImagenEntrenamiento { get; set; }
        public string? DificultadEntrenamiento { get; set; }
    }

    // DTO para añadir un favorito
    public class FavoritoCreateDTO
    {
        public int EntrenamientoID { get; set; }
    }
}