using MyBookCloud.Core.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Core.Api.Connectors
{
    public interface IGoogleBookApiConnector
    {
        Task<GoogleBookVolumeInfoDto?> GetVolumeInfoAsync(string isbn);
    }
}
