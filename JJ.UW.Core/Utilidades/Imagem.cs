using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using JJ.UW.Core.Atributos;
using JJ.UW.Core.Enumerador;

namespace JJ.UW.Core.Utilidades
{
    public static class Imagem
    {
        public static FontIcon Obter(eIconesGlyph eIcones)
        {
            var valor = ObterGlyph(eIcones);
            
            return new FontIcon() { Glyph = valor.Glyph, FontFamily = new FontFamily(valor.FontFamily) };
        }

        public static CodigoGlyph ObterGlyph(eIconesGlyph eIcones) 
        {
            var campo = eIcones.GetType().GetField(eIcones.ToString());

            return (CodigoGlyph)Attribute.GetCustomAttribute(campo, typeof(CodigoGlyph));
        }
    }
}
