using JJ.UW.Data;
using JJ.UW.Data.Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTesteWinUI
{
    public  class Bootstrap
    {
        public static Container Container { get; private set; }
        public static void Iniciar()
        {
            Config.DefinirConexao(JJ.UW.Data.Enumerador.eConexao.SQLite);
            
            Container = new Container();
            Container.Options.DefaultLifestyle = Lifestyle.Scoped;

            Container.Register<IUnitOfWork,  UnitOfWork>(Lifestyle.Singleton);

            Container.Verify();
        }
    }
}
