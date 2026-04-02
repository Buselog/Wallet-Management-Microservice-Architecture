namespace MultiLanguage.Domain.Entities
{
    public class Resource
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
    }
}
