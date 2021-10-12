using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PostClipboard
{
    public class ClipboardContent
    {
        public string content { get; set; }
        public ClipboardContent(string content)
        {
            this.content = content;
        }

    }
    public static class Clipboard
    {
        public static string GetText()
        {
            var powershell = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    FileName = "powershell",
                    Arguments = "-command \"Get-Clipboard\""
                }
            };

            powershell.Start();
            string text = powershell.StandardOutput.ReadToEnd();
            powershell.StandardOutput.Close();
            powershell.WaitForExit();
            return text.TrimEnd();
        }
    }


    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            ClipboardContent content = new ClipboardContent(Clipboard.GetText());
            string jsonString = JsonSerializer.Serialize(content);
            var data = new StringContent(jsonString, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            string url = args[0];
            string result = null;
            try
            {
                var response = await client.PostAsync(url, data);
                result = response.Content.ReadAsStringAsync().Result;
            } catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(result);
        }
    }
}
