using JJ.Net.CrossData.CrossData;
using JJ.Net.CrossData.DTO;
using JJ.Net.CrossData.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Extensao
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddCrossData(this IServiceCollection services, Action<ParametrosConfiguracao> configurarParametros)
        {
            var parametros = new ParametrosConfiguracao();
            configurarParametros(parametros);

            if (string.IsNullOrWhiteSpace(parametros.NomeAplicacao))
                throw new ArgumentException("Nome da aplicação é obrigatório");

            var configuracao = new ConfiguracaoBancoDados();

            try
            {
                configuracao.InicializarAsync(parametros).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Falha ao configurar banco de dados", ex);
            }

            services.AddSingleton<IConfiguracaoBancoDados>(configuracao);

            return services;
        }
    }
}
