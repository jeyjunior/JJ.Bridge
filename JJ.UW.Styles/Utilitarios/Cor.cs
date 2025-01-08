using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using JJ.UW.Styles.Enumerador;

namespace JJ.UW.Styles.Utilitarios
{
    public static class Cor
    {
        public static Brush ObterCor(Cores eCores)
        {
            Brush brush = null;

            try
            {
                brush = (Brush)Application.Current.Resources[eCores.ToString()];
            }
            catch
            {
                brush = (Brush)Application.Current.Resources[Cores.Branco.ToString()];
            }

            return brush;
        }

        public static SolidColorBrush ObterCorSolid(Cores eCores)
        {
            SolidColorBrush solidBrush = null;

            try
            {
                solidBrush = (SolidColorBrush)Application.Current.Resources[eCores.ToString()];
            }
            catch
            {
                solidBrush = (SolidColorBrush)Application.Current.Resources[Cores.Branco.ToString()];
            }

            return solidBrush;
        }

        public static string ObterCorHexadecimal(Cores eCores)
        {
            string corHex = "#FFFFFF";

            try
            {
                var brush = ObterCor(eCores);

                if (brush is SolidColorBrush solidColorBrush)
                {
                    var color = solidColorBrush.Color;
                    corHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                }
            }
            catch
            {
                corHex = "#FFFFFF";
            }

            return corHex;
        }
    }
}
