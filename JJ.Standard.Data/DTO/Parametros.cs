﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JJ.Standard.Data.DTO
{
    public class Parametros
    {
        public Parametro BaseAtiva { get; set; }
        public List<Parametro> BaseDados { get; set; }
    }

    public class Parametro
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Valor { get; set; }
    }
}
