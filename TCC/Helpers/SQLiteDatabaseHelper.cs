using SQLite;
using TCC.Models;

namespace TCC.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string dbPath)
        {
            _conn = new SQLiteAsyncConnection(dbPath);
            _conn.CreateTableAsync<UsuarioModel>().Wait();
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> InserirUsuarioAsync(UsuarioModel usuario)
        {
            return _conn.InsertAsync(usuario);
        }

        // Método de atualização (Semana 4)
        public Task<int> AtualizarUsuarioAsync(UsuarioModel usuario)
        {
            return _conn.UpdateAsync(usuario);
        }

        // Método de exclusão (Semana 4)
        public Task<int> DeletarUsuarioAsync(UsuarioModel usuario)
        {
            return _conn.DeleteAsync(usuario);
        }

        public Task<UsuarioModel> ObterPorEmailAsync(string email)
        {
            return _conn.Table<UsuarioModel>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public Task<UsuarioModel> ObterPorDocumentoAsync(string documento)
        {
            return _conn.Table<UsuarioModel>().Where(u => u.Documento == documento).FirstOrDefaultAsync();
        }

        public Task<List<UsuarioModel>> ListarUsuariosAsync()
        {
            return _conn.Table<UsuarioModel>().ToListAsync();
        }

        public Task<List<UsuarioModel>> ObterPorTipoPerfilAsync(string tipoPerfil)
        {
            return _conn.Table<UsuarioModel>()
                        .Where(u => u.TipoPerfil.ToLower().Contains(tipoPerfil.ToLower()))
                        .ToListAsync();
        }
    }
}