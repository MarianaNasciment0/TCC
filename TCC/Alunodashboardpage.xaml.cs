using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class Alunodashboardpage : ContentPage
{
    private readonly UsuarioModel _alunoLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public Alunodashboardpage(UsuarioModel aluno)
    {
        InitializeComponent();
        _alunoLogado = aluno ?? throw new ArgumentNullException(nameof(aluno));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (lblNomeAluno != null)
            lblNomeAluno.Text = $"Olá, {_alunoLogado.Nome}!";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Verifica em tempo real se a conta foi bloqueada pelo Administrador
        var alunoAtualizado = await _dbHelper.ObterPorEmailAsync(_alunoLogado.Email);
        if (alunoAtualizado != null && alunoAtualizado.IsBloqueado)
        {
            await DisplayAlert("Conta Bloqueada", $"Sua conta foi bloqueada pelo administrador.\n\nMotivo: {alunoAtualizado.MotivoBloqueio}", "Sair");
            await Navigation.PopToRootAsync();
        }
    }

    // Ação do chat unificado
    private async void OnMensagensClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaConversasPage(_alunoLogado));
    }

    // Ação para listar Personal Trainers
    private async void OnPersonalsClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaGenericaProfissionaisPage(_alunoLogado, "Personal"));
    }

    // Ação para listar Nutricionistas
    private async void OnNutricionistasClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaGenericaProfissionaisPage(_alunoLogado, "Nutricionista"));
    }

    // Ação para listar Academias
    private async void OnAcademiasClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaGenericaProfissionaisPage(_alunoLogado, "Academia"));
    }

    // Ação para o aluno excluir a sua própria conta
    private async void OnExcluirContaClicked(object? sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert("Excluir Conta", "Tem certeza que deseja excluir sua conta permanentemente? Esta ação não pode ser desfeita.", "Sim, Excluir", "Cancelar");
        if (confirmacao)
        {
            await _dbHelper.DeletarUsuarioAsync(_alunoLogado);
            await DisplayAlert("Sucesso", "Sua conta foi excluída com sucesso.", "OK");
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnSairClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}