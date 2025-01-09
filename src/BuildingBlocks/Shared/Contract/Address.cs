namespace Shared.Contract;

public class Address
{
    public string AddressText { get; set; }
    public long? Latitude { get; set; }
    public long? Longitude { get; set; }
    public string? GoogleMapsUrl { get; set; }
}