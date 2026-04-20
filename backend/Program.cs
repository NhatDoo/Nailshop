using Microsoft.EntityFrameworkCore;
using backend.context.identity;
using backend.context.common.infrastructure.interceptors;
using backend.context.common.api.middlewares;
using backend.context.booking;
using backend.context.naildesign;
using backend.context.payment;
using backend.context.todo;
using backend.context.notification;
using backend.context.nailservice;
using backend.context.common.application;
using backend.context.common.infrastructure.storage;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Minio;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DomainEventInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Nailshop Identity API", 
        Version = "v1",
        Description = "Hệ thống quản lý danh tính và xác thực cho Nailshop Backend."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập Token theo định dạng: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
});

// Configure JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? "DayLaMotCaiKeyRatDaiVaBaoMatDeTestKetNoiNailshop2024!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "NailshopIdentity",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "NailshopUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// CORS – cho phép Frontend Vite gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register Modules
builder.Services.AddIdentityModule();
builder.Services.AddBookingModule();
builder.Services.AddNailDesignModule();
builder.Services.AddPaymentModule();
builder.Services.AddTodoModule();
builder.Services.AddNotificationModule();
builder.Services.AddNailServiceModule();

// Register MinIO Storage Service
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(builder.Configuration["MinIO:Endpoint"])
    .WithCredentials(
        builder.Configuration["MinIO:AccessKey"],
        builder.Configuration["MinIO:SecretKey"])
    .WithSSL(false));
builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nailshop API v1");
        options.RoutePrefix = string.Empty; // Để Swagger là trang chủ khi chạy
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("FrontendDev");    // CORS phải đứng trước Auth
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.MapGet("/test-db", async (AppDbContext dbContext) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync();
        return canConnect 
            ? Results.Ok(new { status = "success", message = "Database connection successful!" }) 
            : Results.Problem("Cannot connect to database.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
})
.WithName("TestDbConnection");

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    var adminEmail = config["AdminUser:Email"];
    if (!string.IsNullOrEmpty(adminEmail))
    {
        // Kiểm tra xem Admin có chưa (query thẳng bằng EF)
        var adminExists = dbContext.Set<backend.context.identity.domain.entity.User>()
            .Any(u => u.Email == new backend.context.identity.domain.vo.EmailVO(adminEmail));

        if (!adminExists)
        {
            var adminPassword = config["AdminUser:Password"];
            var adminName = config["AdminUser:Name"];
            var adminPhone = config["AdminUser:Phone"];

            var adminUser = backend.context.identity.domain.entity.User.Create(
                ten: adminName,
                sdt: adminPhone,
                email: adminEmail,
                role: "Admin",
                passwordHash: passwordHasher.HashPassword(adminPassword)
            );

            dbContext.Set<backend.context.identity.domain.entity.User>().Add(adminUser);
            dbContext.SaveChanges();
            Console.WriteLine($"[Seeder] Đã tạo thành công tài khoản Admin: {adminEmail}");
        }
    }
}

app.Run();
