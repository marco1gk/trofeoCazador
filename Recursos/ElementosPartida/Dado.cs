using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace trofeoCazador.Recursos.ElementosPartida
{
    public class Dado
    {
        private Image dadoImagen;
        private DispatcherTimer animacionTimer;
        private string[] carasDado;
        public event Action<int> DadoLanzado;
        public Dado(Image dadoImagenControl)
        {
            dadoImagen = dadoImagenControl;
            carasDado = new string[]
            {
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara1.png",
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara2.png",
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara3.png",
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara4.png",
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara5.png",
                "/Recursos/ElementosPartida/ImagenesPartida/Dado/Cara6.png"
            };
            // Configurar el temporizador de animación
            animacionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            animacionTimer.Tick += CambiarCaraDado;
        }
        public void LanzarDado(int resultadoDado)
        {
            if (animacionTimer.IsEnabled)
            {
                animacionTimer.Stop();
            }
            else
            {
                animacionTimer.Start();
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    animacionTimer.Interval = TimeSpan.FromMilliseconds(400);
                    Task.Delay(4000).ContinueWith(t => DetenerAnimacion(resultadoDado));
                });
            }
        }
        private void CambiarCaraDado(object sender, EventArgs e)
        {
            Random random = new Random();
            int indiceAleatorio = random.Next(carasDado.Length);
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, a) =>
            {
                dadoImagen.Source = new BitmapImage(new Uri(carasDado[indiceAleatorio], UriKind.Relative));
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                dadoImagen.BeginAnimation(Image.OpacityProperty, fadeIn);
            };
            dadoImagen.BeginAnimation(Image.OpacityProperty, fadeOut);
        }
        private async void DetenerAnimacion(int resultadoDado)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                animacionTimer.Stop();
                int caraFinal = resultadoDado - 1;
                Metodos.MostrarMensaje($"La cara final es: {caraFinal + 1}");
                dadoImagen.Source = new BitmapImage(new Uri(carasDado[caraFinal], UriKind.Relative));
                DadoLanzado?.Invoke(caraFinal + 1);
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                dadoImagen.BeginAnimation(Image.OpacityProperty, fadeIn);
            });
            await Task.Delay(1000);
        }
    }
}
