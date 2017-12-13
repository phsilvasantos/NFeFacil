﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoICMS.DadosRN
{
    class Tipo40_41_50 : BaseRN
    {
        public string vICMSDeson { get; set; }
        public string motDesICMS { get; set; }

        public Tipo40_41_50(TelasRN.Tipo40_41_50 tela)
        {
            vICMSDeson = tela.vICMSDeson;
            motDesICMS = tela.motDesICMS;
        }

        public override object Processar(DetalhesProdutos prod)
        {
            if (CST == "40")
            {
                return new ICMS40()
                {
                    CST = CST,
                    motDesICMS = motDesICMS,
                    Orig = Origem,
                    vICMSDeson = vICMSDeson
                };
            }
            else if (CST == "41")
            {
                return new ICMS41()
                {
                    CST = CST,
                    motDesICMS = motDesICMS,
                    Orig = Origem,
                    vICMSDeson = vICMSDeson
                };
            }
            else
            {
                return new ICMS50()
                {
                    CST = CST,
                    motDesICMS = motDesICMS,
                    Orig = Origem,
                    vICMSDeson = vICMSDeson
                };
            }
        }
    }
}
