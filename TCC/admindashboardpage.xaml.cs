using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AdminDashboardPage : ContentPage
{
    private readonly UsuarioModel _adminLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public AdminDashboardPage(UsuarioModel admin)
    {
        InitializeComponent();
        _adminLogado = admin ?? throw new ArgumentNullException(nameof(admin));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (lblNomeAdmin != null)
            lblNomeAdmin.Text = $"Olá, {_adminLogado.Nome}!";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
    }

    private async void OnMensagensClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaConversasPage(_adminLogado));
    }

    private async void OnAcademiasClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AdminGerenciarUsuariosPage(_adminLogado, "Academia"));
    }

    private async void OnPersonalsClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AdminGerenciarUsuariosPage(_adminLogado, "Personal"));
    }

    private async void OnNutricionistasClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AdminGerenciarUsuariosPage(_adminLogado, "Nutricionista"));
    }

    private async void OnAlunosClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new AdminGerenciarUsuariosPage(_adminLogado, "Aluno"));
    }

    private async void OnExcluirContaClicked(object? sender, EventArgs e)
    {
        bool confirmacao = await DisplayAlert("Excluir Conta", "Tem certeza que deseja excluir sua conta de Administrador?", "Sim, Excluir", "Cancelar");
        if (confirmacao)
        {
            await _dbHelper.DeletarUsuarioAsync(_adminLogado);
            await DisplayAlert("Sucesso", "Conta excluída.", "OK");
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnSairClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}