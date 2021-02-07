namespace Pinball.OpdbClient.Entities
{
    public class OpdbClientOptions
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; }

        public OpdbClientOptions()
        {
            BaseUrl = "https://opdb.org/api/";
        }
    }
}
