namespace APCServer.Services;

using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;

// Interface (isn't really necessary here because of how small this use case is but
// something something learning experience)
public interface IHashService
{
    public Task<string> HashAsync(string input);
    public Task<bool> CompareAsync(string input, string existingHash);
}

public class Argon2HashService : IHashService
{
    private const int Memory = 32768;
    private const int Parallelism = 1;
    private const int Iterations = 4;
    private const int HashSize = 32;
    private const int SaltSize = 16;

    public async Task<string> HashAsync(string input)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        
        byte[] hash = await HashToBytesAsync(input, salt, Parallelism, Iterations, Memory);

        // standard PHC format
        return $"$argon2id$v=19$m={Memory},t={Iterations},p={Parallelism}${Convert.ToBase64String(salt).TrimEnd('=')}${Convert.ToBase64String(hash).TrimEnd('=')}";
    }

    //Basically a copy paste of above 
    public async Task<bool> CompareAsync(string input, string existingHash)
    {
        string[] splitExisting = existingHash.Trim('$').Split('$');

        if(splitExisting.Length != 5)
            return false;
            

        string[] parameters = splitExisting[2].Split(',');

        if(parameters.Length != 3)
            return false;

        try
        {
            int memory = int.Parse(parameters[0].Replace("m=", ""));
            int iterations = int.Parse(parameters[1].Replace("t=", ""));
            int parallelism = int.Parse(parameters[2].Replace("p=", ""));

            // re-add the padding = or conversion will fail
            byte[] salt = Convert.FromBase64String(RestorePadding(splitExisting[3]));
            byte[] expectedHash = Convert.FromBase64String(RestorePadding(splitExisting[4]));

            byte[] hash = await HashToBytesAsync(input, salt, parallelism, iterations, memory);

            // standard phc format
            return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
        }
        catch
        {
  
            // If convert fails then its malformed and as such invalid
            return false;
        }       
    }


    // DRY
    private async Task<byte[]> HashToBytesAsync(string input, byte[] salt, int parallelism, int iterations, int memory)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        using var argon2 = new Argon2id(inputBytes)
        {
            Salt = salt,
            DegreeOfParallelism = parallelism,
            Iterations = iterations,
            MemorySize = memory  
        };
        
        return await argon2.GetBytesAsync(HashSize);
    }

    // Helper function to restore base64 padding (still don't get why phc standard
    // asks we strip it in the first place???)
    private string RestorePadding(string input)
    {
        int r = input.Length % 4;
        if (r == 0) return input;
        
        return input.PadRight(input.Length + (4 - r), '=');
    }
}
