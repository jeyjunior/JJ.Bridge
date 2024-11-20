using AppTesteWinUI;
using JJ.UW.Data.Extensoes;
using JJ.UW.Data.Interfaces;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppTesteUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IUnitOfWork uow;

        public MainPage()
        {
            this.InitializeComponent();

            uow = Bootstrap.Container.GetInstance<IUnitOfWork>();

            var pessoa = uow.Connection.ObterLista<Pessoa>();

            try
            {
                uow.Begin();

                var ret1 = uow.Connection.CriarTabela<Pessoa>();
                var ret2 = uow.Connection.Adicionar(new Pessoa { PK_Pessoa = 1, Nome = "teste 'SELECT * FROM Pessoa' DROP TABLE PESSOA" }, uow.Transaction);

                uow.Commit();
            }
            catch (Exception)
            {
                uow.Rollback();
            }
        }
    }
}
