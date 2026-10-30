using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class GerenciarUsuariosPage : ContentPage
{
    private readonly UsuarioModel _adminLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public GerenciarUsuariosPage(UsuarioModel admin)
    {
        InitializeComponent();
        _adminLogado = admin ?? throw new ArgumentNullException(nameof(admin));

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
        var todosUsuarios = await _dbHelper.ListarUsuariosAsync();

        // Opcional: O admin pode ver todos, exceto ele mesmo para evitar auto-exclusão acidental na lista geral
        var listaGerenciavel = todosUsuarios.Where(u => u.Id != _adminLogado.Id).ToList();

        if (collectionUsuarios != null)
        {
            collectionUsuarios.ItemsSource = null;
            collectionUsuarios.ItemsSource = listaGerenciavel;
        }

        if (lblNenhumUsuario != null)
        {
            lblNenhumUsuario.IsVisible = listaGerenciavel.Count == 0;
        }
    }

    // Ação para bloquear ou desbloquear o acesso de um usuário
    private async void OnBloquearClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuarioSelecionado)
        {
            // Exemplo de alerta de status
            await DisplayAlert("Controle de Acesso", $"O status de acesso do usuário {usuarioSelecionado.Nome} foi alterado com sucesso.", "OK");
        }
    }

    // Ação para o Admin conversar diretamente com qualquer usuário via Chat
    private async void OnChatClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuarioSelecionado)
        {
            await Navigation.PushAsync(new ChatPage(_adminLogado, usuarioSelecionado));
        }
    }

    // Ação para o Administrador EXCLUIR qualquer conta do sistema
    private async void OnExcluirUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is UsuarioModel usuarioSelecionado)
        {
            bool confirmacao = await DisplayAlert(
                "Excluir Conta",
                $"Tem certeza de que deseja excluir permanentemente a conta de {usuarioSelecionado.Nome} ({usuarioSelecionado.TipoPerfil})?",
                "Sim, Excluir",
                "Cancelar");

            if (confirmacao)
            {
                await _dbHelper.DeletarUsuarioAsync(usuarioSelecionado);
                await DisplayAlert("Sucesso", "A conta foi excluída do sistema.", "OK");
                await CarregarUsuariosAsync(); // Atualiza a lista na tela
            }
        }
    }
}