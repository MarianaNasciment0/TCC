using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class EditarPlanoPersonalPage : ContentPage
{
    private readonly UsuarioModel _personalLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public EditarPlanoPersonalPage(UsuarioModel personal)
    {
        InitializeComponent();
        _personalLogado = personal ?? throw new ArgumentNullException(nameof(personal));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (editorDisponibilidade != null)
            editorDisponibilidade.Text = _personalLogado.Objetivo;

        if (txtValorAvulsa != null && _personalLogado.Valor > 0)
            txtValorAvulsa.Text = _personalLogado.Valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        if (pickerQtdPlanos != null)
            pickerQtdPlanos.SelectedIndex = 0;
    }

    private void OnQtdPlanosChanged(object? sender, EventArgs e)
    {
        int index = pickerQtdPlanos?.SelectedIndex ?? 0;
        int quantidade = index > 0 ? index : 0;

        if (cardPlano1 != null) cardPlano1.IsVisible = quantidade >= 1;
        if (cardPlano2 != null) cardPlano2.IsVisible = quantidade >= 2;
        if (cardPlano3 != null) cardPlano3.IsVisible = quantidade >= 3;
        if (cardPlano4 != null) cardPlano4.IsVisible = quantidade >= 4;
    }

    private async void OnSalvarPlanoClicked(object? sender, EventArgs e)
    {
        double.TryParse(txtValorAvulsa?.Text?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double valorAvulsa);
        _personalLogado.Valor = valorAvulsa;

        int index = pickerQtdPlanos?.SelectedIndex ?? 0;
        int quantidade = index > 0 ? index : 0;
        string planosConcatenados = string.Empty;

        if (valorAvulsa > 0)
        {
            planosConcatenados += $"🥊 Aula Avulsa: R$ {valorAvulsa:F2}\n\n";
        }

        // Plano 1
        if (quantidade >= 1 && cardPlano1 != null && cardPlano1.IsVisible)
        {
            string nome = txtNomePlano1?.Text?.Trim() ?? string.Empty;
            string valor = txtValorPlano1?.Text?.Trim() ?? "0";
            string beneficios = editorBeneficiosPlano1?.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nome))
                planosConcatenados += $"🔹 {nome} - R$ {valor}\n   Benefícios: {beneficios}\n";
        }

        // Plano 2
        if (quantidade >= 2 && cardPlano2 != null && cardPlano2.IsVisible)
        {
            string nome = txtNomePlano2?.Text?.Trim() ?? string.Empty;
            string valor = txtValorPlano2?.Text?.Trim() ?? "0";
            string beneficios = editorBeneficiosPlano2?.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nome))
            {
                if (!string.IsNullOrEmpty(planosConcatenados)) planosConcatenados += "\n";
                planosConcatenados += $"🔹 {nome} - R$ {valor}\n   Benefícios: {beneficios}\n";
            }
        }

        // Plano 3 (Corrigido o erro do '5' aqui)
        if (quantidade >= 3 && cardPlano3 != null && cardPlano3.IsVisible)
        {
            string nome = txtNomePlano3?.Text?.Trim() ?? string.Empty;
            string valor = txtValorPlano3?.Text?.Trim() ?? "0";
            string beneficios = editorBeneficiosPlano3?.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nome))
            {
                if (!string.IsNullOrEmpty(planosConcatenados)) planosConcatenados += "\n";
                planosConcatenados += $"🔹 {nome} - R$ {valor}\n   Benefícios: {beneficios}\n";
            }
        }

        // Plano 4
        if (quantidade >= 4 && cardPlano4 != null && cardPlano4.IsVisible)
        {
            string nome = txtNomePlano4?.Text?.Trim() ?? string.Empty;
            string valor = txtValorPlano4?.Text?.Trim() ?? "0";
            string beneficios = editorBeneficiosPlano4?.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(nome))
            {
                if (!string.IsNullOrEmpty(planosConcatenados)) planosConcatenados += "\n";
                planosConcatenados += $"🔹 {nome} - R$ {valor}\n   Benefícios: {beneficios}\n";
            }
        }

        _personalLogado.DescricaoPlano = planosConcatenados.Trim();
        _personalLogado.Objetivo = editorDisponibilidade?.Text?.Trim() ?? string.Empty;

        await _dbHelper.AtualizarUsuarioAsync(_personalLogado);

        await DisplayAlert("Sucesso", "Preços, planos e disponibilidades atualizados com sucesso!", "OK");
        await Navigation.PopAsync();
    }
}