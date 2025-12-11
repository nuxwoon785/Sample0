using Avalonia;
using Avalonia.Media.Imaging;
using RoiManager.Models;

namespace RoiManager.Services;

public class RoiStorage
{
    private readonly Bitmap _bitmap;
    private readonly string _basePath;

    public RoiStorage(Bitmap bitmap, string basePath)
    {
        _bitmap = bitmap;
        _basePath = basePath;
    }

    public void SaveCrops(IEnumerable<RoiEntry> rois)
    {
        Directory.CreateDirectory(_basePath);
        foreach (var roi in rois)
        {
            var resultFolder = Path.Combine(_basePath, roi.Name, roi.IsOk ? "OK" : "NG");
            Directory.CreateDirectory(resultFolder);
            var rect = roi.Region.ToPixelRect();
            rect = PixelRect.Clamp(rect, new PixelRect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height));
            if (rect.Width <= 0 || rect.Height <= 0) continue;

            using var cropped = new CroppedBitmap(_bitmap, rect);
            var filePath = Path.Combine(resultFolder, $"{roi.Name}_{DateTime.Now:yyyyMMdd_HHmmssfff}.png");
            using var stream = File.Create(filePath);
            cropped.Save(stream);
        }
    }
}

public static class RectExtensions
{
    public static PixelRect ToPixelRect(this Rect rect)
    {
        return new PixelRect(
            (int)Math.Round(rect.X),
            (int)Math.Round(rect.Y),
            (int)Math.Round(rect.Width),
            (int)Math.Round(rect.Height));
    }
}
