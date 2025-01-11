namespace Shared.Contracts;

public class Address
{
    public string AddressText { get; set; }
    public long? Latitude { get; set; }
    public long? Longitude { get; set; }
    public string? GoogleMapsUrl { get; set; }
}