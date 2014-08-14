using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace ClockTile
{
    public static class NotificationScheduler
    {
        const string xml = @"<tile>
                                <visual version=""2"">
                                    <binding template=""TileSquare150x150Text02"">
                                        <text id=""1"">{0}</text>
                                        <text id=""2"">{1}</text>
                                    </binding>
                                    <binding template=""TileWide310x150Text09"">
                                        <text id=""1"">{0}</text>
                                        <text id=""2"">{1}</text>
                                    </binding>  
                                </visual>
                            </tile>";

        public static void CreateSchedule()
        {
            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            var now = DateTime.Now;
            var n = BuildNotification(now);
            tileUpdater.Update(new TileNotification(n) { ExpirationTime = now.AddMinutes(1) });

            var plannedUpdated = tileUpdater.GetScheduledTileNotifications();
            var planTill = now.AddHours(4);

            var updateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
            if (plannedUpdated.Count > 0) updateTime = plannedUpdated.Select(x => x.DeliveryTime.DateTime).Union(new[] { updateTime }).Max();

            for (var time = updateTime; time < planTill; time = time.AddMinutes(1))
            {
                try
                {
                    n = BuildNotification(time);
                    ScheduledTileNotification scheduledNotification = new ScheduledTileNotification(n, new DateTimeOffset(time)) { ExpirationTime = time.AddMinutes(1) };
                    tileUpdater.AddToSchedule(scheduledNotification);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        static XmlDocument BuildNotification(DateTime time)
        {
            var tileXmlNow = string.Format(xml, time.ToString("t"), time.ToString("dddd"));
            XmlDocument documentNow = new XmlDocument();
            documentNow.LoadXml(tileXmlNow);
            return documentNow;
        }
    }
}
