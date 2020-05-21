using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Utility
{
public class PasswordManager
    {

        public static string GetPasswordHashAndSalt(string password)
        {
            // Let us use SHA256 algorithm to 
            // generate the hash from this salted password
            SHA256 sha = new SHA256CryptoServiceProvider();
            byte[] dataBytes = Encoding.ASCII.GetBytes(password);
            byte[] resultBytes = sha.ComputeHash(dataBytes);
            // return the hash string to the caller
            return Encoding.ASCII.GetString(resultBytes);
        }


        public static string GeneratePasswordHash(string plainTextPassword, out string salt)
            {
                salt = SaltGenerator.GetSaltString();

                string finalString = plainTextPassword + salt;

                return GetPasswordHashAndSalt(finalString);
            }


        public static bool IsPasswordMatch(string password, string salt, string hash)
            {
                string finalString = password + salt;
                return hash == GetPasswordHashAndSalt(finalString);
            }
        
        public static string GeneratePassword(int length)
        {
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static string GeneratePassword1()
        {
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            const string valid1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            res.Append(valid1[rnd.Next(valid1.Length)]);
            const string valid2 = "abcdefghijklmnopqrstuvwxyz";
            res.Append(valid2[rnd.Next(valid2.Length)]);
            const string valid3 = "1234567890";
            res.Append(valid3[rnd.Next(valid3.Length)]);
            const string valid4 = "@/+_!-$#~";
            res.Append(valid4[rnd.Next(valid4.Length)]);

            int length1 = rnd.Next(2, 9);
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            while (0 < length1--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);

            }
            string shuffledPassword = Shuffle.StringMixer(res.ToString());

            return shuffledPassword;
        }
    }
    public static class SaltGenerator
    {
        private static RNGCryptoServiceProvider m_cryptoServiceProvider = null;
        private const int SALT_SIZE = 24;

        static SaltGenerator()
        {
            m_cryptoServiceProvider = new RNGCryptoServiceProvider();
        }

        public static string GetSaltString()
        {
            // Lets create a byte array to store the salt bytes
            byte[] saltBytes = new byte[SALT_SIZE];

            // lets generate the salt in the byte array
            m_cryptoServiceProvider.GetNonZeroBytes(saltBytes);

            // Let us get some string representation for this salt
            string saltString = Encoding.ASCII.GetString(saltBytes);

            // Now we have our salt string ready lets return it to the caller
            return saltString;
        }
    }

    class Shuffle
    {
        static System.Random rnd = new System.Random();

        static void RandomizingArray(int[] array)
        {
            int arraysize = array.Length;
            int random;
            int temp;

            for (int i = 0; i < arraysize; i++)
            {
                random = i + (int)(rnd.NextDouble() * (arraysize - i));

                temp = array[random];
                array[random] = array[i];
                array[i] = temp;
            }
        }

        public static string StringMixer(string password)
        {
            string output = "";
            int arraysize = password.Length;
            int[] randomArray = new int[arraysize];

            for (int i = 0; i < arraysize; i++)
            {
                randomArray[i] = i;
            }

            RandomizingArray(randomArray);

            for (int i = 0; i < arraysize; i++)
            {
                output += password[randomArray[i]];
            }

            return output;
        }
    }
}
