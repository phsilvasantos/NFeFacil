﻿using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;
using static NFeFacil.ExtensoesPrincipal;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewNFe.Impostos.DetalhamentoPIS
{
    sealed class DadosOutr : DadosDuplos
    {
        public override object Processar(ProdutoOuServico prod)
        {
            if (TipoCalculo == TiposCalculo.PorAliquota)
            {
                var vBC = prod.ValorTotal;
                return new PIS
                {
                    Corpo = new PISOutr
                    {
                        CST = CST,
                        vBC = ToStr(vBC),
                        pPIS = ToStr(Aliquota, "F4"),
                        vPIS = ToStr(vBC * Aliquota / 100)
                    }
                };
            }
            else
            {
                var qBCProd = prod.QuantidadeComercializada;
                return new PIS
                {
                    Corpo = new PISOutr
                    {
                        CST = CST,
                        qBCProd = ToStr(qBCProd, "F4"),
                        vAliqProd = ToStr(Valor, "F4"),
                        vPIS = ToStr(qBCProd * Valor)
                    }
                };
            }
        }
    }
}
