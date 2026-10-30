using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ListarAlunosPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _dbHelper;
    private readonly bool _permitirBloqueio;

    public ListarAlunosPage(bool permitirBloqueio = false)
    {
        InitializeComponent();
        _permitirBloqueio = permitirBloqueio;

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
        var alunos = todos.Where(u => u.TipoPerfil.Contains("Aluno", StringComparison.OrdinalIgnoreCase)).ToList();

        if (collectionAlunos != null)
            collectionAlunos.ItemsSource = alunos;

        if (lblNenhumAluno != null)
            lblNenhumAluno.IsVisible = alunos.Count == 0;
    }

    private async void OnBloquearPlanoClicked(object sender, EventArgs e)
    {
        if (!_permitirBloqueio) return;

        if (sender is Button btn && btn.CommandParameter is UsuarioModel aluno)
        {
            aluno.IsBloqueado = !aluno.IsBloqueado;
            await _dbHelper.AtualizarUsuarioAsync(aluno);

            string status = aluno.IsBloqueado ? "bloqueado" : "desbloqueado";
            await DisplayAlert("Aviso", $"O plano do aluno {aluno.Nome} foi {status}.", "OK");
            await CarregarAlunosAsync();
        }
    }
}