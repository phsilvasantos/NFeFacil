﻿using BibliotecaCentral.ItensBD;
using BibliotecaCentral.Configuracoes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.ViewManagement;
using static BibliotecaCentral.Configuracoes.ConfiguracoesSincronizacao;
using BibliotecaCentral.Repositorio;

namespace NFeFacil.ViewModel
{
    public sealed class HistoricoSincronizacaoDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ApplicationView View { get; }

        public HistoricoSincronizacaoDataContext()
        {
            View = ApplicationView.GetForCurrentView();
            View.VisibleBoundsChanged += (x, y) => PropertyChanged(this, new PropertyChangedEventArgs("Vertical"));
        }

        public bool IsCliente => Tipo == TipoAppSincronizacao.Cliente;
        public bool IsServidor => Tipo == TipoAppSincronizacao.Servidor;
        public bool Vertical => View.Orientation == ApplicationViewOrientation.Portrait;

        public ObservableCollection<ResultadoSincronizacaoCliente> ResultadosCliente
        {
            get
            {
                using (var db = new ResultadosCliente())
                {
                    return db.Registro.GerarObs();
                }
            }
        }

        public ObservableCollection<ResultadoSincronizacaoServidor> ResultadosServer
        {
            get
            {
                using (var db = new ResultadosServidor())
                {
                    return db.Registro.GerarObs();
                }
            }
        }

    }
}
