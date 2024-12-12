using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using JJ.UW.Core.Componentes.Mensagem;
using JJ.UW.Core.Extensoes;
using JJ.UW.Core.Enumerador;
using JJ.UW.Core.DTOs;

namespace JJ.UW.Core.Componentes.Mensagem
{
    public static class Mensagem
    {
        public static async Task<MensagemResultado> Erro(string mensagem, string titulo = "Erro")
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoSecundario = "OK",
                ExibirBotaoSecundario = true,
                Cor = new SolidColorBrush(Colors.Red),
                TipoResultadoBotaoSecundario = MensagemResultado.OK,
            });
        }

        public static async Task<MensagemResultado> Aviso(string mensagem, string titulo = "Aviso")
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoSecundario = "OK",
                ExibirBotaoSecundario = true,
                Cor = new SolidColorBrush(Colors.Yellow),
                TipoResultadoBotaoSecundario = MensagemResultado.OK,
            });
        }

        public static async Task<MensagemResultado> Sucesso(string mensagem, string titulo = "Sucesso")
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoSecundario = "OK",
                ExibirBotaoSecundario = true,
                Cor = new SolidColorBrush(Colors.Green),
                TipoResultadoBotaoSecundario = MensagemResultado.OK,
            });
        }

        public static async Task<MensagemResultado> Informacao(string mensagem, string titulo = "Informação")
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoSecundario = "OK",
                ExibirBotaoSecundario = true,
                Cor = new SolidColorBrush(Colors.Blue),
                TipoResultadoBotaoSecundario = MensagemResultado.OK,
            });
        }

        public static async Task<MensagemResultado> Pergunta(string titulo, string mensagem, SolidColorBrush cor, string botao1 = "Sim", string botao2 = "Não", MensagemResultado tipoResultadoBotaoPrimario = MensagemResultado.Sim, MensagemResultado tipoResultadoBotaoSecundario = MensagemResultado.Nao)
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoPrimario = botao1,
                ExibirBotaoPrimario = (botao1.ObterValorOuPadrao("").Trim() != ""),
                BotaoSecundario = botao2,
                ExibirBotaoSecundario = (botao2.ObterValorOuPadrao("").Trim() != ""),
                Cor = cor,
                TipoResultadoBotaoPrimario = tipoResultadoBotaoPrimario,
                TipoResultadoBotaoSecundario = tipoResultadoBotaoSecundario,
            });
        }

        private static async Task<MensagemResultado> ExibirMensagem(MensagemRequest parametros)
        {
            var dialog = new MensagemDialog
            {
                DataContext = parametros,
                TipoResultadoBotaoPrimario = parametros.TipoResultadoBotaoPrimario,
                TipoResultadoBotaoSecundario = parametros.TipoResultadoBotaoSecundario,
            };

            await dialog.ShowAsync();

            return dialog.Resultado;
        }
    }
}
