namespace TCC;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnAlunoClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FormularioPage("Aluno / Atleta"));
    }

    private async void OnPersonalClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FormularioPage("Personal Trainer"));
    }

    private async void OnNutricionistaClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FormularioPage("Nutricionista"));
    }

    private async void OnAcademiaClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FormularioPage("Academia"));
    }

    private async void OnAdminClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new FormularioPage("Administrador"));
    }
}