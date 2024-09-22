using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmniCore.Client.Abstractions.Services;

public interface IPlatformInfoService
{
    string GetClientIdentifier();
}
