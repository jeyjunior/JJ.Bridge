﻿using AppTesteWinUI;
using JJ.UW.Cryptography.Enumerador;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var ret = JJ.UW.Cryptography.Criptografia.Criptografar(TipoCriptografia.AES, "Teste123");
            var ret2 = JJ.UW.Cryptography.Criptografia.Descriptografar(TipoCriptografia.AES, ret.Valor, ret.IV);
        }
    }
}
