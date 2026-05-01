using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using Microsoft.SemanticKernel.Connectors.Google;
using System.ComponentModel;


// Build the Kernel with Gemini
var builder = Kernel.CreateBuilder();

var apiKey = EnvironmentSettings.GetGeminiApiKey();

builder.AddGoogleAIGeminiChatCompletion(
    modelId: "gemini-2.5-flash", // Use the stable version available in the free tier
    apiKey: apiKey
);

Console.WriteLine("Enter a stock ticker or industry to research:");
string topic = Console.ReadLine() ?? string.Empty;
if(string.IsNullOrWhiteSpace(topic))
{
    Console.WriteLine("No topic entered. Exiting.");
    return;
}

// Register Plugins
builder.Plugins.AddFromObject(new WebScraperPlugin(), "WebSearch");
builder.Plugins.AddFromObject(new ResearchDigestPlugin(topic), "ResearchTools");

var kernel = builder.Build();

// Configure Agentic Behavior (Auto-Invoke)
var executionSettings = new GeminiPromptExecutionSettings() 
{ 
    ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions 
};

// Research Loop
string prompt = $"""
    Role: Senior Equity Research Lead (Fundamental & Technical Analysis Specialist)
    Objective: Conduct a comprehensive investment research deep-dive into: {topic}.

    Phase 1: Fundamental Scrutiny (The 'Value' Lens)
    Search for and analyze the following metrics:
    - Valuation: Forward P/E, PEG Ratio, and Price-to-Free-Cash-Flow relative to industry peers.
    - Growth: Year-over-year revenue growth and latest quarterly earnings surprise.
    - Balance Sheet: Debt-to-Equity levels and Operating Margins.
    - Moat: Identify the 'Economic Moat' or primary competitive advantage mentioned in recent 10-K or analyst reports.

    Phase 2: Technical Scrutiny (The 'Momentum' Lens)
    Search for data regarding recent price action:
    - Trend: Position of the current price relative to the 50-day and 200-day Simple Moving Averages (SMAs).
    - Momentum: Mention of RSI (Relative Strength Index) levels (Overbought/Oversold).
    - Support/Resistance: Identify key psychological price levels or recent breakout points.

    Phase 3: Risk-Adjusted Synthesis
    Synthesize the 'Hard Data' into a professional Analyst Memo:
    - Investment Thesis: Why an investor would go 'Long' or 'Short' right now.
    - Risk Factors: Macro headwinds (Interest rates, geopolitical) or sector-specific risks.
    - Technical Outlook: Is the current entry point high-probability or a 'chase'?

    Final Action:
    Format the report as a 'High-Conviction Investment Digest' and use the SaveResearch tool to finalize the file. 
    Ensure the output includes a 'Bull Case' and 'Bear Case' for a balanced professional perspective.
    """;

Console.WriteLine("Agent is researching... 🔍");
var result = await kernel.InvokePromptAsync(prompt, new(executionSettings));

Console.WriteLine("\n--- Research Final Answer ---");
Console.WriteLine(result);

// --- Supporting Plugin for the "Digest" Step ---
public class ResearchDigestPlugin
{
    private readonly string topic;
    public ResearchDigestPlugin(string topic)
    {
        this.topic = topic;
    }
    
    [KernelFunction, Description("Saves a research digest to a local file for future email dispatch.")]
    public async Task<string> SaveResearch(
        [Description("The summarized research content")] string content)
    {
        string path = $"research_report_{topic}.txt";
        await File.WriteAllTextAsync(path, content);
        return $"Successfully saved the research to {path}.";
    }
}