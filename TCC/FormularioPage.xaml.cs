namespace TCC;

public partial class FormularioPage : ContentPage
{
    private string _perfilSelecionado;

    public FormularioPage(string perfil)
    {
        InitializeComponent();
        _perfilSelecionado = perfil;
        lblTituloPerfil.Text = $"Perfil: {_perfilSelecionado}";
        pickerAcao.SelectedIndex = 0; // Inicia na aba de Login
        AtualizarVisibilidadeCampos();
    }

    private void OnAcaoChanged(object? sender, EventArgs e)
    {
        AtualizarVisibilidadeCampos();
    }

    private void AtualizarVisibilidadeCampos()
    {
        bool isLogin = (pickerAcao.SelectedIndex == 0);

        // O login aparece apenas se a opção selecionada for Entrar
        layoutLogin.IsVisible = isLogin;

        string perfil = _perfilSelecionado?.Trim().ToLower() ?? "";

        // Controla qual formulário de cadastro aparece com base no botão clicado na MainPage
        layoutCadastroAluno.IsVisible = !isLogin && perfil.Contains("aluno");
        layoutCadastroPersonal.IsVisible = !isLogin && perfil.Contains("personal");

        // Agora o layout da academia cobre tanto "academia" quanto "desempenho"
        layoutCadastroAcademia.IsVisible = !isLogin && (perfil.Contains("academia") || perfil.Contains("desempenho"));
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        if (pickerAcao.SelectedIndex == 0)
        {
            // Validação de Login com segurança de cadastro prévio
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                await DisplayAlertAsync("Atenção", "Preencha o e-mail e a senha para entrar.", "OK");
                return;
            }

            bool usuarioCadastrado = Preferences.Get($"cadastrado_{_perfilSelecionado}", false);

            if (!usuarioCadastrado)
            {
                await DisplayAlertAsync("Acesso Negado", "Nenhum cadastro encontrado para este perfil. Por favor, selecione 'Fazer Cadastro / Registro' primeiro.", "OK");
                return;
            }

            await DisplayAlertAsync("Sucesso", "Login efetuado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            // Validação dos campos obrigatórios dependendo do perfil
            string perfil = _perfilSelecionado?.Trim().ToLower() ?? "";
            bool camposValidos = true;

            if (perfil.Contains("aluno"))
            {
                if (string.IsNullOrWhiteSpace(txtNomeAluno.Text) || string.IsNullOrWhiteSpace(txtCpfAluno.Text))
                    camposValidos = false;
            }
            else if (perfil.Contains("personal"))
            {
                if (string.IsNullOrWhiteSpace(txtNomePersonal.Text) || string.IsNullOrWhiteSpace(txtCrefPersonal.Text))
                    camposValidos = false;
            }
            else if (perfil.Contains("academia") || perfil.Contains("desempenho"))
            {
                if (string.IsNullOrWhiteSpace(txtNomeEmpresa.Text) || string.IsNullOrWhiteSpace(txtCnpjEmpresa.Text))
                    camposValidos = false;
            }

            if (!camposValidos)
            {
                await DisplayAlertAsync("Atenção", "Por favor, preencha os campos obrigatórios do cadastro.", "OK");
                return;
            }

            // Salva na memória que este perfil possui cadastro criado
            Preferences.Set($"cadastrado_{_perfilSelecionado}", true);

            await DisplayAlertAsync("Sucesso", $"Cadastro de {_perfilSelecionado} realizado com sucesso! Agora você já pode fazer o login.", "OK");

            // Retorna para a aba de login automaticamente
            pickerAcao.SelectedIndex = 0;
        }
    }
}