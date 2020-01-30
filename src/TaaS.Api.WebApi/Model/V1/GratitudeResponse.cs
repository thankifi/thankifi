namespace TaaS.Api.WebApi.Model.V1
{
    public class GratitudeResponse
    {
        public GratitudeResponse(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public int Id { get; set; }
        public string Text { get; set; }
    }
}