using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.UW.Core.Extensoes
{
    public static class ObjectExtension
    {
        public static bool EhNulo(this object valor)
        {
            return (valor == null);
        }
    }
}
