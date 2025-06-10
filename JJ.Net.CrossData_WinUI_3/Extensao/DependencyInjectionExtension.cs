using JJ.Net.CrossData_WinUI_3.CrossData;
using JJ.Net.CrossData_WinUI_3.DTO;
using JJ.Net.CrossData_WinUI_3.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Extensao
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddSingletonConfiguracaoAsync(this IServiceCollection services, Action<ParametrosConfiguracao> configurarParametros)
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

        public static IServiceCollection AddSingletonConfiguracao(this IServiceCollection services, Action<ParametrosConfiguracao> configurarParametros)
        {
            var parametros = new ParametrosConfiguracao();
            configurarParametros(parametros);

            if (string.IsNullOrWhiteSpace(parametros.NomeAplicacao))
                throw new ArgumentException("Nome da aplicação é obrigatório");

            var configuracao = new ConfiguracaoBancoDados();

            try
            {
                configuracao.Inicializar(parametros);
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
