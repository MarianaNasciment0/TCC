using SQLite;

namespace TCC.Models;

public class MensagemModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int RemetenteId { get; set; }

    public int DestinatarioId { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTime DataHora { get; set; } = DateTime.Now;

    // Novo campo para sabermos se a mensagem foi lida ou não
    public bool Lida { get; set; } = false;

    // Propriedade auxiliar para controlar o alinhamento visual dos balões de chat
    [Ignore]
    public bool IsMinhaMensagem { get; set; }
}