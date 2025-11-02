using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

[Table("Features")]
public class Feature
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    public Guid ModuleId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    // Multi-tenant support
    [MaxLength(100)]
    public string? ProjectId { get; set; }

    // Navigation properties
    [ForeignKey("ModuleId")]
    public virtual Module Module { get; set; } = null!;

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

