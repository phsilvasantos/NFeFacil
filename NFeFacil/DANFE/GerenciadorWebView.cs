﻿using BibliotecaCentral;
using BibliotecaCentral.ModeloXML.PartesProcesso;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using NFeFacil.DANFE.Pacotes;
using NFeFacil.DANFE.Processamento;
using BibliotecaCentral.ModeloXML;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NFeFacil.DANFE
{
    public abstract class GerenciadorWebView
    {
        protected Geral Dados;
        protected ViewDados ExibiçãoDados;
        protected ViewUI UI;

        protected GerenciadorWebView(Processo processo, ref WebView webView)
        {
            Dados = Converter(processo);
            ExibiçãoDados = new ViewDados(ref webView, Dados);
            UI = new ViewUI(ref webView);
        }

        protected async Task ObterPaginasWeb(Func<int, Task> açaoCadaView)
        {
            var original = UI.ObterDimensoesView();
            try
            {
                for (int i = 0; i < ExibiçãoDados.TotalPaginas; i++)
                {
                    await ExibiçãoDados.ExibirUmaPágina(i);
                    await açaoCadaView?.Invoke(i);
                }
            }
            finally
            {
                UI.DefinirDimensoesView(original.largura, original.altura);
            }
        }

        private static Geral Converter(Processo proc)
        {
            var dadosAdicionais = GetExtras(proc.NFe.Informações.infAdic);
            var dadosCabecalho = GetCabecalho(proc.NFe.Informações.identificação, proc.NFe.Informações.emitente);
            var dadosCliente = GetCliente(proc.NFe.Informações.identificação, proc.NFe.Informações.destinatário);
            var dadosImposto = GetImposto(proc.NFe.Informações.total);
            var dadosMotorista = GetMotorista(proc.NFe.Informações.transp);
            var dadosNFe = GetNFe(proc.NFe.Informações, proc.ProtNFe);
            var dadosProdutos = (from p in proc.NFe.Informações.produtos
                                 select GetProd(p)).ToArray();
            var duplicatas = proc.NFe.Informações.cobr?.Dup;
            var dadosDuplicatas = duplicatas != null ? (from d in duplicatas
                                                        select GetDuplicata(d)).ToArray() : new DadosDuplicata[0];
            return new Geral
            {
                _DadosAdicionais = dadosAdicionais,
                _DadosCabecalho = dadosCabecalho,
                _DadosCliente = dadosCliente,
                _DadosImposto = dadosImposto,
                _DadosMotorista = dadosMotorista,
                _DadosNFe = dadosNFe,
                _DadosProdutos = dadosProdutos,
                _Duplicatas = dadosDuplicatas
            };

            DadosAdicionais GetExtras(InformacoesAdicionais extras)
            {
                return new DadosAdicionais
                {
                    dados = Analisar(extras?.infCpl),
                    fisco = Analisar(extras?.infAdFisco),
                };
            }

            DadosCabecalho GetCabecalho(Identificacao ident, Emitente emit)
            {
                return new DadosCabecalho
                {
                    nomeEmitente = emit.nome,
                    serieNota = ident.Serie.ToString(),
                    numeroNota = ident.Numero.ToString()
                };
            }

            DadosCliente GetCliente(Identificacao ident, Destinatario dest)
            {
                return new DadosCliente
                {
                    DocCliente = dest.Documento,
                    dataEmissao = Convert.ToDateTime(ident.DataHoraEmissão).ToString("dd-MM-yyyy"),
                    dataEntradaSaida = Analisar(Convert.ToDateTime(ident.DataHoraSaídaEntrada).ToString("dd-MM-yyyy")),
                    horaEntradaSaida = Analisar(Convert.ToDateTime(ident.DataHoraSaídaEntrada).ToString("hh:mm:ss")),
                    Endereco = dest.endereco,
                    IECliente = Analisar(dest.inscricaoEstadual),
                    nomeCliente = dest.nome
                };
            }

            DadosImposto GetImposto(Total tot)
            {
                return new DadosImposto
                {
                    baseCalculoICMS = tot.ICMSTot.vBC.ToString(),
                    baseCalculoICMSST = tot.ICMSTot.vBCST.ToString(),
                    desconto = tot.ICMSTot.vDesc.ToString(),
                    despesasAcessorias = tot.ICMSTot.vOutro.ToString(),
                    totalNota = tot.ICMSTot.vNF.ToString(),
                    valorFrete = tot.ICMSTot.vFrete.ToString(),
                    valorICMS = tot.ICMSTot.vICMS.ToString(),
                    valorICMSST = tot.ICMSTot.vST.ToString(),
                    valorIPI = tot.ICMSTot.vIPI.ToString(),
                    valorSeguro = tot.ICMSTot.vSeg.ToString(),
                    valorTotalProdutos = tot.ICMSTot.vProd.ToString()
                };
            }

            DadosMotorista GetMotorista(Transporte transp)
            {
                return new DadosMotorista
                {
                    codigoANTT = Analisar(transp.veicTransp?.RNTC),
                    documentoMotorista = Analisar(transp.transporta?.Documento),
                    enderecoMotorista = Analisar(transp.transporta?.XEnder),
                    especieVolume = Analisar(transp.vol.FirstOrDefault()?.esp),
                    IEMotorista = Analisar(transp.transporta?.InscricaoEstadual),
                    marcaVolume = Analisar(transp.vol.FirstOrDefault()?.marca),
                    modalidadeFrete = transp.modFrete.ToString(),
                    municipioMotorista = Analisar(transp.transporta?.XMun),
                    nomeMotorista = Analisar(transp.transporta?.Nome),
                    numeroVolume = Analisar(transp.vol.FirstOrDefault()?.nVol),
                    pesoBrutoVolume = Analisar(transp.vol.FirstOrDefault()?.pesoB),
                    pesoLiquidoVolume = Analisar(transp.vol.FirstOrDefault()?.pesoL),
                    placa = Analisar(transp.veicTransp?.Placa),
                    quantidadeVolume = Analisar(transp.vol.FirstOrDefault()?.qVol),
                    ufMotorista = Analisar(transp.transporta?.UF),
                    ufPlaca = Analisar(transp.veicTransp?.UF)
                };
            }

            DadosNFe GetNFe(Detalhes detalhes, ProtocoloNFe prot)
            {
                var codigoBarras = detalhes.Id.Substring(detalhes.Id.IndexOf('e') + 1);
                return new DadosNFe
                {
                    Chave = codigoBarras,
                    CNPJEmit = detalhes.emitente.CNPJ,
                    DataHoraRecibo = prot.InfProt.dhRecbto.Replace('T', ' '),
                    Endereco = detalhes.emitente.endereco,
                    IE = detalhes.emitente.inscricaoEstadual,
                    NatOp = detalhes.identificação.NaturezaDaOperação,
                    NomeEmitente = detalhes.emitente.nome,
                    NumeroNota = detalhes.identificação.Numero.ToString(),
                    NumeroProtocolo = prot.InfProt.nProt.ToString(),
                    SerieNota = detalhes.identificação.Serie.ToString(),
                    TipoEmissao = detalhes.identificação.TipoEmissão.ToString(),
                };
            }

            DadosProduto GetProd(DetalhesProdutos prod)
            {
                var consult = new ConsultarImpostos(prod.impostos.ToXElement<Impostos>());
                return new DadosProduto
                {
                    CFOP = prod.Produto.CFOP,
                    cProd = prod.Produto.CodigoProduto,
                    CSTICMS = prod.impostos.GetCSTICMS(),
                    NCM = prod.Produto.NCM,
                    qCom = prod.Produto.QuantidadeComercializada.ToString(),
                    uCom = prod.Produto.UnidadeComercializacao,
                    vUnCom = prod.Produto.ValorUnitario.ToString(),
                    vUnTrib = prod.Produto.ValorUnitarioTributo.ToString(),
                    vProd = prod.Produto.ValorTotal.ToString(),
                    xProd = prod.Produto.Descricao,
                    pICMS = consult.AgregarValor("pICMS", 0).ToString(),
                    vICMS = consult.AgregarValor("vICMS", 0).ToString(),
                    pIPI = consult.AgregarValor("pIPI", 0).ToString(),
                    vIPI = consult.AgregarValor("vIPI", 0).ToString()
                };
            }

            DadosDuplicata GetDuplicata(Duplicata dup)
            {
                return new DadosDuplicata
                {
                    data = dup.DVenc,
                    n = dup.NDup,
                    valor = dup.DDup
                };
            }

            string Analisar(object str) => str != null ? (string)str : string.Empty;
        }
    }
}
