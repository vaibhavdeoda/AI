# README: AI Stock Research Agent

An automated research agent built with .NET and Semantic Kernel that performs deep-dive stock market analysis by scraping real-time web data and synthesizing it into professional investment memos.

---

## 🛠 Tech Stack & Dependencies

*   **Language:** C# (.NET 8.0+)
*   **Orchestration:** Microsoft Semantic Kernel
*   **AI Model:** Google Gemini 2.5 Flash
*   **Scraping:** HtmlAgilityPack
*   **Protocol:** Agentic Function Calling (Auto-Invoke)

---

## 🚀 Key Features

*   **Autonomous Research:** Uses a professional prompt to investigate P/E ratios, PEG ratios, debt levels, and market "moats".
*   **Technical Analysis:** Scans for price action trends relative to 50-day and 200-day moving averages (SMA) and RSI levels.
*   **Stealth Scraping:** The `WebScraperPlugin.cs` utilizes randomized delays (1-3 seconds) and browser-mimicking headers to bypass "Too Many Requests" errors[cite: 2].
*   **No-API Search:** Leverages DuckDuckGo's HTML interface to find data without requiring a Google Search API key.
*   **Automated Filing:** Automatically saves a structured "Bull/Bear Case" report to a local `.txt` file.

---

## 🏃 How to Run

### 1. Setup API Key
The application expects a Google Gemini API key. Ensure it is set in your environment:
- **Windows:** `$env:GeminiApiKey = "your_key"`
- **macOS/Linux:** `export GeminiApiKey="your_key"`

### 2. Install Packages
Run these commands in your terminal:
- `dotnet add package Microsoft.SemanticKernel`
- `dotnet add package Microsoft.SemanticKernel.Connectors.Google`
- `dotnet add package HtmlAgilityPack`

### 3. Execution
1. Open the project in VS Code or your preferred IDE.
2. Run `dotnet run` in the terminal.
3. Enter a stock ticker (e.g., "AAPL") or a sector (e.g., "Semiconductors") when prompted.
4. The agent will scrape the web, process the data, and generate a `research_report_[topic].txt` file in your project folder.

---

## 📂 File Overview

*   **Program.cs**: Configures the Semantic Kernel, registers plugins, and defines the "Senior Equity Research Lead" persona and workflow.
*   **WebScraperPlugin.cs**: Handles raw HTML fetching, text cleaning, and stealth search logic to provide the agent with real-time data.
*   **ResearchDigestPlugin**: A helper class within `Program.cs` that handles the file-saving logic once the analysis is complete.

---
*Note: This tool is for educational purposes. It demonstrates agentic workflows and web scraping integration in .NET.*
