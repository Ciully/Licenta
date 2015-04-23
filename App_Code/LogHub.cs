using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ProiectLicenta { 
public class LogHub : Hub
{
    public static readonly System.Timers.Timer _Timer = new System.Timers.Timer();

    static LogHub()
    {

    }

    static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        var hub = GlobalHost.ConnectionManager.GetHubContext("LogHub");
        hub.Clients.All.logMessage(string.Format("{0} - Still running", DateTime.UtcNow));
    }

    public void NotifyAllClients(string msg)
    {
        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<LogHub>();
        context.Clients.All.displayNotification(msg);
    }

    public void Send(string userId, string message)
    {
        if (message == "notif_check")
        {
            //var asd = Clients.User(userId).send(message);
            var hub = GlobalHost.ConnectionManager.GetHubContext("LogHub");
            //hub.Clients.All.logMessage(string.Format("{0} - Still running", DateTime.UtcNow));
            hub.Clients.User(userId).logMessage(message);
        }
        if (message == "All_Users")
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext("LogHub");
            hub.Clients.All.logMessage(message);
            //hub.Clients.AllExcept(userId).logMessage(message);
           // Clients.Others.logMessage(message);
            
        }
    }

    public void ProgressSignal(string UserId, string Percentage)
    {
         var hub = GlobalHost.ConnectionManager.GetHubContext("LogHub");
         hub.Clients.User(UserId).VidProgress(Percentage);
        
    }

}
}
