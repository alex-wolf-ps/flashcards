using Azure;
using Azure.AI.Translation.Text;
using Azure.AI.Translation.Text.Custom;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddTextTranslationClient(
        new AzureKeyCredential(builder.Configuration["TranslateKey"]),
        new Uri(builder.Configuration["TranslateURL"]),
        "eastus");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin());

app.MapGet("/words", () =>
{
    return File.ReadAllText("words.txt").Split(',');
})
.WithName("words")
.WithOpenApi();

app.MapGet("/translate", async (string targetLanguage, string word,
    TextTranslationClient client) =>
{
    var response = await client.TranslateAsync(targetLanguage, word);
    return response.Value.FirstOrDefault().Translations.FirstOrDefault().Text;
})
.WithName("translate")
.WithOpenApi();

app.Run();
