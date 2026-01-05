using API;
using API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Custom configurations
builder.Services.ConfigureCors();

//configure dbs
builder.Services.ConfigureSqlServer(builder.Configuration); 
builder.Services.ConfigureInMemoryDb(); 
builder.Services.ConfigureDapper();
builder.Services.ConfigureJwtOptions(builder.Configuration);
builder.Services.GeneralConfiguration(builder.Configuration);
builder.Services.ConfigureAutomapper();
builder.Services.ConfigureIdentityService(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureFluentEmail(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureMediatR();
builder.Services.AddSwaggerGen();

builder.Services.AddAntiforgery();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options => {
    {
        options.AddPolicy("Admin_Policy", policy =>
        {
            policy.RequireRole("Admin");
        });
        options.AddPolicy("User_Policy", policy =>
        {
            policy.RequireRole("User"); 
        });
    }
});





builder.ConfigureSwagger();




builder.Host.UseSerilog();

var app = builder.Build();
Log.Information($"eLibrary API Starting Up on port: {builder.Configuration["Host"]}"); 



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();



app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapApiEndpoints();

app.Run();
