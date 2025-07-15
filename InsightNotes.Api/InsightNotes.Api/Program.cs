using InsightNotes.Api.GraphQL;
using InsightNotes.Api.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));
builder.Services.AddSingleton<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<NoteService>();

builder.Services.AddScoped<InsightNotes.Api.GraphQL.Query>();
builder.Services.AddScoped<InsightNotes.Api.GraphQL.Mutation>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<InsightNotes.Api.GraphQL.Query>()
    .AddMutationType<InsightNotes.Api.GraphQL.Mutation>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
app.MapGraphQL();

app.Run();
