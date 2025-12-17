namespace BreakFree.DAL.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SOSActions")]
    public class SOSAction
    {
        [Key]
        [Column("action_id")]
        public int ActionId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("text")]
        public string Text { get; set; } = string.Empty;
    }
}
