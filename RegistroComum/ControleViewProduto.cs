﻿using BaseGeral;
using BaseGeral.ItensBD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Venda.ViewProdutoVenda;

namespace RegistroComum
{
    public sealed class ControleViewProduto : IControleViewProduto
    {
        public bool Concluido { get; private set; }
        public bool PodeConcluir { get; }
        public bool PodeDetalhar { get; }

        RegistroVenda Venda { get; set; }
        public Dictionary<Guid, double> ProdutosAdicionados => Venda.Produtos
            .GroupBy(x => x.IdBase)
            .ToDictionary(x => x.Key, y => y.Sum(x => x.Quantidade));

        public ControleViewProduto()
        {
            Venda = new RegistroVenda
            {
                Emitente = DefinicoesTemporarias.EmitenteAtivo.Id,
                Vendedor = DefinicoesTemporarias.VendedorAtivo?.Id ?? Guid.Empty,
                Produtos = new List<ProdutoSimplesVenda>(),
                DataHoraVenda = DefinicoesTemporarias.DateTimeNow,
                PrazoEntrega = DefinicoesTemporarias.DateTimeNow
            };
            PodeConcluir = false;
            PodeDetalhar = false;
        }

        public ControleViewProduto(RegistroVenda venda)
        {
            Venda = venda;
            PodeConcluir = true;
            PodeDetalhar = false;
        }

        public ObservableCollection<ExibicaoProdutoListaGeral> ObterProdutosIniciais()
        {
            using (var leitura = new BaseGeral.Repositorio.Leitura())
                return (from prod in Venda.Produtos
                        let comp = leitura.ObterProduto(prod.IdBase)
                        select new ExibicaoProdutoListaGeral
                        {
                            Codigo = comp.CodigoProduto,
                            Descricao = comp.Descricao,
                            Quantidade = prod.Quantidade.ToString("N2"),
                            ValorUnitario = prod.ValorUnitario.ToString("C"),
                            TotalLiquido = prod.TotalLíquido.ToString("C")
                        }).GerarObs();
        }

        public ExibicaoProdutoListaGeral Adicionar(AdicionarProduto caixa)
        {
            var novoProdBanco = new ProdutoSimplesVenda
            {
                IdBase = caixa.ProdutoSelecionado.Base.Id,
                ValorUnitario = caixa.ValorUnitario,
                Quantidade = caixa.Quantidade,
                Frete = 0,
                Seguro = caixa.Seguro,
                DespesasExtras = caixa.DespesasExtras
            };
            novoProdBanco.CalcularTotalLíquido();
            Venda.Produtos.Add(novoProdBanco);
            return new ExibicaoProdutoListaGeral
            {
                Codigo = caixa.ProdutoSelecionado.Codigo,
                Descricao = caixa.ProdutoSelecionado.Nome,
                Quantidade = caixa.Quantidade.ToString("N2"),
                TotalLiquido = novoProdBanco.TotalLíquido.ToString("C"),
                ValorUnitario = caixa.ValorUnitario.ToString("C")
            };
        }

        public bool EdicaoLiberada { get; } = false;

        public void Editar(ExibicaoProdutoListaGeral produto) => throw new NotImplementedException();

        public void Remover(ExibicaoProdutoListaGeral produto)
        {
            var index = Venda.Produtos.FindIndex(x => double.Parse(produto.Quantidade) == x.Quantidade
                && produto.ValorUnitario == x.ValorUnitario.ToString("C")
                && produto.TotalLiquido == x.TotalLíquido.ToString("C"));
            Venda.Produtos.RemoveAt(index);
        }

        public bool AnalisarDetalhamento(ProdutoAdicao produto) => throw new NotImplementedException();
        public bool AnalMudançaValorUnit(ProdutoAdicao produto) => throw new NotImplementedException();
        public void Detalhar(AdicionarProduto caixa) => throw new NotImplementedException();

        public void Avancar() => BasicMainPage.Current.Navegar<ManipulacaoRegistroVenda>(Venda);

        public void Concluir()
        {
            using (var repo = new BaseGeral.Repositorio.Escrita())
                repo.SalvarRV(Venda, DefinicoesTemporarias.DateTimeNow);
            Concluido = true;
            BasicMainPage.Current.Retornar();
        }

        public bool Validar()
        {
            return Venda.Produtos.Count > 0;
        }

        public void Voltar()
        {
            BasicMainPage.Current.Retornar();
        }

        public void AtualizarControle(object atualizacao)
        {
            if (atualizacao is RegistroVenda venda) Venda = venda;
            else throw new InvalidCastException();
        }
    }
}