using SQLite;

namespace TCC.Models
{
    public class Produto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        public double Preco { get; set; }
    }
}