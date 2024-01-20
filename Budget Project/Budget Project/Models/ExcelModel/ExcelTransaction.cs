namespace Budget_Project.Models.ExcelModel
{
    public class ExcelTransaction
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public int? CategoryId { get; set; }
        public int? AccountId { get; set; }
    }
}