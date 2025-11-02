using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

[Table("RolePermissions")]
public class RolePermission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid RoleId { get; set; }

    [Required]
    public Guid PermissionId { get; set; }

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    public string? GrantedBy { get; set; }

    // Navigation properties
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("PermissionId")]
    public virtual Permission Permission { get; set; } = null!;
}

