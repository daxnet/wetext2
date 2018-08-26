// ----------------------------------------------------------------------------
//       ___ ___  ___     ___
// |  | |__   |  |__  \_/  |
// |/\| |___  |  |___ / \  |
//
// Yet another WeText application for demonstration.
// MIT License
//
// Copyright (c) 2018 Sunny Chen (daxnet)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ----------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeText.Common
{
    public sealed class CryptoService
    {
        #region Private Fields

        private const CryptoTypes CryptDefaultMethod = CryptoTypes.EncAes;

        private const string CryptDefaultPassword = "CB06cfE507a1";

        private readonly byte[] SaltByteArray = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        private CryptoTypes mCryptoType = CryptDefaultMethod;

        private byte[] mIV = { 65, 110, 68, 26, 69, 178, 200, 219, 117, 93, 12, 31, 106, 199, 201, 20 };

        private byte[] mKey = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

        private string mPassword = CryptDefaultPassword;

        #endregion Private Fields

        #region Public Constructors

        public CryptoService()
        {
            calculateNewKeyAndIV();
        }

        public CryptoService(CryptoTypes cryptoType)
            : this()
        {
            this.CryptoType = cryptoType;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        ///     Passsword Key Property.
        ///     The password key used when encrypting / decrypting
        /// </summary>
        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                if (mPassword != value)
                {
                    mPassword = value;
                    calculateNewKeyAndIV();
                }
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        ///     type of encryption / decryption used
        /// </summary>
        internal CryptoTypes CryptoType
        {
            get
            {
                return mCryptoType;
            }
            set
            {
                if (mCryptoType != value)
                {
                    mCryptoType = value;
                    calculateNewKeyAndIV();
                }
            }
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        /// Encrypts the specified <see cref="string"/>.
        /// </summary>
        /// <param name="source">The <see cref="string"/> to be encrypted.</param>
        /// <param name="salt">The salt string.</param>
        /// <param name="encoding">The <see cref="Encoding"/> instance to be used when encrypting the string.</param>
        /// <returns>The encrypted string.</returns>
        public static string ComputeHash(string source, string salt, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = new UTF8Encoding();
            var provider = SHA1.Create();
            var encryptedPasswordArray = provider.ComputeHash(encoding.GetBytes(string.Concat(source, salt)));
            return Convert.ToBase64String(encryptedPasswordArray);
        }

        public static CryptoService Create(CryptoTypes type)
        {
            return new CryptoService(type);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static CryptoService CreateDefaultCrypto()
        {
            return new CryptoService(CryptoTypes.EncAes);
        }

        /// <summary>
        ///     decrypts a string
        /// </summary>
        /// <param name="inputText">string to decrypt</param>
        /// <returns>a decrypted string</returns>
        public string Decrypt(string inputText)
        {
            //declare a new encoder
            var utf8Encoder = new UTF8Encoding();
            //get byte representation of string
            byte[] inputBytes = Convert.FromBase64String(inputText);

            //convert back to a string
            return utf8Encoder.GetString(EncryptDecrypt(inputBytes, false));
        }

        /// <summary>
        ///     decrypts a string using a user defined password key
        /// </summary>
        /// <param name="inputText">string to decrypt</param>
        /// <param name="password">password to use when decrypting</param>
        /// <returns>a decrypted string</returns>
        public string Decrypt(string inputText, string password)
        {
            if (!string.IsNullOrEmpty(inputText) &&
                !string.IsNullOrEmpty(password))
            {
                this.Password = password;
                return Decrypt(inputText);
            }
            return null;
        }

        /// <summary>
        ///     Encrypt a string
        /// </summary>
        /// <param name="inputText">text to encrypt</param>
        /// <returns>an encrypted string</returns>
        public string Encrypt(string inputText)
        {
            //declare a new encoder
            var utf8Encoder = new UTF8Encoding();
            //get byte representation of string
            byte[] inputBytes = utf8Encoder.GetBytes(inputText);

            //convert back to a string
            return Convert.ToBase64String(EncryptDecrypt(inputBytes, true));
        }

        /// <summary>
        ///     Encrypt string with user defined password
        /// </summary>
        /// <param name="inputText">text to encrypt</param>
        /// <param name="password">password to use when encrypting</param>
        /// <returns>an encrypted string</returns>
        public string Encrypt(string inputText, string password)
        {
            if (!string.IsNullOrEmpty(inputText) &&
                !string.IsNullOrEmpty(password))
            {
                this.Password = password;
                return this.Encrypt(inputText);
            }
            return null;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        ///     decrypts a string acc. to decryption type and user defined password key
        /// </summary>
        /// <param name="inputText">string to decrypt</param>
        /// <param name="password">password key used to decrypt</param>
        /// <param name="cryptoType">type of decryption</param>
        /// <returns>a decrypted string</returns>
        internal string Decrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return Decrypt(inputText, password);
        }

        /// <summary>
        ///     decrypts a string acc. to the decryption type
        /// </summary>
        /// <param name="inputText">string to decrypt</param>
        /// <param name="cryptoType">type of decryption</param>
        /// <returns>a decrypted string</returns>
        internal string Decrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return Decrypt(inputText);
        }

        /// <summary>
        ///     Encrypt string acc. to cryptoType and with user defined password
        /// </summary>
        /// <param name="inputText">text to encrypt</param>
        /// <param name="password">password to use when encrypting</param>
        /// <param name="cryptoType">type of encryption</param>
        /// <returns>an encrypted string</returns>
        internal string Encrypt(string inputText, string password, CryptoTypes cryptoType)
        {
            mCryptoType = cryptoType;
            return this.Encrypt(inputText, password);
        }

        /// <summary>
        ///     Encrypt string acc. to cryptoType
        /// </summary>
        /// <param name="inputText">text to encrypt</param>
        /// <param name="cryptoType">type of encryption</param>
        /// <returns>an encrypted string</returns>
        internal string Encrypt(string inputText, CryptoTypes cryptoType)
        {
            this.CryptoType = cryptoType;
            return this.Encrypt(inputText);
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        ///     calculates the key and IV acc. to the symmetric method from the password
        ///     key and IV size dependant on symmetric method
        /// </summary>
        private void calculateNewKeyAndIV()
        {
            //use salt so that key cannot be found with dictionary attack
            var pdb = new Rfc2898DeriveBytes(mPassword, SaltByteArray);
            SymmetricAlgorithm algo = selectAlgorithm();
            mKey = pdb.GetBytes(algo.KeySize / 8);
            mIV = pdb.GetBytes(algo.BlockSize / 8);
        }

        /// <summary>
        ///     performs the actual enc/dec.
        /// </summary>
        /// <param name="inputBytes">input byte array</param>
        /// <param name="Encrpyt">wheather or not to perform enc/dec</param>
        /// <returns>byte array output</returns>
        private byte[] EncryptDecrypt(byte[] inputBytes, bool Encrpyt)
        {
            //get the correct transform
            var transform = getCryptoTransform(Encrpyt);

            //memory stream for output
            using (var memStream = new MemoryStream())
            {
                try
                {
                    //setup the cryption - output written to memstream
                    var cryptStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);

                    //write data to cryption engine
                    cryptStream.Write(inputBytes, 0, inputBytes.Length);

                    //we are finished
                    cryptStream.FlushFinalBlock();

                    //get result
                    var output = memStream.ToArray();

                    //finished with engine, so close the stream
                    //cryptStream.Close();

                    return output;
                }
                catch (Exception e)
                {
                    //throw an error
                    throw new Exception("Error in symmetric engine. Error : " + e.Message, e);
                }
            }
        }

        /// <summary>
        ///     returns the symmetric engine and creates the encyptor/decryptor
        /// </summary>
        /// <param name="encrypt">whether to return a encrpytor or decryptor</param>
        /// <returns>ICryptoTransform</returns>
        private ICryptoTransform getCryptoTransform(bool encrypt)
        {
            var sa = selectAlgorithm();
            sa.Key = mKey;
            sa.IV = mIV;
            if (encrypt)
            {
                return sa.CreateEncryptor();
            }
            return sa.CreateDecryptor();
        }

        /// <summary>
        ///     returns the specific symmetric algorithm acc. to the cryptotype
        /// </summary>
        /// <returns>SymmetricAlgorithm</returns>
        private SymmetricAlgorithm selectAlgorithm()
        {
            SymmetricAlgorithm sa;
            switch (mCryptoType)
            {
                case CryptoTypes.EncAes:
                    sa = Aes.Create();
                    break;

                case CryptoTypes.EncTypeTripleDes:
                    sa = TripleDES.Create();
                    break;

                default:
                    sa = TripleDES.Create();
                    break;
            }
            return sa;
        }

        #endregion Private Methods

        #region Public Enums

        public enum CryptoTypes
        {
            EncAes = 0,
            EncTypeTripleDes
        }

        #endregion Public Enums
    }

    /// <summary>
    /// Hashing class. Only static members so no need to create an instance
    /// </summary>
    public class Hashing
    {
        #region Public Methods

        public static string Hash(String inputText)
        {
            return ComputeHash(inputText, HashingTypes.MD5);
        }

        public static string Hash(String inputText, HashingTypes hashingType)
        {
            return ComputeHash(inputText, hashingType);
        }

        /// <summary>
        ///     returns true if the input text is equal to hashed text
        /// </summary>
        /// <param name="inputText">unhashed text to test</param>
        /// <param name="hashText">already hashed text</param>
        /// <returns>boolean true or false</returns>
        public static bool isHashEqual(string inputText, string hashText)
        {
            return (Hash(inputText) == hashText);
        }

        public static bool isHashEqual(string inputText, string hashText, HashingTypes hashingType)
        {
            return (Hash(inputText, hashingType) == hashText);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///     computes the hash code and converts it to string
        /// </summary>
        /// <param name="inputText">input text to be hashed</param>
        /// <param name="hashingType">type of hashing to use</param>
        /// <returns>hashed string</returns>
        private static string ComputeHash(string inputText, HashingTypes hashingType)
        {
            var ha = getHashAlgorithm(hashingType);

            //declare a new encoder
            var utf8Encoder = new UTF8Encoding();
            //get byte representation of input text
            byte[] inputBytes = utf8Encoder.GetBytes(inputText);

            //hash the input byte array
            byte[] output = ha.ComputeHash(inputBytes);

            //convert output byte array to a string
            return Convert.ToBase64String(output);
        }

        /// <summary>
        ///     returns the specific hashing alorithm
        /// </summary>
        /// <param name="hashingType">type of hashing to use</param>
        /// <returns>HashAlgorithm</returns>
        private static HashAlgorithm getHashAlgorithm(HashingTypes hashingType)
        {
            switch (hashingType)
            {
                case HashingTypes.MD5:
                    return MD5.Create();

                case HashingTypes.SHA:
                    return SHA1.Create();

                case HashingTypes.SHA256:
                    return SHA256.Create();

                case HashingTypes.SHA384:
                    return SHA384.Create();

                case HashingTypes.SHA512:
                    return SHA512.Create();

                default:
                    return MD5.Create();
            }
        }

        #endregion Private Methods

        #region Public Enums

        //types of hashing available
        public enum HashingTypes
        {
            SHA, SHA256, SHA384, SHA512, MD5
        }

        #endregion Public Enums
    }
}