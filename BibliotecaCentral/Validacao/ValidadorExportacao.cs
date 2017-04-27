﻿using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaCentral.Log;

namespace BibliotecaCentral.Validacao
{
    public sealed class ValidadorExportacao : IValidavel
    {
        private Exportacao Exportacao;

        public ValidadorExportacao(Exportacao exportacao)
        {
            Exportacao = exportacao;
        }

        public bool Validar(ILog log)
        {
            if (Exportacao == null) return false;
            return new ValidarDados().ValidarTudo(log,
                new ConjuntoAnalise(string.IsNullOrEmpty(Exportacao.UFSaidaPais), "Não foi definida uma UF de saída."),
                new ConjuntoAnalise(string.IsNullOrEmpty(Exportacao.xLocDespacho), "Não foi definido um local de despacho"),
                new ConjuntoAnalise(string.IsNullOrEmpty(Exportacao.xLocExporta), "Não foi definido o local de exportação"));
        }
    }
}
