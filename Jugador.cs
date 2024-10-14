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
    
    public partial class Jugador
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Jugador()
        {
            this.Amistad = new HashSet<Amistad>();
            this.Amistad1 = new HashSet<Amistad>();
            this.Baneo = new HashSet<Baneo>();
            this.jugadorPartida = new HashSet<jugadorPartida>();
            this.Partida = new HashSet<Partida>();
        }
    
        public int idJugador { get; set; }
        public string usuario { get; set; }
        public System.DateTime fechaNacimiento { get; set; }
        public Nullable<int> partidasJugadas { get; set; }
        public Nullable<int> partidasGanadas { get; set; }
        public System.DateTime fechaRegistro { get; set; }
        public int idCuenta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Amistad> Amistad { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Amistad> Amistad1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Baneo> Baneo { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<jugadorPartida> jugadorPartida { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Partida> Partida { get; set; }
    }
}
