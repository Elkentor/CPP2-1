using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public static class SaveSystem
{
    // File name
    private static readonly string Filename = "save_encrypted.dat";

    // Passphrase for key derivation (prototype only). Change or prompt user for production.
    private static readonly string Passphrase = "MyPrototypeSecretPassphrase!2025";

    // Salt (should be fixed or can be random then stored with file). We'll use a fixed salt for prototype.
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("PrototypeSalt1234");

    // AES parameters
    private const int KeySizeBits = 256;
    private const int Iterations = 10000; // PBKDF2 iterations

    // ---- Public API ----
    public static void Save(SaveData data)
    {
        try
        {
            // ?? Add this line right here:
            data.SavedAt = DateTime.Now.ToString();

            // 1) Serialize to XML string
            var serializer = new XmlSerializer(typeof(SaveData));
            string xml;
            using (var sw = new StringWriter())
            {
                serializer.Serialize(sw, data);
                xml = sw.ToString();
            }

            // 2) Encrypt XML -> bytes
            byte[] encrypted = EncryptStringToBytes_Aes(xml, Passphrase, Salt);

            // 3) Write to file
            string path = Path.Combine(Application.persistentDataPath, Filename);
            File.WriteAllBytes(path, encrypted);

            Debug.Log($"SaveSystem: Saved encrypted file to: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveSystem.Save exception: {ex}");
        }
    }

    public static SaveData Load()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, Filename);
            if (!File.Exists(path))
            {
                Debug.Log("SaveSystem: No save file found.");
                return null;
            }

            byte[] encrypted = File.ReadAllBytes(path);
            string xml = DecryptStringFromBytes_Aes(encrypted, Passphrase, Salt);

            var serializer = new XmlSerializer(typeof(SaveData));
            using (var sr = new StringReader(xml))
            {
                var data = (SaveData)serializer.Deserialize(sr);
                Debug.Log("SaveSystem: Loaded and decrypted save file.");
                return data;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveSystem.Load exception: {ex}");
            return null;
        }
    }

    // ---- Encryption helpers ----

    private static byte[] EncryptStringToBytes_Aes(string plainText, string passphrase, byte[] salt)
    {
        using (var aes = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(passphrase, salt, Iterations);
            aes.Key = key.GetBytes(KeySizeBits / 8);
            aes.GenerateIV(); // random IV for each encryption

            using (var ms = new MemoryStream())
            {
                // prepend IV to the ciphertext
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return ms.ToArray();
            }
        }
    }

    private static string DecryptStringFromBytes_Aes(byte[] cipherWithIv, string passphrase, byte[] salt)
    {
        using (var aes = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(passphrase, salt, Iterations);
            aes.Key = key.GetBytes(KeySizeBits / 8);

            // Extract IV from beginning
            int ivLength = aes.BlockSize / 8;
            byte[] iv = new byte[ivLength];
            Array.Copy(cipherWithIv, 0, iv, 0, ivLength);

            int cipherIndex = ivLength;
            int cipherLength = cipherWithIv.Length - ivLength;

            using (var ms = new MemoryStream(cipherWithIv, cipherIndex, cipherLength))
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    // Optional utilities
    public static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, Filename);
    }

    public static bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }

    public static void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path)) File.Delete(path);
    }
}