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
        string GerarChavePrincipal(int idUsuario);
        bool ValidarChavePrincipal(string chavePrincipal);
        CriptografiaResult Criptografar(string valor, int idUsuario);
        string Descriptografar(DescriptografiaRequest request);
    }
}
