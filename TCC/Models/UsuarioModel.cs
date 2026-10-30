using SQLite;

namespace TCC.Models;

public class UsuarioModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Senha { get; set; } = string.Empty;

    public string TipoPerfil { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public string Documento { get; set; } = string.Empty;

    public string Cref { get; set; } = string.Empty;

    public string Especialidades { get; set; } = string.Empty;

    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;

    // Campos essenciais para o Bloqueio e Notificação
    public bool IsBloqueado { get; set; } = false;
    public string MotivoBloqueio { get; set; } = string.Empty;

    public double Altura { get; set; }
    public double Peso { get; set; }
    public DateTime DataNascimento { get; set; } = DateTime.Now;

    [Ignore]
    public int Idade { get; set; }

    [Ignore]
    public bool IsExpanded { get; set; }

    [Ignore]
    public double Valor { get; set; }

    [Ignore]
    public string Objetivo { get; set; } = string.Empty;

    [Ignore]
    public string DescricaoPlano { get; set; } = string.Empty;
}