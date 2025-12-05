using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
    private bool _resizing;
    private ResizeHandle _activeHandle = ResizeHandle.None;
    private Annotation? _selected;
    private RectangleF _draftRect;

    private readonly Color[] _palette = new[]
    {
        Color.DeepSkyBlue,
        Color.LimeGreen,
        Color.Orange,
        Color.MediumOrchid,
        Color.Gold,
        Color.Tomato,
        Color.MediumTurquoise,
        Color.HotPink,
        Color.MediumPurple,
        Color.DarkKhaki
    };

    private const float HandleSize = 8f;

    public List<Annotation> Annotations { get; } = new();
    public Annotation? SelectedAnnotation => _selected;

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
        if (hit != null && e.Button == MouseButtons.Left)
        {
            _selected = hit;
            SelectionChanged?.Invoke(this, _selected);
            var handle = HitTestHandle(hit, imagePoint);
            if (handle == ResizeHandle.Center)
            {
                _movingSelection = true;
                Cursor = Cursors.SizeAll;
            }
            else if (handle != ResizeHandle.None)
            {
                _resizing = true;
                _activeHandle = handle;
                Cursor = CursorForHandle(handle);
            }
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
        else if (_resizing && _selected != null)
        {
            var delta = ScreenDeltaToImageDelta(current, _lastMouse);
            _selected.Bounds = ResizeRectangle(_selected.Bounds, delta, _activeHandle);
            Invalidate();
        }
        else if (_selected != null)
        {
            var imagePoint = ScreenToImage(current);
            var handle = HitTestHandle(_selected, imagePoint);
            Cursor = CursorForHandle(handle);
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
        _resizing = false;
        _activeHandle = ResizeHandle.None;
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
            var color = ColorForClass(annotation.ClassId);
            DrawAnnotation(e.Graphics, annotation, color, annotation == _selected);
        }

        if (_drawing)
        {
            DrawAnnotation(e.Graphics, new Annotation(_draftRect, -1), Color.DeepSkyBlue, false);
        }

        if (_selected != null)
        {
            DrawAnchors(e.Graphics, _selected.Bounds, ColorForClass(_selected.ClassId));
            DrawHandles(e.Graphics, _selected.Bounds);
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

    public void SelectAnnotation(Annotation? annotation)
    {
        if (annotation != null && !Annotations.Contains(annotation))
        {
            return;
        }

        if (_selected != annotation)
        {
            _selected = annotation;
            SelectionChanged?.Invoke(this, _selected);
            Invalidate();
        }
    }

    public void MoveSelection(PointF delta)
    {
        if (_selected == null)
        {
            return;
        }

        var rect = _selected.Bounds;
        rect.X += delta.X;
        rect.Y += delta.Y;
        _selected.Bounds = rect;
        Invalidate();
    }

    public void ResizeSelection(SizeF delta)
    {
        if (_selected == null)
        {
            return;
        }

        var rect = _selected.Bounds;
        var width = Math.Max(2f, rect.Width + delta.Width);
        var height = Math.Max(2f, rect.Height + delta.Height);
        _selected.Bounds = new RectangleF(rect.X, rect.Y, width, height);
        Invalidate();
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

    private void DrawAnnotation(Graphics g, Annotation annotation, Color color, bool selected)
    {
        using var fill = new SolidBrush(Color.FromArgb(80, color));
        using var pen = new Pen(color, selected ? 2f : 1.4f);
        g.FillRectangle(fill, annotation.Bounds);
        g.DrawRectangle(pen, annotation.Bounds.X, annotation.Bounds.Y, annotation.Bounds.Width, annotation.Bounds.Height);
    }

    private void DrawHandles(Graphics g, RectangleF bounds)
    {
        using var brush = new SolidBrush(Color.FromArgb(200, Color.DodgerBlue));
        foreach (var handleRect in GetHandleRects(bounds).Values)
        {
            g.FillRectangle(brush, handleRect);
            g.DrawRectangle(Pens.White, handleRect.X, handleRect.Y, handleRect.Width, handleRect.Height);
        }
    }

    private RectangleF NormalizeRectangle(PointF start, PointF end)
    {
        var x = Math.Min(start.X, end.X);
        var y = Math.Min(start.Y, end.Y);
        var width = Math.Abs(start.X - end.X);
        var height = Math.Abs(start.Y - end.Y);
        return new RectangleF(x, y, width, height);
    }

    private Dictionary<ResizeHandle, RectangleF> GetHandleRects(RectangleF bounds)
    {
        var half = HandleSize / 2f;
        var centers = new Dictionary<ResizeHandle, PointF>
        {
            { ResizeHandle.TopLeft, new PointF(bounds.Left, bounds.Top) },
            { ResizeHandle.Top, new PointF(bounds.Left + bounds.Width / 2f, bounds.Top) },
            { ResizeHandle.TopRight, new PointF(bounds.Right, bounds.Top) },
            { ResizeHandle.Right, new PointF(bounds.Right, bounds.Top + bounds.Height / 2f) },
            { ResizeHandle.BottomRight, new PointF(bounds.Right, bounds.Bottom) },
            { ResizeHandle.Bottom, new PointF(bounds.Left + bounds.Width / 2f, bounds.Bottom) },
            { ResizeHandle.BottomLeft, new PointF(bounds.Left, bounds.Bottom) },
            { ResizeHandle.Left, new PointF(bounds.Left, bounds.Top + bounds.Height / 2f) },
        };

        return centers.ToDictionary(
            kvp => kvp.Key,
            kvp => new RectangleF(kvp.Value.X - half, kvp.Value.Y - half, HandleSize, HandleSize));
    }

    private ResizeHandle HitTestHandle(Annotation annotation, PointF imagePoint)
    {
        var handles = GetHandleRects(annotation.Bounds);
        foreach (var kvp in handles)
        {
            if (kvp.Value.Contains(imagePoint))
            {
                return kvp.Key;
            }
        }

        return annotation.Bounds.Contains(imagePoint) ? ResizeHandle.Center : ResizeHandle.None;
    }

    private Cursor CursorForHandle(ResizeHandle handle) => handle switch
    {
        ResizeHandle.TopLeft or ResizeHandle.BottomRight => Cursors.SizeNWSE,
        ResizeHandle.TopRight or ResizeHandle.BottomLeft => Cursors.SizeNESW,
        ResizeHandle.Left or ResizeHandle.Right => Cursors.SizeWE,
        ResizeHandle.Top or ResizeHandle.Bottom => Cursors.SizeNS,
        ResizeHandle.Center => Cursors.SizeAll,
        _ => Cursors.Default
    };

    private RectangleF ResizeRectangle(RectangleF rect, PointF delta, ResizeHandle handle)
    {
        var left = rect.Left;
        var right = rect.Right;
        var top = rect.Top;
        var bottom = rect.Bottom;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
                left += delta.X;
                top += delta.Y;
                break;
            case ResizeHandle.Top:
                top += delta.Y;
                break;
            case ResizeHandle.TopRight:
                right += delta.X;
                top += delta.Y;
                break;
            case ResizeHandle.Right:
                right += delta.X;
                break;
            case ResizeHandle.BottomRight:
                right += delta.X;
                bottom += delta.Y;
                break;
            case ResizeHandle.Bottom:
                bottom += delta.Y;
                break;
            case ResizeHandle.BottomLeft:
                left += delta.X;
                bottom += delta.Y;
                break;
            case ResizeHandle.Left:
                left += delta.X;
                break;
        }

        // Ensure minimum size
        const float minSize = 2f;
        if (right - left < minSize)
        {
            var mid = (left + right) / 2f;
            left = mid - minSize / 2f;
            right = mid + minSize / 2f;
        }

        if (bottom - top < minSize)
        {
            var mid = (top + bottom) / 2f;
            top = mid - minSize / 2f;
            bottom = mid + minSize / 2f;
        }

        return RectangleF.FromLTRB(left, top, right, bottom);
    }

    private enum ResizeHandle
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        Center
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

    private void DrawAnchors(Graphics g, RectangleF bounds, Color color)
    {
        var radius = HandleSize / 2f;
        using var fill = new SolidBrush(color);
        using var outline = new Pen(Color.White, 1f);
        foreach (var point in AnchorPoints(bounds))
        {
            var rect = new RectangleF(point.X - radius, point.Y - radius, HandleSize, HandleSize);
            g.FillEllipse(fill, rect);
            g.DrawEllipse(outline, rect);
        }
    }

    private IEnumerable<PointF> AnchorPoints(RectangleF bounds)
    {
        yield return new PointF(bounds.Left, bounds.Top);
        yield return new PointF(bounds.Right, bounds.Top);
        yield return new PointF(bounds.Right, bounds.Bottom);
        yield return new PointF(bounds.Left, bounds.Bottom);
    }

    private Color ColorForClass(int classId)
    {
        if (classId < 0)
        {
            return Color.LightGray;
        }

        return _palette[classId % _palette.Length];
    }
}
