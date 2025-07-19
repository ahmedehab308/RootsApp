using System;
using System.Collections.Generic;

namespace RootsApp.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool? IsEmailConfirmed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
