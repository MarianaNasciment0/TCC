using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ListaProfissionaisPage : ContentPage
{
    private readonly UsuarioModel _usuarioLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;
    private List<UsuarioModel> _listaProfissionais = new();

    public ListaProfissionaisPage(UsuarioModel usuario)
    {
        InitializeComponent();
        _usuarioLogado = usuario ?? throw new ArgumentNullException(nameof(usuario));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarProfissionaisAsync();
    }

    private async Task CarregarProfissionaisAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();

        // Filtra apenas contas que sejam Personal Trainer ou Nutricionista
        _listaProfissionais = todos.Where(u =>
            u.Id != _usuarioLogado.Id &&
            u.TipoPerfil != null &&
            (u.TipoPerfil.Contains("Personal", StringComparison.OrdinalIgnoreCase) ||
             u.TipoPerfil.Contains("Nutricionista", StringComparison.OrdinalIgnoreCase))
        ).ToList();

        AtualizarLista();
    }

    private void AtualizarLista()
    {
        if (collectionProfissionais != null)
        {
            collectionProfissionais.ItemsSource = null;
            collectionProfissionais.ItemsSource = _listaProfissionais;
        }

        if (lblNenhumProfissional != null)
            lblNenhumProfissional.IsVisible = _listaProfissionais.Count == 0;
    }

    private void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is UsuarioModel profissionalClicado)
        {
            profissionalClicado.IsExpanded = !profissionalClicado.IsExpanded;
            AtualizarLista();
        }
    }

    private async void OnChatDiretoClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel profissionalEscolhido)
        {
            await Navigation.PushAsync(new ChatPage(_usuarioLogado, profissionalEscolhido));
        }
    }
}