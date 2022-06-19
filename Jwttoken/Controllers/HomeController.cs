using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Jwttoken.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;

namespace Jwttoken.Controllers
{
    
    public class HomeController : Controller
    {

        string path = @"C:\ProgramData\users.txt";
        int? trycount;


       

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            if (User.IsInRole("admin"))
            {
                var users = ConvertText();
                return View("/Views/Admin/AdminView.cshtml", users);
            }

            if (User.IsInRole("user"))
            {
                return Redirect("/User/Users");
            }


            return View();
        }


        [HttpPost]
        public ActionResult Index(string login, string password)
        {
            string test = login;
            var users = ConvertText();
            foreach (var user in users)
            {
                if (login == user.Login)
                {
                    if (BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password) == true)
                    {
                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, login), new Claim(ClaimTypes.Role, user.Role) };
                        // создаем JWT-токен

                        var jwt = new JwtSecurityToken(
                                issuer: AuthOptions.ISSUER,
                                audience: AuthOptions.AUDIENCE,
                                claims: claims,
                                expires: DateTime.Now.Add(TimeSpan.FromSeconds(30)),
                                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)); ;

                        var jwtsecuritytoken = new JwtSecurityTokenHandler().WriteToken(jwt);
                        Response.Cookies.Append("jwt", jwtsecuritytoken, new CookieOptions { Expires = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(10)) });
                        //return Redirect("/Home/AlreadyAuth");
                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        trycount = (int?)TempData["WrongPassword"];
                        if (trycount == null) { trycount = 0; }
                        trycount++;
                        TempData["WrongPassword"] = trycount;
                        if (trycount >= 3) { return Redirect("/Alert/Index"); }
                        return Redirect("/Home/Index");
                    }

                }
                else { TempData["AlertMessage"] = "Такого пользователя не существует"; return Redirect("/Home/Index"); }
            }
            return Redirect("/Home/Index");

        }



        private string[] ReadFile() //считывание из файла построчно в string-массив
        {
            TempData["path"] = @"C:\ProgramData\users.txt";
            path = (string)TempData["path"]!;
            int linescount = 0;
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()!) != null)
                {
                    linescount++;
                }
            }
            string[] text = new string[linescount];
            using (StreamReader fs = new StreamReader(path))
            {
                for (int i = 0; ; i++)
                {
                    string? line = fs.ReadLine();
                    if (line == null) break;
                    text[i] = line;
                }
            }
            return text;
        }

        private List<User> ConvertText()//разбиение построчного строчного массива в лист экземляров юзера
        {
            string[] text = ReadFile();
            List<User> users = new List<User>();
            foreach (var elem in text)
            {
                string[] line = elem.Split('|');
                users.Add(new User { Login = line[0], Password = line[1], Role = line[2] });
            }
            return users;
        }

        /*public IActionResult AlreadyAuth()
        {
          
            if (User.IsInRole("admin"))
            {
                var users = ConvertText();
                return View("/Views/Admin/AdminView.cshtml", users); 
            }
            else { return Redirect("/User/Users"); }

        }
        
        */
        public IActionResult CookieClear()
        {
            Response.Cookies.Delete("jwt");
            return Redirect("/Home/Index");
        }
    }
}