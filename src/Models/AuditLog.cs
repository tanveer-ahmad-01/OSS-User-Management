using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public AuditAction Action { get; set; }

    public Guid? UserId { get; set; }

    public Guid? EntityId { get; set; }

    [MaxLength(100)]
    public string? EntityType { get; set; }

    [MaxLength(1000)]
    public string? Details { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? ProjectId { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

