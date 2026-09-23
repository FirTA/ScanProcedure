using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScanProcedure.Models
{
    [Table("mapping_menu")]
    public class MappingMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("menu_id")]
        public int MenuId { get; set; }

        [Column("procedure_id")]
        public int? ProcedureId { get; set; }

        [Column("table_id")]
        public int? TableId { get; set; }

        [Column("table_access")]
        public AccessType TableAccess { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(MenuId))]
        public Menu Menu { get; set; } = null!;

        [ForeignKey(nameof(ProcedureId))]
        public Procedure? Procedure { get; set; }

        [ForeignKey(nameof(TableId))]
        public Table? Table { get; set; }
    }
}
