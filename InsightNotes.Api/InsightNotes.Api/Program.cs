using InsightNotes.Api.GraphQL;
using InsightNotes.Api.Services;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));
builder.Services.AddSingleton<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddSingleton<QdrantService>();
builder.Services.AddSingleton<NoteService>();
//sk - proj - 0ljZY7sLFta1DaLRoobpp5yLdxdKixuZD_jpC7N5N7H_V1xk - hWrKG2W9Tnvf2jEJ9Bl7yQMUpT3BlbkFJoD_Vge7rUnwfVfpyDp1QQfJ0DFHwvPhNYqhjTOUtoC1S5MnjE - UM6dgI2zSfKoYj3omz_nnTYA

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();
app.MapGraphQL();

app.Run();
