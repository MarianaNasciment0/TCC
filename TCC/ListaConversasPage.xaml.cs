using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ListaConversasPage : ContentPage
{
    private readonly UsuarioModel _usuarioLogado;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public ListaConversasPage(UsuarioModel usuario)
    {
        InitializeComponent();
        _usuarioLogado = usuario ?? throw new ArgumentNullException(nameof(usuario));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarConversasAtivasAsync();
    }

    private async Task CarregarConversasAtivasAsync()
    {
        var todosUsuarios = await _dbHelper.ListarUsuariosAsync();
        var listaExibicao = new List<ContatoConversaItemModel>();

        foreach (var usuario in todosUsuarios.Where(u => u.Id != _usuarioLogado.Id))
        {
            var mensagens = await _dbHelper.ObterMensagensEntreUsuariosAsync(_usuarioLogado.Id, usuario.Id);

            if (mensagens != null && mensagens.Count > 0)
            {
                bool temNaoLida = await _dbHelper.PossuiMensagensNaoLidasDeUsuarioAsync(_usuarioLogado.Id, usuario.Id);

                listaExibicao.Add(new ContatoConversaItemModel
                {
                    UsuarioOriginal = usuario,
                    Nome = usuario.Nome,
                    TipoPerfil = usuario.TipoPerfil ?? "Usuário",
                    TemMensagemNaoLida = temNaoLida
                });
            }
        }

        if (collectionContatos != null)
        {
            collectionContatos.ItemsSource = null;
            collectionContatos.ItemsSource = listaExibicao;
        }

        if (lblNenhumContato != null)
        {
            lblNenhumContato.IsVisible = listaExibicao.Count == 0;
        }
    }

    private async void OnContatoSelecionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ContatoConversaItemModel itemSelecionado)
        {
            ((CollectionView)sender).SelectedItem = null;
            await Navigation.PushAsync(new ChatPage(_usuarioLogado, itemSelecionado.UsuarioOriginal));
        }
    }
}

// Model auxiliar para exibir o status de não lida na lista
public class ContatoConversaItemModel
{
    public UsuarioModel UsuarioOriginal { get; set; } = new();
    public string Nome { get; set; } = string.Empty;
    public string TipoPerfil { get; set; } = string.Empty;
    public bool TemMensagemNaoLida { get; set; }
}