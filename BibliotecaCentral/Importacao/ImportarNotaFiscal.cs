﻿using BibliotecaCentral.ItensBD;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BibliotecaCentral.Importacao
{
    public sealed class ImportarNotaFiscal : Importacao
    {
        public ImportarNotaFiscal() : base(".xml") { }

        public override async Task<List<Exception>> ImportarAsync()
        {
            var arquivos = await ImportarArquivos();
            var retorno = new List<Exception>();
            Dictionary<NFeDI, XElement> conjuntos = new Dictionary<NFeDI, XElement>();
            for (int i = 0; i < arquivos.Count; i++)
            {
                try
                {
                    using (var stream = await arquivos[i].OpenStreamForReadAsync())
                    {
                        var xmlAtual = XElement.Load(stream);

                        if (xmlAtual.Name.LocalName != "nfeProc" && xmlAtual.Name.LocalName != "NFe")
                        {
                            retorno.Add(new XmlNaoReconhecido(arquivos[i].Name, xmlAtual.Name.LocalName, "nfeProc", "NFe"));
                        }
                        else
                        {
                            const string namespaceAtual = "http://www.portalfiscal.inf.br/nfe";

                            var filhoIdent = xmlAtual.Name.LocalName == "NFe"
                                ? xmlAtual : xmlAtual.Element(XName.Get("NFe", namespaceAtual));
                            filhoIdent = filhoIdent.Element(XName.Get("infNFe", namespaceAtual));
                            filhoIdent = filhoIdent.Element(XName.Get("ide", namespaceAtual));

                            var antigaDataEmi = filhoIdent.Element(XName.Get("dEmi", namespaceAtual));
                            if (antigaDataEmi != null)
                            {
                                antigaDataEmi.Name = XName.Get("dhEmi", namespaceAtual);
                                antigaDataEmi.Value = DateTime.ParseExact(
                                    antigaDataEmi.Value,
                                    "yyyy-MM-dd",
                                    CultureInfo.InvariantCulture).ToStringPersonalizado();
                            }

                            var antigaDataSaidaEntrada = filhoIdent.Element(XName.Get("dSaiEnt", namespaceAtual));
                            if (antigaDataSaidaEntrada != null)
                            {
                                antigaDataSaidaEntrada.Name = XName.Get("dhSaiEnt", namespaceAtual);
                                antigaDataSaidaEntrada.Value = DateTime.ParseExact(
                                    antigaDataSaidaEntrada.Value,
                                    "yyyy-MM-dd",
                                    CultureInfo.InvariantCulture).ToStringPersonalizado();
                            }

                            var diAtual = NFeDI.Converter(xmlAtual);
                            if (conjuntos.Keys.Count(x => x.Id == diAtual.Id) == 0)
                            {
                                conjuntos.Add(diAtual, xmlAtual);
                            }
                            else
                            {
                                var atual = conjuntos.Single(x => x.Key.Id == diAtual.Id);
                                if (atual.Key.Status < diAtual.Status)
                                {
                                    conjuntos.Remove(atual.Key);
                                    conjuntos.Add(diAtual, xmlAtual);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            using (var db = new Repositorio.MudancaOtimizadaBancoDados())
            {
                await db.AdicionarNotasFiscais(conjuntos);
            }
            return retorno;
        }
    }
}