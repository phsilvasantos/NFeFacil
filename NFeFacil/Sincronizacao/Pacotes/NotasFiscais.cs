﻿using System.Xml.Linq;

namespace NFeFacil.Sincronizacao.Pacotes
{
    public sealed class NotasFiscais : PacoteBase
    {
        public XElement[] XMLs { get; set; }
    }
}
