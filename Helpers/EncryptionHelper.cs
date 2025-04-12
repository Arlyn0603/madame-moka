using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace madame_moka.Helpers
{
	public static class EncryptionHelper
	{
		private static readonly string key = "1234567890123456"; // Clave de 16 caracteres
		private static readonly string iv = "6543210987654321";  // IV de 16 caracteres

		public static string Encrypt(string plainText)
		{
			if (string.IsNullOrEmpty(plainText)) return plainText;

			using (Aes aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(key);
				aes.IV = Encoding.UTF8.GetBytes(iv);

				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
					using (StreamWriter sw = new StreamWriter(cs))
					{
						sw.Write(plainText);
					}

					return Convert.ToBase64String(ms.ToArray());
				}
			}
		}

		public static string Decrypt(string cipherText)
		{
			if (string.IsNullOrEmpty(cipherText)) return cipherText;

			try
			{
				using (Aes aes = Aes.Create())
				{
					aes.Key = Encoding.UTF8.GetBytes(key);
					aes.IV = Encoding.UTF8.GetBytes(iv);

					byte[] buffer = Convert.FromBase64String(cipherText);

					using (MemoryStream ms = new MemoryStream(buffer))
					{
						using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
						using (StreamReader sr = new StreamReader(cs))
						{
							return sr.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error al desencriptar: {ex.Message}");
				return "ERROR";
			}
		}
	}
}
