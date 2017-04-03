﻿using NFeFacil.ItensBD;
using NFeFacil.ModeloXML;
using NFeFacil.ModeloXML.PartesProcesso;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace NFeFacil.ViewModel
{
    public sealed class RelatorioVendasAnuais : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<int> AnosDisponiveis
        {
            get
            {
                using (var db = new AplicativoContext())
                {
                    var anos = from dado in db.NotasFiscais
                               let ano = Convert.ToDateTime(dado.DataEmissao).Year
                               orderby ano ascending
                               select ano;
                    return anos.Distinct().GerarObs();
                }
            }
        }

        private IEnumerable<NFe> notas;

        private int anoEscolhido;
        public int AnoEscolhido
        {
            get { return anoEscolhido; }
            set
            {
                anoEscolhido = value;
                AnoMudou();
            }
        }

        private void AnoMudou()
        {
            using (var db = new AplicativoContext())
            {
                var pasta = new PastaNotasFiscais();
                var Obter = new Func<int, string, Task<NFe>>(async (tipo,id) =>
                {
                    return tipo < 4 ? await pasta.Retornar<NFe>(id) : (await pasta.Retornar<Processo>(id)).NFe;
                });
                notas = Task.WhenAll(from dado in db.NotasFiscais
                                     where Convert.ToDateTime(dado.DataEmissao).Year == anoEscolhido
                                     select Obter(dado.Status, dado.Id)).Result;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(ResultadoCliente)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(ResultadoMes)));
        }

        public ObservableCollection<TotalPorMes> ResultadoMes
        {
            get
            {
                if (notas != null)
                {
                    var total = new List<TotalPorMes>(12);
                    for (int i = 1; i < 13; i++)
                    {
                        total.Add(new TotalPorMes { Mês = i.ToString() });
                    }
                    foreach (var item in notas)
                    {
                        var det = item.Informações;
                        var data = Convert.ToDateTime(det.identificação.DataHoraEmissão);
                        foreach (var prod in det.produtos)
                        {
                            total[data.Month - 1].Quantidade += prod.Produto.QuantidadeComercializada;
                        }
                        total[data.Month - 1].Total += det.total.ICMSTot.vNF;
                    }
                    total.Add(new TotalPorMes
                    {
                        Mês = "Total",
                        Quantidade = total.Sum(x => x.Quantidade),
                        Total = total.Sum(x => x.Total)
                    });
                    return total.GerarObs();
                }
                else return null;
            }
        }

        public ObservableCollection<TotalPorCliente> ResultadoCliente
        {
            get
            {
                if (notas != null)
                {
                    var total = new List<TotalPorCliente>();
                    foreach (var item in notas)
                    {
                        var det = item.Informações;
                        if (total.Count(x => x.Doc == det.destinatário.obterDocumento) == 0)
                            total.Add(new TotalPorCliente { Doc = det.destinatário.obterDocumento });
                        var tot = total.Single(x => x.Doc == det.destinatário.obterDocumento);
                        tot.Mun = det.destinatário.endereco.NomeMunicipio;
                        tot.Nome = det.destinatário.nome;
                        foreach (var prod in det.produtos)
                        {
                            tot.Quantidade += prod.Produto.QuantidadeComercializada;
                        }
                        tot.Total += det.total.ICMSTot.vNF;
                    }
                    total.Add(new TotalPorCliente
                    {
                        Nome = "Total",
                        Quantidade = total.Sum(x => x.Quantidade),
                        Total = total.Sum(x => x.Total)
                    });
                    return total.GerarObs();
                }
                else return null;
            }
        }

        public sealed class TotalPorCliente
        {
            public double Quantidade { get; set; }
            public double Total { get; set; }
            public string Nome { get; set; }
            public string Doc { get; set; }
            public string Mun { get; set; }
        }

        public sealed class TotalPorMes
        {
            public double Quantidade { get; set; }
            public double Total { get; set; }
            public string Mês { get; set; }
        }
    }
}
