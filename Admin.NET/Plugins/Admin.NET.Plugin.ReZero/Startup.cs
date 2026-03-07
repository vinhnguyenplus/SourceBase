// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Admin.NET.Core;
using Admin.NET.Plugin.ReZero.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReZero;
using ReZero.SuperAPI;

namespace Admin.NET.Plugin.ReZero;

[AppStartup(100)]
public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var reZeroOpt = App.GetConfig<ReZeroOptions>("ReZero", true);

        // Get the default database configuration (the first one)
        var dbOptions = App.GetConfig<DbConnectionOptions>("DbConnection", true);
        var superAPIOption = new SuperAPIOptions()
        {
            DatabaseOptions = new DatabaseOptions()
            {
                ConnectionConfig = new SuperAPIConnectionConfig()
                {
                    DbType = dbOptions.ConnectionConfigs[0].DbType,
                    ConnectionString = dbOptions.ConnectionConfigs[0].ConnectionString
                }
            },
            UiOptions = new UiOptions() { DefaultIndexSource = "/index.html" },
            InterfaceOptions = new InterfaceOptions()
            {
                AuthorizationLocalStorageName = reZeroOpt.AccessTokenKey, // The key name of the browser's local storage LocalStorage storage Token
                SuperApiAop = new SuperApiAop() // Super API Interceptor
            }
        };

        // Register Super API
        services.AddReZeroServices(api =>
        {
            api.EnableSuperApi(superAPIOption);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}