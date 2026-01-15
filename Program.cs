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
        Version = "v1",
        Description = "Main API endpoints"
    });

    options.SwaggerDoc("Master", new OpenApiInfo
    {
        Title = "Master Data Management",
        Version = "v1",
        Description = "Master data CRUD operations (37 controllers)"
    });

    options.SwaggerDoc("Student", new OpenApiInfo
    {
        Title = "Student Management",
        Version = "v1",
        Description = "Student-related operations"
    });

    options.SwaggerDoc("User", new OpenApiInfo
    {
        Title = "User Management",
        Version = "v1",
        Description = "User authentication, roles, and menu management"
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
        if (docName == "Student")
            return apiDesc.GroupName == "Student";
        if (docName == "User")
            return apiDesc.GroupName == "User";

        return apiDesc.GroupName == null;
    });
});

var app = builder.Build();

// ✅ CRITICAL: Enable detailed error pages FIRST (before any other middleware)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}

// ✅ Swagger (should be early in pipeline)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/Default/swagger.json", "ATM Global API");
    c.SwaggerEndpoint("/swagger/Master/swagger.json", "Master Data Management");
    c.SwaggerEndpoint("/swagger/Student/swagger.json", "Student Management");
    c.SwaggerEndpoint("/swagger/User/swagger.json", "User Management");
});

app.UseHttpsRedirection();

// ✅ CORS (use only once, early in pipeline)
app.UseCors("AllowAll");

// ✅ Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers (should be last)
app.MapControllers();

app.Run();