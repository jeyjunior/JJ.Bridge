using JJ.Standard.Data.Enumerador;
using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.Standard.Data.DTO
{
    public class ConfigRequest
    {
        public Conexao Conexao { get; set; }
        public string NomeAplicacao { get; set; } = "JeyJunior";
        public string CaminhoDestino { get; set; }

        public string Erro { get; set; }
    }
}
