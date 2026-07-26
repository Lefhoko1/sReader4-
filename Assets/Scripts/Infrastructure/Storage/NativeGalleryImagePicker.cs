using System;
using System.IO;
using System.Threading.Tasks;
using SReader.Domains.Education.Services;

namespace SReader.Infrastructure.Storage
{
    /// <summary>
    /// Image picker backed by the NativeGallery plugin (yasirkula). Works on
    /// Android, iOS and in the Unity Editor (NativeGallery opens a file dialog
    /// there), so it replaces the editor-only ImagePicker everywhere.
    /// </summary>
    public sealed class NativeGalleryImagePicker : IImagePicker
    {
        public Task<PickedImage> PickImageAsync()
        {
            var tcs = new TaskCompletionSource<PickedImage>();

            // NativeGallery handles runtime permission prompts internally and
            // calls back with null if the user cancels or denies access.
            NativeGallery.GetImageFromGallery(path =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    tcs.TrySetResult(PickedImage.Cancel());
                    return;
                }

                try
                {
                    var bytes = File.ReadAllBytes(path);
                    var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext)) ext = "jpg";
                    tcs.TrySetResult(PickedImage.Ok(bytes, ext));
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(PickedImage.Fail($"Couldn't read the image: {ex.Message}"));
                }
            }, "Select proof of payment", "image/*");

            return tcs.Task;
        }
    }
}
