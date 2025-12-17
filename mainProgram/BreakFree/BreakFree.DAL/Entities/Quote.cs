namespace BreakFree.DAL.Entities
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Quotes")]
    public class Quote
    {
        [Column("quote_id")]
        public int QuoteId { get; set; }

        [Column("text")]
        public string Text { get; set; } = string.Empty;
    }
}
