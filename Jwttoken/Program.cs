using Jwttoken.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
        options.AddPolicy("AdminOrUser", policy => policy.RequireRole("admin", "user"));
        //админ или юзер я сделал ради интереса, но я думаю в крупном проекте может быть возможность админа зайти на свой аккаунт в качестве пользователя или в качестве админа(на выбор)

    }
    );


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.Use(async (context, next) =>
{
    var jwt = context.Request.Cookies["jwt"];
    
    if (!string.IsNullOrEmpty(jwt))
    {

        context.Request.Headers.Add("Authorization", "Bearer " + jwt);

        var handler = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        if (handler.ValidTo < DateTime.UtcNow)
        {
           // if (context.Request.Path != "/Home/Index")
            //{
                context.Response.Cookies.Delete("jwt");
                context.Response.Redirect("/Home/Index");
                return;
            //}
        }
    }
    

    await next();
});

app.UseAuthentication();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
