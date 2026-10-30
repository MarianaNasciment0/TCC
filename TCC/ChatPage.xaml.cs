using Microsoft.Maui.Controls;
using System.Globalization;
using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ChatPage : ContentPage
{
    private readonly UsuarioModel _usuarioLogado;
    private readonly UsuarioModel _destinatario;
    private readonly SQLiteDatabaseHelper _dbHelper;

    public ChatPage(UsuarioModel usuarioLogado, UsuarioModel destinatario)
    {
        InitializeComponent();
        _usuarioLogado = usuarioLogado ?? throw new ArgumentNullException(nameof(usuarioLogado));
        _destinatario = destinatario ?? throw new ArgumentNullException(nameof(destinatario));

        // Define os Converters no ResourceDictionary da página dinamicamente para alinhar as mensagens
        Resources.Add("BooleanToColorConverter", new BooleanToColorConverter());
        Resources.Add("BooleanToAlignmentConverter", new BooleanToAlignmentConverter());

        // Define o nome e o perfil do destinatário de forma garantida no topo
        if (lblNomeDestinatario != null)
            lblNomeDestinatario.Text = !string.IsNullOrEmpty(_destinatario.Nome) ? _destinatario.Nome : "Usuário";

        if (lblTipoPerfilDestinatario != null)
            lblTipoPerfilDestinatario.Text = !string.IsNullOrEmpty(_destinatario.TipoPerfil) ? _destinatario.TipoPerfil : "Contato";

        Title = lblNomeDestinatario?.Text;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Marca as mensagens como lidas ao abrir a conversa
        await _dbHelper.MarcarMensagensComoLidasAsync(_usuarioLogado.Id, _destinatario.Id);

        await CarregarMensagensAsync();
    }

    private async Task CarregarMensagensAsync()
    {
        var mensagens = await _dbHelper.ObterMensagensEntreUsuariosAsync(_usuarioLogado.Id, _destinatario.Id);

        foreach (var msg in mensagens)
        {
            msg.IsMinhaMensagem = (msg.RemetenteId == _usuarioLogado.Id);
        }

        if (collectionMensagens != null)
        {
            collectionMensagens.ItemsSource = null;
            collectionMensagens.ItemsSource = mensagens;

            // Rola para a última mensagem se houver itens
            if (mensagens.Count > 0)
            {
                var ultimaMensagem = mensagens[mensagens.Count - 1];
                collectionMensagens.ScrollTo(ultimaMensagem, position: ScrollToPosition.End, animate: true);
            }
        }
    }

    private async void OnEnviarClicked(object sender, EventArgs e)
    {
        string textoMensagem = txtMensagem?.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(textoMensagem)) return;

        var novaMsg = new MensagemModel
        {
            RemetenteId = _usuarioLogado.Id,
            DestinatarioId = _destinatario.Id,
            Texto = textoMensagem,
            DataHora = DateTime.Now,
            Lida = false
        };

        await _dbHelper.EnviarMensagemAsync(novaMsg);

        if (txtMensagem != null)
            txtMensagem.Text = string.Empty;

        await CarregarMensagensAsync();
    }
}

// Conversores auxiliares para alinhar e colorir os balões de conversa (Minha mensagem vs Mensagem do outro)
public class BooleanToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isMinha = value is bool b && b;
        return isMinha ? Color.FromArgb("#005C4B") : Color.FromArgb("#202C33"); // Estilo WhatsApp (Verde escuro / Cinza escuro)
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BooleanToAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isMinha = value is bool b && b;
        return isMinha ? LayoutOptions.End : LayoutOptions.Start;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}