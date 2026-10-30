using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class FormularioPage : ContentPage
{
    private readonly string _perfilSelecionado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public FormularioPage(string perfil)
    {
        InitializeComponent();
        _perfilSelecionado = perfil ?? "Aluno / Atleta";
        lblTituloPerfil.Text = $"Perfil: {_perfilSelecionado}";
        pickerAcao.SelectedIndex = 0;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        AtualizarVisibilidadeCampos();
    }

    public FormularioPage() : this("Aluno / Atleta") { }

    private void OnAcaoChanged(object? sender, EventArgs e)
    {
        AtualizarVisibilidadeCampos();
    }

    private void AtualizarVisibilidadeCampos()
    {
        bool isLogin = (pickerAcao.SelectedIndex == 0);
        layoutLogin.IsVisible = isLogin;

        string perfil = _perfilSelecionado?.Trim().ToLower() ?? "";

        layoutCadastroAluno.IsVisible = !isLogin && perfil.Contains("aluno");
        layoutCadastroPersonal.IsVisible = !isLogin && perfil.Contains("personal");
        layoutCadastroNutri.IsVisible = !isLogin && perfil.Contains("nutricionista");
        layoutCadastroAcademia.IsVisible = !isLogin && perfil.Contains("academia");
        layoutCadastroAdmin.IsVisible = !isLogin && (perfil.Contains("administrador") || perfil.Contains("admin"));
    }

    private async void OnSalvarClicked(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await button.ScaleTo(0.95, 50);
            await button.ScaleTo(1, 50);
        }

        if (pickerAcao.SelectedIndex == 0)
        {
            string emailLogin = txtEmail?.Text?.Trim() ?? string.Empty;
            string senhaLogin = txtSenha?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(emailLogin) || string.IsNullOrWhiteSpace(senhaLogin))
            {
                await DisplayAlert("Atenção", "Preencha o e-mail e a senha para entrar.", "OK");
                return;
            }

            var usuarioEncontrado = await _dbHelper.ObterPorEmailAsync(emailLogin);

            if (usuarioEncontrado == null || usuarioEncontrado.Senha != senhaLogin)
            {
                await DisplayAlert("Acesso Negado", "E-mail ou senha incorretos, ou nenhum cadastro encontrado.", "OK");
                return;
            }

            if (usuarioEncontrado.IsBloqueado)
            {
                string motivo = string.IsNullOrWhiteSpace(usuarioEncontrado.MotivoBloqueio)
                    ? "Violação dos termos de uso da plataforma."
                    : usuarioEncontrado.MotivoBloqueio;

                await DisplayAlert("CONTA BLOQUEADA 🚫", $"A sua conta foi bloqueada pelo Administrador.\n\nMotivo: {motivo}\n\nEntre em contato com o suporte para mais informações.", "OK");
                return;
            }

            await DisplayAlert("Sucesso", $"Bem-vindo de volta, {usuarioEncontrado.Nome}!", "OK");

            string tipo = usuarioEncontrado.TipoPerfil.ToLower();

            if (tipo.Contains("aluno"))
            {
                await Navigation.PushAsync(new Alunodashboardpage(usuarioEncontrado));
            }
            else if (tipo.Contains("personal"))
            {
                await Navigation.PushAsync(new PersonalDashboardPage(usuarioEncontrado));
            }
            else if (tipo.Contains("nutricionista"))
            {
                await Navigation.PushAsync(new NutricionistaDashboardPage(usuarioEncontrado));
            }
            else if (tipo.Contains("administrador") || tipo.Contains("admin"))
            {
                await Navigation.PushAsync(new AdminDashboardPage(usuarioEncontrado));
            }
            else if (tipo.Contains("academia"))
            {
                await Navigation.PushAsync(new AcademiaDashboardPage(usuarioEncontrado));
            }
            else
            {
                // Correção: Passa objetos UsuarioModel e nunca strings soltas
                await Navigation.PushAsync(new ProfissionaisPage(usuarioEncontrado, usuarioEncontrado));
            }

            Navigation.RemovePage(this);
        }
        else
        {
            string perfil = _perfilSelecionado?.Trim().ToLower() ?? "";
            string nome = string.Empty;
            string email = string.Empty;
            string senha = string.Empty;
            string documento = string.Empty;
            string especialidade = string.Empty;
            string objetivo = string.Empty;
            string telefone = string.Empty;
            string rua = string.Empty;
            string numero = string.Empty;
            DateTime nascimento = DateTime.Now;
            double altura = 0;
            double peso = 0;

            if (perfil.Contains("aluno"))
            {
                nome = txtNomeAluno?.Text?.Trim() ?? string.Empty;
                string cpf = txtCpfAluno?.Text?.Trim() ?? string.Empty;
                telefone = txtTelefoneAluno?.Text?.Trim() ?? string.Empty;
                rua = txtRuaAluno?.Text?.Trim() ?? string.Empty;
                numero = txtNumeroAluno?.Text?.Trim() ?? string.Empty;
                nascimento = dateNascAluno?.Date ?? DateTime.Now;

                email = txtEmailAluno?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaAluno?.Text ?? string.Empty;
                documento = cpf;

                double.TryParse(txtAlturaAluno?.Text?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out altura);
                double.TryParse(txtPesoAluno?.Text?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out peso);
                objetivo = pickerObjetivoAluno?.SelectedItem?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cpf) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios do Aluno.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("personal"))
            {
                nome = txtNomePersonal?.Text?.Trim() ?? string.Empty;
                string cpf = txtCpfPersonal?.Text?.Trim() ?? string.Empty;
                string cref = txtCrefPersonal?.Text?.Trim() ?? string.Empty;
                telefone = txtTelefonePersonal?.Text?.Trim() ?? string.Empty;
                nascimento = dateNascPersonal?.Date ?? DateTime.Now;
                email = txtEmailPersonal?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaPersonal?.Text ?? string.Empty;
                documento = $"CPF: {cpf} | CREF: {cref}";
                especialidade = txtEspecialidadesPersonal?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cref) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios do Personal.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("nutricionista"))
            {
                nome = txtNomeNutri?.Text?.Trim() ?? string.Empty;
                string cpf = txtCpfNutri?.Text?.Trim() ?? string.Empty;
                string crn = txtCrnNutri?.Text?.Trim() ?? string.Empty;
                telefone = txtTelefoneNutri?.Text?.Trim() ?? string.Empty;
                nascimento = dateNascNutri?.Date ?? DateTime.Now;
                email = txtEmailNutri?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaNutri?.Text ?? string.Empty;
                documento = $"CPF: {cpf} | CRN: {crn}";
                especialidade = txtEspecialidadesNutri?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(crn) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios da Nutricionista.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("academia"))
            {
                nome = txtNomeEmpresa?.Text?.Trim() ?? string.Empty;
                string cnpj = txtCnpjEmpresa?.Text?.Trim() ?? string.Empty;
                telefone = txtTelefoneEmpresa?.Text?.Trim() ?? string.Empty;
                rua = txtRuaEmpresa?.Text?.Trim() ?? string.Empty;
                numero = txtNumeroEmpresa?.Text?.Trim() ?? string.Empty;
                email = txtEmailEmpresa?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaEmpresa?.Text ?? string.Empty;
                documento = cnpj;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cnpj) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios da Academia.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("administrador"))
            {
                nome = txtNomeAdmin?.Text?.Trim() ?? string.Empty;
                string docAdmin = txtDocumentoAdmin?.Text?.Trim() ?? string.Empty;
                email = txtEmailAdmin?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaAdmin?.Text ?? string.Empty;
                documento = docAdmin;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(docAdmin) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios do Administrador.", "OK");
                    return;
                }
            }

            var emailExistente = await _dbHelper.ObterPorEmailAsync(email);
            if (emailExistente != null)
            {
                await DisplayAlert("Atenção", "Este e-mail já está cadastrado no sistema.", "OK");
                return;
            }

            int idadeCalculada = DateTime.Now.Year - nascimento.Year;
            if (nascimento.Date > DateTime.Now.AddYears(-idadeCalculada)) idadeCalculada--;

            var novoUsuario = new UsuarioModel
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                TipoPerfil = _perfilSelecionado,
                Documento = documento,
                Especialidades = especialidade,
                Objetivo = objetivo,
                Altura = altura,
                Peso = peso,
                Telefone = telefone,
                Rua = rua,
                Numero = numero,
                DataNascimento = nascimento,
                Idade = idadeCalculada
            };

            await _dbHelper.InserirUsuarioAsync(novoUsuario);

            await DisplayAlert("Sucesso", $"Cadastro de {_perfilSelecionado} realizado com sucesso!", "OK");

            pickerAcao.SelectedIndex = 0;
        }
    }
}