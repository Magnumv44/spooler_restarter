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

            // TODO Цю частину коду потрібно переробити
            Console.WriteLine("Текущие права службы");
            string currentPermissions = GetServicePermissions("spooler");
            Console.WriteLine(currentPermissions);

            if (IsServicePermissionSet(currentPermissions))
            {
                Console.WriteLine("Доступ к службе уже был предоставлен.");
            }
            else
            {
                Console.WriteLine("Устанавливаем права на запуск службы");
                SetServicePermissions("spooler");
            }

            Console.WriteLine("Проверяем еще раз, чтобы убедиться, что права изменились");
            string updatedPermissions = GetServicePermissions("spooler");
            Console.WriteLine(updatedPermissions);

            Console.ReadLine();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static string GetServicePermissions(string serviceName)
        {
            string command = $"sc sdshow {serviceName}";
            return ExecuteCommand(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        static void SetServicePermissions(string serviceName)
        {
            string command = $"sc sdset {serviceName} D:(A;;0x30;;;WD)(A;;CCLCSWLOCRRC;;;AU)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWRPWPDTLOCRRC;;;SY)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)";
            ExecuteCommand(command);
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="permissionsOutput"></param>
        /// <returns></returns>
        static bool IsServicePermissionSet(string permissionsOutput)
        {
            // Проверяем наличие определенной строки в выводе команды
            return permissionsOutput.Contains("(A;;0x30;;;WD)(A;;CCLCSWLOCRRC;;;AU)(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;BA)(A;;CCLCSWRPWPDTLOCRRC;;;SY)S:(AU;FA;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;WD)");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 
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