using JJ.UW.Core.Componentes.Mensagem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using JJ.UW.Core.Enumerador;

namespace JJ.UW.Core.DTOs
{
    internal class MensagemRequest
    {
        public string Titulo { get; set; } = "";
        public string Mensagem { get; set; } = "";
        public bool ExibirBotaoPrimario { get; set; } = false;
        public string BotaoPrimario { get; set; } = "";
        public bool ExibirBotaoSecundario { get; set; } = false;
        public string BotaoSecundario { get; set; } = "";
        public SolidColorBrush Cor { get; set; } = new SolidColorBrush(Colors.White);
        public MensagemResultado TipoResultadoBotaoPrimario { get; set; } = MensagemResultado.Sim;
        public MensagemResultado TipoResultadoBotaoSecundario { get; set; } = MensagemResultado.Nao;
    }

    internal class VisibilidadeComponentes : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
