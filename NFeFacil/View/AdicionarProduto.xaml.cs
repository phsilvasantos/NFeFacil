﻿using NFeFacil.ItensBD;
using NFeFacil.Log;
using NFeFacil.Validacao;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AdicionarProduto : Page, IEsconde
    {
        private ProdutoDI Prod => DataContext as ProdutoDI;
        private ILog Log = new Popup();

        public AdicionarProduto()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = (ProdutoDI)e.Parameter;
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            if (new ValidadorProduto(Prod).Validar(Log))
            {
                using (var db = new AplicativoContext())
                {
                    db.Add(new ProdutoDI(Prod));
                    db.SaveChanges();
                }
                Log.Escrever(TitulosComuns.Sucesso, "Produto salvo com sucesso.");
                Propriedades.Intercambio.Retornar();
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Propriedades.Intercambio.Retornar();
        }

        public async Task EsconderAsync()
        {
            ocultarGrid.Begin();
            await Task.Delay(250);
        }
    }
}
