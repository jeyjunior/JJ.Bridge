﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using JJ.Net.WinUI3.CrossData.Enumerador;

namespace JJ.Net.WinUI3.CrossData.Interface
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Conexao Conexao { get; set; }
        TEntity Obter(int id);
        IEnumerable<TEntity> ObterLista(string condition = "", object parameters = null);
        int Adicionar(TEntity entity);
        int Atualizar(TEntity entity);
        int Deletar(object id);
        bool CriarTabela(string query);
        int ExecutarQuery(string query);
    }
}
