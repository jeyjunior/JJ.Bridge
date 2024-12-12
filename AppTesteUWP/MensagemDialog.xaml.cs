using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace AppTesteUWP
{
    public sealed partial class MensagemDialog : ContentDialog
    {
        public ContentDialogResult DialogResult { get; private set; }

        public MensagemDialog()
        {
            this.InitializeComponent();
        }

        private void BtnPrimario_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ContentDialogResult.Primary;
            this.Hide();
        }

        private void BtnSecundario_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ContentDialogResult.Secondary;
            this.Hide();
        }
    }
}


