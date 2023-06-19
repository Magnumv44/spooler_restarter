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
                // Якщо програма не запущена від імені адміністратора системи, робимо перезапуск з запитом цих прав.
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
                Console.WriteLine("\nУстанавливаем права на запуск службы");
                SetServicePermissions("spooler");
            }

            // Викликаємо метод перевірки доступу до джерела подій в журналі
            EventLogSourceCheck();
            
            Console.WriteLine("Нажмите клавишу Enter для выхода.");
            Console.ReadLine();
        }

        #region GetServicePermissions
        /// <summary>
        /// Метод, що отримує поточні значення дозволу доступу до служби.
        /// </summary>
        /// <param name="serviceName">Нарзву необхідної служби в форматі string</param>
        /// <returns>Строку з набором символів у вигляді синтаксису Security Description Definition Language</returns>
        static string GetServicePermissions(string serviceName)
        {
            string command = $"sc sdshow {serviceName}";
            return ExecuteCommand(command);
        }
        #endregion

        #region SetServicePermissions
        /// <summary>
        /// Метод для встановлення прав на зміну стану служби вокристовуючи синтаксис Security Description Definition Language
        /// </summary>
        /// <param name="serviceName">Нарзву необхідної служби в форматі string</param>
        static void SetServicePermissions(string serviceName)
        {
            string command = $"sc sdset {serviceName} D:(A;;0x30;;;WD)(A;;CCLCSWLOCRRC;;;AU)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWRPWPDTLOCRRC;;;SY)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)";
            ExecuteCommand(command);
        }
        #endregion

        #region ExecuteCommand
        /// <summary>
        /// Метод для вконання команд через командну строку Windows
        /// </summary>
        /// <param name="command">Код команди в форматі string</param>
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
        #endregion

        #region IsServicePermissionSet
        /// <summary>
        /// Метод пошуку пыдстроки, що вказує чи були надані права на зміну стану служби, або ні.
        /// </summary>
        /// <param name="permissionsOutput"></param>
        /// <returns>true - якщо доступ вже надано, false - якщо ні</returns>
        static bool IsServicePermissionSet(string permissionsOutput)
        {
            // Перевіряємо, чи містить вхідна строка необхідну підстроку
            return permissionsOutput.Contains("(A;;RPWP;;;WD)");
        }
        #endregion

        #region IsRunAsAdmin
        /// <summary>
        /// Метод, що перевіряє чи було запущено програму з правами адміністратора системи.
        /// </summary>
        /// <returns>true - якщо так, false - якщо ні</returns>
        static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion

        #region RestartAsAdmin
        /// <summary>
        /// Метод для для перезапуска програмі за запитом надати права адміністратора системи.
        /// </summary>
        static void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
            startInfo.Verb = "runas"; // Запуск процесу з правами адміністратора
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка перезапуска программы с правами администратора: " + ex.Message);
            }
        }
        #endregion

        #region EventLogSourceCheck
        /// <summary>
        /// Метод перевірки наявності джерела подій в журналі, та в разі необхідності, його створення
        /// </summary>
        static void EventLogSourceCheck()
        {
            string sourceForLog = "SpoolerRestarter";
            string logName = "Application";

            // Перевіряємо, чи існує джерело журналу подій
            if (!EventLog.SourceExists(sourceForLog))
            {
                // Створюємо нове джерело журналу подій
                EventLog.CreateEventSource(sourceForLog, logName);
                Console.WriteLine("Доступ предоставлен для журнала событий \"{0}\"", logName);
            }
            else
            {
                Console.WriteLine("Источник для журнала событий \"{0}\" уже существует", logName);
            }
        }
        #endregion
    }
}