using SQLite;

namespace TCC.Models
{
    public class UsuarioModel 
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string TipoPerfil { get; set; } = string.Empty; // "Aluno / Atleta", "Personal Trainer", "Nutricionista", "Academia", "Administrador"
        public string Documento { get; set; } = string.Empty; // CPF, CREF, CRN ou CNPJ

        // Campos específicos opcionais
        public string Especialidades { get; set; } = string.Empty; // Para Personal ou Nutricionista
        public string Objetivo { get; set; } = string.Empty; // Para Aluno
        public double Altura { get; set; }
        public double Peso { get; set; }
    }
}