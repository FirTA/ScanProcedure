using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScanProcedure.Models
{
    [Table("procedures")]
    public class Procedure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("procedure_name")]
        public string ProcedureName { get; set; } = string.Empty;

        // Navigation Properties
        public MappingProcedure? MappingProcedure { get; set; }
        public ICollection<MappingMenu> MappingMenus { get; set; } = new List<MappingMenu>();
    }
}
