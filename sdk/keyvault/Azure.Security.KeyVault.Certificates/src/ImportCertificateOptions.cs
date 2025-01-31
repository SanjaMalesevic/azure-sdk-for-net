﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Azure.Core;

namespace Azure.Security.KeyVault.Certificates
{
    /// <summary>
    /// Options for a certificate to be imported into Azure Key Vault.
    /// </summary>
    public class ImportCertificateOptions : IJsonSerializable
    {
        private static readonly JsonEncodedText s_valuePropertyNameBytes = JsonEncodedText.Encode("value");
        private static readonly JsonEncodedText s_policyPropertyNameBytes = JsonEncodedText.Encode("policy");
        private static readonly JsonEncodedText s_passwordPropertyNameBytes = JsonEncodedText.Encode("pwd");
        private static readonly JsonEncodedText s_attributesPropertyNameBytes = JsonEncodedText.Encode("attributes");
        private static readonly JsonEncodedText s_enabledPropertyNameBytes = JsonEncodedText.Encode("enabled");
        private static readonly JsonEncodedText s_tagsPropertyNameBytes = JsonEncodedText.Encode("tags");

        private Dictionary<string, string> _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCertificateOptions"/> class.
        /// </summary>
        /// <param name="name">A name for the imported certificate.</param>
        /// <param name="value">The PFX or PEM formatted value of the certificate containing both the x509 certificates and the private key.</param>
        /// <param name="policy">The policy which governs the lifecycle of the imported certificate and it's properties when it is rotated.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is empty.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="policy"/>, or <paramref name="value"/> is null.</exception>
        public ImportCertificateOptions(string name, byte[] value, CertificatePolicy policy)
        {
            Argument.AssertNotNullOrEmpty(name, nameof(name));
            Argument.AssertNotNull(value, nameof(value));
            Argument.AssertNotNull(policy, nameof(policy));

            Name = name;
            Value = value;
            Policy = policy;
        }

        /// <summary>
        /// The name of the certificate to import.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The PFX or PEM formatted value of the certificate containing both the x509 certificates and the private key.
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// The policy which governs the lifecycle of the imported certificate and it's properties when it is rotated.
        /// </summary>
        public CertificatePolicy Policy { get; }

        /// <summary>
        /// The password protecting the certificate specified in the Value.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the merged certificate should be enabled. If null, the server default will be used.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Tags to be applied to the imported certificate.
        /// </summary>
        public IDictionary<string, string> Tags => LazyInitializer.EnsureInitialized(ref _tags);

        void IJsonSerializable.WriteProperties(Utf8JsonWriter json)
        {
            if (Value != null)
            {
                string encoded = Base64Url.Encode(Value);
                json.WriteString(s_valuePropertyNameBytes, encoded);
            }

            if (!string.IsNullOrEmpty(Password))
            {
                json.WriteString(s_passwordPropertyNameBytes, Password);
            }

            if (Policy != null)
            {
                json.WriteStartObject(s_policyPropertyNameBytes);

                ((IJsonSerializable)Policy).WriteProperties(json);

                json.WriteEndObject();
            }

            if (Enabled.HasValue)
            {
                json.WriteStartObject(s_attributesPropertyNameBytes);

                json.WriteBoolean(s_enabledPropertyNameBytes, Enabled.Value);

                json.WriteEndObject();
            }

            if (!_tags.IsNullOrEmpty())
            {
                json.WriteStartObject(s_tagsPropertyNameBytes);

                foreach (KeyValuePair<string, string> kvp in _tags)
                {
                    json.WriteString(kvp.Key, kvp.Value);
                }

                json.WriteEndObject();
            }
        }
    }
}
