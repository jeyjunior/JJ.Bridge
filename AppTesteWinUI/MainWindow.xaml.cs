using JJ.Standard.Data.Interfaces;
using JJ.Standard.Data.Extensoes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppTesteWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IUnitOfWork uow;
        public MainWindow()
        {
            this.InitializeComponent();
            uow = Bootstrap.Container.GetInstance<IUnitOfWork>();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            var pessoa = uow.Connection.ObterLista<Pessoa>();

            try
            {
                uow.Begin();
                var ret = uow.Connection.Adicionar(new Pessoa { PK_Pessoa = 1, Nome = "teste 'SELECT * FROM Pessoa' DROP TABLE PESSOA" }, uow.Transaction);

                uow.Commit();
            }
            catch (Exception)
            {
                uow.Rollback();
            }
        }
    }
}
