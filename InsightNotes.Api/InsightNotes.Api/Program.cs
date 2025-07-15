using InsightNotes.Api.GraphQL;
using InsightNotes.Api.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Could be used instead of HuggingFaceEmbeddingService if OpenAI is preferred
//builder.Services.AddSingleton(new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));
//builder.Services.AddSingleton<IEmbeddingService, OpenAIEmbeddingService>();

builder.Services.AddHttpClient<IEmbeddingService, HuggingFaceEmbeddingService>();

builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<NoteService>();

builder.Services.AddScoped<Query>();
builder.Services.AddScoped<Mutation>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
app.MapGraphQL();

app.Run();
