﻿using NFeFacil.ItensBD;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x416

namespace NFeFacil
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal static MainPage Current { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            Current = this;
            AnalisarBarraTituloAsync();
            btnRetornar.Click += (x, y) => Retornar();
            SystemNavigationManager.GetForCurrentView().BackRequested += (x,e) =>
            {
                e.Handled = true;
                Retornar();
            };
            frmPrincipal.CacheSize = 4;
            Navegar<Login.EscolhaEmitente>();
            using (var db = new AplicativoContext())
            {
                Propriedades.EmitenteAtivo = db.Emitentes.FirstOrDefault();
            }
        }

        private void AnalisarBarraTituloAsync()
        {
            var familia = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (familia.Contains("Mobile"))
            {
                btnRetornar.Visibility = Visibility.Collapsed;
                var barra = StatusBar.GetForCurrentView();
                var cor = new View.Estilos.Auxiliares.BibliotecaCores().Cor1;
                barra.BackgroundColor = cor;
                barra.BackgroundOpacity = 1;
            }
            else if (familia.Contains("Desktop"))
            {
                Window.Current.CoreWindow.KeyDown += (x, y) => System.Diagnostics.Debug.WriteLine(y.VirtualKey);
                CoreApplicationViewTitleBar tb = CoreApplication.GetCurrentView().TitleBar;
                tb.ExtendViewIntoTitleBar = true;
                tb.LayoutMetricsChanged += (sender, e) => TitleBar.Height = sender.Height;

                Window.Current.SetTitleBar(MainTitleBar);
                Window.Current.Activated += (sender, e) => TitleBar.Opacity = e.WindowActivationState != CoreWindowActivationState.Deactivated ? 1 : 0.5;

                var novoTB = ApplicationView.GetForCurrentView().TitleBar;
                var corBackground = new Color { A = 0 };
                novoTB.ButtonBackgroundColor = corBackground;
                novoTB.ButtonInactiveBackgroundColor = corBackground;
                novoTB.ButtonHoverBackgroundColor = new Color { A = 50 };
                novoTB.ButtonPressedBackgroundColor = new Color { A = 100 };
            }
        }

        public void Navegar<T>(object parametro = null) where T : Page, new()
        {
            frmPrincipal.Navigate(typeof(T), parametro);
        }

        public void SeAtualizar(Symbol símbolo, string texto)
        {
            txtTitulo.Text = texto;
            symTitulo.Content = new SymbolIcon(símbolo);
            if (frmPrincipal.Content is IHambuguer hambuguer)
            {
                menuTemporario.ItemsSource = hambuguer.ConteudoMenu;
                menuTemporario.SelectedIndex = 0;
                hambuguer.MainMudou += (sender, e) => menuTemporario.SelectedIndex = ((NewIndexEventArgs)e).NewIndex;
                splitView.CompactPaneLength = 48;
            }
            else
            {
                menuTemporario.ItemsSource = null;
                splitView.CompactPaneLength = 0;
            }
        }

        public void SeAtualizar(string glyph, string texto)
        {
            txtTitulo.Text = texto;
            symTitulo.Content = new FontIcon { Glyph = glyph };
        }

        public async void Retornar()
        {
            if (frmPrincipal.Content is IValida retorna)
            {
                if (!await retorna.Verificar())
                {
                    return;
                }
            }
            else if ((frmPrincipal.Content as FrameworkElement).DataContext is IValida retornaDC)
            {
                if (!await retornaDC.Verificar())
                {
                    return;
                }
            }

            if (frmPrincipal.CanGoBack)
            {
                frmPrincipal.GoBack();
            }
            else
            {
                var familia = AnalyticsInfo.VersionInfo.DeviceFamily;
                if (familia.Contains("Mobile"))
                {
                    Application.Current.Exit();
                }
            }
        }

        private void AbrirHamburguer(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        void SelecaoMudou(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var item = e.AddedItems[0];
                if (item is EmitenteDI novoEmit)
                {
                    Propriedades.EmitenteAtivo = novoEmit;
                }
                else if (item is Vendedor vendedor)
                {
                    Propriedades.VendedorAtivo = vendedor;
                }
            }
        }

        private void MudouSubpaginaEscolhida(object sender, SelectionChangedEventArgs e)
        {
            if (menuTemporario.ItemsSource != null)
            {
                var hamb = (IHambuguer)frmPrincipal.Content;
                hamb.AtualizarMain(menuTemporario.SelectedIndex);
            }
        }

        public async Task AtualizarInformaçõesGerais()
        {
            grdInfoGeral.Visibility = Visibility.Visible;
            using (var db = new AplicativoContext())
            {
                var img = db.Imagens.Find(Propriedades.EmitenteAtivo.Id);
                if (img != null)
                {
                    imgLogotipo.Source = await img.GetSourceAsync();
                }
                txtNomeEmitente.Text = Propriedades.EmitenteAtivo.Nome;
                txtNomeEmpresa.Text = Propriedades.EmitenteAtivo.NomeFantasia;

                if (Propriedades.VendedorAtivo != null)
                {
                    img = db.Imagens.Find(Propriedades.VendedorAtivo.Id);
                    if (img != null)
                    {
                        imgVendedor.Source = await img.GetSourceAsync();
                    }
                    txtNomeVendedor.Text = Propriedades.VendedorAtivo.Nome;
                }
                else
                {
                    txtNomeVendedor.Text = "Administrador";
                }
            }
        }
    }
}
