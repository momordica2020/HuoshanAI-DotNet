// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HuoshanAI.Tests
{
    internal class TestFixture_01_Models : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_GetModels()
        {
            Assert.IsNotNull(HuoshanAIClient.ModelsEndpoint);
            var results = await HuoshanAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public async Task Test_2_RetrieveModelDetails()
        {
            Assert.IsNotNull(HuoshanAIClient.ModelsEndpoint);
            var models = await HuoshanAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotEmpty(models);
            Console.WriteLine($"Found {models.Count} models!");

            foreach (var model in models.OrderBy(model => model.Id))
            {
                Console.WriteLine($"{model.Id} | Owner: {model.OwnedBy} | Created: {model.CreatedAt}");

                try
                {
                    var result = await HuoshanAIClient.ModelsEndpoint.GetModelDetailsAsync(model.Id);
                    Assert.IsNotNull(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"No Model details found for {model.Id}\n{e}");
                }
            }
        }
    }
}
