// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text.Json;

namespace HuoshanAI
{
    /// <summary>
    /// Represents authentication to the HuoshanAI API endpoint
    /// </summary>
    public sealed class HuoshanAuthentication
    {
        public const string CONFIG_FILE = ".openai";
        public const string OPENAI_KEY = nameof(OPENAI_KEY);
        public const string OPENAI_API_KEY = nameof(OPENAI_API_KEY);
        public const string OPENAI_SECRET_KEY = nameof(OPENAI_SECRET_KEY);
        public const string OPENAI_PROJECT_ID = nameof(OPENAI_PROJECT_ID);
        public const string OPEN_AI_PROJECT_ID = nameof(OPEN_AI_PROJECT_ID);
        public const string TEST_OPENAI_SECRET_KEY = nameof(TEST_OPENAI_SECRET_KEY);
        public const string OPENAI_ORGANIZATION_ID = nameof(OPENAI_ORGANIZATION_ID);
        public const string OPEN_AI_ORGANIZATION_ID = nameof(OPEN_AI_ORGANIZATION_ID);

        public readonly AuthInfo authInfo;

        /// <summary>
        /// The API key, required to access the API endpoint.
        /// </summary>
        public string ApiKey => authInfo.ApiKey;

        /// <summary>
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </summary>
        public string OrganizationId => authInfo.OrganizationId;

        /// <summary>
        /// For users that specify specific projects.
        /// </summary>
        public string ProjectId => authInfo.ProjectId;

        /// <summary>
        /// Allows implicit casting from a string, so that a simple string API key can be provided in place of an instance of <see cref="HuoshanAuthentication"/>.
        /// </summary>
        /// <param name="key">The API key to convert into a <see cref="HuoshanAuthentication"/>.</param>
        public static implicit operator HuoshanAuthentication(string key) => new(key);

        private HuoshanAuthentication(AuthInfo authInfo) => this.authInfo = authInfo;

        /// <summary>
        /// Instantiates a new Authentication object with the given <paramref name="apiKey"/>, which may be <see langword="null"/>.
        /// </summary>
        /// <param name="apiKey">The API key, required to access the API endpoint.</param>
        public HuoshanAuthentication(string apiKey) => authInfo = new AuthInfo(apiKey);

        /// <summary>
        /// Instantiates a new Authentication object with the given <paramref name="apiKey"/>, which may be <see langword="null"/>.
        /// </summary>
        /// <param name="apiKey">The API key, required to access the API endpoint.</param>
        /// <param name="organization">
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </param>
        /// <param name="projectId">
        /// Optional, Project id to specify.
        /// </param>
        public HuoshanAuthentication(string apiKey, string organization, string projectId = null) => authInfo = new AuthInfo(apiKey, organization, projectId);

        private static HuoshanAuthentication cachedDefault;

        /// <summary>
        /// The default authentication to use when no other auth is specified.
        /// This can be set manually, or automatically loaded via environment variables or a config file.
        /// <seealso cref="LoadFromEnvironment"/><seealso cref="LoadFromDirectory"/>
        /// </summary>
        public static HuoshanAuthentication Default
        {
            get
            {
                if (cachedDefault != null)
                {
                    return cachedDefault;
                }

                var auth = LoadFromDirectory() ??
                           LoadFromDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)) ??
                           LoadFromEnvironment();
                cachedDefault = auth ?? throw new UnauthorizedAccessException("Failed to load a valid API Key!");
                return auth;
            }
            set => cachedDefault = value;
        }

       
        /// <summary>
        /// Attempts to load api keys from environment variables, as "HuoshanAI_KEY" (or "HuoshanAI_SECRET_KEY", for backwards compatibility)
        /// </summary>
        /// <param name="organizationId">
        /// For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request.
        /// Usage from these API requests will count against the specified organization's subscription quota.
        /// </param>
        /// <returns>
        /// Returns the loaded <see cref="HuoshanAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if there were no matching environment vars.
        /// </returns>
        public static HuoshanAuthentication LoadFromEnvironment(string organizationId = null)
        {
            var apiKey = Environment.GetEnvironmentVariable(OPENAI_KEY);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(OPENAI_API_KEY);
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(OPENAI_SECRET_KEY);
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable(TEST_OPENAI_SECRET_KEY);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                organizationId = Environment.GetEnvironmentVariable(OPEN_AI_ORGANIZATION_ID);
            }

            if (string.IsNullOrWhiteSpace(organizationId))
            {
                organizationId = Environment.GetEnvironmentVariable(OPENAI_ORGANIZATION_ID);
            }

            var projectId = Environment.GetEnvironmentVariable(OPEN_AI_PROJECT_ID);

            if (string.IsNullOrWhiteSpace(projectId))
            {
                projectId = Environment.GetEnvironmentVariable(OPENAI_PROJECT_ID);
            }

            return string.IsNullOrEmpty(apiKey) ? null : new HuoshanAuthentication(apiKey, organizationId, projectId);
        }

        /// <summary>
        /// Attempts to load api keys from a specified configuration file.
        /// </summary>
        /// <param name="path">The specified path to the configuration file.</param>
        /// <returns>
        /// Returns the loaded <see cref="HuoshanAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static HuoshanAuthentication LoadFromPath(string path)
            => LoadFromDirectory(Path.GetDirectoryName(path), Path.GetFileName(path), false);

        /// <summary>
        /// Attempts to load api keys from a configuration file, by default ".HuoshanAI" in the current directory,
        /// optionally traversing up the directory tree.
        /// </summary>
        /// <param name="directory">
        /// The directory to look in, or <see langword="null"/> for the current directory.
        /// </param>
        /// <param name="filename">
        /// The filename of the config file.
        /// </param>
        /// <param name="searchUp">
        /// Whether to recursively traverse up the directory tree if the <paramref name="filename"/> is not found in the <paramref name="directory"/>.
        /// </param>
        /// <returns>
        /// Returns the loaded <see cref="HuoshanAuthentication"/> any api keys were found,
        /// or <see langword="null"/> if it was not successful in finding a config
        /// (or if the config file didn't contain correctly formatted API keys)
        /// </returns>
        public static HuoshanAuthentication LoadFromDirectory(string directory = null, string filename = CONFIG_FILE, bool searchUp = true)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Environment.CurrentDirectory;
            }

            AuthInfo authInfo = null;

            var currentDirectory = new DirectoryInfo(directory);

            while (authInfo == null && currentDirectory.Parent != null)
            {
                var filePath = Path.Combine(currentDirectory.FullName, filename);

                if (File.Exists(filePath))
                {
                    try
                    {
                        authInfo = JsonSerializer.Deserialize<AuthInfo>(File.ReadAllText(filePath));
                        break;
                    }
                    catch (Exception)
                    {
                        // try to parse the old way for backwards support.
                    }

                    var lines = File.ReadAllLines(filePath);
                    string apiKey = null;
                    string projectId = null;
                    string organization = null;

                    foreach (var line in lines)
                    {
                        var parts = line.Split('=', ':');

                        for (var i = 0; i < parts.Length - 1; i++)
                        {
                            var part = parts[i];
                            var nextPart = parts[i + 1];

                            switch (part)
                            {
                                case OPENAI_KEY:
                                case OPENAI_API_KEY:
                                case OPENAI_SECRET_KEY:
                                case TEST_OPENAI_SECRET_KEY:
                                    apiKey = nextPart.Trim();
                                    break;
                                case OPEN_AI_ORGANIZATION_ID:
                                case OPENAI_ORGANIZATION_ID:
                                    organization = nextPart.Trim();
                                    break;
                                case OPENAI_PROJECT_ID:
                                case OPEN_AI_PROJECT_ID:
                                    projectId = nextPart.Trim();
                                    break;
                            }
                        }
                    }

                    authInfo = new AuthInfo(apiKey, organization, projectId);
                }

                if (searchUp)
                {
                    currentDirectory = currentDirectory.Parent;
                }
                else
                {
                    break;
                }
            }

            if (authInfo == null ||
                string.IsNullOrEmpty(authInfo.ApiKey))
            {
                return null;
            }

            return new HuoshanAuthentication(authInfo);
        }
    }
}
