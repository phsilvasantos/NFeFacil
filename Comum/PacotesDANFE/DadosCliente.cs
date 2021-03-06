﻿using BaseGeral.ModeloXML;

namespace Comum.PacotesDANFE
{
    public sealed class DadosCliente
    {
        public string NomeCliente { get; set; }
        public string DocCliente { get; set; }
        public string DataEmissao { get; set; }
        public string DataEntradaSaida { get; set; }
        public string IECliente { get; set; }
        public string HoraEntradaSaida { get; set; }
        public EnderecoCompleto Endereco { get; set; }
    }
}
