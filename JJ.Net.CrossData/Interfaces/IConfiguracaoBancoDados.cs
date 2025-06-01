using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Enumerador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Interfaces
{
    public interface IConfiguracaoBancoDados
    {
        IDbConnection ConexaoAtiva { get; }
        TipoBancoDados TipoBanco { get; }
        Task InicializarAsync(ParametrosConfiguracao parametros);
    }
}
