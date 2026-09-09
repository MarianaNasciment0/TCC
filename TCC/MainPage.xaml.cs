namespace TCC;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnPersonalClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        await Navigation.PushAsync(new FormularioPage("Personal Trainer"));
    }

    private async void OnAlunoClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        await Navigation.PushAsync(new FormularioPage("Aluno / Atleta"));
    }

    private async void OnAcademiaClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        // Passando exatamente a palavra "Academia" para o formulário reconhecer
        await Navigation.PushAsync(new FormularioPage("Academia"));
    }

    private async void OnDesempenhoClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        await Navigation.PushAsync(new FormularioPage("Desempenho e Metas"));
    }
}