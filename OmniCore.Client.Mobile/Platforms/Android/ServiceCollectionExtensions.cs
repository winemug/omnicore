﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniCore.Client.Interfaces.Services;
using OmniCore.Client.Mobile.Platforms.Android;

namespace OmniCore.Client.Mobile.Services
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterPlatformServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IPlatformPermissionService, PlatformPermissionService>()
                .AddSingleton<IPlatformForegroundService, PlatformForegroundService>();
        }
    }
}
