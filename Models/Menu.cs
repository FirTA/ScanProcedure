using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScanProcedure.Models
{
    [Table("menus")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("nama_menu")]
        public string NamaMenu { get; set; } = string.Empty;

        // Navigation Properties
        public MappingMenu? MappingMenu { get; set; }

    }
}
