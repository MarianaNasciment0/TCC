using TCC.Helpers;
using TCC.Models;

namespace TCC;

public partial class FormularioMedicoPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _dbHelper;
    private readonly UsuarioModel _usuarioLogado;
    private FormularioMedicoModel? _formExistente;

    public FormularioMedicoPage(UsuarioModel usuario)
    {
        InitializeComponent();
        _usuarioLogado = usuario ?? throw new ArgumentNullException(nameof(usuario));

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _formExistente = await _dbHelper.ObterFormularioPorUsuarioAsync(_usuarioLogado.Id);
        if (_formExistente != null)
        {
            txtLesoes.Text = _formExistente.LesoesDoencas;
            txtMedicamentos.Text = _formExistente.UsoMedicamentos;
            txtCirurgias.Text = _formExistente.CirurgiasRecentes;
            txtObs.Text = _formExistente.Observacoes;
        }
    }

    private async void OnSalvarFormularioClicked(object sender, EventArgs e)
    {
        if (_formExistente == null)
        {
            _formExistente = new FormularioMedicoModel { UsuarioId = _usuarioLogado.Id };
        }

        _formExistente.LesoesDoencas = txtLesoes.Text?.Trim() ?? "";
        _formExistente.UsoMedicamentos = txtMedicamentos.Text?.Trim() ?? "";
        _formExistente.CirurgiasRecentes = txtCirurgias.Text?.Trim() ?? "";
        _formExistente.Observacoes = txtObs.Text?.Trim() ?? "";

        await _dbHelper.SalvarFormularioMedicoAsync(_formExistente);
        await DisplayAlert("Sucesso", "Formulário médico salvo com sucesso!", "OK");
        await Navigation.PopAsync();
    }
}