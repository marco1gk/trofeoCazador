using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace trofeoCazador
{
    public class SingletonSesion
    {
        // Campo estático privado que contendrá la única instancia de la clase.
        private static SingletonSesion instanciaUnica = null;

        // Objeto para controlar el acceso en entornos multihilo.
        private static readonly object bloqueo = new object();

       //preguntar si esta bien esto public static ServicioDelJuego.JugadorDataContract jugador {  get; set; }

        // Constructor privado para evitar que se creen instancias desde fuera de la clase.
        private SingletonSesion() { }

        // Propiedad pública para acceder a la única instancia del Singleton.
        public static SingletonSesion Instancia
        {
            get
            {
                // Doble verificación de bloqueo para garantizar que la instancia es única en un entorno multihilo.
                if (instanciaUnica == null)
                {
                    lock (bloqueo)
                    {
                        if (instanciaUnica == null)
                        {
                            instanciaUnica = new SingletonSesion();
                        }
                    }
                }
                return instanciaUnica;
            }
        }

        // Atributos del jugador
        public int JugadorId { get; set; }
        public string NombreUsuario { get; set; }
        public int NumeroFotoPerfil { get; set; }
        public string Correo { get; set; }
    }

}
