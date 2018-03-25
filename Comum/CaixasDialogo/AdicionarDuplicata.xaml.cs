﻿using BaseGeral.ModeloXML.PartesDetalhes;
using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace Comum.CaixasDialogo
{
    public sealed partial class AdicionarDuplicata : ContentDialog
    {
        public Duplicata Contexto { get; } = new Duplicata();

        public AdicionarDuplicata()
        {
            InitializeComponent();
        }
    }
}
