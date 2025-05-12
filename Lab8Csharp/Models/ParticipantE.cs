using System;
using System.Collections.Generic;

namespace Lab8Csharp.Models;

public partial class ParticipantE
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public virtual ICollection<InscriereE> Inscrieres { get; set; } = new List<InscriereE>();
}
