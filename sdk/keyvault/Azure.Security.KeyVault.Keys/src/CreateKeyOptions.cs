﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Azure.Security.KeyVault.Keys
{
    /// <summary>
    /// The key-specific properties needed to create a key using the <see cref="KeyClient"/>.
    /// </summary>
    public class CreateKeyOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateKeyOptions"/> class.
        /// </summary>
        public CreateKeyOptions()
        {
        }

        /// <summary>
        /// Gets a list of <see cref="KeyOperation"/> values the key should support.
        /// </summary>
        public IList<KeyOperation> KeyOperations { get; } = new List<KeyOperation>();

        /// <summary>
        /// Gets or sets a <see cref="DateTimeOffset"/> of when the key will be valid.
        /// </summary>
        public DateTimeOffset? NotBefore { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DateTimeOffset"/> of when the key will expire.
        /// </summary>
        public DateTimeOffset? ExpiresOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key will be enabled. If null, the service default is used.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets a dictionary of tags with specific metadata about the key.
        /// </summary>
        public IDictionary<string, string> Tags { get; } = new Dictionary<string, string>();
    }
}
