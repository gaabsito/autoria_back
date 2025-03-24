namespace GymAPI.Models
{
     public class MedidaCorporal
    {
        public int MedidaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public float Peso { get; set; }
        public float? Altura { get; set; }
    }
}