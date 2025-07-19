using System;
using System.Collections.Generic;

namespace RootsApp.Models;

public partial class EmailVerification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string VerificationCode { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public virtual User User { get; set; } = null!;
}
