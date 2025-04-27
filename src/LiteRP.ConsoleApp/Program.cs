using System;
using System.Collections.Generic;
using LiteRP.ConsoleApp;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// --- Host Setup ---
var builder = Host.CreateApplicationBuilder();

var ollamaSettings = builder.Configuration.GetRequiredSection("Ollama").Get<OllamaSettings>();

if (ollamaSettings == null)
{
    throw new InvalidOperationException(
        "Ollama ModelName is not configured. " +
        "Please set 'Ollama:ModelName' in appsettings.json (e.g., 'llama3').");
}

var ollamaClient = new OllamaChatClient(new Uri(ollamaSettings.Endpoint), ollamaSettings.ModelName);
builder.Services.AddChatClient(ollamaClient);

var app = builder.Build();

// --- Application Logic ---
var chatClient = app.Services.GetRequiredService<IChatClient>();

Console.WriteLine("LiteRP Console Chat (Direct IChatClient)");
Console.WriteLine("---------------------------------------");
Console.WriteLine($"Model: {ollamaSettings.ModelName}");
Console.WriteLine("Type 'exit' to quit.");
Console.WriteLine("Enter your message below.");

var chatHistory = new List<ChatMessage>();

// Optional: Add a system prompt
// chatHistory.Add(new ChatMessage(ChatRole.System, "You are a helpful AI assistant."));

while (true)
{
    Console.Write("> ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) continue;
    if (userInput.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    try
    {
        chatHistory.Add(new ChatMessage(ChatRole.User, userInput));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("AI: ");
        await foreach (var messagePart in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            Console.Write(messagePart.Text);
        }

        Console.WriteLine();
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"\nAn error occurred: {ex.Message}");
        Console.ResetColor();

        // Remove the user message that caused the error from history
        // so we don't resend it potentially in a broken state next turn.
        if (chatHistory.Count > 0 && chatHistory[^1].Role == ChatRole.User)
        {
             chatHistory.RemoveAt(chatHistory.Count - 1);
             Console.WriteLine("(Last user message removed from history due to error)");
        }
    }
}

Console.WriteLine("Exiting LiteRP. Goodbye!");