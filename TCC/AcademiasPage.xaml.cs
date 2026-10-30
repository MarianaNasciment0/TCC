using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AcademiasPage : ContentPage
{
    private readonly UsuarioModel? _usuarioLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;
    private List<UsuarioModel> _listaAcademias = new();

    // Construtor caso seja aberto por um usuário logado (ex: Aluno)
    public AcademiasPage(UsuarioModel? usuario = null)
    {
        InitializeComponent();
        _usuarioLogado = usuario;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAcademiasAsync();
    }

    private async Task CarregarAcademiasAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();

        // Filtra apenas os perfis cadastrados como "Academia"
        _listaAcademias = todos.Where(u =>
            u.TipoPerfil != null &&
            u.TipoPerfil.Contains("Academia", StringComparison.OrdinalIgnoreCase)
        ).ToList();

        AtualizarLista();
    }

    private void AtualizarLista()
    {
        if (collectionAcademias != null)
        {
            collectionAcademias.ItemsSource = null;
            collectionAcademias.ItemsSource = _listaAcademias;
        }

        if (lblNenhumaAcademia != null)
            lblNenhumaAcademia.IsVisible = _listaAcademias.Count == 0;
    }

    private void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is UsuarioModel academiaClicada)
        {
            academiaClicada.IsExpanded = !academiaClicada.IsExpanded;
            AtualizarLista();
        }
    }

    private async void OnChatAcademiaClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel academiaEscolhida)
        {
            if (_usuarioLogado != null)
            {
                await Navigation.PushAsync(new ChatPage(_usuarioLogado, academiaEscolhida));
            }
            else
            {
                await DisplayAlert("Aviso", "Faça login com sua conta de aluno para iniciar o chat.", "OK");
            }
        }
    }
}