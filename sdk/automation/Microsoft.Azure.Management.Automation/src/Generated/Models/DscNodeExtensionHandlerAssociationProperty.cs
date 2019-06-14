// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Management.Automation.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// The dsc extensionHandler property associated with the node
    /// </summary>
    public partial class DscNodeExtensionHandlerAssociationProperty
    {
        /// <summary>
        /// Initializes a new instance of the
        /// DscNodeExtensionHandlerAssociationProperty class.
        /// </summary>
        public DscNodeExtensionHandlerAssociationProperty()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// DscNodeExtensionHandlerAssociationProperty class.
        /// </summary>
        /// <param name="name">Gets or sets the name of the extension
        /// handler.</param>
        /// <param name="version">Gets or sets the version of the extension
        /// handler.</param>
        public DscNodeExtensionHandlerAssociationProperty(string name = default(string), string version = default(string))
        {
            Name = name;
            Version = version;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the name of the extension handler.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version of the extension handler.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

    }
}