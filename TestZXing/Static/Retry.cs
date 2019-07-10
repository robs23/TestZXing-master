using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Interfaces;
using Xamarin.Forms;

namespace TestZXing.Static
{
    public static class Retry
    {
        public async static void Do(
        Action action,
        TimeSpan retryInterval,
        int maxAttemptCount = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        public async static Task<T> Do<T>(
            Func<T> action,
            TimeSpan retryInterval,
            int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                    {
                        Task.Delay(retryInterval).Wait();
                    }
                    DependencyService.Get<IToaster>().LongAlert($"Próba {attempted+1}");
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            return (T)Convert.ChangeType(1,typeof(T));
            //throw new AggregateException(exceptions);
        }
    }
}
