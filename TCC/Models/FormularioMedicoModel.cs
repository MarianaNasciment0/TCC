    using SQLite;

    namespace TCC.Models
    {
        public class FormularioMedicoModel
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public int UsuarioId { get; set; }
            public string LesoesDoencas { get; set; } = string.Empty;
            public string UsoMedicamentos { get; set; } = string.Empty;
            public string CirurgiasRecentes { get; set; } = string.Empty;
            public string Observacoes { get; set; } = string.Empty;
        }
    }