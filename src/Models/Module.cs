using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

[Table("Modules")]
public class Module
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // Unique code for API access

    public Guid? ParentModuleId { get; set; } // For submodules

    public int Order { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    // Multi-tenant support
    [MaxLength(100)]
    public string? ProjectId { get; set; }

    // Navigation properties
    [ForeignKey("ParentModuleId")]
    public virtual Module? ParentModule { get; set; }

    public virtual ICollection<Module> SubModules { get; set; } = new List<Module>();

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}

