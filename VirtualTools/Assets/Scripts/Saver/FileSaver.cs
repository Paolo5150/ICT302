using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class FileSaver
{
    private static readonly string keyString = "111 192 34 149 21 46 249 203 233 24 21 152 226 218 169 215 104 43 18 180 104 19 12 20 37 3 7 223 58 70 222 98";
    private static readonly byte[] key = GetBytes(keyString);

    static AesManaged m_aes;
    static ICryptoTransform m_encryptor;
    static ICryptoTransform m_decryptor;
    public static void Init()
    {
        m_aes = new AesManaged();
        m_aes.Key = key;
        m_aes.GenerateIV();
        m_encryptor = m_aes.CreateEncryptor(key, m_aes.IV);
        m_decryptor = m_aes.CreateDecryptor(key, m_aes.IV);
    }

    public static byte[] Encrypt(string plainText)
    {
        byte[] encrypted;
   
        // Create encryptor    
        ICryptoTransform encryptor = m_aes.CreateEncryptor(m_aes.Key, m_aes.IV);
        // Create MemoryStream    
        using (MemoryStream ms = new MemoryStream())
        {
            // Create crypto stream using the CryptoStream class. This class is the key to encryption    
            // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
            // to encrypt    
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                // Create StreamWriter and write data to a stream    
                using (StreamWriter sw = new StreamWriter(cs))
                    sw.Write(plainText);
                encrypted = ms.ToArray();
            }
        }
  
        // Return encrypted data    
        return encrypted;
    }

    public static string Decrypt(byte[] cipherText)
    {
        string plaintext = null;

        // Create the streams used for decryption.    
        using (MemoryStream ms = new MemoryStream(cipherText))
        {
            // Create crypto stream    
            using (CryptoStream cs = new CryptoStream(ms, m_decryptor, CryptoStreamMode.Read))
            {
                // Read crypto stream    
                using (StreamReader reader = new StreamReader(cs))
                    plaintext = reader.ReadToEnd();
            }
        }
        return plaintext;
    }

    static byte[] GetBytes(string pData)
    {
        string[] encrypted = pData.Split(char.Parse(" "));
        byte[] bytes = new byte[encrypted.Length];
        int len = encrypted.Length;

        for (int i = 0; i < len; ++i)
        { bytes[i] = byte.Parse(encrypted[i]); }
        return bytes;
    }
}
