﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BirdsiteLive.Domain.Factories;

namespace BirdsiteLive.Domain
{
    public interface ICryptoService
    {
        Task<string> GetUserPem(string id);
        Task<string> SignAndGetSignatureHeader(DateTime date, string actor, string host, string digest, string inbox);
        string ComputeSha256Hash(string data);
    }

    public class CryptoService : ICryptoService
    {
        private readonly IMagicKeyFactory _magicKeyFactory;

        #region Ctor
        public CryptoService(IMagicKeyFactory magicKeyFactory)
        {
            _magicKeyFactory = magicKeyFactory;
        }
        #endregion

        public async Task<string> GetUserPem(string id)
        {
            var key = await _magicKeyFactory.GetMagicKey();
            return key.AsPEM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="actor">in the form of https://domain.io/actor</param>
        /// <param name="host">in the form of domain.io</param>
        /// <returns></returns>
        public async Task<string> SignAndGetSignatureHeader(DateTime date, string actor, string targethost, string digest, string inbox)
        {
            var usedInbox = "/inbox";
            if (!string.IsNullOrWhiteSpace(inbox))
                usedInbox = inbox;

            var httpDate = date.ToString("r");

            var signedString = $"(request-target): post {usedInbox}\nhost: {targethost}\ndate: {httpDate}\ndigest: SHA-256={digest}";
            var signedStringBytes = Encoding.UTF8.GetBytes(signedString);
            var key = await _magicKeyFactory.GetMagicKey();
            var signature = key.Sign(signedStringBytes);
            var sig64 = Convert.ToBase64String(signature);

            var header = "keyId=\"" + actor + "\",algorithm=\"rsa-sha256\",headers=\"(request-target) host date digest\",signature=\"" + sig64 + "\"";
            return header;
        }

        public string ComputeSha256Hash(string data)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}