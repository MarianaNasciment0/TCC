using SQLite;

namespace TCC.Models
{
    public class VinculoAcademiaModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int AcademiaId { get; set; }
        public int UsuarioId { get; set; }
        public string TipoUsuario { get; set; } = string.Empty; // "Aluno" ou "Personal"
        public bool IsBloqueado { get; set; } = false;
    }
}