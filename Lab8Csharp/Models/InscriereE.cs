using System;
using System.Collections.Generic;

namespace Lab8Csharp.Models;

public partial class InscriereE
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }

    public int ProbaId { get; set; }

    public virtual ParticipantE Participant { get; set; } = null!;

    public virtual ProbaE Proba { get; set; } = null!;
}
