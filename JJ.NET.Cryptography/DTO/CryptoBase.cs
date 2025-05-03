using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.NET.Cryptography.DTO
{
    public abstract class CryptoBase
    {
        public string ValorCriptografado { get; set; }
        public string Salt { get; set; }
    }
    public class CriptografiaResult : CryptoBase
    {
    }

    public class DescriptografiaRequest : CryptoBase
    {

    }
}
