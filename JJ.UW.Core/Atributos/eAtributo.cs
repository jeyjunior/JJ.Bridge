using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.UW.Core.Atributos
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CodigoGlyph : Attribute
    {
        public string Glyph { get; }
        public string FontFamily { get; }

        public CodigoGlyph(string glyph, string fontFamily)
        {
            Glyph = glyph;
            FontFamily = fontFamily;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class Fonte : Attribute
    {
        public string FontFamily { get; }

        public Fonte(string fontFamily)
        {
            FontFamily = fontFamily;
        }
    }
}
