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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace trofeoCazador.Vistas.Amigos
{
    /// <summary>
    /// Lógica de interacción para XAMLAmigos.xaml
    /// </summary>
    public partial class XAMLAmigos : Page
    {
        public XAMLAmigos()
        {
            InitializeComponent();
        }

        public void BtnCloseFriendsMenu_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("este");
        }


        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
