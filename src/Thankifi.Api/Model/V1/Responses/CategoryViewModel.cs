namespace Thankifi.Api.Model.V1.Responses
{
    public record CategoryViewModel
    {
        public int Id { get; init; }
        public string Slug { get; init; }
        public string Title { get; init; }
    }
}