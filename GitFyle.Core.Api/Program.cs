// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.Storages;
using GitFyle.Core.Api.Services.Foundations.Configurations;
using GitFyle.Core.Api.Services.Foundations.Contributions;
using GitFyle.Core.Api.Services.Foundations.ContributionTypes;
using GitFyle.Core.Api.Services.Foundations.Repositories;
using GitFyle.Core.Api.Services.Foundations.Sources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GitFyle.Core.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder =
                WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StorageBroker>();
            AddBrokers(builder.Services);
            AddFoundationServices(builder.Services);

            WebApplication webApplication =
                builder.Build();

            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
            }

            webApplication.UseHttpsRedirection();
            webApplication.UseAuthorization();
            webApplication.MapControllers();
            webApplication.Run();
        }

        private static void AddBrokers(IServiceCollection services)
        {
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<ISourceService, SourceService>();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IContributionService, ContributionService>();
            services.AddTransient<IRepositoryService, RepositoryService>();
            services.AddTransient<IContributionTypeService, ContributionTypeService>();
        }
    }
}
