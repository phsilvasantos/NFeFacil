﻿using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class HistoricoSincronizacao : Page
    {
        public HistoricoSincronizacao()
        {
            InitializeComponent();
            MainPage.Current.SeAtualizar("\uE81C", "Histórico de sincronização");
        }
    }
}
