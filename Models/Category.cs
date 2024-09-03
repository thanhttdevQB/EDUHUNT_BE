namespace EDUHUNT_BE.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ScholarshipCategory> ScholarshipCategories { get; set; } = default!;
    }
}
