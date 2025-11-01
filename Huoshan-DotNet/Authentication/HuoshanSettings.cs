// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace HuoshanAI
{
    /// <summary>
    /// The client settings for configuring Azure HuoshanAI or custom domain.
    /// </summary>
    public sealed class HuoshanSettings
    {
        public const string WSS = "wss://";
        public const string Http = "http://";
        public const string Https = "https://";
        public const string HuoshanAIDomain = "https://ark.cn-beijing.volces.com/api";
        public const string DefaultHuoshanAIApiVersion = "v3";
        //internal const string AzureHuoshanAIDomain = "openai.azure.com";
        //internal const string DefaultAzureApiVersion = "2024-10-21";

        /// <summary>
        /// Creates a new instance of <see cref="HuoshanSettings"/> for use with HuoshanAI.
        /// </summary>
        public HuoshanSettings()
        {
            ResourceName = HuoshanAIDomain;
            ApiVersion = DefaultHuoshanAIApiVersion;
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"{Https}{ResourceName}{BaseRequest}{{0}}";
            BaseWebSocketUrlFormat = $"{WSS}{ResourceName}{BaseRequest}{{0}}";
            UseOAuthAuthentication = true;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HuoshanSettings"/> for use with HuoshanAI.
        /// </summary>
        /// <param name="domain">Base api domain.</param>
        /// <param name="apiVersion">The version of the HuoshanAI api you want to use.</param>
        public HuoshanSettings(string domain, string apiVersion = DefaultHuoshanAIApiVersion)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = HuoshanAIDomain;
            }

            if (!domain.Contains('.') &&
                !domain.Contains(':'))
            {
                throw new ArgumentException($"You're attempting to pass a \"resourceName\" parameter to \"{nameof(domain)}\". Please specify \"resourceName:\" for this parameter in constructor.");
            }

            if (string.IsNullOrWhiteSpace(apiVersion))
            {
                apiVersion = DefaultHuoshanAIApiVersion;
            }

            var protocol = Https;

            if (domain.StartsWith(Http))
            {
                protocol = Http;
                domain = domain.Replace(Http, string.Empty);
            }
            else if (domain.StartsWith(Https))
            {
                protocol = Https;
                domain = domain.Replace(Https, string.Empty);
            }

            ResourceName = $"{protocol}{domain}";
            ApiVersion = apiVersion;
            DeploymentId = string.Empty;
            BaseRequest = $"/{ApiVersion}/";
            BaseRequestUrlFormat = $"{ResourceName}{BaseRequest}{{0}}";
            BaseWebSocketUrlFormat = $"{WSS}{HuoshanAIDomain}{BaseRequest}{{0}}";
            UseOAuthAuthentication = true;
        }

        ///// <summary>
        ///// Creates a new instance of the <see cref="HuoshanSettings"/> for use with Azure HuoshanAI.<br/>
        ///// <see href="https://learn.microsoft.com/en-us/azure/cognitive-services/HuoshanAI/"/>
        ///// </summary>
        ///// <param name="resourceName">
        ///// The name of your Azure HuoshanAI Resource.
        ///// </param>
        ///// <param name="deploymentId">
        ///// The name of your model deployment. You're required to first deploy a model before you can make calls.
        ///// </param>
        ///// <param name="apiVersion">
        ///// Optional, defaults to 2024-10-21.
        ///// </param>
        ///// <param name="useActiveDirectoryAuthentication">
        ///// Optional, set to true if you want to use Azure Active Directory for Authentication.
        ///// </param>
        ///// Optional, override the azure domain, if you need to use a different one (e.g., for Azure Government or other regions).
        ///// <param name="azureDomain">
        ///// Optional, defaults to "HuoshanAI.azure.com".
        ///// </param>
        //public HuoshanSettings(
        //    string resourceName,
        //    string deploymentId,
        //    string apiVersion = DefaultAzureApiVersion,
        //    bool useActiveDirectoryAuthentication = false,
        //    string azureDomain = AzureHuoshanAIDomain)
        //{
        //    if (string.IsNullOrWhiteSpace(resourceName))
        //    {
        //        throw new ArgumentNullException(nameof(resourceName));
        //    }

        //    if (resourceName.Contains('.') ||
        //        resourceName.Contains(':'))
        //    {
        //        throw new ArgumentException($"You're attempting to pass a \"domain\" parameter to \"{nameof(resourceName)}\". Please specify \"domain:\" for this parameter in constructor.");
        //    }

        //    if (string.IsNullOrWhiteSpace(apiVersion))
        //    {
        //        apiVersion = DefaultAzureApiVersion;
        //    }

        //    if (string.IsNullOrWhiteSpace(azureDomain))
        //    {
        //        azureDomain = AzureHuoshanAIDomain;
        //    }

        //    IsAzureHuoshanAI = true;
        //    ResourceName = resourceName;
        //    DeploymentId = deploymentId;
        //    ApiVersion = apiVersion;
        //    BaseRequest = "/HuoshanAI/";
        //    BaseRequestUrlFormat = $"{Https}{ResourceName}.{azureDomain}{BaseRequest}{{0}}";
        //    BaseWebSocketUrlFormat = $"{WSS}{ResourceName}.{azureDomain}{BaseRequest}{{0}}";
        //    defaultQueryParameters.Add("api-version", ApiVersion);
        //    UseOAuthAuthentication = useActiveDirectoryAuthentication;
        //}

        public string ResourceName { get; }

        public string DeploymentId { get; }

        public string ApiVersion { get; }

        public string BaseRequest { get; }

        public string BaseRequestUrlFormat { get; }

        public string BaseWebSocketUrlFormat { get; }

        public bool UseOAuthAuthentication { get; }

        public bool IsAzureHuoshanAI { get; }

        private readonly Dictionary<string, string> defaultQueryParameters = new();

        internal IReadOnlyDictionary<string, string> DefaultQueryParameters => defaultQueryParameters;

        public static HuoshanSettings Default { get; } = new();
    }
}
