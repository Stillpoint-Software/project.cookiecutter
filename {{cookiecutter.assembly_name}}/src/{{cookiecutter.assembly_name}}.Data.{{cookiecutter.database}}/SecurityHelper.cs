using System.Security.Cryptography;
using System.Text;

namespace {{cookiecutter.assembly_name}}.Data.Mongo;

public static class SecurityHelper
{
    private static readonly string _encryptionKey = "mysecretkey";
    public static string EncryptValue( string value )
    {
        if (string.IsNullOrEmpty( value ))
            return value;


        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes( _encryptionKey );  // 16 bytes key for AES-128

        aesAlg.GenerateIV();
        byte[] iv = aesAlg.IV;

        using var encryptor = aesAlg.CreateEncryptor( aesAlg.Key, iv );
        byte[] valueBytes = Encoding.UTF8.GetBytes( value );
        byte[] encryptedBytes = encryptor.TransformFinalBlock( valueBytes, 0, valueBytes.Length );

        // Convert the encrypted byte array to a Base64 string
        string encryptedString = Convert.ToBase64String( encryptedBytes );

        string ivBase64 = Convert.ToBase64String( iv );
        return ivBase64 + ":" + encryptedString;  // Store IV and encrypted value together
    }

    // Method to decrypt the property value using AES
    public static string DecryptValue( string encryptedValue )
    {
        if (string.IsNullOrEmpty( encryptedValue ))
            return encryptedValue;


        // Split the stored value into IV and encrypted data
        var parts = encryptedValue.Split( ':' );
        if (parts.Length != 2)
            throw new FormatException( "Invalid encrypted value format." );

        var ivBase64 = parts[0];
        var encryptedBase64 = parts[1];

        byte[] iv = Convert.FromBase64String( ivBase64 );
        byte[] encryptedBytes = Convert.FromBase64String( encryptedBase64 );

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes( _encryptionKey );  // 16 bytes key for AES-128

        using var decryptor = aesAlg.CreateDecryptor( aesAlg.Key, iv );
        byte[] decryptedBytes = decryptor.TransformFinalBlock( encryptedBytes, 0, encryptedBytes.Length );

        // Convert decrypted byte array back to string
        return Encoding.UTF8.GetString( decryptedBytes );
    }
}