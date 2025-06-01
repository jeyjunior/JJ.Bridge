using JJ.Net.CrossData.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JJ.Net.CrossData.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TipoBancoDados Conexao { get; set; }
        TEntity Obter(int id);
        IEnumerable<TEntity> ObterLista(string condition = "", object parameters = null);
        int Adicionar(TEntity entity);
        int Atualizar(TEntity entity);
        int Deletar(object id);
        bool CriarTabela(string query);
        int ExecutarQuery(string query);
    }
}
