using Azure;
using Azure.AI.OpenAI;

namespace DermAI.Services;
public class OpenAIService
{
    OpenAIClient client;
    static readonly char[] trimChars = new char[] { '\n', '?' };
    public void Initialize(string openAIKey, string? openAIEndpoint = null)
    {
        client = !string.IsNullOrWhiteSpace(openAIEndpoint)
            ? new OpenAIClient(
                new Uri(openAIEndpoint),
                new AzureKeyCredential(openAIKey))
            : new OpenAIClient(openAIKey);
    }

    internal async Task<string>CallOpenAI(string recommendationType)
    {
        string prompt = GeneratePrompt(recommendationType);
        Response<Completions> response = await client.GetCompletionsAsync(
            "gpt-3.5-turbo-instruct",
            prompt);
        StringWriter sw = new StringWriter();
        foreach (Choice choice in response.Value.Choices)
        {
            var text = choice.Text.TrimStart(trimChars);
            sw.WriteLine(text);
        }
        var message = sw.ToString();
        return message;
    }

    private static string GeneratePrompt(string recommendationType)
    {
        return $"What is a recommended {recommendationType}";
    }
}
