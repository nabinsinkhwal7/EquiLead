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

        public NotificationScheduler(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
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

        public Task StopAsync(CancellationToken stoppingToken)
        {

            _timerNotification?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerNotification?.Dispose();

        }
    }
}
