// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using HuoshanAI.Models;
using System.Threading.Tasks;

namespace HuoshanAI.Tests
{
    internal class TestFixture_06_Embeddings : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_CreateEmbedding()
        {
            Assert.IsNotNull(HuoshanAIClient.EmbeddingsEndpoint);
            var embedding = await HuoshanAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
            Assert.IsNotNull(embedding);
            Assert.IsNotEmpty(embedding.Data);
        }

        [Test]
        public async Task Test_02_CreateEmbeddingWithDimensions()
        {
            Assert.IsNotNull(HuoshanAIClient.EmbeddingsEndpoint);
            var embedding = await HuoshanAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...",
                Model.Embedding_3_Small, dimensions: 512);
            Assert.IsNotNull(embedding);
            Assert.IsNotEmpty(embedding.Data);
            Assert.AreEqual(512, embedding.Data[0].Embedding.Count);
        }

        [Test]
        public async Task Test_03_CreateEmbeddingsWithMultipleInputs()
        {
            Assert.IsNotNull(HuoshanAIClient.EmbeddingsEndpoint);
            var embeddings = new[]
            {
                "The food was delicious and the waiter...",
                "The food was terrible and the waiter..."
            };
            var embedding = await HuoshanAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddings);
            Assert.IsNotNull(embedding);
            Assert.AreEqual(embedding.Data.Count, 2);
        }
    }
}
