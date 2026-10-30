using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AcademiaDashboardPage : ContentPage
{
    private readonly UsuarioModel _academiaLogada;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public AcademiaDashboardPage(UsuarioModel academia)
    {
        InitializeComponent();
        _academiaLogada = academia ?? throw new ArgumentNullException(nameof(academia));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (lblNomeAcademia != null)
            lblNomeAcademia.Text = $"Olá, {_academiaLogada.Nome}!";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnMensagensClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaConversasPage(_academiaLogada));
    }

    private async void OnPersonalsClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AcademiaPersonaisPage(_academiaLogada));
    }

    private async void OnAlunosClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AcademiaAlunosPage(_academiaLogada));
    }

    // Ação para excluir exclusivamente a conta da academia logada
    private async void OnExcluirContaClicked(object? sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert(
            "Excluir Conta",
            "Tem certeza que deseja excluir permanentemente a conta desta academia? Esta ação não pode ser desfeita.",
            "Sim, Excluir",
            "Cancelar");

        if (confirmacao)
        {
            await _dbHelper.DeletarUsuarioAsync(_academiaLogada);
            await DisplayAlert("Sucesso", "A conta da academia foi excluída.", "OK");
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnSairClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}