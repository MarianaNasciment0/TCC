using SQLite;

namespace TCC.Models
{
    public class PlanoModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProfissionalId { get; set; }

        // Propriedade principal para o nome
        public string Nome { get; set; } = string.Empty;

        // Alias para aceitar 'NomePlano' caso algum arquivo do projeto o chame por esse nome
        [Ignore]
        public string NomePlano
        {
            get => Nome;
            set => Nome = value;
        }

        public string Descricao { get; set; } = string.Empty;
        public double Valor { get; set; }
    }
}   