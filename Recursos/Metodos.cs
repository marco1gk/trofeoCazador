using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using trofeoCazador.ServicioDelJuego;

namespace trofeoCazador.Recursos
{
    public static class Metodos
    {
        public static int ObtenerIdJugador()
        {
            SingletonSesion sesion = SingletonSesion.Instancia;
            return sesion.JugadorId;
        }

        public static JugadorDataContract ObtenerDatosJugador(int idJugador)
        {
            if (EsJugadorInvitado(idJugador)) // Método para identificar si es un invitado
            {
                return CrearJugadorInvitado();
            }

            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            return proxy.ObtenerJugador(idJugador);
        }

        // Método para identificar jugadores invitados
        private static bool EsJugadorInvitado(int idJugador)
        {
            return idJugador <= 0; // Define un criterio claro; por ejemplo, IDs negativos o 0 para invitados
        }

        // Método para crear datos ficticios para invitados
        private static JugadorDataContract CrearJugadorInvitado()
        {
            return new JugadorDataContract
            {
                JugadorId = -1, // ID ficticio
                NombreUsuario = "Invitado", // Nombre genérico
                EsInvitado = true, // Marca como invitado
                NumeroFotoPerfil = 2 // Imagen de perfil predeterminada
            };
        }


        public static bool ValidarEntradaVacia(string cadena)
        {
            return !string.IsNullOrWhiteSpace(cadena);
        }

        public static bool ValidarEntradaIgual(string valorActual, string valorNuevo)
        {
            return !string.Equals(valorActual?.Trim(), valorNuevo?.Trim(), StringComparison.Ordinal);
        }

        public static bool ValidarLongitudDeEntrada(string texto, int longitudMaxima)
        {
            return texto.Length <= longitudMaxima;
        }

        public static GestionCuentaServicioClient EstablecerConexionServidor()
        {
            return new GestionCuentaServicioClient();
        }
        public static void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(mensaje);
        }
    }
}
