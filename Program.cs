using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // args[0] is the edi file name
        Console.WriteLine($"Posting data from {args[0]} to EdiParseApi...");
        string ediFileData = string.Join('\n',File.ReadAllLines(args[0]));

        try
        {
            // Call the API asynchronously
            string result = await CallApiAsync(args[0], ediFileData);
            Console.WriteLine($"Response from API: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task<string> CallApiAsync(string firstString, string secondString)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = "http://localhost:5260/api/Edi/process"; // Replace with your API endpoint URL

            // Prepare the form-data content
            var formData = new MultipartFormDataContent
            {
                { new StringContent(firstString), "filename" },
                { new StringContent(secondString), "ediContent" }
            };

            // Send POST request
            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Handle response
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
