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
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

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
        }

    }
}
