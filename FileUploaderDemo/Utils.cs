namespace FileUploaderDemo
{
    public class CoreHttpContext
    {
        private static Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostEnviroment;
        public static string WebPath => _hostEnviroment.WebRootPath;

        public static string MapPath(string path)
        {
            return Path.Combine(_hostEnviroment.ContentRootPath, path);
        }

        internal static void Configure(Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostEnviroment)
        {
            _hostEnviroment = hostEnviroment;
        }
    }
    public static class StaticHostEnviromentExtensions
    {
        public static IApplicationBuilder UseStaticHostEnviroment(this IApplicationBuilder app)
        {
            var webHostEnvironment = app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            CoreHttpContext.Configure(webHostEnvironment);
            return app;
        }
    }
}
