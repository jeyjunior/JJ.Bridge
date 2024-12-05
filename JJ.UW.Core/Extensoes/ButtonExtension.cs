using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace JJ.UW.Core.Extensoes
{
    public static class ButtonExtension
    {
        public static bool ObterTagBoleanaInt32(this Button btn)
        {
            if (btn == null) 
                return false;

            if(btn.Tag == null)
            {
                btn.Tag = 0;
                return false;
            }

            try
            {
                var tag = (int)btn.Tag;

                btn.Tag = (tag == 0) ? 1 : 0;
            }
            catch 
            {
                btn.Tag = 0;
            }

            return ((int)btn.Tag == 1);
        }
    }
}
