namespace AuthServer.Data.Entities;

public class ClientSecret
{
    public int Id { get; set; }
    public string Secret { get; set; }
    public int ClientId { get; set; }
}