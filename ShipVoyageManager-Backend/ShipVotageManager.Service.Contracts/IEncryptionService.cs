namespace ShipVoyageManager.Service.Contracts;
public interface IEncryptionService
{
    public string GenerateHashedPassword(string password);
}
