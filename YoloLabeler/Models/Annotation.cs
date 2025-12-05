using System.Drawing;

namespace YoloLabeler.Models;

public class Annotation
{
    public RectangleF Bounds { get; set; }
    public int ClassId { get; set; }

    public Annotation(RectangleF bounds, int classId)
    {
        Bounds = bounds;
        ClassId = classId;
    }
}
