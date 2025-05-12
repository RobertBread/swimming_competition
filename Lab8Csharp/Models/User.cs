using System;
using System.Collections.Generic;

namespace Lab8Csharp.Models;

public partial class UserE
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string HashedPassword { get; set; } = null!;
}
