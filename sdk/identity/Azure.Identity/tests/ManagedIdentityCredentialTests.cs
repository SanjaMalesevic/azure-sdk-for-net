﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Testing;
using Azure.Identity.Tests.Mock;
using NUnit.Framework;

namespace Azure.Identity.Tests
{
    public class ManagedIdentityCredentialTests : ClientTestBase
    {
        [SetUp]
        public void ResetManagedIdenityClient()
        {
            typeof(ManagedIdentityClient).GetField("s_msiType", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0);
            typeof(ManagedIdentityClient).GetField("s_endpoint", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
        }

        public ManagedIdentityCredentialTests(bool isAsync) : base(isAsync)
        {
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyImdsRequestMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", null))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": \"3600\" }}");

                var mockTransport = new MockTransport(response, response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential(options: options));

                AccessToken actualToken = await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[mockTransport.Requests.Count - 1];

                string query = request.Uri.Query;

                Assert.IsTrue(query.Contains("api-version=2018-02-01"));

                Assert.IsTrue(query.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(request.Headers.TryGetValue("Metadata", out string metadataValue));

                Assert.AreEqual("true", metadataValue);
            }
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyImdsRequestWithClientIdMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", null))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": \"3600\" }}");

                var mockTransport = new MockTransport(response, response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential("mock-client-id", options));

                AccessToken actualToken = await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[mockTransport.Requests.Count - 1];

                string query = request.Uri.Query;

                Assert.IsTrue(query.Contains("api-version=2018-02-01"));

                Assert.IsTrue(query.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(query.Contains($"client_id=mock-client-id"));

                Assert.IsTrue(request.Headers.TryGetValue("Metadata", out string metadataValue));

                Assert.AreEqual("true", metadataValue);
            }
        }

        [NonParallelizable]
        [Test]
        public void VerifyImdsUnavailableImmediateFailureMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", null))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var mockTransport = new MockTransport(request => throw new Exception("mock imds probe exception"));

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential("mock-client-id", options));

                Assert.ThrowsAsync<CredentialUnavailableException>(async () => await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default)));

                MockRequest request = mockTransport.Requests[0];

                string query = request.Uri.Query;

                Assert.IsTrue(query.Contains("api-version=2018-02-01"));

                Assert.False(request.Headers.TryGetValue("Metadata", out string _));
            }
        }

        [NonParallelizable]
        [Test]
        public void VerifyImdsAvailableUserCanceledMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", null))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var mockTransport = new MockTransport(request => throw new OperationCanceledException("mock user canceled exception"));

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential("mock-client-id", options));

                CancellationTokenSource cancellationSource = new CancellationTokenSource();

                cancellationSource.Cancel();

                Assert.CatchAsync<OperationCanceledException>(async () => await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default), cancellationSource.Token));
            }
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyAppServiceMsiRequestMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", "https://mock.msi.endpoint/"))
            using (new TestEnvVar("MSI_SECRET", "mock-msi-secret"))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": \"{DateTimeOffset.UtcNow.ToString()}\" }}");

                var mockTransport = new MockTransport(response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential(options: options));

                AccessToken actualToken = await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[0];

                Assert.IsTrue(request.Uri.ToString().StartsWith("https://mock.msi.endpoint/"));

                string query = request.Uri.Query;

                Assert.IsTrue(query.Contains("api-version=2017-09-01"));

                Assert.IsTrue(query.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(request.Headers.TryGetValue("secret", out string actSecretValue));

                Assert.AreEqual("mock-msi-secret", actSecretValue);
            }
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyAppServiceMsiRequestWithClientIdMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", "https://mock.msi.endpoint/"))
            using (new TestEnvVar("MSI_SECRET", "mock-msi-secret"))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": \"{DateTimeOffset.UtcNow.ToString()}\" }}");

                var mockTransport = new MockTransport(response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential("mock-client-id", options));

                AccessToken actualToken = await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[0];

                Assert.IsTrue(request.Uri.ToString().StartsWith("https://mock.msi.endpoint/"));

                string query = request.Uri.Query;

                Assert.IsTrue(query.Contains("api-version=2017-09-01"));

                Assert.IsTrue(query.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(query.Contains($"client_id=mock-client-id"));

                Assert.IsTrue(request.Headers.TryGetValue("secret", out string actSecretValue));

                Assert.AreEqual("mock-msi-secret", actSecretValue);
            }
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyCloudShellMsiRequestMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", "https://mock.msi.endpoint/"))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": {(DateTimeOffset.UtcNow + TimeSpan.FromSeconds(3600)).ToUnixTimeSeconds()} }}");

                var mockTransport = new MockTransport(response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential credential = InstrumentClient(new ManagedIdentityCredential(options: options));

                AccessToken actualToken = await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[0];

                Assert.IsTrue(request.Uri.ToString().StartsWith("https://mock.msi.endpoint/"));

                Assert.IsTrue(request.Content.TryComputeLength(out long contentLen));

                var content = new byte[contentLen];

                MemoryStream contentBuff = new MemoryStream(content);

                request.Content.WriteTo(contentBuff, default);

                string body = Encoding.UTF8.GetString(content);

                Assert.IsTrue(body.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(request.Headers.TryGetValue("Metadata", out string actMetadata));

                Assert.AreEqual("true", actMetadata);
            }
        }

        [NonParallelizable]
        [Test]
        public async Task VerifyCloudShellMsiRequestWithClientIdMockAsync()
        {
            using (new TestEnvVar("MSI_ENDPOINT", "https://mock.msi.endpoint/"))
            using (new TestEnvVar("MSI_SECRET", null))
            {
                var response = new MockResponse(200);

                var expectedToken = "mock-msi-access-token";

                response.SetContent($"{{ \"access_token\": \"{expectedToken}\", \"expires_on\": {(DateTimeOffset.UtcNow + TimeSpan.FromSeconds(3600)).ToUnixTimeSeconds()} }}");

                var mockTransport = new MockTransport(response);

                var options = new TokenCredentialOptions() { Transport = mockTransport };

                ManagedIdentityCredential client = InstrumentClient(new ManagedIdentityCredential("mock-client-id", options));

                AccessToken actualToken = await client.GetTokenAsync(new TokenRequestContext(MockScopes.Default));

                Assert.AreEqual(expectedToken, actualToken.Token);

                MockRequest request = mockTransport.Requests[0];

                Assert.IsTrue(request.Uri.ToString().StartsWith("https://mock.msi.endpoint/"));

                Assert.IsTrue(request.Content.TryComputeLength(out long contentLen));

                var content = new byte[contentLen];

                MemoryStream contentBuff = new MemoryStream(content);

                request.Content.WriteTo(contentBuff, default);

                string body = Encoding.UTF8.GetString(content);

                Assert.IsTrue(body.Contains($"resource={Uri.EscapeDataString(ScopeUtilities.ScopesToResource(MockScopes.Default))}"));

                Assert.IsTrue(body.Contains($"client_id=mock-client-id"));

                Assert.IsTrue(request.Headers.TryGetValue("Metadata", out string actMetadata));

                Assert.AreEqual("true", actMetadata);
            }
        }

        [Test]
        public async Task VerifyMsiUnavailableCredentialException()
        {
            var mockClient = new MockManagedIdentityClient { MsiTypeFactory = () => MsiType.Unavailable };

            var credential = InstrumentClient(new ManagedIdentityCredential(null, CredentialPipeline.GetInstance(null), mockClient));

            var ex = Assert.ThrowsAsync<CredentialUnavailableException>(async () => await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default)));

            Assert.AreEqual(ManagedIdentityCredential.MsiUnavailableError, ex.Message);

            await Task.CompletedTask;
        }

        [Test]
        public async Task VerifyClientGetMsiTypeThrows()
        {
            var mockClient = new MockManagedIdentityClient { MsiTypeFactory = () => throw new MockClientException("message") };

            var credential = InstrumentClient(new ManagedIdentityCredential(null, CredentialPipeline.GetInstance(null), mockClient));

            var ex = Assert.ThrowsAsync<AuthenticationFailedException>(async () => await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default)));

            Assert.IsInstanceOf(typeof(MockClientException), ex.InnerException);

            await Task.CompletedTask;
        }

        [Test]
        public async Task VerifyClientAuthenticateThrows()
        {
            var mockClient = new MockManagedIdentityClient { MsiTypeFactory = () => MsiType.Imds, TokenFactory = () => throw new MockClientException("message") };

            var credential = InstrumentClient(new ManagedIdentityCredential(null, CredentialPipeline.GetInstance(null), mockClient));

            var ex = Assert.ThrowsAsync<AuthenticationFailedException>(async () => await credential.GetTokenAsync(new TokenRequestContext(MockScopes.Default)));

            Assert.IsInstanceOf(typeof(MockClientException), ex.InnerException);

            await Task.CompletedTask;
        }
    }
}
