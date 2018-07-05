﻿using BaseGeral.View;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Venda.Impostos.DetalhamentoCOFINS
{
    [DetalhePagina("COFINS")]
    public sealed partial class DetalharQtde : Page
    {
        public double Valor { get; private set; }

        public DetalharQtde()
        {
            InitializeComponent();
        }
    }
}