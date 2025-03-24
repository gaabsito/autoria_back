using System;

namespace GymAPI.DTOs
{
    public class MedidaCorporalDTO
    {
        public int MedidaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaRegistro { get; set; }
        public float Peso { get; set; }
        public float? Altura { get; set; }
        public float? IMC { get; set; }
    }

    public class MedidaCorporalCreateDTO
    {
        public float Peso { get; set; }
        public float? Altura { get; set; }
    }

    public class MedidaCorporalUpdateDTO
    {
        public float? Peso { get; set; }
        public float? Altura { get; set; }
    }
}