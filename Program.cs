using MaterializedPathTreeAPI;
using MaterializedPathTreeAPI.DB;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEntityFrameworkNpgsql()
                .AddDbContext<TreeContext>(opt =>
                    opt.UseNpgsql(builder.Configuration.GetConnectionString("TreeConection")));
builder.Services.AddScoped<TreeRepository>();
builder.Services.AddScoped<TreeService>();

var app = builder.Build();


app.MapControllers();

app.Run();
