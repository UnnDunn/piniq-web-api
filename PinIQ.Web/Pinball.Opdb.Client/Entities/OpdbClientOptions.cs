namespace Pinball.OpdbClient.Entities
{
    public class OpdbClientOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://opdb.org/api/";
    }
}
