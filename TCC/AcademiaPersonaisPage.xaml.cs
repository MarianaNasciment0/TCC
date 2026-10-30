using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AcademiaPersonaisPage : ContentPage
{
    private readonly UsuarioModel _academiaLogada;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public AcademiaPersonaisPage(UsuarioModel academia)
    {
        InitializeComponent();
        _academiaLogada = academia ?? throw new ArgumentNullException(nameof(academia));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPersonaisAsync();
    }

    private async Task CarregarPersonaisAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();
        var personals = todos.Where(u => u.TipoPerfil != null && u.TipoPerfil.Contains("Personal", StringComparison.OrdinalIgnoreCase)).ToList();

        if (collectionPersonais != null)
        {
            collectionPersonais.ItemsSource = null;
            collectionPersonais.ItemsSource = personals;
        }
    }

    private async void OnBloquearClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel personal)
        {
            await DisplayAlert("Aviso", $"Acesso alterado para {personal.Nome}", "OK");
        }
    }

    private async void OnChatDiretoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel personal)
        {
            await Navigation.PushAsync(new ChatPage(_academiaLogada, personal));
        }
    }

    private async void OnExcluirContaClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel personal)
        {
            bool confirmar = await DisplayAlert("Confirmação", $"Deseja realmente excluir a conta de {personal.Nome}?", "Sim", "Não");
            if (confirmar)
            {
                await _dbHelper.DeletarUsuarioAsync(personal);
                await CarregarPersonaisAsync();
            }
        }
    }
}