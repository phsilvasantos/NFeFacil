﻿using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto.PartesImpostos;

namespace NFeFacil.ViewModel.ImpostosProduto
{
    public sealed class PISDataContext : INotifyPropertyChanged, IImpostosUnidos
    {
        ProdutoOuServico produtoReferente;
        internal ProdutoOuServico ProdutoReferente
        {
            get => produtoReferente;
            set
            {
                produtoReferente = value;
                AtualizarImposto();
                AtualizarImpostoST();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(params string[] parametros)
        {
            for (int i = 0; i < parametros.Length; i++)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parametros[i]));
            }
        }

        private ConjuntoPIS Conjunto = new ConjuntoPIS();
        public ConteinerPIS Imposto { get; set; }
        public ConteinerPISST ImpostoST { get; set; }

        public bool CalculoAliquota { get; private set; }
        public bool CalculoValor { get; private set; }
        public bool Valor { get; private set; }
        public bool ComboTipoCalculo { get; private set; }

        public bool CalculoAliquotaST { get; private set; }
        public bool CalculoValorST { get; private set; }
        public bool ValorST { get; private set; }
        public bool ComboTipoCalculoST { get; private set; }

        public string CSTSelecionado
        {
            get => Conjunto.PIS.Corpo != null ? Conjunto.PIS.Corpo.CST : string.Empty;
            set
            {
                var valor = int.Parse(value);
                if (new int[] { 1, 2 }.Contains(valor))
                {
                    CalculoAliquota = true;
                    CalculoValor = false;
                    Valor = true;
                    ComboTipoCalculo = false;
                    Conjunto.PIS.Corpo = new PISAliq();
                }
                else if (valor == 3)
                {
                    CalculoAliquota = false;
                    CalculoValor = true;
                    Valor = true;
                    ComboTipoCalculo = false;
                    Conjunto.PIS.Corpo = new PISQtde();
                }
                else if (new int[] { 4, 5, 6, 7, 8, 9 }.Contains(valor))
                {
                    CalculoAliquota = false;
                    CalculoValor = false;
                    Valor = false;
                    ComboTipoCalculo = false;
                    Conjunto.PIS.Corpo = new PISNT();
                }
                else
                {
                    CalculoAliquota = false;
                    CalculoValor = false;
                    Valor = true;
                    ComboTipoCalculo = true;
                    Conjunto.PIS.Corpo = new PISOutr();
                }
                Conjunto.PIS.Corpo.CST = valor.ToString("00");

                CalculoAliquotaST = false;
                CalculoValorST = false;
                if (valor == 5)
                {
                    ValorST = true;
                    ComboTipoCalculoST = true;
                    Conjunto.PISST = new PISST();
                }
                else
                {
                    ValorST = false;
                    ComboTipoCalculoST = false;
                    Conjunto.PISST = null;
                }

                OnPropertyChanged(nameof(CalculoAliquota), nameof(CalculoValor), nameof(Valor),
                    nameof(ComboTipoCalculo), nameof(ComboTipoCalculoST),
                    nameof(CalculoAliquotaST), nameof(CalculoValorST), nameof(ValorST));
                AtualizarImposto();
                AtualizarImpostoST();
            }
        }

        private string tipoCalculo;
        public string TipoCalculo
        {
            get => tipoCalculo;
            set
            {
                tipoCalculo = value;
                if (Conjunto.PIS.Corpo is PISOutr)
                {
                    switch (value == "Por alíquota" ? TiposCalculo.PorAliquota : TiposCalculo.PorValor)
                    {
                        case TiposCalculo.PorAliquota:
                            CalculoAliquota = true;
                            CalculoValor = false;
                            break;
                        case TiposCalculo.PorValor:
                            CalculoAliquota = false;
                            CalculoValor = true;
                            break;
                    }
                    Conjunto.PIS.Corpo = new PISOutr { CST = Conjunto.PIS.Corpo.CST };
                    OnPropertyChanged(nameof(CalculoAliquota), nameof(CalculoValor));
                    AtualizarImposto();
                }
            }
        }

        private string tipoCalculoST;
        public string TipoCalculoST
        {
            get => tipoCalculoST;
            set
            {
                tipoCalculoST = value;
                if (Conjunto.PISST != null)
                {
                    Conjunto.PISST = new PISST();
                    switch (value == "Por alíquota" ? TiposCalculo.PorAliquota : TiposCalculo.PorValor)
                    {
                        case TiposCalculo.PorAliquota:
                            CalculoAliquotaST = true;
                            CalculoValorST = false;
                            break;
                        case TiposCalculo.PorValor:
                            CalculoAliquotaST = false;
                            CalculoValorST = true;
                            break;
                    }
                    OnPropertyChanged(nameof(CalculoAliquotaST), nameof(CalculoValorST));
                    AtualizarImpostoST();
                }
            }
        }

        void AtualizarImposto()
        {
            Imposto = new ConteinerPIS(() => OnPropertyChanged(nameof(Imposto)), Conjunto.PIS.Corpo, ProdutoReferente);
            OnPropertyChanged(nameof(Imposto));
        }

        void AtualizarImpostoST()
        {
            ImpostoST = new ConteinerPISST(() => OnPropertyChanged(nameof(ImpostoST)), Conjunto.PISST, ProdutoReferente);
            OnPropertyChanged(nameof(ImpostoST));
        }

        public PISDataContext()
        {
            AtualizarImposto();
        }
        public PISDataContext(ConjuntoPIS conjunto, ProdutoOuServico produtoReferente)
        {
            this.produtoReferente = produtoReferente;
            if (conjunto.PIS.Corpo != null)
            {
                var corpo = conjunto.PIS.Corpo;
                CSTSelecionado = corpo.CST;
                if (corpo is PISOutr outr)
                {
                    if (string.IsNullOrEmpty(outr.pPIS))
                    {
                        TipoCalculo = "Pelo valor por unidade";
                    }
                    else
                    {
                        TipoCalculo = "Por alíquota";
                    }
                }
                Conjunto.PIS = conjunto.PIS;

                if (conjunto.PISST != null)
                {
                    var st = conjunto.PISST;
                    if (string.IsNullOrEmpty(st.pPIS))
                    {
                        TipoCalculoST = "Pelo valor por unidade";
                    }
                    else
                    {
                        TipoCalculoST = "Por alíquota";
                    }
                    Conjunto.PISST = conjunto.PISST;
                    AtualizarImpostoST();
                }
                OnPropertyChanged(nameof(CSTSelecionado));
            }
            AtualizarImposto();
        }

        public IEnumerable<Imposto> SepararImpostos() => Conjunto.SepararImpostos();
    }
}
