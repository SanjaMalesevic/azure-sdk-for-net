﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text.Json;

namespace Azure.Security.KeyVault.Certificates
{
    /// <summary>
    /// An Azure Key Vault certificate.
    /// </summary>
    public class KeyVaultCertificate : IJsonDeserializable
    {
        private const string KeyIdPropertyName = "kid";
        private const string SecretIdPropertyName = "sid";
        private const string ContentTypePropertyName = "contentType";
        private const string CERPropertyName = "cer";

        private string _keyId;
        private string _secretId;

        internal KeyVaultCertificate(CertificateProperties properties = null)
        {
            Properties = properties ?? new CertificateProperties();
        }

        /// <summary>
        /// The Id of the certificate.
        /// </summary>
        public Uri Id => Properties.Id;

        /// <summary>
        /// The name of the certificate.
        /// </summary>
        public string Name => Properties.Name;

        /// <summary>
        /// The Id of the Key Vault Key backing the certifcate.
        /// </summary>
        public Uri KeyId
        {
            get => new Uri(_keyId);
            internal set => _keyId = value?.ToString();
        }

        /// <summary>
        /// The Id of the Key Vault Secret which contains the PEM of PFX formatted content of the certficate and it's private key.
        /// </summary>
        public Uri SecretId
        {
            get => new Uri(_secretId);
            internal set => _secretId = value?.ToString();
        }

        /// <summary>
        /// The content type of the key vault Secret corresponding to the certificate.
        /// </summary>
        public CertificateContentType ContentType { get; internal set; }

        /// <summary>
        /// Additional properties of the <see cref="KeyVaultCertificate"/>.
        /// </summary>
        public CertificateProperties Properties { get; }

        /// <summary>
        /// The CER formatted public X509 certificate
        /// </summary>
        public byte[] Cer { get; internal set; }

        internal virtual void ReadProperty(JsonProperty prop)
        {
            switch (prop.Name)
            {
                case KeyIdPropertyName:
                    _keyId = prop.Value.GetString();
                    break;

                case SecretIdPropertyName:
                    _secretId = prop.Value.GetString();
                    break;

                case ContentTypePropertyName:
                    ContentType = prop.Value.GetString();
                    break;

                case CERPropertyName:
                    Cer = Base64Url.Decode(prop.Value.GetString());
                    break;

                default:
                    Properties.ReadProperty(prop);
                    break;
            }
        }

        void IJsonDeserializable.ReadProperties(JsonElement json)
        {
            foreach (JsonProperty prop in json.EnumerateObject())
            {
                ReadProperty(prop);
            }
        }
    }
}
