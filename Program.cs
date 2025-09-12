using BankDecisionApi.Policies;
using BankDecisionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repo (DB)
builder.Services.AddSingleton<IPolicyRepository, InMemoryPolicyRepository>();

// RulesEngine Provider (load JSON once)
builder.Services.AddSingleton<IRulesEngineProvider>(sp =>
{
    var env = sp.GetRequiredService<IHostEnvironment>();
    var rulesPath = Path.Combine(env.ContentRootPath, "Rules", "transaction-rules.json");
    var json = File.ReadAllText(rulesPath);
    return new RulesEngineProvider(json);
});

// Services
builder.Services.AddScoped<IBankingDecisionServiceNaive, BankingDecisionService_Naive>();
builder.Services.AddScoped<IBankingDecisionServiceRules, BankingDecisionService_Rules>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
