using JJ.Net.Cryptography.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.Cryptography.Interfaces
{
    public interface ISeguranca
    {
        string GerarChavePrincipal(int idUsuario);
        bool ValidarChavePrincipal(string chavePrincipal);
        CriptografiaResult Criptografar(string valor, int idUsuario);
        string Descriptografar(DescriptografiaRequest request);
    }
}
