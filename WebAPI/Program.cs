using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch { }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())//AutoFac Container Configuration start
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacBusinessModule());
                })//AutoFac Container Configuration end
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog();
                });
        public static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"log.txt",outputTemplate: "{Timestamp:yyyy-mm-dd} {MachineName} {ThreadId} {Message} {Exception:1} {Newline}")
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .CreateLogger();
        }
    }
}
