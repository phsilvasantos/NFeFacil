﻿namespace Produto.Impostos
{
    public struct DadoPronto : IDetalhamentoImposto
    {
        public PrincipaisImpostos Tipo => ImpostoPronto.Tipo;
        public ImpostoArmazenado ImpostoPronto { get; set; }
    }
}
