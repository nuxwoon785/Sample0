using System.Globalization;
using YoloLabeler.Models;

namespace YoloLabeler.Services;

public class AnnotationService
{
    public IReadOnlyList<Annotation> LoadAnnotations(string imagePath, int imageWidth, int imageHeight)
    {
        var results = new List<Annotation>();
        var annotationPath = GetAnnotationPath(imagePath);
        if (!File.Exists(annotationPath))
        {
            return results;
        }

        foreach (var line in File.ReadAllLines(annotationPath))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 5)
            {
                continue;
            }

            if (!int.TryParse(parts[0], out var classId) ||
                !float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var xCenter) ||
                !float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var yCenter) ||
                !float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var width) ||
                !float.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out var height))
            {
                continue;
            }

            var rect = FromYolo(xCenter, yCenter, width, height, imageWidth, imageHeight);
            results.Add(new Annotation(rect, classId));
        }

        return results;
    }

    public void SaveAnnotations(string imagePath, int imageWidth, int imageHeight, IEnumerable<Annotation> annotations)
    {
        var lines = annotations.Select(a => ToYolo(a.Bounds, imageWidth, imageHeight, a.ClassId));
        File.WriteAllLines(GetAnnotationPath(imagePath), lines);
    }

    public string GetAnnotationPath(string imagePath)
    {
        var directory = Path.GetDirectoryName(imagePath) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(imagePath);
        return Path.Combine(directory, fileName + ".txt");
    }

    private static string ToYolo(RectangleF rect, int imageWidth, int imageHeight, int classId)
    {
        var xCenter = (rect.X + rect.Width / 2f) / imageWidth;
        var yCenter = (rect.Y + rect.Height / 2f) / imageHeight;
        var width = rect.Width / imageWidth;
        var height = rect.Height / imageHeight;

        return string.Join(' ', new[]
        {
            classId.ToString(CultureInfo.InvariantCulture),
            xCenter.ToString("0.######", CultureInfo.InvariantCulture),
            yCenter.ToString("0.######", CultureInfo.InvariantCulture),
            width.ToString("0.######", CultureInfo.InvariantCulture),
            height.ToString("0.######", CultureInfo.InvariantCulture)
        });
    }

    private static RectangleF FromYolo(float xCenter, float yCenter, float width, float height, int imageWidth, int imageHeight)
    {
        var rectWidth = width * imageWidth;
        var rectHeight = height * imageHeight;
        var x = xCenter * imageWidth - rectWidth / 2f;
        var y = yCenter * imageHeight - rectHeight / 2f;
        return new RectangleF(x, y, rectWidth, rectHeight);
    }
}
