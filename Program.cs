using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length <= 0){
            Console.WriteLine("Currently only works with two URLS: \n 1) http://localhost:5260/api/Edi/process \n 2) http://localhost:5260/api/Edi/parseisa");
            Console.WriteLine("Usage: WebApiCaller [edi-filepath] [url]");
            Console.WriteLine("Provide valid params & try again.");
            return;
        }
        // args[0] is the edi file name
        Console.WriteLine($"Posting data from {args[0]} to EdiParseApi...");
        var allLines = File.ReadAllLines(args[0]);
        if (allLines[0].Length > 105){
            allLines[0] = allLines[0].Insert(105,Environment.NewLine);
        }
        string ediFileData = string.Join('\n',allLines);
        var targetUrl = String.Empty;
        if (args.Length == 2){
            targetUrl = args[1];
        }else{
            targetUrl = "http://localhost:5260/api/Edi/process";
        }

        try
        {
            // Call the API asynchronously
            string result = await CallApiAsync(targetUrl, args[0], ediFileData);
            Console.WriteLine($"Response from API: {result}");
            if (result.ToUpper().Contains("INVALID")){
                File.AppendAllText("ErrorsDuringRun.log", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}✅{args[0]}✅{result}✅{allLines[0]}{Environment.NewLine}");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            File.AppendAllText("ErrorsDuringRun.log", $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}✅{args[0]}✅{ex.Message}{Environment.NewLine}");
        }
    }

    static async Task<string> CallApiAsync(string userUrl, string filePath, string fileData)
    {
        using (HttpClient client = new HttpClient())
        {
            string apiUrl = userUrl;

            // Prepare the form-data content
            var formData = new MultipartFormDataContent
            {
                { new StringContent(filePath), "filename" },
                { new StringContent(fileData), "ediContent" }
            };

            // Send POST request
            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);

            // Handle response
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
