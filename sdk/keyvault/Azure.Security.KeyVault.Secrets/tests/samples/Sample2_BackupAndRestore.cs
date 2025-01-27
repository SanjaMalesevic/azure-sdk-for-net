﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Core.Testing;
using Azure.Identity;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Azure.Security.KeyVault.Secrets.Samples
{
    /// <summary>
    /// This sample demonstrates how to back up and restore a secret from Azure Key Vault using synchronous methods of <see cref="SecretClient"/>.
    /// </summary>
    [LiveOnly]
    public partial class BackupAndRestore
    {
        [Test]
        [Ignore("https://github.com/Azure/azure-sdk-for-net/issues/6514")]
        public void BackupAndRestoreSync()
        {
            // Environment variable with the Key Vault endpoint.
            string keyVaultUrl = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URL");
            BackupAndRestoreSync(keyVaultUrl);
        }

        private void BackupAndRestoreSync(string keyVaultUrl)
        {
            #region Snippet:SecretsSample2SecretClient
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            #endregion

            #region Snippet:SecretsSample2CreateSecret
            string secretName = $"StorageAccountPassword{Guid.NewGuid()}";

            var secret = new KeyVaultSecret(secretName, "f4G34fMh8v");
            secret.Properties.ExpiresOn = DateTimeOffset.Now.AddYears(1);

            KeyVaultSecret storedSecret = client.SetSecret(secret);
            #endregion

            #region Snippet:SecretsSample2BackupSecret
            string backupPath = Path.GetTempFileName();
            byte[] secretBackup = client.BackupSecret(secretName);

            File.WriteAllBytes(backupPath, secretBackup);
            #endregion

            // The storage account secret is no longer in use so you delete it.
            DeleteSecretOperation operation = client.StartDeleteSecret(secretName);

            // Before it can be purged, you need to wait until the secret is fully deleted.
            while (!operation.HasCompleted)
            {
                Thread.Sleep(2000);

                operation.UpdateStatus();
            }

            // If the Key Vault is soft delete-enabled and you want to permanently delete the secret before its `ScheduledPurgeDate`,
            // the deleted secret needs to be purged.
            client.PurgeDeletedSecret(secretName);

            #region Snippet:SecretsSample2RestoreSecret
            byte[] secretBackupToRestore = File.ReadAllBytes(backupPath);

            SecretProperties restoreSecret = client.RestoreSecretBackup(secretBackupToRestore);
            #endregion

            AssertSecretsEqual(storedSecret.Properties, restoreSecret);
        }

        private static void AssertSecretsEqual(SecretProperties exp, SecretProperties act)
        {
            Assert.AreEqual(exp.Name, act.Name);
            Assert.AreEqual(exp.Version, act.Version);
            Assert.AreEqual(exp.Managed, act.Managed);
            Assert.AreEqual(exp.RecoveryLevel, act.RecoveryLevel);
            Assert.AreEqual(exp.ExpiresOn, act.ExpiresOn);
            Assert.AreEqual(exp.NotBefore, act.NotBefore);
        }
    }
}
