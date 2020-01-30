namespace TaaS.Core.Entity
{
    public class Gratitude
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public GratitudeType Type { get; set; }
        public string Text { get; set; }
    }

    public enum GratitudeType
    {
        Standard,
        Named,
        Signed,
        NamedAndSigned
    }
}