using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class AdminGerenciarUsuariosPage : ContentPage
{
    private readonly UsuarioModel _adminLogado;
    private readonly string _filtroCategoria;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public AdminGerenciarUsuariosPage(UsuarioModel admin, string filtroCategoria)
    {
        InitializeComponent();
        _adminLogado = admin ?? throw new ArgumentNullException(nameof(admin));
        _filtroCategoria = filtroCategoria ?? string.Empty;

        if (lblTituloSecao != null)
            lblTituloSecao.Text = $"Gerenciar: {_filtroCategoria}s";

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarUsuariosAsync();
    }

    private async Task CarregarUsuariosAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();

        var filtrados = todos.Where(u =>
            u.Id != _adminLogado.Id &&
            u.TipoPerfil != null &&
            u.TipoPerfil.Contains(_filtroCategoria, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        if (collectionUsuarios != null)
        {
            collectionUsuarios.ItemsSource = null;
            collectionUsuarios.ItemsSource = filtrados;
        }

        if (lblNenhum != null)
            lblNenhum.IsVisible = filtrados.Count == 0;
    }

    // Ação de Bloquear/Desbloquear com Motivo Personalizado
    private async void OnBloquearClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuario)
        {
            if (!usuario.IsBloqueado)
            {
                // Solicita o motivo do bloqueio ao Administrador
                string motivo = await DisplayPromptAsync("Bloquear Conta", $"Digite o motivo do bloqueio para {usuario.Nome}:", "Bloquear", "Cancelar", placeholder: "Ex: Violação dos termos de uso");

                if (motivo == null) return; // Cancelou

                usuario.IsBloqueado = true;
                usuario.MotivoBloqueio = string.IsNullOrWhiteSpace(motivo) ? "Nenhum motivo informado." : motivo;

                await _dbHelper.AtualizarUsuarioAsync(usuario);

                // Envia uma mensagem automática de notificação de bloqueio para o chat do usuário
                await _dbHelper.EnviarMensagemAsync(new MensagemModel
                {
                    RemetenteId = _adminLogado.Id,
                    DestinatarioId = usuario.Id,
                    Texto = $"⚠️ AVISO DO SISTEMA: Sua conta foi bloqueada. Motivo: {usuario.MotivoBloqueio}",
                    DataHora = DateTime.Now,
                    Lida = false
                });

                await DisplayAlert("Sucesso", $"O usuário {usuario.Nome} foi bloqueado e notificado.", "OK");
            }
            else
            {
                // Desbloqueia a conta
                usuario.IsBloqueado = false;
                usuario.MotivoBloqueio = string.Empty;

                await _dbHelper.AtualizarUsuarioAsync(usuario);

                await _dbHelper.EnviarMensagemAsync(new MensagemModel
                {
                    RemetenteId = _adminLogado.Id,
                    DestinatarioId = usuario.Id,
                    Texto = "✅ AVISO DO SISTEMA: Sua conta foi desbloqueada. Você já pode utilizar o aplicativo normalmente.",
                    DataHora = DateTime.Now,
                    Lida = false
                });

                await DisplayAlert("Sucesso", $"O usuário {usuario.Nome} foi desbloqueado.", "OK");
            }

            await CarregarUsuariosAsync();
        }
    }

    private async void OnChatClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuario)
        {
            await Navigation.PushAsync(new ChatPage(_adminLogado, usuario));
        }
    }

    private async void OnExcluirUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuario)
        {
            bool confirmacao = await DisplayAlert(
                "Excluir Conta",
                $"Tem certeza que deseja excluir permanentemente a conta de {usuario.Nome}?",
                "Sim, Excluir",
                "Cancelar");

            if (confirmacao)
            {
                await _dbHelper.DeletarUsuarioAsync(usuario);
                await DisplayAlert("Sucesso", "A conta foi excluída do sistema.", "OK");
                await CarregarUsuariosAsync();
            }
        }
    }
}