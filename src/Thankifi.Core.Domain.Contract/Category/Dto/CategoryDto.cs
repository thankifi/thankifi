namespace Thankifi.Core.Domain.Contract.Category.Dto
{
    public record CategoryDto
    {
        public int Id { get; init; }
        public string Slug { get; init; }
    }
}