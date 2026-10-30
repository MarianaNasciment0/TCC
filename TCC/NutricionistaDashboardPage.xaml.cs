using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class NutricionistaDashboardPage : ContentPage
{
    private readonly UsuarioModel _nutriLogada;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public NutricionistaDashboardPage(UsuarioModel nutri)
    {
        InitializeComponent();
        _nutriLogada = nutri ?? throw new ArgumentNullException(nameof(nutri));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (lblNomeNutri != null)
            lblNomeNutri.Text = $"Olá, {_nutriLogada.Nome}!";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var atualizado = await _dbHelper.ObterPorEmailAsync(_nutriLogada.Email);
        if (atualizado != null)
        {
            _nutriLogada.DescricaoPlano = atualizado.DescricaoPlano;
            _nutriLogada.Objetivo = atualizado.Objetivo;
            _nutriLogada.Telefone = atualizado.Telefone;
            _nutriLogada.Valor = atualizado.Valor;
        }
    }

    private async void OnAlunosClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListarAlunosPage(permitirBloqueio: false));
    }

    // ABRIR O CHAT UNIFICADO IGUAL AO PERSONAL
    private async void OnMensagensClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ListaConversasPage(_nutriLogada));
    }

    private async void OnEditarPlanoClicked(object? sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new EditarPlanoNutriPage(_nutriLogada));
    }

    private async void OnSairClicked(object? sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}