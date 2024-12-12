using JJ.UW.Core;
using JJ.UW.Core.Enumerador;
using JJ.UW.Core.Extensoes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace JJ.UW.Core.Componentes.Mensagem
{
    public sealed partial class MensagemDialog : ContentDialog
    {
        public MensagemResultado Resultado { get; private set; }
        public MensagemResultado TipoResultadoBotaoPrimario { get; set; }
        public MensagemResultado TipoResultadoBotaoSecundario { get; set; }

        public MensagemDialog()
        {
            this.InitializeComponent();

        }

        private void BtnPrimario_Click(object sender, RoutedEventArgs e)
        {
            Resultado = TipoResultadoBotaoPrimario;
            this.Hide();
        }

        private void BtnSecundario_Click(object sender, RoutedEventArgs e)
        {
            Resultado = TipoResultadoBotaoSecundario;
            this.Hide();
        }
    }
}
