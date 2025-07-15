using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;



// Configure suas credenciais e endpoint via variáveis de ambiente ou arquivo seguro
string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? ""; // Exemplo: https://application-core.openai.azure.com/
string deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? ""; // Exemplo: gpt-4.1
string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY") ?? "";
AzureKeyCredential credential = new AzureKeyCredential(key);

// Cria o IChatClient usando apenas a URL base e o nome do deployment
IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), credential)
    .GetChatClient(deployment)
    .AsIChatClient();

// Start the conversation with context for the AI model
List<ChatMessage> chatHistory =
    [
        new ChatMessage(ChatRole.System, """
            Você é um entusiasta amigável de trilhas que ajuda as pessoas a descobrir caminhadas divertidas na região delas.
            Você se apresenta quando fala olá pela primeira vez.
            Ao ajudar as pessoas, você sempre pede estas informações
            para informar a recomendação de trilha que irá fornecer:

            1. O local onde a pessoa gostaria de fazer trilha
            2. O nível de intensidade da trilha que ela procura

            Depois de receber essas informações, forneça três sugestões de trilhas próximas que variam em comprimento.
            Também compartilhe um fato interessante sobre a natureza local dessas trilhas ao fazer a recomendação. Ao final da sua resposta, pergunte se há mais alguma coisa com que você possa ajudar.
        """)
    ];

// Loop to get user input and stream AI response
while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Digite sua mensagem:");
    string? userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI resposta:");
    string response = "";
    await foreach (ChatResponseUpdate item in
        chatClient.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}