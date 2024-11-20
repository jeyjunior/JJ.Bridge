﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.UW.Core.Atributos
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ChavePrimaria : Attribute
    {
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
}