// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using HuoshanAI.Moderations;
using System;
using System.Threading.Tasks;

namespace HuoshanAI.Tests
{
    internal class TestFixture_10_Moderations : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Moderate()
        {
            Assert.IsNotNull(HuoshanAIClient.ModerationsEndpoint);
            var isViolation = await HuoshanAIClient.ModerationsEndpoint.GetModerationAsync("I want to kill them.");
            Assert.IsTrue(isViolation);
        }

        [Test]
        public async Task Test_02_Moderate_Scores()
        {
            Assert.IsNotNull(HuoshanAIClient.ModerationsEndpoint);
            var response = await HuoshanAIClient.ModerationsEndpoint.CreateModerationAsync(new ModerationsRequest("I love you"));
            Assert.IsNotNull(response);
            Console.WriteLine(response.Results?[0]?.Scores?.ToString());
        }

        [Test]
        public async Task Test_03_Moderation_Chunked()
        {
            Assert.IsNotNull(HuoshanAIClient.ModerationsEndpoint);
            var isViolation = await HuoshanAIClient.ModerationsEndpoint.GetModerationChunkedAsync("I don't want to kill them. I want to kill them. I want to kill them.", chunkSize: "I don't want to kill them.".Length, chunkOverlap: 4);
            Assert.IsTrue(isViolation);
        }
    }
}
