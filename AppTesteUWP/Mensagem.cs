using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AppTesteUWP
{
    public static class Mensagem
    {
        public static async Task<ContentDialogResult> Erro(string mensagem)
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = "Erro",
                Mensagem = mensagem,
                BotaoPrimario = "OK",
                ExibirBotaoPrimario = true,
                TipoMensagem = TipoMensagem.Erro
            });
        }

        public static async Task<ContentDialogResult> Aviso(string mensagem)
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = "Aviso",
                Mensagem = mensagem,
                BotaoPrimario = "OK",
                ExibirBotaoPrimario = true,
                TipoMensagem = TipoMensagem.Aviso
            });
        }

        public static async Task<ContentDialogResult> Sucesso(string mensagem)
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = "Sucesso",
                Mensagem = mensagem,
                BotaoPrimario = "OK",
                ExibirBotaoPrimario = true,
                TipoMensagem = TipoMensagem.Sucesso
            });
        }

        public static async Task<ContentDialogResult> Informacao(string mensagem)
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = "Informação",
                Mensagem = mensagem,
                BotaoPrimario = "OK",
                ExibirBotaoPrimario = true,
                TipoMensagem = TipoMensagem.Informacao
            });
        }

        public static async Task<ContentDialogResult> Pergunta(string titulo, string mensagem, SolidColorBrush cor, string btn1 = "Sim", string btn2 = "Não")
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoPrimario = btn1,
                BotaoSecundario = btn2,
                ExibirBotaoPrimario = !string.IsNullOrWhiteSpace(btn1),
                ExibirBotaoSecundario = !string.IsNullOrWhiteSpace(btn2),
                TipoMensagem = TipoMensagem.Pergunta,
                Cor = cor
            });
        }

        private static async Task<ContentDialogResult> ExibirMensagem(MensagemRequest parametros)
        {
            var dialog = new MensagemDialog
            {
                DataContext = parametros
            };

            // Configura a cor do título com base no tipo de mensagem
            switch (parametros.TipoMensagem)
            {
                case TipoMensagem.Erro:
                    parametros.Cor = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
                case TipoMensagem.Sucesso:
                    parametros.Cor = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case TipoMensagem.Aviso:
                    parametros.Cor = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case TipoMensagem.Informacao:
                    parametros.Cor = new SolidColorBrush(Windows.UI.Colors.Blue);
                    break;
                case TipoMensagem.Pergunta:
                    break;
            }

            await dialog.ShowAsync();

            return dialog.DialogResult;
        }
    }

    public enum TipoMensagem
    {
        Erro,
        Sucesso,
        Aviso,
        Informacao,
        Pergunta,
    }

    internal class MensagemRequest
    {
        public string Titulo { get; set; } = "";
        public string Mensagem { get; set; } = "";

        public bool ExibirBotaoPrimario { get; set; } = false;
        public string BotaoPrimario { get; set; } = "";

        public bool ExibirBotaoSecundario { get; set; } = false;
        public string BotaoSecundario { get; set; } = "";

        public TipoMensagem TipoMensagem { get; set; } = TipoMensagem.Erro;

        public SolidColorBrush Cor { get; set; } = new SolidColorBrush(Windows.UI.Colors.White);
    }
}
