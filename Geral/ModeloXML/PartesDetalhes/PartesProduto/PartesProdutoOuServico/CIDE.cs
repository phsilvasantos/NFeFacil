﻿using BaseGeral.View;
using System.Xml.Serialization;

namespace BaseGeral.ModeloXML.PartesDetalhes.PartesProduto.PartesProdutoOuServico
{
    public sealed class CIDE
    {
        [XmlElement("qBCProd", Order = 0), DescricaoPropriedade("Informar a BC da CIDE em quantidade")]
        public double QBCProd { get; set; }

        [XmlElement("vAliqProd", Order = 1), DescricaoPropriedade("Valor da alíquota da CIDE")]
        public double VAliqProd { get; set; }

        [XmlElement("vCIDE", Order = 2), DescricaoPropriedade("Valor da CIDE")]
        public double VCIDE { get; set; }
    }
}