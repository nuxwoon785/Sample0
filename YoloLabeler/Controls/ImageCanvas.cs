using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YoloLabeler.Models;

namespace YoloLabeler.Controls;

public class ImageCanvas : Control
{
    private Image? _image;
    private float _scale = 1f;
    private PointF _pan = PointF.Empty;
    private Point _lastMouse;
    private bool _panning;
    private bool _drawing;
    private bool _movingSelection;
    private Annotation? _selected;
    private RectangleF _draftRect;

    public List<Annotation> Annotations { get; } = new();

    public Image? CanvasImage
    {
        get => _image;
        set
        {
            _image = value;
            _scale = 1f;
            _pan = PointF.Empty;
            _selected = null;
            Annotations.Clear();
            Invalidate();
        }
    }

    public event EventHandler<Annotation?>? SelectionChanged;
    public event EventHandler? DraftCompleted;

    public ImageCanvas()
    {
        DoubleBuffered = true;
        BackColor = Color.Black;
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    public void SetAnnotations(IEnumerable<Annotation> annotations)
    {
        Annotations.Clear();
        Annotations.AddRange(annotations);
        _selected = null;
        Invalidate();
    }

    public void ResetView()
    {
        _scale = 1f;
        _pan = PointF.Empty;
        Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (_image == null)
        {
            return;
        }

        var delta = e.Delta > 0 ? 0.1f : -0.1f;
        var newScale = Math.Clamp(_scale + delta, 0.2f, 5f);
        var cursorBefore = ScreenToImage(e.Location);
        _scale = newScale;
        var cursorAfter = ImageToScreen(cursorBefore);
        _pan = new PointF(_pan.X + (e.Location.X - cursorAfter.X), _pan.Y + (e.Location.Y - cursorAfter.Y));
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _lastMouse = e.Location;
        if (_image == null)
        {
            return;
        }

        if (e.Button == MouseButtons.Right)
        {
            _panning = true;
            Cursor = Cursors.Hand;
            return;
        }

        var imagePoint = ScreenToImage(e.Location);
        var hit = FindAnnotation(imagePoint);
        if (hit != null)
        {
            _selected = hit;
            _movingSelection = true;
            SelectionChanged?.Invoke(this, _selected);
            Cursor = Cursors.SizeAll;
        }
        else if (e.Button == MouseButtons.Left)
        {
            _draftRect = new RectangleF(imagePoint, SizeF.Empty);
            _drawing = true;
            _selected = null;
            SelectionChanged?.Invoke(this, null);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (_image == null)
        {
            return;
        }

        var current = e.Location;
        if (_panning)
        {
            _pan = new PointF(_pan.X + (current.X - _lastMouse.X), _pan.Y + (current.Y - _lastMouse.Y));
            Invalidate();
        }
        else if (_drawing)
        {
            var start = _draftRect.Location;
            var end = ScreenToImage(current);
            _draftRect = NormalizeRectangle(start, end);
            Invalidate();
        }
        else if (_movingSelection && _selected != null)
        {
            var delta = ScreenDeltaToImageDelta(current, _lastMouse);
            var rect = _selected.Bounds;
            rect.X += delta.X;
            rect.Y += delta.Y;
            _selected.Bounds = rect;
            Invalidate();
        }

        _lastMouse = current;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        Cursor = Cursors.Default;
        if (_image == null)
        {
            return;
        }

        if (e.Button == MouseButtons.Right)
        {
            _panning = false;
            return;
        }

        if (_drawing && _draftRect.Width > 2 && _draftRect.Height > 2)
        {
            var annotation = new Annotation(_draftRect, -1);
            Annotations.Add(annotation);
            _selected = annotation;
            DraftCompleted?.Invoke(this, EventArgs.Empty);
        }

        _drawing = false;
        _movingSelection = false;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(BackColor);
        if (_image == null)
        {
            return;
        }

        using var transform = GetImageTransform();
        e.Graphics.Transform = transform;
        e.Graphics.DrawImage(_image, 0, 0, _image.Width, _image.Height);

        foreach (var annotation in Annotations)
        {
            DrawAnnotation(e.Graphics, annotation, annotation == _selected ? Pens.Lime : Pens.Orange);
        }

        if (_drawing)
        {
            DrawAnnotation(e.Graphics, new Annotation(_draftRect, -1), Pens.DeepSkyBlue);
        }
    }

    public void DeleteSelected()
    {
        if (_selected != null)
        {
            Annotations.Remove(_selected);
            _selected = null;
            SelectionChanged?.Invoke(this, null);
            Invalidate();
        }
    }

    public void SetSelectionClass(int classId)
    {
        if (_selected != null)
        {
            _selected.ClassId = classId;
            Invalidate();
        }
    }

    private Annotation? FindAnnotation(PointF imagePoint)
    {
        for (int i = Annotations.Count - 1; i >= 0; i--)
        {
            if (Annotations[i].Bounds.Contains(imagePoint))
            {
                return Annotations[i];
            }
        }

        return null;
    }

    private void DrawAnnotation(Graphics g, Annotation annotation, Pen pen)
    {
        g.DrawRectangle(pen, annotation.Bounds.X, annotation.Bounds.Y, annotation.Bounds.Width, annotation.Bounds.Height);
    }

    private RectangleF NormalizeRectangle(PointF start, PointF end)
    {
        var x = Math.Min(start.X, end.X);
        var y = Math.Min(start.Y, end.Y);
        var width = Math.Abs(start.X - end.X);
        var height = Math.Abs(start.Y - end.Y);
        return new RectangleF(x, y, width, height);
    }

    private Matrix GetImageTransform()
    {
        var transform = new Matrix();
        transform.Translate(ClientSize.Width / 2f + _pan.X, ClientSize.Height / 2f + _pan.Y);
        transform.Scale(_scale, _scale);
        if (_image != null)
        {
            transform.Translate(-_image.Width / 2f, -_image.Height / 2f);
        }

        return transform;
    }

    private Matrix GetInverseTransform()
    {
        var transform = GetImageTransform();
        transform.Invert();
        return transform;
    }

    private PointF ScreenToImage(Point screenPoint)
    {
        using var inverse = GetInverseTransform();
        var pts = new[] { new PointF(screenPoint.X, screenPoint.Y) };
        inverse.TransformPoints(pts);
        return pts[0];
    }

    private PointF ImageToScreen(PointF imagePoint)
    {
        using var transform = GetImageTransform();
        var pts = new[] { imagePoint };
        transform.TransformPoints(pts);
        return pts[0];
    }

    private PointF ScreenDeltaToImageDelta(Point current, Point previous)
    {
        var imageCurrent = ScreenToImage(current);
        var imagePrev = ScreenToImage(previous);
        return new PointF(imageCurrent.X - imagePrev.X, imageCurrent.Y - imagePrev.Y);
    }
}
