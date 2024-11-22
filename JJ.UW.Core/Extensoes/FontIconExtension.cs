using JJ.UW.Core.Enumerador;
using JJ.UW.Core.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JJ.UW.Core.Extensoes
{
    public static class FontIconExtension
    {
        public static void AtualizarIcone(this FontIcon ficon, eIconesGlyph icone, Brush cor)
        {
            var codigo = Imagem.ObterGlyph(icone);

            ficon.Glyph = codigo.Glyph;
            ficon.FontFamily = new FontFamily(codigo.FontFamily);
            ficon.Foreground = cor;
        }
    }
}
