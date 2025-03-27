using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SimpleInjector;
using JJ.UWP.Core.Validador;
using JJ.UWP.CrossData.Atributo;
using JJ.UWP.Data.Interfaces;
using JJ.UWP.CrossData.Enumerador;
using JJ.UWP.Data;
using JJ.UWP.CrossData;

namespace UWPTesteDLL
{
    public class Bootstrap
    {
        public static Container Container { get; private set; }
        public static void Inicializar()
        {
            try
            {
                string caminhoDestino = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                ConfiguracaoBancoDados.IniciarConfiguracao(Conexao.SQLite, "Gerenciador de Senhas", caminhoDestino);

                Container = new Container();
                Container.Options.DefaultLifestyle = Lifestyle.Scoped;

                Container.Register<IUnitOfWork>(() => new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()), Lifestyle.Singleton);


                Container = Bootstrap.Container;
                Container.Verify();

                IniciarBaseDeDados();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao se conectar ao banco de dados.\n", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao acessar arquivos de configuração.\n", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado.\n", ex);
            }
        }
        private static void IniciarBaseDeDados()
        {
            var uow = Container.GetInstance<IUnitOfWork>();

            CriarTabelas(uow);
            //InserirInformacoesTeste();
        }
        private static void CriarTabelas(IUnitOfWork uow)
        {
            bool gSCategoria = false;
            bool gSCredencial = false;

            try
            {

            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao verificar a existência das tabelas", ex);
            }

            if (gSCategoria && gSCredencial)
                return;

            try
            {
                uow.Begin();


                uow.Commit();
            }
            catch (SqlException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao criar as tabelas no banco de dados", ex);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw new Exception("Erro inesperado ao criar as tabelas", ex);
            }
        }
    }
}


public class GSCredencial
{
    [ChavePrimaria, Obrigatorio]
    public int PK_GSCredencial { get; set; }

    [Obrigatorio]
    public string Credencial { get; set; }
    [Obrigatorio]
    public string Senha { get; set; }
    [Obrigatorio]
    public string IVSenha { get; set; }

    [Relacionamento("GSCategoria", "PK_GSCategoria")]
    public int? FK_GSCategoria { get; set; }

    [Obrigatorio]
    public DateTime DataCriacao { get; set; }
    public DateTime? DataModificacao { get; set; }

    [Editavel(false)]
    public GSCategoria GSCategoria { get; set; }

    [Editavel(false)]
    public ValidarResultado ValidarResultado { get; set; } = new ValidarResultado();
}


public class GSCredencialPesquisaRequest
{
    public string Valor { get; set; }
    public TipoDePesquisa TipoDePesquisa { get; set; }
    public TipoDeOrdenacao TipoDeOrdenacao { get; set; }

    public ValidarResultado ValidarResultado { get; set; }
}

public class GSConfiguracao
{
}

public class CriptografiaRequest
{
    public string Valor { get; set; }
    public string IV { get; set; }

    public ValidarResultado ValidarResultado { get; set; }
}

public class GSCategoria
{
    [ChavePrimaria, Obrigatorio]
    public int PK_GSCategoria { get; set; }

    [Obrigatorio]
    public string Categoria { get; set; }

    [Editavel(false)]
    public ValidarResultado Validar { get; set; } = new ValidarResultado();
}

public enum TipoDePesquisa
{
    Todos = 0,
    Categoria = 1,
    Credencial = 2,
}

public enum TipoDeOrdenacao
{
    Cadastro = 0,
    Modificação = 1,
    Categoria = 2,
    Credencial = 3
}