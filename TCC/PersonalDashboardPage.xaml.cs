using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class PersonalDashboardPage : ContentPage
{
    private readonly UsuarioModel _personalLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public PersonalDashboardPage(UsuarioModel personal)
    {
        InitializeComponent();
        _personalLogado = personal ?? throw new ArgumentNullException(nameof(personal));
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (lblNomePersonal != null)
            lblNomePersonal.Text = $"Olá, {_personalLogado.Nome}!";

        var atualizado = await _dbHelper.ObterPorEmailAsync(_personalLogado.Email);
        if (atualizado != null)
        {
            _personalLogado.Valor = atualizado.Valor;
            _personalLogado.DescricaoPlano = atualizado.DescricaoPlano;
        }
    }

    private async void OnAlunosClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListarAlunosPage());
    }

    private async void OnAcademiasClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaGenericaProfissionaisPage(_personalLogado, "Academia"));
    }

    private async void OnFormularioMedicoClicked(object? sender, TappedEventArgs e)
    {
        // Navegação para formulário médico, se aplicável
    }

    // ABRIR O CHAT UNIFICADO
    private async void OnMensagensClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaConversasPage(_personalLogado));
    }

    private async void OnEditarPlanoClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditarPlanoPersonalPage(_personalLogado));
    }

    private async void OnSairClicked(object? sender, EventArgs e) => await Navigation.PopToRootAsync();

    private async void OnDeletarContaClicked(object? sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Excluir Conta", "Tem certeza que deseja excluir sua conta?", "Sim", "Não");
        if (confirmar)
        {
            await _dbHelper.DeletarUsuarioAsync(_personalLogado);
            await Navigation.PopToRootAsync();
        }
    }
}