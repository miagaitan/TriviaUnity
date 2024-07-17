using Postgrest.Models;
using Postgrest.Attributes;
using System.Collections.Generic;

public class intentos : BaseModel
{
    [Column("id"), PrimaryKey]
    public int id { get; set; }

    [Column("puntaje")]
    public int puntaje { get; set; }

    [Column("usuario_id")]
    public int usuario_id { get; set; }

    [Column("categoria_id")]
    public int categoria_id { get; set; }
}

