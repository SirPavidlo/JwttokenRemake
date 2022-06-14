using Jwttoken.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Jwttoken.Controllers
{
    public class RegisterController : Controller
    {
        string path = @"C:\ProgramData\users.txt";

        public IActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registration(string login, string password, string passwordrepeat, string role)
        {
            TempData["TryResult"] = "Ошибка!";
            if (login == null || password ==null|| passwordrepeat==null|| role==null) { TempData["AlertMessage"] = "Заполните все поля!";  return RedirectToAction("Registration"); }
            else if (password != passwordrepeat) { TempData["AlertMessage"] = "Пароли должны совападать!"; return RedirectToAction("Registration"); }
            else if (DublicateLogin(login, path) == true) { TempData["AlertMessage"] = "Пользователь с таким именем уже создан!"; return RedirectToAction("Registration"); }
            else
            {
                string texttofile = login + "|" + BCrypt.Net.BCrypt.EnhancedHashPassword(password) + "|" + role + "\n";

                WriteFile( texttofile);
                TempData["TryResult"] = "Успех!";
                TempData["AlertMessage"] = "Аккаунт успешно создан!";
                return RedirectPermanent("/Home/Index");

            }
        }


        private async void WriteFile(string text)
        {
            using (FileStream fstream = new FileStream(path, FileMode.Append))
            {
                // преобразуем строку в байты
                byte[] buffer = Encoding.Default.GetBytes(text);
                // запись массива байтов в файл
                await fstream.WriteAsync(buffer, 0, buffer.Length);
            }
        }


        private bool DublicateLogin(string login, string path) //если хотя бьы один логин совпадает вернуть true
        {
            if (System.IO.File.Exists(path))
            {
                var users = ConvertText(path);
                foreach (var user in users)
                {
                    if (user.Login == login) { return true; }
                }
                return false;
            }
            else return false;
        }

        private string[] ReadFile(string path) //считывание из файла построчно в string-массив
        {
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

            using(StreamReader fs = new StreamReader(path))
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

        private List<User> ConvertText(string path)//разбиение построчного строчного массива в лист экземляров юзера
        {
            string[] text = ReadFile(path);
            List<User> users = new List<User>();
            foreach (var elem in text)
            {
                string[] line = elem.Split('|');
                users.Add(new User { Login = line[0], Password=line[1], Role=line[2] });
            }
            return users;
        }
    }
}
