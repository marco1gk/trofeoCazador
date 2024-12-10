using System;

namespace trofeoCazador
{
    public class SingletonSesion
    {
        private static SingletonSesion instanciaUnica = null;
        private static readonly object bloqueo = new object();

        private SingletonSesion() { }
        public static SingletonSesion Instancia
        {
            get
            {
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

        public int JugadorId { get; set; }
        public string NombreUsuario { get; set; }
        public int NumeroFotoPerfil { get; set; }
        public string Correo { get; set; }


        public string NuevoCorreo { get; set; } 
        public string CodigoVerificacion { get; set; } 

        public bool EstaActivo { get; set; } = false; 
        public void LimpiarSesion()
        {
            JugadorId = 0;
            NombreUsuario = null;
            NumeroFotoPerfil = 0;
            Correo = null;
            NuevoCorreo = null;
            CodigoVerificacion = null;
        }


    }


}
