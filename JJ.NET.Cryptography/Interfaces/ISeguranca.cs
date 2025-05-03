using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using JJ.NET.Cryptography.DTO;

namespace JJ.NET.Cryptography.Interfaces
{
    public interface ISeguranca
    {
        string GerarChavePrincipal();
        string RegistrarChavePrincipal(string km);
        CriptografiaResult Criptografar(string valor);
        string Descriptografar(DescriptografiaRequest request);
    }
}
