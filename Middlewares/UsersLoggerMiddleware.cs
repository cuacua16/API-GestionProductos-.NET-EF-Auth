using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;

namespace GestionProductos.Middlewares {
	public class UsersLoggerMiddleware {

		private readonly RequestDelegate next;
		private readonly ILogger<UsersLoggerMiddleware> logger;

		public UsersLoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory) {
			this.next = next;
			logger = loggerFactory.CreateLogger<UsersLoggerMiddleware>();
		}

		public async Task InvokeAsync(HttpContext context) {
			var user = context.User?.Identity?.Name ?? "anónimo";
			var role = context.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.ToString().Replace("http://schemas.microsoft.com/ws/2008/06/identity/claims/role: ", "") ?? "desconocido";
			logger.LogInformation($"{DateTime.Now}: Usuario \"{user}\" con rol \"{role}\" accedió al path: {context.Request.Path}.");
			await next.Invoke(context);
			Console.WriteLine($"\tResponse Status Code: {context.Response.StatusCode}.");
		}
	}


	public static class LoggingMiddlewareExtension {
		public static IApplicationBuilder UseUsersLoggerMiddleware(this IApplicationBuilder builder) {
			return builder.UseMiddleware<UsersLoggerMiddleware>();
		}
	}
}
