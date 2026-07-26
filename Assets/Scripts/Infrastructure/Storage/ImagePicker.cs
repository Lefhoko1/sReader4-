using System.IO;
using System.Threading.Tasks;
using SReader.Domains.Education.Services;

namespace SReader.Infrastructure.Storage
{
    /// <summary>
    /// Image picker. In the Unity Editor it opens a native file dialog so the
    /// payment-proof flow is fully testable. On a device it returns a clear
    /// "not supported" result — drop in a gallery plugin (e.g. NativeGallery)
    /// here without touching the rest of the app.
    /// </summary>
    public sealed class ImagePicker : IImagePicker
    {
        public Task<PickedImage> PickImageAsync()
        {
#if UNITY_EDITOR
            var path = UnityEditor.EditorUtility.OpenFilePanel("Select proof of payment", "", "png,jpg,jpeg");
            if (string.IsNullOrEmpty(path))
                return Task.FromResult(PickedImage.Cancel());

            try
            {
                var data = File.ReadAllBytes(path);
                var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
                if (string.IsNullOrEmpty(ext)) ext = "png";
                return Task.FromResult(PickedImage.Ok(data, ext));
            }
            catch (System.Exception ex)
            {
                return Task.FromResult(PickedImage.Fail($"Couldn't read the image: {ex.Message}"));
            }
#else
            return Task.FromResult(PickedImage.Fail(
                "Picking an image isn't available on this device build yet."));
#endif
        }
    }
}
