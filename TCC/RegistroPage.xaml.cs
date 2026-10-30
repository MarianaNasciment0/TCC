using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class RegistroPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _dbHelper;

    public RegistroPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        if (pickerTipoPerfil != null)
            pickerTipoPerfil.SelectedIndex = 0;
    }

    private async void OnCadastrarClicked(object? sender, EventArgs e)
    {
        string nome = txtNome?.Text?.Trim() ?? string.Empty;
        string email = txtEmail?.Text?.Trim() ?? string.Empty;
        string senha = txtSenha?.Text?.Trim() ?? string.Empty;
        string tipoPerfil = pickerTipoPerfil?.SelectedItem?.ToString() ?? "Aluno";

        string telefone = txtTelefone?.Text?.Trim() ?? string.Empty;
        DateTime dataNascimento = datePickerNascimento?.Date ?? new DateTime(2000, 1, 1);

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Aviso", "Preencha os campos obrigatórios (Nome, E-mail e Senha).", "OK");
            return;
        }

        var usuarioExistente = await _dbHelper.ObterPorEmailAsync(email);
        if (usuarioExistente != null)
        {
            await DisplayAlert("Erro", "Já existe uma conta cadastrada com este e-mail.", "OK");
            return;
        }

        var novoUsuario = new UsuarioModel
        {
            Nome = nome,
            Email = email,
            Senha = senha,
            TipoPerfil = tipoPerfil,
            Telefone = telefone,
            DataNascimento = dataNascimento
        };

        await _dbHelper.SalvarUsuarioAsync(novoUsuario);

        await DisplayAlert("Sucesso", "Conta criada com sucesso!", "OK");
        await Navigation.PopAsync();
    }
}