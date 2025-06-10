using JJ.Net.CrossData_WinUI_3.DTO;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Interfaces
{
    public interface IConfiguracaoBancoDados
    {
        IDbConnection ConexaoAtiva { get; }
        TipoBancoDados TipoBanco { get; }
        Task InicializarAsync(ParametrosConfiguracao parametros);
        void Inicializar(ParametrosConfiguracao parametros);
    }
}
