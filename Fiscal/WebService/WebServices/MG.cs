﻿namespace Fiscal.WebService.WebServices
{
    // Falta verificar os web services porque o site ainda está em construção
    internal struct MG : IWebService
    {
        public string ConsultarProducao => "https://nfe.fazenda.mg.gov.br/nfe2/services/NfeConsulta2";
        public string AutorizarProducao => "https://nfe.fazenda.mg.gov.br/nfe2/services/NfeAutorizacao";
        public string RespostaAutorizarProducao => "https://nfe.fazenda.mg.gov.br/nfe2/services/NfeRetAutorizacao";
        public string RecepcaoEventoProducao => "https://nfe.fazenda.mg.gov.br/nfe2/services/RecepcaoEvento";
        public string InutilizacaoProducao => "https://nfe.fazenda.mg.gov.br/nfe2/services/NfeInutilizacao2";

        public string ConsultarHomologacao => "https://hnfe.fazenda.mg.gov.br/nfe2/services/NfeConsulta2";
        public string AutorizarHomologacao => "https://hnfe.fazenda.mg.gov.br/nfe2/services/NfeAutorizacao";
        public string RespostaAutorizarHomologacao => "https://hnfe.fazenda.mg.gov.br/nfe2/services/NfeRetAutorizacao";
        public string RecepcaoEventoHomologacao => "https://hnfe.fazenda.mg.gov.br/nfe2/services/RecepcaoEvento";
        public string InutilizacaoHomologacao => "https://hnfe.fazenda.mg.gov.br/nfe2/services/NfeInutilizacao2";
    }
}