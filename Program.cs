using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using atmglobalapi.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ✅ JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ✅ CORS
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin();
    });
});

// ✅ JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = config["JWT:ValidIssuer"],
        ValidAudience = config["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["JWT:Secret"]))
    };
});

builder.Services.AddAuthorization();

// ✅ Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("Default", new OpenApiInfo
    {
        Title = "ATM Global API",
        Version = "v1"
    });

    options.SwaggerDoc("Master", new OpenApiInfo
    {
        Title = "Master APIs",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT Token: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName == "Master")
            return apiDesc.GroupName == "Master";

        return apiDesc.GroupName == null;
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/Default/swagger.json", "ATM Global API");
    c.SwaggerEndpoint("/swagger/Master/swagger.json", "Master APIs");
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
