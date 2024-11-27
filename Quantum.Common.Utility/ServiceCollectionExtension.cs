using Microsoft.Extensions.DependencyInjection;
using Quantum.Utility.Services;
using Quantum.Utility.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Utility
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddSevicesQuantumUtility(this IServiceCollection services)
		{
			services.AddHostedService<QueuedHostedService>();
			services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

			return services;
		}
	}
}
