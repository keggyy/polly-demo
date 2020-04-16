using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.API.DAL
{
    public class DelayInterceptor: DbCommandInterceptor
    {
        public static bool EnableInterceptor { get; set; } = false;
        private static void Delay()
        {
            if (EnableInterceptor)
            {
                var rand = new Random();
                var sleep = rand.Next(4, 8);
                System.Threading.Thread.Sleep(sleep * 1000);
            }
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            Delay();
            return base.ReaderExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Delay();
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            Delay();
            return base.ScalarExecuting(command, eventData, result);
        }

        public override Task<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            Delay();
            return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
        }
    }
}
