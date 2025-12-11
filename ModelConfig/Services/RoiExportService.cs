using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ModelConfig.Models;
using SkiaSharp;

namespace ModelConfig.Services;

public class RoiExportService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<IReadOnlyList<RoiAnnotation>> LoadConfigurationAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            throw new FileNotFoundException("ROI 설정 파일을 찾을 수 없습니다.", path);
        }

        await using var stream = File.OpenRead(path);
        var loaded = await JsonSerializer.DeserializeAsync<List<RoiAnnotation>>(stream, _jsonOptions) ?? new List<RoiAnnotation>();
        return loaded;
    }

    public async Task SaveConfigurationAsync(IEnumerable<RoiAnnotation> rois, string outputRoot)
    {
        ArgumentNullException.ThrowIfNull(rois);
        Directory.CreateDirectory(outputRoot);
        var path = Path.Combine(outputRoot, "rois.json");
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, rois, _jsonOptions);
    }

    public async Task ExportCropsAsync(IEnumerable<RoiAnnotation> rois, string sourceImagePath, string outputRoot)
    {
        ArgumentNullException.ThrowIfNull(rois);
        if (string.IsNullOrWhiteSpace(sourceImagePath) || !File.Exists(sourceImagePath))
        {
            throw new FileNotFoundException("Source image not found.", sourceImagePath);
        }

        Directory.CreateDirectory(outputRoot);

        using var bitmap = SKBitmap.Decode(sourceImagePath);
        if (bitmap == null)
        {
            throw new InvalidOperationException("Unable to decode source image.");
        }

        var baseImageName = Path.GetFileNameWithoutExtension(sourceImagePath);

        foreach (var roi in rois)
        {
            var targetFolder = Path.Combine(outputRoot, roi.Name, roi.Classification == RoiClassification.Ok ? "OK" : "NG");
            Directory.CreateDirectory(targetFolder);
            var rect = ToSkiaRect(roi.Bounds, bitmap.Width, bitmap.Height);

            using var subset = new SKBitmap(rect.Width, rect.Height);
            bitmap.ExtractSubset(subset, rect);
            using var image = SKImage.FromBitmap(subset);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            var filename = Path.Combine(targetFolder, $"{baseImageName}_{roi.Name}.png");
            await using var fileStream = File.Create(filename);
            data.SaveTo(fileStream);
        }
    }

    private static SKRectI ToSkiaRect(RectangleF roi, int width, int height)
    {
        var left = (int)Math.Clamp(Math.Floor(roi.Left), 0, width - 1);
        var top = (int)Math.Clamp(Math.Floor(roi.Top), 0, height - 1);
        var right = (int)Math.Clamp(Math.Ceiling(roi.Right), 0, width);
        var bottom = (int)Math.Clamp(Math.Ceiling(roi.Bottom), 0, height);
        var rect = SKRectI.Create(left, top, Math.Max(1, right - left), Math.Max(1, bottom - top));
        return rect;
    }
}
