﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using trofeoCazador.ServicioDelJuego;
//using trofeoCazador.ReferenciaServicio;



namespace trofeoCazador
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           
            InitializeComponent();
            MainFrame.NavigationService.Navigate(new Uri("Vistas/InicioSesion/XAMLInicioSesion.xaml", UriKind.Relative));
            
           /*  GestionCuentaServicioClient proxy = new GestionCuentaServicioClient();
            JugadorDataContract jugador = new JugadorDataContract();
            //CuentaDataContract cuenta = new CuentaDataContract();
            jugador.NombreUsuario = "Juan";
            jugador.NumeroFotoPerfil = 1;
            jugador.ContraseniaHash = "juandos";
            jugador.Correo = "juan@gmail.com";
            proxy.AgregarJugador(jugador);
           */

            
    
        }
    }

        
    }

