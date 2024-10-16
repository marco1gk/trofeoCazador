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

namespace trofeoCazador.Vistas.RegistroUsuario
{
    /// <summary>
    /// Interaction logic for XAMLRegistroUsuario.xaml
    /// </summary>
    public partial class XAMLRegistroUsuario : Page
    {
        public XAMLRegistroUsuario()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void ImagenCLicAtras(object sender, MouseButtonEventArgs e)
        {
                NavigationService.GoBack();
        }

        private void DpFechaNacimiento_Cargado(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {//veoveo
                datePicker.DisplayDateEnd = new DateTime(DateTime.Today.Year, 12, 31);
            }
        }

    }
}
