﻿namespace BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesProdutoOuServico
{
    public sealed class Medicamento
    {
        /// <summary>
        /// Número do Lote.
        /// </summary>
        public string nLote { get; set; }

        /// <summary>
        /// Quantidade de produtos.
        /// </summary>
        public double qLote { get; set; }

        /// <summary>
        /// Database.Principal de fabricação.
        /// Formato: “AAAA-MM-DD”.
        /// </summary>
        public string dFab { get; set; }

        /// <summary>
        /// Database.Principal de validade.
        /// Formato: “AAAA-MM-DD”.
        /// </summary>
        public string dVal { get; set; }

        /// <summary>
        /// Preço máximo consumidor.
        /// </summary>
        public double vPMC { get; set; }
    }
}