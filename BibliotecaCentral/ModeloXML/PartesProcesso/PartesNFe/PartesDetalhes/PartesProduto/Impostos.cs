﻿using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto
{
    public sealed class Impostos
    {
        public Impostos() { }
        public Impostos(IEnumerable<Imposto> lista)
        {
            impostos = lista.ToList();
        }

        public string vTotTrib { get; set; }

        [XmlElement(nameof(ICMS), Type = typeof(ICMS)), XmlElement(nameof(COFINS), Type = typeof(COFINS)), XmlElement(nameof(COFINSST), Type = typeof(COFINSST)), XmlElement(nameof(II), Type = typeof(II)), XmlElement(nameof(IPI), Type = typeof(IPI)), XmlElement(nameof(ISSQN), Type = typeof(ISSQN)), XmlElement(nameof(PIS), Type = typeof(PIS)), XmlElement(nameof(PISST), Type = typeof(PISST))]
        public List<Imposto> impostos = new List<Imposto>();

        public string GetCSTICMS()
        {
            foreach (var item in impostos)
            {
                if (item is ICMS icms)
                {
                    if (icms.Corpo is IRegimeNormal normal) return normal.CST;
                    else if (icms.Corpo is ISimplesNacional simples) return simples.CSOSN;
                }
            }
            return null;
        }
    }
}
