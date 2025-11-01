// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using HuoshanAI.Batch;
using HuoshanAI.Files;
using System.IO;
using System.Threading.Tasks;

namespace HuoshanAI.Tests
{
    internal class TestFixture_12_Batches : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Batches()
        {
            Assert.IsNotNull(HuoshanAIClient.BatchEndpoint);

            const string testFilePath = "batch.txt";
            await File.WriteAllTextAsync(testFilePath, "{\"custom_id\": \"request-1\", \"method\": \"POST\", \"url\": \"/v1/chat/completions\", \"body\": {\"model\": \"gpt-3.5-turbo\", \"messages\": [{\"role\": \"system\", \"content\": \"You are a helpful assistant.\"}, {\"role\": \"user\", \"content\": \"What is 2+2?\"}]}}\r\n");
            Assert.IsTrue(File.Exists(testFilePath));
            FileResponse file = null;

            try
            {
                try
                {
                    file = await HuoshanAIClient.FilesEndpoint.UploadFileAsync(testFilePath, FilePurpose.Batch);
                }
                finally
                {
                    if (File.Exists(testFilePath))
                    {
                        File.Delete(testFilePath);
                    }

                    Assert.IsFalse(File.Exists(testFilePath));
                }

                BatchResponse batch = null;

                try
                {
                    // create batch
                    var batchRequest = new CreateBatchRequest(file, Endpoint.ChatCompletions);
                    batch = await HuoshanAIClient.BatchEndpoint.CreateBatchAsync(batchRequest);
                    Assert.NotNull(batch);

                    // list batches
                    var listResponse = await HuoshanAIClient.BatchEndpoint.ListBatchesAsync();
                    Assert.NotNull(listResponse);
                    Assert.NotNull(listResponse.Items);

                    // retrieve batch
                    var retrievedBatch = await HuoshanAIClient.BatchEndpoint.RetrieveBatchAsync(batch);
                    Assert.NotNull(retrievedBatch);
                }
                finally
                {
                    // cancel batch
                    if (batch != null)
                    {
                        var isCancelled = await HuoshanAIClient.BatchEndpoint.CancelBatchAsync(batch);
                        Assert.IsTrue(isCancelled);
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    var isDeleted = await HuoshanAIClient.FilesEndpoint.DeleteFileAsync(file);
                    Assert.IsTrue(isDeleted);
                }
            }
        }
    }
}
