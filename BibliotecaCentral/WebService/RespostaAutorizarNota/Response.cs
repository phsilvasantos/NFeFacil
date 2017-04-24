﻿using System.Xml.Serialization;

namespace BibliotecaCentral.WebService.RespostaAutorizarNota
{
    [XmlRoot("nfeRetAutorizacaoLoteResult", Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NfeRetAutorizacao")]
    public struct Response
    {
        [XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe")]
        public CorpoResponse retConsReciNFe { get; set; }
    }
}