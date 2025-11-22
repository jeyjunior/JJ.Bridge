using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData_WinUI_3.Atributo
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ChavePrimaria : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EntidadeAttribute : Attribute
    {
        public string NomeTabela { get; }

        public EntidadeAttribute(string nomeTabela = null)
        {
            NomeTabela = nomeTabela;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Editavel : Attribute
    {
        public bool HabilitarEdicao { get; private set; }
        public Editavel(bool valor)
        {
            HabilitarEdicao = valor;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TamanhoString : Attribute
    {
        public int Tamanho { get; private set; }
        public TamanhoString(int tamanho)
        {
            Tamanho = tamanho;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TamanhoDecimal : Attribute
    {
        public int Tamanho { get; private set; }
        public int Decimais { get; private set; }
        public TamanhoDecimal(int tamanho, int decimais)
        {
            Tamanho = tamanho;
            Decimais = decimais;
        }

    }

    public class Relacionamento : Attribute
    {
        public string Tabela { get; private set; }
        public string ChavePrimaria { get; private set; }

        public Relacionamento(string tabela, string chavePrimaria = "Id")
        {
            Tabela = tabela;
            ChavePrimaria = chavePrimaria;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Obrigatorio : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class CodigoGlyph : Attribute
    {
        public string Glyph { get; }
        public string FontFamily { get; }

        public CodigoGlyph(string glyph, string fontFamily)
        {
            Glyph = glyph;
            FontFamily = fontFamily;
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class Fonte : Attribute
    {
        public string FontFamily { get; }

        public Fonte(string fontFamily)
        {
            FontFamily = fontFamily;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Identity : Attribute
    {
        public bool HabilitarIdentity { get; private set; }
        public string ConfiguracaoPersonalizada { get; private set; }

        public Identity(bool habilitar = true, string configuracaoPersonalizada = null)
        {
            HabilitarIdentity = habilitar;
            ConfiguracaoPersonalizada = configuracaoPersonalizada;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Unique : Attribute
    {
        public bool EhUnico { get; private set; }

        public Unique(bool ehUnico = true)
        {
            EhUnico = ehUnico;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValue : Attribute
    {
        public string Valor { get; private set; }
        public bool UsarFuncaoBanco { get; private set; }

        public DefaultValue(string valor, bool usarFuncaoBanco = false)
        {
            Valor = valor;
            UsarFuncaoBanco = usarFuncaoBanco;
        }
    }
}
