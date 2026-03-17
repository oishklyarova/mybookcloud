using MyBookCloud.Application.Dto;

namespace MyBookCloud.Application.Connectors
{
    public interface IGoogleBookApiConnector
    {
        Task<GoogleBookVolumeInfoDto?> GetVolumeInfoAsync(string isbn);
    }
}
