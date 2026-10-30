using TCC.Helpers;
using TCC.Models;

namespace TCC
{
    public partial class GerenciarPlanosPage : ContentPage
    {
        private readonly SQLiteDatabaseHelper _dbHelper;
        private readonly UsuarioModel _profissional;

        public GerenciarPlanosPage(UsuarioModel profissional)
        {
            InitializeComponent();
            _profissional = profissional ?? throw new ArgumentNullException(nameof(profissional));

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "tcc_database.db3");
            _dbHelper = new SQLiteDatabaseHelper(dbPath);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarPlanosAsync();
        }

        private async Task CarregarPlanosAsync()
        {
            var planos = await _dbHelper.ListarPlanosPorProfissionalAsync(_profissional.Id);
            if (collectionPlanos != null)
            {
                collectionPlanos.ItemsSource = planos;
            }
        }

        private async void OnAdicionarPlanoClicked(object? sender, EventArgs e)
        {
            string nome = await DisplayPromptAsync("Novo Plano", "Digite o nome do plano:");
            if (string.IsNullOrWhiteSpace(nome)) return;

            string desc = await DisplayPromptAsync("Descrição", "Descreva o plano:");
            string valorStr = await DisplayPromptAsync("Valor (R$)", "Digite o preço:");

            if (double.TryParse(valorStr?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double valor))
            {
                var plano = new PlanoModel
                {
                    ProfissionalId = _profissional.Id,
                    NomePlano = nome,
                    Descricao = desc ?? "",
                    Valor = valor
                };

                await _dbHelper.SalvarPlanoAsync(plano);
                await CarregarPlanosAsync();
            }
        }

        private async void OnExcluirPlanoClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is PlanoModel plano)
            {
                await _dbHelper.DeletarPlanoAsync(plano);
                await CarregarPlanosAsync();
            }
        }
    }
}