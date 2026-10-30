using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class EditarPlanoNutriPage : ContentPage
{
    private readonly UsuarioModel _nutriLogada;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public EditarPlanoNutriPage(UsuarioModel nutri)
    {
        InitializeComponent();
        _nutriLogada = nutri ?? throw new ArgumentNullException(nameof(nutri));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        CarregarDadosAtuais();
    }

    private void CarregarDadosAtuais()
    {
        if (txtValorConsulta != null)
            txtValorConsulta.Text = _nutriLogada.Valor > 0 ? _nutriLogada.Valor.ToString("F2") : string.Empty;

        if (editorDisponibilidade != null)
            editorDisponibilidade.Text = _nutriLogada.Objetivo;

        if (txtRuaConsultorio != null)
            txtRuaConsultorio.Text = _nutriLogada.Rua;

        if (txtNumeroConsultorio != null)
            txtNumeroConsultorio.Text = _nutriLogada.Numero;

        // Ajusta o Picker de acordo com o que já estiver gravado
        if (pickerModalidade != null)
        {
            if (_nutriLogada.DescricaoPlano.Contains("Presencial e Online", StringComparison.OrdinalIgnoreCase) ||
                _nutriLogada.DescricaoPlano.Contains("Online e Presencial", StringComparison.OrdinalIgnoreCase))
            {
                pickerModalidade.SelectedIndex = 2;
            }
            else if (_nutriLogada.DescricaoPlano.Contains("Presencial", StringComparison.OrdinalIgnoreCase))
            {
                pickerModalidade.SelectedIndex = 1;
            }
            else
            {
                pickerModalidade.SelectedIndex = 0; // Padrão: Apenas Online
            }
        }
    }

    private void OnModalidadeChanged(object? sender, EventArgs e)
    {
        int index = pickerModalidade?.SelectedIndex ?? 0;
        // Se for Index 1 (Apenas Presencial) ou 2 (Online e Presencial), exibe o campo de endereço
        bool precisaEndereco = (index == 1 || index == 2);

        if (cardEndereco != null)
            cardEndereco.IsVisible = precisaEndereco;
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        double.TryParse(txtValorConsulta?.Text?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double valor);

        string modalidade = pickerModalidade?.SelectedItem?.ToString() ?? "Apenas Online";
        string rua = txtRuaConsultorio?.Text?.Trim() ?? string.Empty;
        string numero = txtNumeroConsultorio?.Text?.Trim() ?? string.Empty;
        string disponibilidade = editorDisponibilidade?.Text?.Trim() ?? string.Empty;

        // Se for apenas Online, limpa o endereço
        if (pickerModalidade?.SelectedIndex == 0)
        {
            rua = "Atendimento 100% Online";
            numero = string.Empty;
        }

        // Atualiza os campos no modelo
        _nutriLogada.Valor = valor;
        _nutriLogada.DescricaoPlano = $"Modalidade: {modalidade}\nValor da Consulta: R$ {valor:F2}";
        _nutriLogada.Rua = rua;
        _nutriLogada.Numero = numero;
        _nutriLogada.Objetivo = disponibilidade;

        await _dbHelper.AtualizarUsuarioAsync(_nutriLogada);

        await DisplayAlert("Sucesso", "Configurações da consultoria salvas com sucesso!", "OK");
        await Navigation.PopAsync();
    }
}