using JJ.UW.Core.Atributos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace JJ.UW.Core.Enumerador
{
    public enum Conexao
    {
        SQLite = 1,
        SQLServer = 2,
        MySql = 3,
    }

    public enum IconesGlyph
    {
        [CodigoGlyph("\u25B2", "Calibri")]
        TrianguloCima,
        [CodigoGlyph("\u25B6", "Calibri")]
        TrianguloDireita,
        [CodigoGlyph("\u25BC", "Calibri")]
        TrianguloBaixo,
        [CodigoGlyph("\u25C0", "Calibri")]
        TrianguloEsquerda,

        [CodigoGlyph("\u25CF", "Segoe UI Symbol")]
        Circulo,

        [CodigoGlyph("\uF4CA", "Segoe UI")]
        Dashboard,
        [CodigoGlyph("\uF4B5", "Segoe UI")]
        Transacao,

        [CodigoGlyph("\uE0E7", "Segoe UI Symbol")]
        Check,
        [CodigoGlyph("\uE094", "Segoe UI Symbol")]
        Pesquisa,
        [CodigoGlyph("\uE0B6", "Segoe UI Symbol")]
        Adicionar,
        [CodigoGlyph("\uE121", "Segoe UI Symbol")]
        Relogio,
        [CodigoGlyph("\uE171", "Segoe UI Symbol")]
        Exclamacao,
        [CodigoGlyph("\uE115", "Segoe UI Symbol")]
        Configuracao,
    }

    public enum FamiliaFonte
    {
        [Fonte("Calibri")]
        Calibri,

        [Fonte("Segoe UI")]
        SegoeUI,

        [Fonte("Segoe UI Symbol")]
        SegoeUISymbol,
    }

    public enum MensagemResultado
    {
        Nenhum = 0,
        Sim = 1,
        Nao = 2,
        OK = 3,
        Cancelar = 4,
    }

    public enum TipoMensagem
    {
        Nenhuma = 0,
        Informacao = 1,
        Sucesso = 2,
        Alerta = 3,
        Erro = 4
    }
}
