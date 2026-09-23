using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScanProcedure.Models
{
    [Table("tables")]
    public class Table
    {
        [Key]
        [DatabaseGenerated(databaseGeneratedOption: DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        [Column("table_name")]
        public string TableName { get; set; } = string.Empty;
        // Navigation Properties
        public ICollection<MappingProcedure> MappingProcedures { get; set; } = new List<MappingProcedure>();
        public ICollection<MappingMenu> MappingMenus { get; set; } = new List<MappingMenu>();
    }
}
