using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.Cryptography.DTO
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
        public int IDUsuario { get; set; }
    }

    public class KMEntry
    {
        public Guid UUID { get; set; }
        public int IDUsuario { get; set; }
        public string KM { get; set; }
    }
}
