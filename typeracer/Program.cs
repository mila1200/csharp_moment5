using ClassLibrary;

class Program
{
    static async Task Main(string[] args)
    {
        ApiSentences apiSentences = new ApiSentences();
        string url = "https://api.kanye.rest";

        string apiData = await apiSentences.sentencesFromApi(url);
        Console.WriteLine(apiData);
    }
}