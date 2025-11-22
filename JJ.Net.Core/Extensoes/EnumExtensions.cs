using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.Core.Extensoes
{
    public static class EnumExtensions
    {
        public static string ObterDescricao(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return "";

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null) 
                return "";

            return attribute.Description;
        }
    }
}
