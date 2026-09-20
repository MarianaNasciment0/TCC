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
        pickerAcao.SelectedIndex = 0; // Inicia em Login

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
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1, 50);
        }

        if (pickerAcao.SelectedIndex == 0)
        {
            // === LOGIN ===
            string emailLogin = txtEmail?.Text?.Trim() ?? string.Empty;
            string senhaLogin = txtSenha?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(emailLogin) || string.IsNullOrWhiteSpace(senhaLogin))
            {
                await DisplayAlertAsync("Atenção", "Preencha o e-mail e a senha para entrar.", "OK");
                return;
            }

            var usuarioEncontrado = await _dbHelper.ObterPorEmailAsync(emailLogin);

            if (usuarioEncontrado == null || usuarioEncontrado.Senha != senhaLogin)
            {
                await DisplayAlertAsync("Acesso Negado", "E-mail ou senha incorretos, ou nenhum cadastro encontrado.", "OK");
                return;
            }

            await DisplayAlertAsync("Sucesso", $"Bem-vindo de volta, {usuarioEncontrado.Nome}!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            // === CADASTRO ===
            string perfil = _perfilSelecionado?.Trim().ToLower() ?? "";
            string nome = string.Empty;
            string email = string.Empty;
            string senha = string.Empty;
            string documento = string.Empty;
            string especialidade = string.Empty;

            if (perfil.Contains("aluno"))
            {
                nome = txtNomeAluno?.Text?.Trim() ?? string.Empty;
                string cpf = txtCpfAluno?.Text?.Trim() ?? string.Empty;
                email = txtEmailAluno?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaAluno?.Text ?? string.Empty;
                documento = cpf;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cpf) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios do Aluno.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("personal"))
            {
                nome = txtNomePersonal?.Text?.Trim() ?? string.Empty;
                string cref = txtCrefPersonal?.Text?.Trim() ?? string.Empty;
                email = txtEmailPersonal?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaPersonal?.Text ?? string.Empty;
                documento = cref;
                especialidade = txtEspecialidadesPersonal?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cref) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios do Personal.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("nutricionista"))
            {
                nome = txtNomeNutri?.Text?.Trim() ?? string.Empty;
                string crn = txtCrnNutri?.Text?.Trim() ?? string.Empty;
                email = txtEmailNutri?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaNutri?.Text ?? string.Empty;
                documento = crn;
                especialidade = txtEspecialidadesNutri?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(crn) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios da Nutricionista.", "OK");
                    return;
                }
            }
            else if (perfil.Contains("academia"))
            {
                nome = txtNomeEmpresa?.Text?.Trim() ?? string.Empty;
                string cnpj = txtCnpjEmpresa?.Text?.Trim() ?? string.Empty;
                email = txtEmailEmpresa?.Text?.Trim() ?? string.Empty;
                senha = txtSenhaEmpresa?.Text ?? string.Empty;
                documento = cnpj;

                if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(cnpj) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
                {
                    await DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios da Academia.", "OK");
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
                    await DisplayAlertAsync("Atenção", "Preencha todos os campos obrigatórios do Administrador.", "OK");
                    return;
                }
            }

            // 1. Verifica se o e-mail já existe
            var emailExistente = await _dbHelper.ObterPorEmailAsync(email);
            if (emailExistente != null)
            {
                await DisplayAlertAsync("Atenção", "Este e-mail já está cadastrado no sistema.", "OK");
                return;
            }

            // 2. Verifica se o documento já existe
            var todosUsuarios = await _dbHelper.ListarUsuariosAsync();
            bool documentoExiste = todosUsuarios.Any(u => !string.IsNullOrEmpty(u.Documento) && u.Documento.Trim().Equals(documento, StringComparison.OrdinalIgnoreCase));

            if (documentoExiste)
            {
                await DisplayAlertAsync("Atenção", "Este número de registro já se encontra cadastrado no sistema.", "OK");
                return;
            }

            // Salva no Banco de Dados SQLite
            var novoUsuario = new UsuarioModel
            {
                Nome = nome,
                Email = email,
                Senha = senha,
                TipoPerfil = _perfilSelecionado,
                Documento = documento,
                Especialidades = especialidade
            };

            await _dbHelper.InserirUsuarioAsync(novoUsuario);

            await DisplayAlertAsync("Sucesso", $"Cadastro de {_perfilSelecionado} realizado com sucesso!", "OK");

            pickerAcao.SelectedIndex = 0; // Volta para a tela de login
        }
    }
}