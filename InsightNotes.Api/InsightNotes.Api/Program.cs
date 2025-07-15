using InsightNotes.Api.GraphQL;
using InsightNotes.Api.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient for HuggingFaceEmbeddingService
builder.Services.AddHttpClient<IEmbeddingService, HuggingFaceEmbeddingService>(client =>
{
    client.BaseAddress = new Uri("https://router.huggingface.co/hf-inference/");
    client.Timeout = TimeSpan.FromSeconds(90);
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", "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");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});


builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<NoteService>();
builder.Services.AddScoped<Query>();
builder.Services.AddScoped<Mutation>();
builder.Services.AddLogging();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var qdrantService = scope.ServiceProvider.GetRequiredService<QdrantService>();
    await qdrantService.CreateCollectionIfNotExistsAsync();
}

app.UseHttpsRedirection();
app.MapGraphQL();
app.Run();