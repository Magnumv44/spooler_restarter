using Spooler.Properties;
using System.Diagnostics;
using System.ServiceProcess;

ServiceController spoolService = new ServiceController("spooler");

void EventLogWrite(string message, EventLogEntryType type, int codeEventId)
{
    string sourceForLog = "SpoolerRestarter";
    EventLog.WriteEntry(sourceForLog, message, type, codeEventId);
}

try
{
    switch (spoolService.Status)
    {
        case ServiceControllerStatus.Running:
            spoolService.Stop();
            spoolService.WaitForStatus(ServiceControllerStatus.Stopped);
            spoolService.Start();
            spoolService.WaitForStatus(ServiceControllerStatus.Running);

            EventLogWrite(Resources.Restarted, EventLogEntryType.Warning, 150);
            break;
        case ServiceControllerStatus.Stopped:
            spoolService.Start();
            spoolService.WaitForStatus(ServiceControllerStatus.Running);

            EventLogWrite(Resources.Started, EventLogEntryType.Warning, 150);
            break;
        case ServiceControllerStatus.Paused:
            spoolService.Start();
            spoolService.WaitForStatus(ServiceControllerStatus.Running);

            EventLogWrite(Resources.Started, EventLogEntryType.Warning, 150);
            break;
        default:
            spoolService.Stop();
            spoolService.WaitForStatus(ServiceControllerStatus.Stopped);
            spoolService.Start();
            spoolService.WaitForStatus(ServiceControllerStatus.Running);

            EventLogWrite(Resources.UnknownRestarted, EventLogEntryType.Warning, 150);
            break;
    }
}
catch (InvalidOperationException)
{
    EventLogWrite(Resources.UnknownError, EventLogEntryType.Error, 160);
}

spoolService.Close();