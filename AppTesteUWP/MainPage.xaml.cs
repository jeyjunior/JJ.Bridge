using AppTesteWinUI;
using JJ.UW.Core.Componentes.Mensagem;
using JJ.UW.Data.Extensoes;
using JJ.UW.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppTesteUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnMessageDialog_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("Esta é uma MessageDialog do UWP.", "Título da Mensagem");
            messageDialog.Commands.Add(new UICommand("OK", (command) => { /* ação opcional */ }));
            messageDialog.ShowAsync();
        }

        private async void btnContentDialog_Click(object sender, RoutedEventArgs e)
        {
            var contentDialog = new ContentDialog
            {
                Title = "Título do ContentDialog",
                Content = "Este é um ContentDialog. Você pode adicionar mais conteúdo aqui.",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancelar"
            };

            contentDialog.PrimaryButtonClick += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Botão OK pressionado.");
            };

            contentDialog.SecondaryButtonClick += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Botão Cancelar pressionado.");
            };

            ContentDialogResult resultado = await contentDialog.ShowAsync();

            if(resultado == ContentDialogResult.Primary)
            {
                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                var toastText = toastXml.GetElementsByTagName("text")[0];
                toastText.AppendChild(toastXml.CreateTextNode("Esta é uma Toast Notification!"));

                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
        }

        private void btnToastNotification_Click(object sender, RoutedEventArgs e)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var toastText = toastXml.GetElementsByTagName("text")[0];
            toastText.AppendChild(toastXml.CreateTextNode("Esta é uma Toast Notification!"));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private async void btnTeste_Click(object sender, RoutedEventArgs e)
        {
            var resultado = await JJ.UW.Core.Componentes.Mensagem.Mensagem.Erro("Teste mensagem erro JUNIOR");
            resultado = await JJ.UW.Core.Componentes.Mensagem.Mensagem.Aviso("Teste mensagem erro JUNIOR");
            resultado = await JJ.UW.Core.Componentes.Mensagem.Mensagem.Sucesso("Teste mensagem erro JUNIOR");
            resultado = await JJ.UW.Core.Componentes.Mensagem.Mensagem.Informacao("Teste mensagem erro JUNIOR");
            resultado = await JJ.UW.Core.Componentes.Mensagem.Mensagem
                .Pergunta("Pergunta: ", "Qual a cor do cavalo branco de napoleão?", new SolidColorBrush(Colors.Blue), "Branco", "Azul");

            //if (resultado == JJ.UW.Core.Enumerador.MensagemResultado.Sim)
            //{
            //    System.Diagnostics.Debug.WriteLine("Botão Primário clicado");
            //}
            //else if (resultado == JJ.UW.Core.Enumerador.MensagemResultado.Nao)
            //{
            //    System.Diagnostics.Debug.WriteLine("Botão Secundário clicado");
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("Fechado sem clicar em um botão");
            //}
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var resultado = await Mensagem.Erro("Teste de Mensagem");

            if (resultado == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine("Botão Primário clicado");
            }
            else if (resultado == ContentDialogResult.Secondary)
            {
                System.Diagnostics.Debug.WriteLine("Botão Secundário clicado");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Fechado sem clicar em um botão");
            }
        }
    }
}
