using EquidCMS.Controllers;

namespace EquidCMS.Services
{
    public class NotificationScheduler : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private System.Threading.Timer _timerNotification;
        public IConfiguration _iconfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;
        private Timer _jobAlertTimer;

        public NotificationScheduler(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {

            ScheduleDailyJobAlert();
            _timerNotification = new Timer(RunJob, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            _timerNotification = new Timer(RunHourlyJob, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            //_timerNotification = new Timer(RunNofificationJob, null, TimeSpan.FromHours(3), TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void RunJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    //Place your code here which you want to schedule on regular intervals
                    var store = scrope.ServiceProvider.GetService<NotificationController>();
                    store.SendNotificationBucket("Days");
                }
                catch (Exception ex)
                {

                }
                Interlocked.Increment(ref executionCount);
            }
        }

        private void RunHourlyJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    Console.Write("hourly schedular called");
                    //Place your code here which you want to schedule on regular intervals
                    var store = scrope.ServiceProvider.GetService<NotificationController>();
                    store.SendNotificationBucket("Hours");
                }
                catch (Exception ex)
                {

                }
                Interlocked.Increment(ref executionCount);
            }
        }
        private void ScheduleDailyJobAlert()
        {
            var now = DateTime.Now;
            var scheduledTime = DateTime.Today.AddHours(17); // 11:00 PM

            if (now > scheduledTime)
            {
                // If current time is already past 11 PM, schedule for tomorrow
                scheduledTime = scheduledTime.AddDays(1);
            }

            var initialDelay = scheduledTime - now;

            _jobAlertTimer = new Timer(async state =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var controller = scope.ServiceProvider.GetRequiredService<NotificationController>();
                    await controller.SendJobAlerts();  // or call SendJobAlerts directly
                }

                // Reschedule for next 24 hours
                _jobAlertTimer.Change(TimeSpan.FromDays(1), Timeout.InfiniteTimeSpan);

            }, null, initialDelay, Timeout.InfiniteTimeSpan);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {

            _timerNotification?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerNotification?.Dispose();
            _jobAlertTimer?.Dispose();

        }
    }
}
