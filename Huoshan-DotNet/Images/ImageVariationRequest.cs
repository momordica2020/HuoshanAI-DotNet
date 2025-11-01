// Licensed under the MIT License. See LICENSE in the project root for license information.

using HuoshanAI.Models;
using System;
using System.IO;

namespace HuoshanAI.Images
{
    public sealed class ImageVariationRequest : AbstractBaseImageRequest, IDisposable
    {
        public ImageVariationRequest(
            string imagePath,
            int? numberOfResults = null,
            string size = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : this((Path.GetFileName(imagePath), File.OpenRead(imagePath)), numberOfResults, size, user, responseFormat, model)
        {
        }

        public ImageVariationRequest(
            (string, Stream) image,
            int? numberOfResults = null,
            string size = null,
            string user = null,
            ImageResponseFormat responseFormat = 0,
            Model model = null)
            : base(model, numberOfResults, size, responseFormat, user)
        {
            var (imageName, imageStream) = image;
            Image = imageStream ?? throw new ArgumentNullException(nameof(imageStream));
            ImageName = string.IsNullOrWhiteSpace(imageName) ? "image.png" : imageName;
        }

       

        ~ImageVariationRequest() => Dispose(false);

        /// <summary>
        /// The image to use as the basis for the variation(s). Must be a valid PNG file, less than 4MB, and square.
        /// </summary>
        public Stream Image { get; }

        public string ImageName { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
