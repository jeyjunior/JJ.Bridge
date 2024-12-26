using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using JJ.UW.Core.Atributos;
using JJ.UW.Core.Enumerador;

namespace JJ.UW.Core.Utilidades
{
    public static class Texto
    {
        public static Fonte ObterFonte(FamiliaFonte eFontFamily)
        {
            var campo = eFontFamily.GetType().GetField(eFontFamily.ToString());

            return (Fonte)Attribute.GetCustomAttribute(campo, typeof(Fonte));
        }

        public static FontFamily ObterFontFamily(FamiliaFonte eFontFamily)
        {
            var valor = ObterFonte(eFontFamily);

            return new FontFamily(valor.FontFamily.ToString());
        }

        public static string ObterFonteString(FamiliaFonte eFontFamily)
        {
            return ObterFonte(eFontFamily).FontFamily.ToString();
        }
    }
}
