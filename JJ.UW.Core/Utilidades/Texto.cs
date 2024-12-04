using JJ.UW.Core.Atributos;
using JJ.UW.Core.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JJ.UW.Core.Utilidades
{
    public static class Texto
    {
        public static Fonte ObterFonte(eFontFamily eFontFamily)
        {
            var campo = eFontFamily.GetType().GetField(eFontFamily.ToString());

            return (Fonte)Attribute.GetCustomAttribute(campo, typeof(Fonte));
        }

        public static FontFamily ObterFontFamily(eFontFamily eFontFamily)
        {
            var valor = ObterFonte(eFontFamily);

            return new FontFamily(valor.FontFamily.ToString());
        }

        public static string ObterFonteString(eFontFamily eFontFamily)
        {
            return ObterFonte(eFontFamily).FontFamily.ToString();
        }
    }
}
