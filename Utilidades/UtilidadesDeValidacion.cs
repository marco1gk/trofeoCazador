using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace trofeoCazador.Utilidades
{
    public static class UtilidadesDeValidacion
    {
        private const string NOMBRE_USUARIO_VALIDO = "^[a-zA-Z0-9_]+$";
        private const string CORREO_VALIDO = "^(?=.{5,100}$)[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
        private const string CONTRASENA_VALIDA = "^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[\\W_])[A-Za-z\\d\\W_]{12,32}$";
        private const string SIMBOLOS_VALIDOS = "^(?=.*[\\W_])";
        private const string MAYUSCULAS_VALIDAS = "^(?=.*[A-Z])";
        private const string MINUSCULAS_VALIDAS = "^(?=.*[a-z])";
        private const string NUMEROS_VALIDOS = "^(?=.*\\d)";


        public static bool EsNombreUsuarioValido(string nombreUsuario)
        {
            Regex regex = new Regex(NOMBRE_USUARIO_VALIDO);

            return regex.IsMatch(nombreUsuario.Trim());
        }

        public static bool EsCorreoValido(string correo)
        {
            Regex regex = new Regex(CORREO_VALIDO);

            return regex.IsMatch(correo.Trim());
        }

        public static bool EsContrasenaValida(string contrasena)
        {
            Regex regex = new Regex(CONTRASENA_VALIDA);

            return regex.IsMatch(contrasena.Trim());
        }

        public static bool EsSimboloValido(string contrasena)
        {
            Regex regex = new Regex(SIMBOLOS_VALIDOS);

            return regex.IsMatch(contrasena.Trim());
        }

        public static bool EsMayusculaValida(string contrasena)
        {
            Regex regex = new Regex(MAYUSCULAS_VALIDAS);

            return regex.IsMatch(contrasena.Trim());
        }

        public static bool EsMinusculaValida(string contrasena)
        {
            Regex regex = new Regex(MINUSCULAS_VALIDAS);

            return regex.IsMatch(contrasena.Trim());
        }

        public static bool EsNumeroValido(string contrasena)
        {
            Regex regex = new Regex(NUMEROS_VALIDOS);

            return regex.IsMatch(contrasena.Trim());
        }
    }
}
