using FlyingDutchmanAirlines;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

InitalizeHost();

static void InitalizeHost() =>
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
                builder.UseUrls("http://0.0.0.0:8080");
            }).Build().Run();
