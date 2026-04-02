namespace MultiLanguage.Domain.Entities
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CultureCode { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<Resource> Resources { get; set; }
    }
}
