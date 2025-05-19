using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lab8Csharp.Models;

public partial class ProbaE
{
    [Key]
    public int Id { get; set; }

    public string Distanta { get; set; } = null!;

    public string Stil { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<InscriereE> Inscrieres { get; set; } = new List<InscriereE>();
}
