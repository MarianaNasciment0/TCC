using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class ProfissionaisPage : ContentPage
{
    private readonly UsuarioModel _usuarioLogado;
    private readonly UsuarioModel _profissionalSelecionado;

    public ProfissionaisPage(UsuarioModel usuarioLogado, UsuarioModel profissionalSelecionado)
    {
        InitializeComponent();
        _usuarioLogado = usuarioLogado ?? throw new ArgumentNullException(nameof(usuarioLogado));
        _profissionalSelecionado = profissionalSelecionado ?? throw new ArgumentNullException(nameof(profissionalSelecionado));

        PreencherDados();
    }

    private void PreencherDados()
    {
        if (lblNome != null) lblNome.Text = _profissionalSelecionado.Nome;
        if (lblTipoPerfil != null) lblTipoPerfil.Text = _profissionalSelecionado.TipoPerfil;
        if (lblEmail != null) lblEmail.Text = $"E-mail: {_profissionalSelecionado.Email}";
        if (lblTelefone != null) lblTelefone.Text = $"Telefone: {_profissionalSelecionado.Telefone}";
        if (lblDocumento != null) lblDocumento.Text = $"Registro: {_profissionalSelecionado.Documento}";
        if (lblEspecialidades != null) lblEspecialidades.Text = $"Especialidades: {_profissionalSelecionado.Especialidades}";
        if (lblPlanos != null) lblPlanos.Text = string.IsNullOrWhiteSpace(_profissionalSelecionado.DescricaoPlano) ? "Sem planos descritos." : _profissionalSelecionado.DescricaoPlano;
    }

    private async void OnChatComProfissionalClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChatPage(_usuarioLogado, _profissionalSelecionado));
    }
}