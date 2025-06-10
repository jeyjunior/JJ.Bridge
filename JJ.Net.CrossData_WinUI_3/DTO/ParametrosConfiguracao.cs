using JJ.Net.CrossData_WinUI_3.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.DTO
{
    public class ParametrosConfiguracao
    {
        public TipoBancoDados TipoBanco { get; set; }

        /// <summary>
        /// Para conexões que utilizam string de conexão
        /// </summary>
        public string? StringConexao { get; set; } // 
        public string NomeAplicacao { get; set; } = string.Empty;
        public string? CaminhoArquivoConfig { get; set; } // Opcional para outros bancos
    }
}
