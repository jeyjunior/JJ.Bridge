using JJ.UW.Core.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace JJ.UW.Core.Enumerador
{
    public enum eIconesGlyph
    {
        [CodigoGlyph("\u25B2", "Calibri")]
        TrianguloCima,
        [CodigoGlyph("\u25B6", "Calibri")]
        TrianguloDireita,
        [CodigoGlyph("\u25BC", "Calibri")]
        TrianguloBaixo,
        [CodigoGlyph("\u25C0", "Calibri")]
        TrianguloEsquerda,

        [CodigoGlyph("\uF4CA", "Segoe UI")]
        Dashboard,
        [CodigoGlyph("\uF4B5", "Segoe UI")]
        Transacao,
    }
}
