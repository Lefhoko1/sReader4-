using System.Threading.Tasks;

namespace SReader.Domains.Education.Services
{
    /// <summary>An image chosen by the user, or a cancel/failure outcome.</summary>
    public struct PickedImage
    {
        public bool Success;
        public bool Cancelled;
        public byte[] Data;
        public string Extension; // "png" / "jpg"
        public string Error;

        public static PickedImage Ok(byte[] data, string ext) => new PickedImage { Success = true, Data = data, Extension = ext };
        public static PickedImage Cancel() => new PickedImage { Cancelled = true };
        public static PickedImage Fail(string error) => new PickedImage { Error = error };
    }

    /// <summary>
    /// Picks an image from the device. Editor uses a file dialog; on-device this
    /// needs a gallery plugin (swap the implementation, the seam is here).
    /// </summary>
    public interface IImagePicker
    {
        Task<PickedImage> PickImageAsync();
    }
}
