using JJ.Net.CrossData.Provider;
using JJ.Net.CrossData_WinUI_3.Dicionario;
using JJ.Net.CrossData_WinUI_3.DTO;
using JJ.Net.CrossData_WinUI_3.Enumerador;
using JJ.Net.CrossData_WinUI_3.Extensao;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using JJ.Net.CrossData_WinUI_3.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.CrossData
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

            ConexaoAtiva = await provider.CriarConexaoAsync(parametros);
            TipoBanco = parametros.TipoBanco;

            SQLTradutorFactory.TipoBancoDados = TipoBanco;
            DapperExtension.TipoBancoDados = TipoBanco;
        }

        public void Inicializar(ParametrosConfiguracao parametros)
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
