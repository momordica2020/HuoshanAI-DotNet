// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using HuoshanAI.Chat;
using HuoshanAI.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HuoshanAI.Tests
{
    internal class TestFixture_08_Files : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_UploadFile()
        {
            Assert.IsNotNull(HuoshanAIClient.FilesEndpoint);
            var testData = new Conversation(new List<Message> { new(Role.Assistant, "I'm a learning language model") });
            await File.WriteAllTextAsync("test.jsonl", testData);
            Assert.IsTrue(File.Exists("test.jsonl"));
            var result = await HuoshanAIClient.FilesEndpoint.UploadFileAsync("test.jsonl", FilePurpose.FineTune);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FileName == "test.jsonl");
            Console.WriteLine($"{result.Id} -> {result.Object}");
            File.Delete("test.jsonl");
            Assert.IsFalse(File.Exists("test.jsonl"));
        }

        [Test]
        public async Task Test_02_ListFiles()
        {
            Assert.IsNotNull(HuoshanAIClient.FilesEndpoint);
            var fileList = await HuoshanAIClient.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(fileList);
            Assert.IsNotEmpty(fileList);

            foreach (var file in fileList)
            {
                var fileInfo = await HuoshanAIClient.FilesEndpoint.GetFileInfoAsync(file);
                Assert.IsNotNull(fileInfo);
                Console.WriteLine($"{fileInfo.Id} -> {fileInfo.Object}: {fileInfo.FileName} | {fileInfo.Size} bytes");
            }
        }

        [Test]
        public async Task Test_03_DownloadFile()
        {
            Assert.IsNotNull(HuoshanAIClient.FilesEndpoint);
            var fileList = await HuoshanAIClient.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(fileList);
            Assert.IsNotEmpty(fileList);

            var testFileData = fileList[0];
            var result = await HuoshanAIClient.FilesEndpoint.DownloadFileAsync(testFileData, Directory.GetCurrentDirectory());

            Assert.IsNotNull(result);
            Console.WriteLine(result);
            Assert.IsTrue(File.Exists(result));

            File.Delete(result);
            Assert.IsFalse(File.Exists(result));
        }

        [Test]
        public async Task Test_04_RetrieveFileStreamAsync()
        {
            Assert.IsNotNull(HuoshanAIClient.FilesEndpoint);
            var fileList = await HuoshanAIClient.FilesEndpoint.ListFilesAsync();

            Assert.IsNotNull(fileList);
            Assert.IsNotEmpty(fileList);

            var testFileData = fileList[0];
            var result = await HuoshanAIClient.FilesEndpoint.RetrieveFileStreamAsync(testFileData);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.CanRead);
        }

        [Test]
        public async Task Test_05_DeleteFiles()
        {
            Assert.IsNotNull(HuoshanAIClient.FilesEndpoint);
            var fileList = await HuoshanAIClient.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(fileList);
            Assert.IsNotEmpty(fileList);

            foreach (var file in fileList)
            {
                var isDeleted = await HuoshanAIClient.FilesEndpoint.DeleteFileAsync(file);
                Assert.IsTrue(isDeleted);
                Console.WriteLine($"{file.Id} -> deleted");
            }

            fileList = await HuoshanAIClient.FilesEndpoint.ListFilesAsync();
            Assert.IsNotNull(fileList);
            Assert.IsEmpty(fileList);
        }
    }
}
