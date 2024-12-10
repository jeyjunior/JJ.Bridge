using JJ.UW.Core.Extensoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JJ.UW.Core.Componentes.Mensagem
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

        public static async Task<ContentDialogResult> Pergunta(string titulo, string mensagem, SolidColorBrush cor, string btn1 = "Sim", string btn2 = "Não" )
        {
            return await ExibirMensagem(new MensagemRequest
            {
                Titulo = titulo,
                Mensagem = mensagem,
                BotaoPrimario = btn1,
                BotaoSecundario = btn2,
                ExibirBotaoPrimario = (btn1.ObterValorOuPadrao("").Trim() != ""),
                ExibirBotaoSecundario = (btn2.ObterValorOuPadrao("").Trim() != ""),
                TipoMensagem = TipoMensagem.Pergunta,
                Cor = cor,
            });
        }

        private static async Task<ContentDialogResult> ExibirMensagem(MensagemRequest parametros)
        {
            var contentDialog = new ContentDialog()
            {
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
            };

            var stackPanel = new StackPanel 
            { 
                Orientation = Orientation.Vertical, 
                Spacing = 4,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            if (parametros.Titulo.ObterValorOuPadrao("").Trim() != "")
            {
                var frame = new Frame
                {
                    Width = 4,
                    Height = 20,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };

                switch (parametros.TipoMensagem)
                {
                    case TipoMensagem.Erro:
                        frame.Background = new SolidColorBrush(Colors.Red);
                        break;
                    case TipoMensagem.Sucesso:
                        frame.Background = new SolidColorBrush(Colors.Green);
                        break;
                    case TipoMensagem.Aviso:
                        frame.Background = new SolidColorBrush(Colors.Yellow);
                        break;
                    case TipoMensagem.Informacao:
                        frame.Background = new SolidColorBrush(Colors.Blue);
                        break;
                    case TipoMensagem.Pergunta:
                        frame.Background = parametros.Cor;
                        break;
                    default:
                        break;
                }

                var titulo = new TextBlock
                {
                    Text = parametros.Titulo,
                    FontSize = 20,
                    FontWeight = new Windows.UI.Text.FontWeight() { Weight = 600 },
                    Padding = new Thickness(0,0,0,2),
                };

                var horizontalPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                };

                horizontalPanel.Children.Add(frame);
                horizontalPanel.Children.Add(titulo);
                stackPanel.Children.Add(horizontalPanel);
            }
            
            if (parametros.Mensagem.ObterValorOuPadrao("").Trim() != "")
            {
                var mensagemTextBlock = new TextBlock
                {
                    Text = parametros.Mensagem,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Windows.UI.Xaml.Thickness(10)
                };

                stackPanel.Children.Add(mensagemTextBlock);
            }

            if (parametros.ExibirBotaoPrimario && parametros.BotaoPrimario.ObterValorOuPadrao("").Trim() != "")
            {
                contentDialog.IsPrimaryButtonEnabled = true;
                contentDialog.PrimaryButtonText = parametros.BotaoPrimario;
            }

            if (parametros.ExibirBotaoSecundario && parametros.BotaoSecundario.ObterValorOuPadrao("").Trim() != "")
            {
                contentDialog.IsSecondaryButtonEnabled = true;
                contentDialog.SecondaryButtonText = parametros.BotaoSecundario;
            }

            contentDialog.Content = stackPanel;

            return await contentDialog.ShowAsync();
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

        public SolidColorBrush Cor { get; set; } = new SolidColorBrush(Colors.White);
    }

}
