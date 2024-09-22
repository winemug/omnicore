using OmniCore.Client.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.Settings;

namespace OmniCore.Client.Platforms
{
    public class PlatformInfoService : IPlatformInfoService
    {
        public string GetClientIdentifier()
        {
            return DeviceInfo.Current.Name;
        }
    }
}
