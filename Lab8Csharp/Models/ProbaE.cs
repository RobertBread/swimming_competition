using System;
using System.Collections.Generic;

namespace Lab8Csharp.Models;

public partial class ProbaE
{
    public int Id { get; set; }

    public string Distanta { get; set; } = null!;

    public string Stil { get; set; } = null!;

    public virtual ICollection<InscriereE> Inscrieres { get; set; } = new List<InscriereE>();
}
