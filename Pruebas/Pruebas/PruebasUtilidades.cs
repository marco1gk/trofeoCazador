    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Threading;
    using trofeoCazador.Utilidades;
    using Xunit;


    namespace Pruebas.Pruebas
    {
        public class PruebasUtilidades
        {
        
        [Fact]
        public void NombredeUsuarioValidoExitoso()
        {
            bool resultadoActual = false;
            string texto = "Juan_10";

            resultadoActual = UtilidadesDeValidacion.EsNombreUsuarioValido(texto);

            Assert.True(resultadoActual);
        }

        [Fact]
        public void PruebaEsNombreUsuarioVacioInvalido()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsNombreUsuarioValido(texto);

            Assert.False(resultadoActual);
        }
        [Fact]
        public void PruebaEsNombreUsuarioInvalido()
        {
            bool resultadoActual = false;
            string texto = "Luis*-/";

            resultadoActual = UtilidadesDeValidacion.EsNombreUsuarioValido(texto);

            Assert.False(resultadoActual);
        }

        [Fact]
        public void PruebaEsCorreoValidoExitoso()
        {
            bool resultadoActual = false;
            string texto = "marco@gmail.com";

            resultadoActual = UtilidadesDeValidacion.EsCorreoValido(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void PruebaEsCorreoValidoVacioFallido()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsCorreoValido(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarCorreoElectrónicoInválido()
        {
            bool resultadoActual = false;
            string texto = "marco@gmail";

            resultadoActual = UtilidadesDeValidacion.EsCorreoValido(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarContraseñaVálidaExitosa()
        {
            bool resultadoActual = false;
            string texto = "M_10Rco1A/tOnL";

            resultadoActual = UtilidadesDeValidacion.EsContrasenaValida(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void ProbarContraseñaVacíaFallida()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsContrasenaValida(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarContraseñaInválida()
        {
            bool resultadoActual = false;
            string texto = "M_01arc12";

            resultadoActual = UtilidadesDeValidacion.EsContrasenaValida(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarSímboloVálidoExitosa()
        {
            bool resultadoActual = false;
            string texto = "S_01Arc1Z/tOnL";

            resultadoActual = UtilidadesDeValidacion.EsSimboloValido(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void ProbarSímboloVacioFallido()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsSimboloValido(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarSímboloInválido()
        {
            bool resultadoActual = false;
            string texto = "M10Axf1AtOnL11";

            resultadoActual = UtilidadesDeValidacion.EsSimboloValido(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMayúsculaVálidaExitosa()
        {
            bool resultadoActual = false;
            string texto = "B_01Arc1K/tOnL";

            resultadoActual = UtilidadesDeValidacion.EsMayusculaValida(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMayúsculaVacíaFallida()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsMayusculaValida(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMayúsculaInválida()
        {
            bool resultadoActual = false;
            string texto = "m_01arc1l/t0nl";

            resultadoActual = UtilidadesDeValidacion.EsMayusculaValida(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMinúsculaVálidaExitosa()
        {
            bool resultadoActual = false;
            string texto = "M_10Frc1O/tOnL";

            resultadoActual = UtilidadesDeValidacion.EsMinusculaValida(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMinúsculaVacíaFallida()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsMinusculaValida(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarLetraMinúsculaInválida()
        {
            bool resultadoActual = false;
            string texto = "M_108ARC1O/OLGL3";

            resultadoActual = UtilidadesDeValidacion.EsMinusculaValida(texto);
            Assert.False(resultadoActual);
        }
        [Fact]
        public void ProbarNúmeroVálidoExitosa()
        {
            bool resultadoActual = false;
            string texto = "Q_10Ell1L/tOnL";

            resultadoActual = UtilidadesDeValidacion.EsNumeroValido(texto);
            Assert.True(resultadoActual);
        }

        [Fact]
        public void ProbarNúmeroVacíoFallido()
        {
            bool resultadoActual = false;
            string texto = " ";

            resultadoActual = UtilidadesDeValidacion.EsNumeroValido(texto);
            Assert.False(resultadoActual);
        }

        [Fact]
        public void ProbarNúmeroInválido()
        {
            bool resultadoActual = false;
            string texto = "X_EaAssJH/tRnL";

            resultadoActual = UtilidadesDeValidacion.EsNumeroValido(texto);
            Assert.False(resultadoActual);
        }




    }
}
