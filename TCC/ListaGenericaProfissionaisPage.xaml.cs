using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ListaGenericaProfissionaisPage : ContentPage
{
    private readonly UsuarioModel _usuarioLogado;
    private readonly string _categoriaFiltro;
    private readonly SQLiteDatabaseHelper _dbHelper;
    private List<ItemExibicaoModel> _listaItens = new();

    public ListaGenericaProfissionaisPage(UsuarioModel usuario, string categoria)
    {
        InitializeComponent();
        _usuarioLogado = usuario ?? throw new ArgumentNullException(nameof(usuario));
        _categoriaFiltro = categoria ?? "Profissional";

        if (lblTituloSecao != null)
            lblTituloSecao.Text = _categoriaFiltro switch
            {
                "Academia" => "Academias Parceiras",
                "Personal" => "Personal Trainers",
                "Nutricionista" => "Nutricionistas",
                _ => "Profissionais"
            };

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        var todos = await _dbHelper.ListarUsuariosAsync();

        var filtrados = todos.Where(u =>
            u.Id != _usuarioLogado.Id &&
            u.TipoPerfil != null &&
            u.TipoPerfil.Contains(_categoriaFiltro, StringComparison.OrdinalIgnoreCase)
        ).ToList();

        _listaItens = filtrados.Select(u => new ItemExibicaoModel
        {
            UsuarioOriginal = u,
            Nome = u.Nome,
            Email = u.Email,
            Telefone = u.Telefone,
            Documento = u.Documento,
            TipoPerfil = u.TipoPerfil,
            IconePerfil = _categoriaFiltro switch
            {
                "Academia" => "🏢",
                "Personal" => "🏋️‍♂️",
                "Nutricionista" => "🥗",
                _ => "👤"
            },
            SubtituloCard = _categoriaFiltro == "Academia" ? $"Endereço: {u.Rua}, {u.Numero}" : $"Especialidade: {u.Especialidades}"
        }).ToList();

        AtualizarLista();
    }

    private void AtualizarLista()
    {
        if (collectionItens != null)
        {
            collectionItens.ItemsSource = null;
            collectionItens.ItemsSource = _listaItens;
        }

        if (lblNenhumItem != null)
            lblNenhumItem.IsVisible = _listaItens.Count == 0;
    }

    // Ação ao clicar no botão de chat direto do usuário
    private async void OnChatDiretoClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ItemExibicaoModel itemEscolhido)
        {
            await Navigation.PushAsync(new ChatPage(_usuarioLogado, itemEscolhido.UsuarioOriginal));
        }
    }
}

// Model auxiliar de visualização
public class ItemExibicaoModel
{
    public UsuarioModel UsuarioOriginal { get; set; } = new();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string TipoPerfil { get; set; } = string.Empty;
    public string IconePerfil { get; set; } = "👤";
    public string SubtituloCard { get; set; } = string.Empty;
}