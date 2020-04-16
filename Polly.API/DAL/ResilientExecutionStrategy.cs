using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Polly.API.DAL
{
    public class ResilientExecutionStrategy : ExecutionStrategy
    {
        public ResilientExecutionStrategy([NotNull] DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) : 
            base(context, maxRetryCount, maxRetryDelay)
        {
        }

        public ResilientExecutionStrategy([NotNull] ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay) : 
            base(dependencies, maxRetryCount, maxRetryDelay)
        {
        }

        protected override bool ShouldRetryOn([NotNull] Exception exception)
        {
            //Add logic for retry
            return true;
        }
    }
}
