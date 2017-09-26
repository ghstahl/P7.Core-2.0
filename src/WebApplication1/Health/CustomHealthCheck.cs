using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.HealthChecks;

namespace WebApplication1.Health
{
    public class CustomHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomHealthCheck(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
            => new ValueTask<IHealthCheckResult>(HealthCheckResult.FromStatus(_serviceProvider == null ? CheckStatus.Unhealthy : CheckStatus.Healthy, "Testing DI support"));
    }
}
