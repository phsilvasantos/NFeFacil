﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static BaseGeral.ExtensoesPrincipal;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Comum.PartesDANFE
{
    public sealed partial class DadosNFe : UserControl
    {
        public PacotesDANFE.DadosNFe Contexto { get; set; }

        DimensoesPadrao Dimensoes { get; } = new DimensoesPadrao();

        GridLength Coluna0 => CMToLength(8);
        GridLength Coluna1 => CMToLength(3);
        GridLength Coluna2 => CMToLength(8);

        GridLength Linha0 => CMToLength(3.4);

        public DadosNFe()
        {
            InitializeComponent();
        }

        public void DefinirPagina(int paginaAtual, int total)
        {
            txtPagAtual.Text = paginaAtual.ToString();
            txtPagTotal.Text = total.ToString();
        }
    }
}
