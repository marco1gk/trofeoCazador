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
        private Random random;
        private string[] carasDado;
        public event Action<int> DadoLanzado;
        public Dado(Image dadoImagenControl)
        {
            dadoImagen = dadoImagenControl;
            random = new Random();
            // Rutas de las caras del dado
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
            animacionTimer = new DispatcherTimer();
            animacionTimer.Interval = TimeSpan.FromMilliseconds(300); // Cambia cada 300 ms para más visibilidad
            animacionTimer.Tick += CambiarCaraDado;
        }
        // Maneja el clic en el dado
        public void LanzarDado()
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
                    Task.Delay(4000).ContinueWith(t => DetenerAnimacion()); // Duración total de 4 segundos
                });
            }
        }
        private void CambiarCaraDado(object sender, EventArgs e)
        {
            int indiceAleatorio = random.Next(carasDado.Length);
            // Animación de opacidad más rápida (200 ms) para que sea visible pero sutil
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, a) =>
            {
                dadoImagen.Source = new BitmapImage(new Uri(carasDado[indiceAleatorio], UriKind.Relative));
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                dadoImagen.BeginAnimation(Image.OpacityProperty, fadeIn);
            };
            // Iniciar el efecto de desvanecimiento
            dadoImagen.BeginAnimation(Image.OpacityProperty, fadeOut);
        }
        private void DetenerAnimacion()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                animacionTimer.Stop();
                int caraFinal = random.Next(carasDado.Length);
                dadoImagen.Source = new BitmapImage(new Uri(carasDado[caraFinal], UriKind.Relative));
                // Emitir el evento con el resultado del dado (1 a 6)
                DadoLanzado?.Invoke(caraFinal + 1);
                // Mostrar la cara final con un desvanecimiento breve
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                dadoImagen.BeginAnimation(Image.OpacityProperty, fadeIn);
            });
        }
    }
}
