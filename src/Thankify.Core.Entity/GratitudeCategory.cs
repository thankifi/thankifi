namespace Thankify.Core.Entity
{
    public class GratitudeCategory
    {
        public int GratitudeId { get; set; }
        public Gratitude Gratitude { get; set; }
        
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}