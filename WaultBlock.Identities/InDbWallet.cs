using System;
using System.Linq;
using Hyperledger.Indy;
using Hyperledger.Indy.WalletApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaultBlock.Data;
using WaultBlock.Models;

namespace WaultBlock.Identities
{
    public class InDbWallet : ICustomWallet
    {
        private ApplicationDbContext _dbContext;
        private WalletData _walletData;

        public InDbWallet(WalletData walletData, ApplicationDbContext dbContext)
        {
            _walletData = walletData;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Allows an implementer to get a value from the wallet.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The value obtained from the wallet.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks>
        /// If the key does not exist the method should return <see cref="F:Hyperledger.Indy.ErrorCode.WalletNotFoundError" />.
        /// </remarks>
        public ErrorCode Get(string key, out string value)
        {
            value = null;
            var record = _dbContext.WalletRecords.FirstOrDefault(p =>
                        p.Key == key &&
                        p.WalletName == _walletData.Name &&
                        p.UserId == _walletData.UserId);
            if (record == null)
            {
                return ErrorCode.WalletNotFoundError;
            }

            value = record.Value;
            return ErrorCode.Success;
        }

        /// <summary>
        /// Allows an implementer to get a value from the wallet if it has not expired.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">The value obtained from the wallet.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>

        /// <remarks>
        /// If the key does not exist or the record associated with the key has
        /// expired then the method should return <see cref="F:Hyperledger.Indy.ErrorCode.WalletNotFoundError" />.
        /// </remarks>
        public ErrorCode GetNotExpired(string key, out string value)
        {
            value = null;

            var record = _dbContext.WalletRecords.FirstOrDefault(p =>
                        p.Key == key &&
                        p.WalletName == _walletData.Name &&
                        p.UserId == _walletData.UserId);
            if (record == null)
            {
                return ErrorCode.WalletNotFoundError;
            }

            var recordAge = DateTime.UtcNow - record.TimeCreated;
            if (recordAge.Seconds > _walletData.FreshnessDuration)
            {
                return ErrorCode.WalletNotFoundError;
            }

            value = record.Value;
            return ErrorCode.Success;
        }

        /// <summary>
        /// Allows an implementer to get a list of values from the wallet that match a key prefix.
        /// </summary>
        /// <param name="keyPrefix">The key prefix for the values requested.</param>
        /// <param name="valuesJson">The JSON string containing the values associated with the key prefix.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>

        /// <remarks>
        /// The method should return a JSON string that conforms to the following format:
        /// <code>
        /// {
        /// "values":[
        /// {"key":"key_1", "value":"value_1"},
        /// ...
        /// ]
        /// }
        /// </code>
        /// If no values matching the <paramref name="keyPrefix" /> parameter are found the <c>values</c>
        /// array in the JSON should be empty.
        /// </remarks>
        public ErrorCode List(string keyPrefix, out string valuesJson)
        {
            var records = _dbContext.WalletRecords.Where(p => p.Key.StartsWith(keyPrefix)).ToList();
            var valuesArray = new JArray();
            foreach (var record in records)
            {
                var value = new JObject
                {
                    { "key", record.Key },
                    { "value", record.Value }
                };

                valuesArray.Add(value);
            }

            var valuesJObject = new JObject
            {
                { "values", valuesArray }
            };

            valuesJson = valuesJObject.ToString(Formatting.None);

            return ErrorCode.Success;
        }

        /// <summary>
        /// Allows an implementer to set a value in the wallet.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>
        /// An <see cref="T:Hyperledger.Indy.ErrorCode" /> value indicating the outcome of the operation.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ErrorCode Set(string key, string value)
        {
            var record = new WalletRecord
            {
                Key = key,
                Value = value,
                TimeCreated = DateTime.UtcNow,
                WalletName = _walletData.Name,
                UserId = _walletData.UserId
            };

            InsertOrUpdateRecord(record);

            return ErrorCode.Success;
        }

        internal void Close()
        {
            var walletData = _dbContext.WalletDatas.FirstOrDefault(p => p.Name == _walletData.Name);
            if (walletData != null)
            {
                walletData.IsOpen = false;
                _dbContext.Update(walletData);
                _dbContext.SaveChanges();
            }
        }

        private void InsertOrUpdateRecord(WalletRecord record)
        {
            var existedRecord = _dbContext.WalletRecords
                                          .FirstOrDefault(
                                                p => p.Key == record.Key &&
                                                     p.WalletName == record.WalletName);
            if (existedRecord != null)
            {
                _dbContext.Remove(existedRecord);
            }

            _dbContext.Add(record);

            _dbContext.SaveChanges();
        }
    }
}
