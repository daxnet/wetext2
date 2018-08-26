using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WeText.Common
{
    public static class Utils
    {
        private static readonly CryptoService cryptoService = CryptoService.Create(CryptoService.CryptoTypes.EncTypeTripleDes);

        public static void ConcurrentDictionarySafeRegister<TKey, TValue>(TKey key, TValue value, ConcurrentDictionary<TKey, List<TValue>> registry)
        {
            if (registry.TryGetValue(key, out List<TValue> registryItem))
            {
                if (registryItem != null)
                {
                    if (!registryItem.Contains(value))
                    {
                        registry[key].Add(value);
                    }
                }
                else
                {
                    registry[key] = new List<TValue> { value };
                }
            }
            else
            {
                registry.TryAdd(key, new List<TValue> { value });
            }
        }

        /// <summary>
        /// Encrypts the password by using the given salt.
        /// </summary>
        /// <param name="password">The password to be encrypted.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The encrypted password string.</returns>
        public static string EncryptPassword(string password, string salt)
            => cryptoService.Encrypt(password, salt);
    }
}
