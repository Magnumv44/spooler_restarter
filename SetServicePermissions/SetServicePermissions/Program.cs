// TODO добавить в описание Readme.md ссылку на источник иконки с https://icons8.com
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace SetServicePermissions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!IsRunAsAdmin())
            {
                // Если программа не запущена с правами администратора, перезапускаем ее с правами администратора
                RestartAsAdmin();
                return;
            }

            string currentPermissions = GetServicePermissions("spooler");

            if (IsServicePermissionSet(currentPermissions))
            {
                Console.WriteLine("Доступ к службе уже был предоставлен.");
            }
            else
            {
                Console.WriteLine("Текущие права службы:");
                Console.WriteLine(currentPermissions);
                
                Console.WriteLine("\nУстанавливаем права на запуск службы");
                SetServicePermissions("spooler");
            }

            Console.WriteLine("Нажмите клавишу Enter для выхода.");
            Console.ReadLine();
        }

        /// <summary>
        /// Метод получающий теущий уровень разрешения доступа к службе.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>Строку с набором символов с синтаксисом Security Description Definition Language</returns>
        static string GetServicePermissions(string serviceName)
        {
            string command = $"sc sdshow {serviceName}";
            return ExecuteCommand(command);
        }

        /// <summary>
        /// Метод для установки прав на менипуляции со службой используя синтаксис Security Description Definition Language
        /// </summary>
        /// <param name="serviceName"></param>
        static void SetServicePermissions(string serviceName)
        {
            string command = $"sc sdset {serviceName} D:(A;;0x30;;;WD)(A;;CCLCSWLOCRRC;;;AU)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWRPWPDTLOCRRC;;;SY)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)";
            ExecuteCommand(command);
        }

        /// <summary>
        /// Метод для выполнения команд через командную строку Windows
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        static string ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            return output;
        }

        /// <summary>
        /// Метод поиска подстроки указывающей на наличие уже установленного доступа на остановку и запуск службы
        /// </summary>
        /// <param name="permissionsOutput"></param>
        /// <returns>true - если доступ уже предоставлен, false - если нет</returns>
        static bool IsServicePermissionSet(string permissionsOutput)
        {
            // Проверяем наличие определенной строки в выводе команды
            return permissionsOutput.Contains("(A;;RPWP;;;WD)");
        }

        /// <summary>
        /// Метод для проверки, запущен ли код с правами администратора
        /// </summary>
        /// <returns></returns>
        static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Метод для перезпуска программы с запросом предоставить права администратора
        /// </summary>
        static void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            startInfo.Verb = "runas"; // Запуск процесса с правами администратора

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка перезапуска программы с правами администратора: " + ex.Message);
            }
        }
    }
}