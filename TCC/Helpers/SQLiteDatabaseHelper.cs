using SQLite;
using TCC.Models;

namespace TCC.Helpers
{
    public class SQLiteDatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public SQLiteDatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<UsuarioModel>().Wait();
            _database.CreateTableAsync<FormularioMedicoModel>().Wait();
            _database.CreateTableAsync<PlanoModel>().Wait();
            _database.CreateTableAsync<VinculoAcademiaModel>().Wait();
            _database.CreateTableAsync<MensagemModel>().Wait();

            InicializarUsuariosPadraoAsync().ConfigureAwait(false);
        }

        private async Task InicializarUsuariosPadraoAsync()
        {
            var totalUsuarios = await _database.Table<UsuarioModel>().CountAsync();
            if (totalUsuarios == 0)
            {
                await _database.InsertAsync(new UsuarioModel { Nome = "Administrador", Email = "admin@tcc.com", Senha = "123", TipoPerfil = "Administrador" });
                await _database.InsertAsync(new UsuarioModel { Nome = "Aluno Teste", Email = "aluno@tcc.com", Senha = "123", TipoPerfil = "Aluno / Atleta" });
                await _database.InsertAsync(new UsuarioModel { Nome = "Academia Parceira", Email = "academia@tcc.com", Senha = "123", TipoPerfil = "Academia" });
                await _database.InsertAsync(new UsuarioModel { Nome = "Personal Trainer", Email = "personal@tcc.com", Senha = "123", TipoPerfil = "Personal" });
                await _database.InsertAsync(new UsuarioModel { Nome = "Nutricionista", Email = "nutri@tcc.com", Senha = "123", TipoPerfil = "Nutricionista" });
            }
        }

        public Task<List<UsuarioModel>> ListarUsuariosAsync()
        {
            return _database.Table<UsuarioModel>().ToListAsync();
        }

        public Task<UsuarioModel> ObterPorEmailAsync(string email)
        {
            return _database.Table<UsuarioModel>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public Task<int> InserirUsuarioAsync(UsuarioModel usuario)
        {
            return _database.InsertAsync(usuario);
        }

        public Task<int> AtualizarUsuarioAsync(UsuarioModel usuario)
        {
            return _database.UpdateAsync(usuario);
        }

        public Task<int> SalvarUsuarioAsync(UsuarioModel usuario)
        {
            if (usuario.Id != 0)
                return _database.UpdateAsync(usuario);
            else
                return _database.InsertAsync(usuario);
        }

        public Task<int> DeletarUsuarioAsync(UsuarioModel usuario)
        {
            return _database.DeleteAsync(usuario);
        }

        // --- PLANOS ---
        public Task<int> SalvarPlanoAsync(PlanoModel plano)
        {
            if (plano.Id != 0)
                return _database.UpdateAsync(plano);
            else
                return _database.InsertAsync(plano);
        }

        public Task<List<PlanoModel>> ListarPlanosPorProfissionalAsync(int profissionalId)
        {
            return _database.Table<PlanoModel>().Where(p => p.ProfissionalId == profissionalId).ToListAsync();
        }

        public Task<int> DeletarPlanoAsync(PlanoModel plano)
        {
            return _database.DeleteAsync(plano);
        }

        // --- FORMULÁRIO MÉDICO E VÍNCULOS ---
        public Task<int> SalvarFormularioMedicoAsync(FormularioMedicoModel formulario)
        {
            if (formulario.Id != 0)
                return _database.UpdateAsync(formulario);
            else
                return _database.InsertAsync(formulario);
        }

        public Task<FormularioMedicoModel> ObterFormularioPorUsuarioAsync(int usuarioId)
        {
            return _database.Table<FormularioMedicoModel>().Where(f => f.UsuarioId == usuarioId).FirstOrDefaultAsync();
        }

        // --- CHAT E MENSAGENS ---
        public Task<int> EnviarMensagemAsync(MensagemModel mensagem)
        {
            return _database.InsertAsync(mensagem);
        }

        public async Task<List<MensagemModel>> ObterMensagensEntreUsuariosAsync(int usuario1Id, int usuario2Id)
        {
            var mensagens = await _database.Table<MensagemModel>()
                .Where(m => (m.RemetenteId == usuario1Id && m.DestinatarioId == usuario2Id) ||
                            (m.RemetenteId == usuario2Id && m.DestinatarioId == usuario1Id))
                .ToListAsync();

            return mensagens.OrderBy(m => m.DataHora).ToList();
        }

        public async Task MarcarMensagensComoLidasAsync(int usuarioLogadoId, int remetenteId)
        {
            var mensagensNaoLidas = await _database.Table<MensagemModel>()
                .Where(m => m.RemetenteId == remetenteId && m.DestinatarioId == usuarioLogadoId && !m.Lida)
                .ToListAsync();

            foreach (var msg in mensagensNaoLidas)
            {
                msg.Lida = true;
                await _database.UpdateAsync(msg);
            }
        }

        public async Task<bool> PossuiMensagensNaoLidasDeUsuarioAsync(int usuarioLogadoId, int outroUsuarioId)
        {
            var count = await _database.Table<MensagemModel>()
                .Where(m => m.RemetenteId == outroUsuarioId && m.DestinatarioId == usuarioLogadoId && !m.Lida)
                .CountAsync();

            return count > 0;
        }
    }
}