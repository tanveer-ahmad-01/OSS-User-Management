using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [MaxLength(500)]
    public string? ReplacedByToken { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    public bool IsActive => RevokedAt == null && !IsExpired;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

