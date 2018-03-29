using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hyperledger.Indy;
using Hyperledger.Indy.WalletApi;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaultBlock.Data;
using WaultBlock.Models;
using WaultBlock.Utils;

namespace WaultBlock.Identities
{
    public class InDbWalletType : WalletType
    {
        public const string WAULT_TYPE = "indb";
        private static int _nextWalletHandle = 0;
        private ApplicationDbContext _dbContext;
        private IDictionary<int, InDbWallet> _openedWallets;

        public InDbWalletType(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _openedWallets = new ConcurrentDictionary<int, InDbWallet>();
        }

        public override ErrorCode Close(int walletHandle)
        {
            InDbWallet indyWallet;
            try
            {
                indyWallet = (InDbWallet)GetWalletByHandle(walletHandle);
            }
            catch (Exception)
            {
                return ErrorCode.WalletInvalidHandle;
            }

            indyWallet.Close();
            _openedWallets.Remove(walletHandle);
            return ErrorCode.Success;
        }

        /// <summary>
        /// Allows an implementer to create a new wallet.
        /// </summary>
        /// <param name="name">The name of the wallet to create.</param>
        /// <param name="config">The configuration for the wallet.</param>
        /// <param name="credentialsString">The credentials for the wallet.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>
        /// <remarks>
        /// When implementing a custom wallet this method is responsible for creating the new wallet
        /// and storing its configuration and credentials.
        /// </remarks>
        public override ErrorCode Create(string name, string config, string credentialsString)
        {
            ParamsGuard.NotNullOrEmpty(name, nameof(name));

            var credentials = ParseCredentials(credentialsString);

            if (_dbContext.WaultWallets.Any(p => p.Name == name && p.UserId == credentials.UserId))
            {
                return ErrorCode.WalletAlreadyExistsError;
            }

            double? freshnessDuration = 1000;

            if (!string.IsNullOrEmpty(config))
            {
                var configObj = JObject.Parse(config);
                var configuredFreshness = configObj.Value<double?>("freshness_time");
                if (configuredFreshness.HasValue)
                {
                    freshnessDuration = configuredFreshness;
                }
            }

            var wallet = new WaultWallet
            {
                FreshnessDuration = freshnessDuration,
                Name = name,
                UserId = credentials.UserId,
                IsOpen = false
            };

            _dbContext.WaultWallets.Add(wallet);
            _dbContext.SaveChanges();

            return ErrorCode.Success;
        }

        /// <summary>
        /// Allows an implementer to delete a wallet.
        /// </summary>
        /// <param name="name">The name of the wallet being deleted</param>
        /// <param name="config">The configuration of the wallet.</param>
        /// <param name="credentialsString">The credentials of the wallet.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>
        /// <remarks>
        /// When implementing a custom wallet this method is responsible for deleting a wallet created
        /// earlier via the <see cref="M:Hyperledger.Indy.WalletApi.WalletType.Create(System.String,System.String,System.String)" /> method.  The value of the
        /// <paramref name="credentials" /> parameter should be used to control access whether or not
        /// the wallet can be deleted.
        /// </remarks>
        public override ErrorCode Delete(string name, string config, string credentialsString)
        {
            ParamsGuard.NotNullOrEmpty(name, nameof(name));

            var credentialObj = ParseCredentials(credentialsString);

            var wallet = _dbContext.WaultWallets
                .Include(p => p.Records)
                .FirstOrDefault(p => p.Name == name && p.UserId == credentialObj.UserId);

            if (wallet == null)
            {
                return ErrorCode.CommonInvalidState;
            }

            if (wallet.IsOpen)
            {
                return ErrorCode.CommonInvalidState;
            }
            _dbContext.WaultWalletRecords.RemoveRange(wallet.Records);
            _dbContext.WaultWallets.Remove(wallet);
            _dbContext.SaveChanges();

            return ErrorCode.Success;
        }

        public override ErrorCode Open(string name, string config, string runtimeConfig, string credentialsString, out int walletHandle)
        {
            ParamsGuard.NotNullOrEmpty(name, nameof(name));

            var credentialObj = ParseCredentials(credentialsString);

            walletHandle = -1;
            var wallet = _dbContext.WaultWallets.FirstOrDefault(p => p.Name == name && p.UserId == credentialObj.UserId);
            if (wallet == null)
            {
                return ErrorCode.CommonInvalidState;
            }

            if (wallet.IsOpen)
            {
                return ErrorCode.WalletAlreadyOpenedError;
            }

            wallet.IsOpen = true;
            _dbContext.Update(wallet);
            _dbContext.SaveChanges();

            walletHandle = GetNextWalletHandle();

            var indyWallet = new InDbWallet(wallet, _dbContext);
            _openedWallets.Add(walletHandle, indyWallet);

            return ErrorCode.Success;
        }

        protected override ICustomWallet GetWalletByHandle(int walletHandle)
        {
            return _openedWallets[walletHandle];
        }

        private int GetNextWalletHandle()
        {
            return Interlocked.Increment(ref _nextWalletHandle);
        }

        private Credentials ParseCredentials(string credentialsString)
        {
            ParamsGuard.NotNullOrEmpty(credentialsString, nameof(credentialsString));

            var result = JsonConvert.DeserializeObject<Credentials>(credentialsString);
            if (result == null)
            {
                throw new ArgumentException("Invalid credentials format", "credentials");
            }

            return result;
        }

        private class Credentials
        {
            [JsonProperty("userId")]
            public string UserId { get; set; }
        }
    }
}
