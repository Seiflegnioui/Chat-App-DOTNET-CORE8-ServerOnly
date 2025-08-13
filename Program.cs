using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using p4.Data;
using p4.Hubs;
using p4.Middlewares;
using p4.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSomeOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();

    });
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Logging.SetMinimumLevel(LogLevel.Debug);
var config = builder.Configuration;

// How it connects to SignalR by adding the onreceivermessage event 

// When a client connects to /chat?access_token=YOUR_JWT, this happens:

// SignalR uses the same JWT middleware because you're using AddAuthentication("Bearer").

// Your OnMessageReceived picks the token from the query string instead of headers.

// Middleware validates it and sets Context.User.

// Now inside your ChatHub, Context.User is filled and Context.UserIdentifier is populated (from ClaimTypes.NameIdentifier by default).
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidIssuer = config["jwt_infos:issuer"],

        ValidateAudience = true,
        ValidAudience = config["jwt_infos:audience"],

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["jwt_infos:key"]!)
        )
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("AUTH FAILED: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("TOKEN VALIDATED");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            Console.WriteLine(accessToken);

            var path = context.HttpContext.Request.Path;
            Console.WriteLine(path);
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
            {
                // "Hey, the token isn’t in the Authorization header — it’s in the query string. Please validate this one."
                // BECAUSE WHEN THE MIDDLEWARE GETS THE TOKEN FROM THE AUTHORIZATION IT SET IT TO CONTEXT.TOKEN THEN VALIDATE SINCE
                // WE VE CHANGED THE TOKEN PLACE FROM ATHORIZATION TO QQUERY WE MUST EXPLICITLY SET IT TO TOKEN THEN THE VALIDATION LogiC
                // WILL RUN NORMALY CAUSE THOSE EVENTS EXECUTED BEFORE
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },


    };
});

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

var conn = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(conn, ServerVersion.AutoDetect(conn));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSomeOrigins");
app.UseHttpsRedirection();
app.MapHub<ChatHub>("/chat");
app.UseAuthentication();
app.UseCustomDebuger();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();

