﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using trofeoCazador.ServicioDelJuego;

namespace trofeoCazador.VentanasReutilizables
{
    /// <summary>
    /// Lógica de interacción para ValidarCodigoRegistro.xaml
    /// </summary>
    public partial class ValidarCodigoRegistro : Window
    {
        private JugadorDataContract _jugador;
        private string _codigoEnviado;

        public ValidarCodigoRegistro(JugadorDataContract jugador, string codigoEnviado)
        {
            InitializeComponent();
            _jugador = jugador;  // Guardamos la información del jugador
            _codigoEnviado = codigoEnviado; // Guardamos el código enviado
        }

        private void BtnEnviar(object sender, RoutedEventArgs e)
        {
            string codigoIngresado = tbxCode.Text.Trim();

            // Validar si los códigos coinciden
            GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            if (proxy.ValidarCodigo(codigoIngresado, _codigoEnviado))
            {
                // Si el código es correcto, creamos la cuenta
                proxy.AgregarJugador(_jugador);
                MessageBox.Show("Cuenta creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Cerramos la ventana de validación
            }
            else
            {
                MessageBox.Show("El código ingresado es incorrecto. Inténtalo nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Evento para cerrar la ventana
        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close(); // Cerrar la ventana cuando el usuario hace clic en la imagen de "cerrar"
        }
    }



}
