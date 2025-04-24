// AnomalyDetection.Web/Services/DataIngestion/NetworkTrafficLogMap.cs
using AnomalyDetection.Core.Entities;
using CsvHelper.Configuration;

namespace AnomalyDetection.Web.Services.DataIngestion;

public class NetworkTrafficLogMap : ClassMap<NetworkTrafficLog>
{
    public NetworkTrafficLogMap()
    {
        Map(m => m.Timestamp).Name("timestamp");
        Map(m => m.SourceIp).Name("src_ip");
        Map(m => m.DestinationIp).Name("dst_ip");
        Map(m => m.SourcePort).Name("src_port");
        Map(m => m.DestinationPort).Name("dst_port");
        Map(m => m.Protocol).Name("protocol");
        Map(m => m.PacketSize).Name("bytes");
        Map(m => m.Duration).Name("duration");
        Map(m => m.PacketCount).Name("packets");
        Map(m => m.Flags).Name("flags");
    }
}
