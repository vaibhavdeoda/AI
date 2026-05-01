using Microsoft.SemanticKernel;
using System.ComponentModel;
using HtmlAgilityPack;
using System.Net.Http;

public class WebScraperPlugin
{
    private readonly HttpClient _httpClient = new();
    private readonly Random _random = new();

    [KernelFunction]
    [Description("Searches the web for a topic and returns the top snippets without using an API.")]
    public async Task<string> SearchWebAsync(
        [Description("The topic to search for")] string query)
    {
        try
        {
           Console.WriteLine($"Scraping the web for: {query}... 🌐");

           // 1. Stealth Delay: Wait 1-3 seconds before searching to mimic human behavior
            await Task.Delay(_random.Next(1000, 3000));

            // We use DuckDuckGo's HTML-only version for easy scraping
            string url = $"https://html.duckduckgo.com/html/?q={Uri.EscapeDataString(query)}";
            
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            // Cycle through a realistic User-Agent
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");

            var response = await _httpClient.SendAsync(request);            
            
            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // DuckDuckGo HTML results are in nodes with class 'result__body'
            var nodes = doc.DocumentNode.SelectNodes("//a[@class='result__a']");
            
            if (nodes == null) return "No results found.";

            var results = nodes.Take(5).Select(n => $"- {n.InnerText} (Source: {n.GetAttributeValue("href", "")})");
            
            Console.WriteLine(results);
            return "Search Results:\n" + string.Join("\n", results);
        }
        catch (Exception ex)
        {
            return $"Scraping error: {ex.Message}";
        }
    }

    [KernelFunction]
    [Description("Reads the full text content of a specific webpage URL.")]
    public async Task<string> ReadPageContentAsync(
        [Description("The URL of the webpage to read")] string url)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Remove script and style tags to get clean text
            doc.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());

            var text = doc.DocumentNode.InnerText;
            
            // Clean up whitespace
            var cleanText = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
            
            // Limit text size so we don't blow up the Gemini context
            return cleanText.Length > 5000 ? cleanText.Substring(0, 5000) + "..." : cleanText;
        }
        catch (Exception ex)
        {
            return $"Error reading page: {ex.Message}";
        }
    }
}