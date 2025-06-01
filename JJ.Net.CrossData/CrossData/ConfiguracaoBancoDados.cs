using JJ.Net.CrossData.Dicionario;
using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Enumerador;
using JJ.Net.CrossData.Extensao;
using JJ.Net.CrossData.Interfaces;
using JJ.Net.CrossData.Provider;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.CrossData
{
    public class ConfiguracaoBancoDados : IConfiguracaoBancoDados
    {
        private readonly Dictionary<TipoBancoDados, IBancoDadosProvider> _providers;

        public IDbConnection ConexaoAtiva { get; private set; }
        public TipoBancoDados TipoBanco { get; private set; }

        public ConfiguracaoBancoDados()
        {
            _providers = new Dictionary<TipoBancoDados, IBancoDadosProvider>
        {
            { TipoBancoDados.SQLite, new SqliteProvider() },
            { TipoBancoDados.SQLServer, new SqlServerProvider() },
            { TipoBancoDados.MySQL, new MySqlProvider() }
        };
        }

        public async Task InicializarAsync(ParametrosConfiguracao parametros)
        {
            if (!_providers.TryGetValue(parametros.TipoBanco, out var provider))
                throw new NotSupportedException("Tipo de banco não suportado");

            ConexaoAtiva = provider.CriarConexao(parametros);
            TipoBanco = parametros.TipoBanco;

            SQLTradutorFactory.TipoBancoDados = TipoBanco;
            DapperExtension.TipoBancoDados = TipoBanco;
        }
    }
}
