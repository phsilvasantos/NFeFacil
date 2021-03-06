﻿using BaseGeral;
using BaseGeral.ItensBD;
using BaseGeral.ModeloXML;
using BaseGeral.Validacao;
using Fiscal;
using Fiscal.WebService;
using Fiscal.WebService.Pacotes;
using Fiscal.WebService.Pacotes.PartesEnvEvento;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Fiscal.Certificacao;
using BaseGeral.View;
using System.Security.Cryptography.X509Certificates;

namespace Consumidor
{
    public sealed class ControleViewNFCe : IControleView
    {
        public bool IsNFCe { get; } = true;

        public async Task<bool> Cancelar(NFeDI nota)
        {
            bool retorno = true;
            var processo = XElement.Parse(nota.XML).FromXElement<ProcessoNFCe>();
            var informacoes = processo.NFe.Informacoes;
            var protNFe = processo.ProtNFe;

            var estado = informacoes.identificacao.CódigoUF;
            var cnpj = informacoes.Emitente.CNPJ;
            var chave = informacoes.ChaveAcesso;
            var tipoAmbiente = protNFe.InfProt.tpAmb;
            var nProtocolo = protNFe.InfProt.nProt;

            var entrada = new CancelarNFe();
            if (await entrada.ShowAsync() == ContentDialogResult.Primary)
            {
                var infoEvento = new InformacoesEvento(estado, cnpj, chave, nProtocolo, entrada.Motivo, tipoAmbiente);
                var envio = new EnvEvento(infoEvento);

                AssinaFacil assinador = new AssinaFacil();
                var gerenciador = new GerenciadorGeral<EnvEvento, RetEnvEvento>(estado, Operacoes.RecepcaoEvento, tipoAmbiente == 2, true);
                Progresso progresso = null;
                progresso = new Progresso(async x =>
                {
                    var cert = (X509Certificate2)x;
                    var resultado = await envio.PrepararEventos(assinador, cert);
                    if (!resultado.Item1)
                    {
                        retorno = resultado.Item1;
                        return resultado;
                    }
                    await progresso.Update(1);

                    var resposta = await gerenciador.EnviarAsync(envio);
                    if (resposta.ResultadorEventos[0].InfEvento.CStat == 135)
                    {
                        using (var repo = new BaseGeral.Repositorio.Escrita())
                        {
                            var xml = new ProcEventoCancelamento()
                            {
                                Eventos = envio.Eventos,
                                RetEvento = resposta.ResultadorEventos,
                                Versao = resposta.Versao
                            }.ToXElement<ProcEventoCancelamento>();
                            repo.SalvarItemSimples(new RegistroCancelamento()
                            {
                                ChaveNFe = chave,
                                DataHoraEvento = resposta.ResultadorEventos[0].InfEvento.DhRegEvento,
                                TipoAmbiente = tipoAmbiente,
                                XML = xml.ToString()
                            }, DefinicoesTemporarias.DateTimeNow);

                            nota.Status = (int)StatusNota.Cancelada;
                            repo.SalvarItemSimples(nota, DefinicoesTemporarias.DateTimeNow);
                            await progresso.Update(6);

                            using (var opEx = new BaseGeral.Repositorio.OperacoesExtras())
                            {
                                var rvVinculado = opEx.GetRVVinculado(informacoes.Id);
                                if (rvVinculado != null)
                                {
                                    if (rvVinculado.Cancelado)
                                    {
                                        var dialog = new MessageDialog("Um registro de venda já cancelado está vinculado a esta nota fiscal, você ainda deseja aplicar as alterações no estoque?", "Aviso");
                                        dialog.Commands.Add(new UICommand("Sim", y => AtualizarEstoques()));
                                        dialog.Commands.Add(new UICommand("Não"));
                                        await dialog.ShowAsync();
                                    }
                                    else
                                    {
                                        var dialog = new MessageDialog("Um registro de venda válido está vinculado a esta nota fiscal, você ainda deseja aplicar as alterações no estoque?", "Aviso");
                                        dialog.Commands.Add(new UICommand("Sim", y => AtualizarEstoques()));
                                        dialog.Commands.Add(new UICommand("Não"));
                                        await dialog.ShowAsync();

                                        dialog = new MessageDialog("Esta nota foi cancelada com sucesso, você deseja também cancelar o registro de venda?", "Aviso");
                                        dialog.Commands.Add(new UICommand("Sim", y =>
                                        {
                                            using (var escr = new BaseGeral.Repositorio.Escrita())
                                            {
                                                escr.CancelarRV(rvVinculado, new CancelamentoRegistroVenda()
                                                {
                                                    Id = rvVinculado.Id,
                                                    MomentoCancelamento = DefinicoesTemporarias.DateTimeNow,
                                                    Motivo = "Cancelamento decorrente de cancelamento da nota fiscal correspondente."
                                                }, DefinicoesTemporarias.DateTimeNow);
                                            }
                                        }));
                                        dialog.Commands.Add(new UICommand("Não"));
                                        await dialog.ShowAsync();
                                    }
                                }
                                else
                                {
                                    AtualizarEstoques();
                                }
                            }

                            void AtualizarEstoques()
                            {
                                using (var leit = new BaseGeral.Repositorio.Leitura())
                                {
                                    repo.AtualizarEstoques(DefinicoesTemporarias.DateTimeNow,
                                        (from prod in informacoes.produtos
                                         let orig = leit.ObterProduto(prod.Produto.CodigoProduto)
                                         where orig != null
                                         select (orig.Id, prod.Produto.QuantidadeComercializada)).ToArray());
                                }
                            }
                        }
                        retorno = true;
                        return (true, "NFe cancelada com sucesso.");
                    }
                    else
                    {
                        retorno = false;
                        return (false, resposta.ResultadorEventos[0].InfEvento.XMotivo);
                    }
                }, assinador.CertificadosDisponiveis, "Subject",
                "Preparar eventos com assinatura do emitente",
                "Preparar conexão",
                "Obter conteúdo da requisição",
                "Enviar requisição",
                "Processar resposta",
                "Salvar registro de cancelamento no banco de dados");
                gerenciador.ProgressChanged += async (x, y) => await progresso.Update(y + 1);
                await progresso.ShowAsync();
            }
            return retorno;
        }

        public async Task CriarCopia(NFeDI nota)
        {
            var processo = XElement.Parse(nota.XML).FromXElement<ProcessoNFCe>();
            var nfe = processo.NFe;
            var analisador = new AnalisadorNFCe(ref nfe);
            analisador.Desnormalizar();
            var controle = new ControleNFCe(nfe);
            await new Criador(controle).ShowAsync();
        }

        public void Exibir(NFeDI nota)
        {
            var acoes = new AcoesNFCe(nota);
            BasicMainPage.Current.Navegar<Visualizacao>(acoes);
        }
    }
}
