// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System;
using System.IO;
using System.Security.Authentication;
using System.Text.Json;

namespace HuoshanAI.Tests
{
    internal class TestFixture_00_01_Authentication
    {
        [SetUp]
        public void Setup()
        {
            var authJson = new AuthInfo("sk-test12", "org-testOrg", "proj_testProject");
            var authText = JsonSerializer.Serialize(authJson);
            File.WriteAllText(HuoshanAuthentication.CONFIG_FILE, authText);
            Assert.IsTrue(File.Exists(HuoshanAuthentication.CONFIG_FILE));
        }

        [Test]
        public void Test_01_GetAuthFromEnv()
        {
            var auth = HuoshanAuthentication.LoadFromEnvironment();
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);

            auth = HuoshanAuthentication.LoadFromEnvironment("org-testOrg");
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.IsNotEmpty(auth.ApiKey);
            Assert.IsNotNull(auth.OrganizationId);
            Assert.IsNotEmpty(auth.OrganizationId);
        }

        [Test]
        public void Test_02_GetAuthFromFile()
        {
            var auth = HuoshanAuthentication.LoadFromPath(Path.GetFullPath(HuoshanAuthentication.CONFIG_FILE));
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-test12", auth.ApiKey);
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testOrg", auth.OrganizationId);
            Assert.IsNotNull(auth.ProjectId);
            Assert.AreEqual("proj_testProject", auth.ProjectId);
        }

        [Test]
        public void Test_03_GetAuthFromNonExistentFile()
        {
            var auth = HuoshanAuthentication.LoadFromDirectory(filename: "bad.config");
            Assert.IsNull(auth);
        }

        [Test]
        public void Test_04_GetDefault()
        {
            var auth = HuoshanAuthentication.Default;
            Assert.IsNotNull(auth);
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-test12", auth.ApiKey);
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testOrg", auth.OrganizationId);
        }

        [Test]
        public void Test_05_Authentication()
        {
            var defaultAuth = HuoshanAuthentication.Default;
            var manualAuth = new HuoshanAuthentication("sk-testAA", "org-testAA", "proj_testProject");
            using var api = new HuoshanClient();
            var shouldBeDefaultAuth = api.HuoshanAIAuthentication;
            Assert.IsNotNull(shouldBeDefaultAuth);
            Assert.IsNotNull(shouldBeDefaultAuth.ApiKey);
            Assert.IsNotNull(shouldBeDefaultAuth.OrganizationId);
            Assert.AreEqual(defaultAuth.ApiKey, shouldBeDefaultAuth.ApiKey);
            Assert.AreEqual(defaultAuth.OrganizationId, shouldBeDefaultAuth.OrganizationId);
            Assert.AreEqual(defaultAuth.ProjectId, shouldBeDefaultAuth.ProjectId);

            HuoshanAuthentication.Default = new HuoshanAuthentication("sk-testAA", "org-testAA", "proj_testProject");
            using var api2 = new HuoshanClient();
            var shouldBeManualAuth = api2.HuoshanAIAuthentication;
            Assert.IsNotNull(shouldBeManualAuth);
            Assert.IsNotNull(shouldBeManualAuth.ApiKey);
            Assert.IsNotNull(shouldBeManualAuth.OrganizationId);
            Assert.AreEqual(manualAuth.ApiKey, shouldBeManualAuth.ApiKey);
            Assert.AreEqual(manualAuth.OrganizationId, shouldBeManualAuth.OrganizationId);
            Assert.AreEqual(manualAuth.ProjectId, shouldBeDefaultAuth.ProjectId);

            HuoshanAuthentication.Default = defaultAuth;
        }

        [Test]
        public void Test_06_GetKey()
        {
            var auth = new HuoshanAuthentication("sk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testAA", auth.ApiKey);
        }

        [Test]
        public void Test_07_GetKeyFailed()
        {
            HuoshanAuthentication auth = null;

            try
            {
                auth = new HuoshanAuthentication("fail-key");
            }
            catch (InvalidCredentialException)
            {
                Assert.IsNull(auth);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, $"Expected exception {nameof(InvalidCredentialException)} but got {e.GetType().Name}");
            }
        }

        [Test]
        public void Test_08_ParseKey()
        {
            var auth = new HuoshanAuthentication("sk-testAA");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testAA", auth.ApiKey);
            auth = "sk-testCC";
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testCC", auth.ApiKey);

            auth = new HuoshanAuthentication("sk-testBB");
            Assert.IsNotNull(auth.ApiKey);
            Assert.AreEqual("sk-testBB", auth.ApiKey);
        }

        [Test]
        public void Test_09_GetOrganization()
        {
            var auth = new HuoshanAuthentication("sk-testAA", "org-testAA");
            Assert.IsNotNull(auth.OrganizationId);
            Assert.AreEqual("org-testAA", auth.OrganizationId);
        }

        [Test]
        public void Test_10_GetOrgFailed()
        {
            HuoshanAuthentication auth = null;

            try
            {
                auth = new HuoshanAuthentication("sk-testAA", "fail-org");
            }
            catch (InvalidCredentialException)
            {
                Assert.IsNull(auth);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, $"Expected exception {nameof(InvalidCredentialException)} but got {e.GetType().Name}");
            }
        }

        //[Test]
        //public void Test_11_AzureConfigurationSettings()
        //{
        //    var auth = new HuoshanAuthentication("testKeyAaBbCcDd");
        //    const string domain = "custom.azure.HuoshanAI.com";
        //    const string resource = "test-resource";
        //    const string deploymentId = "deployment-id-test";
        //    var settings = new HuoshanSettings(resourceName: resource, deploymentId: deploymentId, azureDomain: domain);
        //    var api = new HuoshanClient(auth, settings);
        //    Console.WriteLine(api.Settings.BaseRequest);
        //    Console.WriteLine(api.Settings.BaseRequestUrlFormat);
        //    Assert.AreEqual(deploymentId, api.Settings.DeploymentId);
        //    Assert.AreEqual($"https://{resource}.{domain}/HuoshanAI/{{0}}", api.Settings.BaseRequestUrlFormat);
        //}

        [Test]
        public void Test_12_CustomDomainConfigurationSettings()
        {
            var auth = new HuoshanAuthentication("sess-customIssuedToken");
            const string domain = "api.your-custom-domain.com";
            var settings = new HuoshanSettings(domain: domain);
            var api = new HuoshanClient(auth, settings);
            Console.WriteLine(api.Settings.BaseRequest);
            Console.WriteLine(api.Settings.BaseRequestUrlFormat);
            Console.WriteLine(api.Settings.BaseWebSocketUrlFormat);
            Assert.AreEqual($"https://{domain}/v1/{{0}}", api.Settings.BaseRequestUrlFormat);
            Assert.AreEqual($"wss://{HuoshanSettings.HuoshanAIDomain}/v1/{{0}}", api.Settings.BaseWebSocketUrlFormat);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(HuoshanAuthentication.CONFIG_FILE))
            {
                File.Delete(HuoshanAuthentication.CONFIG_FILE);
            }

            Assert.IsFalse(File.Exists(HuoshanAuthentication.CONFIG_FILE));
        }
    }
}
