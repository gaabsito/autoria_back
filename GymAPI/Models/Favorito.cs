namespace GymAPI.Models
{
    public class Favorito
    {
        public int UsuarioID { get; set; }
        public int EntrenamientoID { get; set; }
        public DateTime FechaAgregado { get; set; } = DateTime.Now;
        
        // Navegación (opcional)
        public Entrenamiento? Entrenamiento { get; set; }
        public Usuario? Usuario { get; set; }
    }
}