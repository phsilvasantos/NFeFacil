﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Caixa de Diálogo de Conteúdo está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.Importacao
{
    public sealed partial class DefinirSenhaCertificado : ContentDialog
    {
        internal string Senha { get; private set; }

        public DefinirSenhaCertificado()
        {
            this.InitializeComponent();
        }
    }
}
