using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AcademiaAlunosPage : ContentPage
{
    private readonly UsuarioModel _academiaLogada;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public AcademiaAlunosPage(UsuarioModel academia)
    {
        InitializeComponent();
        _academiaLogada = academia ?? throw new ArgumentNullException(nameof(academia));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAlunosAsync();
    }

    private async Task CarregarAlunosAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();
        var alunos = todos.Where(u => u.TipoPerfil != null && u.TipoPerfil.Contains("Aluno", StringComparison.OrdinalIgnoreCase)).ToList();

        if (collectionAlunos != null)
        {
            collectionAlunos.ItemsSource = null;
            collectionAlunos.ItemsSource = alunos;
        }
    }

    private async void OnBloquearClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel aluno)
        {
            await DisplayAlert("Aviso", $"Acesso alterado para {aluno.Nome}", "OK");
        }
    }

    private async void OnChatDiretoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel aluno)
        {
            await Navigation.PushAsync(new ChatPage(_academiaLogada, aluno));
        }
    }

    private async void OnExcluirContaClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel aluno)
        {
            bool confirmar = await DisplayAlert("Confirmação", $"Deseja realmente excluir a conta de {aluno.Nome}?", "Sim", "Não");
            if (confirmar)
            {
                await _dbHelper.DeletarUsuarioAsync(aluno);
                await CarregarAlunosAsync();
            }
        }
    }
}