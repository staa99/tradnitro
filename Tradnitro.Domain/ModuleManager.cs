using System.Net.Mime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tradnitro.Data;


namespace Tradnitro.Domain
{
	public static class ModuleManager
	{
		public static void AddTradnitroDomain(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDatabase(configuration);
		}
	}
}
