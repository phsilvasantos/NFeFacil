﻿using BaseGeral.View;
using BaseGeral.ModeloXML.PartesDetalhes;
using System.Xml.Serialization;
using System.Collections.Generic;
using static BaseGeral.ExtensoesPrincipal;

namespace BaseGeral.ModeloXML
{
    [XmlRoot("infNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public sealed class InformacoesNFCe : InformacoesBase
    {
        [DescricaoPropriedade("Produtos")]
        [XmlElement(ElementName = "det", Namespace = "http://www.portalfiscal.inf.br/nfe", Order = 3)]
        public List<DetalhesProdutos> produtos { get; set; }

        [DescricaoPropriedade("Total")]
        [XmlElement(Order = 4)]
        public Total total { get; set; }

        [DescricaoPropriedade("Transporte")]
        [XmlElement(Order = 5)]
        public Transporte transp { get; set; }

        [DescricaoPropriedade("Formas de pagamento")]
        [XmlElement("pag", Namespace = "http://www.portalfiscal.inf.br/nfe", Order = 6)]
        public List<Pagamento> FormasPagamento { get; set; }

        [DescricaoPropriedade("Informações Adicionais")]
        [XmlElement(Order = 7)]
        public InformacoesAdicionais infAdic { get; set; }
    }

    public sealed class Pagamento
    {
        [XmlElement("tPag", Order = 0)]
        public string Forma { get; set; } = "01";

        [XmlIgnore]
        public double vPag { get; set; }
        [XmlElement("vPag", Order = 1)]
        public string VPag { get => ToStr(vPag); set => vPag = Parse(value); }

        [XmlElement("card", Order = 2)]
        public Cartao Cartao { get; set; }
    }

    public sealed class Cartao
    {
        [XmlElement(Order = 0)]
        public string CNPJ { get; set; }

        [XmlElement("tBand", Order = 1)]
        public string Bandeira { get; set; }

        [XmlElement("cAut", Order = 2)]
        public string Autorizacao { get; set; }
    }
}