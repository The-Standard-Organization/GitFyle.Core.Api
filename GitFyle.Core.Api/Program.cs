// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using GitFyle.Core.Api.Brokers.Loggings;
using GitFyle.Core.Api.Brokers.DateTimes;
using GitFyle.Core.Api.Brokers.Storages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GitFyle.Core.Api;

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

        builder.Services.AddTransient<
            IStorageBroker,
            StorageBroker>();

        builder.Services.AddTransient<
            ILoggingBroker,
            LoggingBroker>();

        builder.Services.AddTransient<
            IDateTimeBroker, 
            DateTimeBroker>();

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
}
