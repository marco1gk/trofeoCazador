//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace trofeoCazador
{
    using System;
    using System.Collections.Generic;
    
    public partial class Baneo
    {
        public int idBaneo { get; set; }
        public int idJugador { get; set; }
        public string motivo { get; set; }
        public System.DateTime fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFin { get; set; }
    
        public virtual Jugador Jugador { get; set; }
    }
}
