using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ModelConfig.Models;

namespace ModelConfig.Controls;

public class RoiCanvas : Control
{
    private enum DragMode
    {
        None,
        Creating,
        Moving,
        Resizing
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
        Left
    }

    private const int HandleSize = 8;

    private Bitmap? _image;
    private RectangleF _imageRect;
    private DragMode _dragMode;
    private ResizeHandle _activeHandle = ResizeHandle.None;
    private PointF _dragStartImage;
    private PointF _lastImagePoint;

    public event EventHandler? RoiListChanged;
    public event EventHandler? SelectedRoiChanged;

    public IList<RoiAnnotation> Rois { get; set; } = new List<RoiAnnotation>();

    public RoiAnnotation? SelectedRoi { get; private set; }

    public Bitmap? ImageBitmap
    {
        get => _image;
        set
        {
            _image = value;
            UpdateImageLayout();
            Invalidate();
        }
    }

    public RoiCanvas()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = Color.Black;
    }

    public void SelectRoi(RoiAnnotation? roi)
    {
        if (SelectedRoi == roi)
        {
            return;
        }

        SelectedRoi = roi;
        SelectedRoiChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateImageLayout();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.Clear(Color.FromArgb(30, 30, 30));

        if (_image != null)
        {
            e.Graphics.DrawImage(_image, _imageRect);
        }

        foreach (var roi in Rois)
        {
            var rect = MapImageToControl(roi.Bounds);
            using var pen = new Pen(roi.Classification == RoiClassification.Ok ? Color.Lime : Color.Red, 2);
            e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            if (roi == SelectedRoi)
            {
                DrawHandles(e.Graphics, rect);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (_image == null || e.Button != MouseButtons.Left)
        {
            return;
        }

        var imagePoint = MapControlToImage(e.Location);
        _dragStartImage = imagePoint;
        _lastImagePoint = imagePoint;

        var (hitRoi, handle) = HitTest(e.Location);
        if (hitRoi != null)
        {
            SelectRoi(hitRoi);

            if (handle != ResizeHandle.None)
            {
                _dragMode = DragMode.Resizing;
                _activeHandle = handle;
            }
            else
            {
                _dragMode = DragMode.Moving;
            }
        }
        else
        {
            SelectRoi(null);
            _dragMode = DragMode.Creating;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_image == null)
        {
            return;
        }

        var imagePoint = MapControlToImage(e.Location);

        if (_dragMode == DragMode.None)
        {
            UpdateCursor(e.Location);
            return;
        }

        if (_dragMode == DragMode.Creating)
        {
            var rect = CreateRect(_dragStartImage, imagePoint);
            SelectedRoi = null;
            Invalidate();
            DrawPreview(rect);
        }
        else if (_dragMode == DragMode.Moving && SelectedRoi != null)
        {
            var delta = new SizeF(imagePoint.X - _lastImagePoint.X, imagePoint.Y - _lastImagePoint.Y);
            var rect = SelectedRoi.Bounds;
            rect.X += delta.Width;
            rect.Y += delta.Height;
            SelectedRoi.Bounds = NormalizeRect(rect);
            OnRoiChanged();
        }
        else if (_dragMode == DragMode.Resizing && SelectedRoi != null)
        {
            var rect = SelectedRoi.Bounds;
            rect = ResizeRect(rect, imagePoint, _activeHandle);
            SelectedRoi.Bounds = NormalizeRect(rect);
            OnRoiChanged();
        }

        _lastImagePoint = imagePoint;
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (_image == null || e.Button != MouseButtons.Left)
        {
            return;
        }

        var imagePoint = MapControlToImage(e.Location);

        if (_dragMode == DragMode.Creating)
        {
            var rect = CreateRect(_dragStartImage, imagePoint);
            rect = NormalizeRect(rect);
            if (rect.Width > 5 && rect.Height > 5)
            {
                var roi = new RoiAnnotation
                {
                    Name = $"ROI {Rois.Count + 1}",
                    Classification = RoiClassification.Ok,
                    Bounds = rect
                };
                Rois.Add(roi);
                SelectRoi(roi);
                RoiListChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        else if (_dragMode == DragMode.Moving || _dragMode == DragMode.Resizing)
        {
            RoiListChanged?.Invoke(this, EventArgs.Empty);
        }

        _dragMode = DragMode.None;
        _activeHandle = ResizeHandle.None;
        Invalidate();
    }

    private void DrawHandles(Graphics g, RectangleF rect)
    {
        foreach (var handle in Enum.GetValues(typeof(ResizeHandle)).Cast<ResizeHandle>())
        {
            if (handle == ResizeHandle.None)
            {
                continue;
            }

            var handleRect = GetHandleRect(rect, handle);
            g.FillRectangle(Brushes.White, handleRect);
            g.DrawRectangle(Pens.Black, handleRect.X, handleRect.Y, handleRect.Width, handleRect.Height);
        }
    }

    private void UpdateCursor(Point location)
    {
        var (_, handle) = HitTest(location);
        Cursor = handle switch
        {
            ResizeHandle.TopLeft or ResizeHandle.BottomRight => Cursors.SizeNWSE,
            ResizeHandle.TopRight or ResizeHandle.BottomLeft => Cursors.SizeNESW,
            ResizeHandle.Top or ResizeHandle.Bottom => Cursors.SizeNS,
            ResizeHandle.Left or ResizeHandle.Right => Cursors.SizeWE,
            _ => Cursors.Default
        };
    }

    private (RoiAnnotation? roi, ResizeHandle handle) HitTest(Point location)
    {
        foreach (var roi in Rois)
        {
            var rect = MapImageToControl(roi.Bounds);
            var handle = GetHandleHit(rect, location);
            if (handle != ResizeHandle.None)
            {
                return (roi, handle);
            }

            if (rect.Contains(location))
            {
                return (roi, ResizeHandle.None);
            }
        }

        return (null, ResizeHandle.None);
    }

    private RectangleF CreateRect(PointF start, PointF end)
    {
        return new RectangleF(start.X, start.Y, end.X - start.X, end.Y - start.Y);
    }

    private RectangleF NormalizeRect(RectangleF rect)
    {
        var x = Math.Min(rect.Left, rect.Right);
        var y = Math.Min(rect.Top, rect.Bottom);
        var width = Math.Abs(rect.Width);
        var height = Math.Abs(rect.Height);
        return new RectangleF(x, y, width, height);
    }

    private RectangleF ResizeRect(RectangleF rect, PointF point, ResizeHandle handle)
    {
        switch (handle)
        {
            case ResizeHandle.TopLeft:
                rect.Width += rect.X - point.X;
                rect.Height += rect.Y - point.Y;
                rect.X = point.X;
                rect.Y = point.Y;
                break;
            case ResizeHandle.Top:
                rect.Height += rect.Y - point.Y;
                rect.Y = point.Y;
                break;
            case ResizeHandle.TopRight:
                rect.Width = point.X - rect.X;
                rect.Height += rect.Y - point.Y;
                rect.Y = point.Y;
                break;
            case ResizeHandle.Right:
                rect.Width = point.X - rect.X;
                break;
            case ResizeHandle.BottomRight:
                rect.Width = point.X - rect.X;
                rect.Height = point.Y - rect.Y;
                break;
            case ResizeHandle.Bottom:
                rect.Height = point.Y - rect.Y;
                break;
            case ResizeHandle.BottomLeft:
                rect.Width += rect.X - point.X;
                rect.Height = point.Y - rect.Y;
                rect.X = point.X;
                break;
            case ResizeHandle.Left:
                rect.Width += rect.X - point.X;
                rect.X = point.X;
                break;
        }

        return rect;
    }

    private ResizeHandle GetHandleHit(RectangleF rect, Point location)
    {
        foreach (var handle in Enum.GetValues(typeof(ResizeHandle)).Cast<ResizeHandle>())
        {
            if (handle == ResizeHandle.None)
            {
                continue;
            }

            var handleRect = GetHandleRect(rect, handle);
            if (handleRect.Contains(location))
            {
                return handle;
            }
        }

        return ResizeHandle.None;
    }

    private RectangleF GetHandleRect(RectangleF rect, ResizeHandle handle)
    {
        var x = rect.X;
        var y = rect.Y;
        var width = rect.Width;
        var height = rect.Height;

        return handle switch
        {
            ResizeHandle.TopLeft => new RectangleF(x - HandleSize / 2, y - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.Top => new RectangleF(x + width / 2 - HandleSize / 2, y - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.TopRight => new RectangleF(x + width - HandleSize / 2, y - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.Right => new RectangleF(x + width - HandleSize / 2, y + height / 2 - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.BottomRight => new RectangleF(x + width - HandleSize / 2, y + height - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.Bottom => new RectangleF(x + width / 2 - HandleSize / 2, y + height - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.BottomLeft => new RectangleF(x - HandleSize / 2, y + height - HandleSize / 2, HandleSize, HandleSize),
            ResizeHandle.Left => new RectangleF(x - HandleSize / 2, y + height / 2 - HandleSize / 2, HandleSize, HandleSize),
            _ => RectangleF.Empty
        };
    }

    private PointF MapControlToImage(PointF point)
    {
        if (_image == null || _imageRect.Width <= 0 || _imageRect.Height <= 0)
        {
            return point;
        }

        var normalizedX = (point.X - _imageRect.X) / _imageRect.Width;
        var normalizedY = (point.Y - _imageRect.Y) / _imageRect.Height;
        normalizedX = Math.Clamp(normalizedX, 0, 1);
        normalizedY = Math.Clamp(normalizedY, 0, 1);

        return new PointF(normalizedX * _image.Width, normalizedY * _image.Height);
    }

    private RectangleF MapImageToControl(RectangleF rect)
    {
        var topLeft = MapImageToControl(new PointF(rect.X, rect.Y));
        var bottomRight = MapImageToControl(new PointF(rect.Right, rect.Bottom));
        return new RectangleF(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
    }

    private PointF MapImageToControl(PointF point)
    {
        if (_image == null)
        {
            return point;
        }

        var normalizedX = point.X / _image.Width;
        var normalizedY = point.Y / _image.Height;

        return new PointF(
            _imageRect.X + normalizedX * _imageRect.Width,
            _imageRect.Y + normalizedY * _imageRect.Height);
    }

    private void UpdateImageLayout()
    {
        if (_image == null)
        {
            _imageRect = RectangleF.Empty;
            return;
        }

        var controlWidth = ClientSize.Width;
        var controlHeight = ClientSize.Height;

        if (controlWidth <= 0 || controlHeight <= 0)
        {
            _imageRect = RectangleF.Empty;
            return;
        }

        var scale = Math.Min((float)controlWidth / _image.Width, (float)controlHeight / _image.Height);
        var drawWidth = _image.Width * scale;
        var drawHeight = _image.Height * scale;
        var offsetX = (controlWidth - drawWidth) / 2f;
        var offsetY = (controlHeight - drawHeight) / 2f;
        _imageRect = new RectangleF(offsetX, offsetY, drawWidth, drawHeight);
    }

    private void DrawPreview(RectangleF imageRect)
    {
        Invalidate();
        using var previewGraphics = CreateGraphics();
        var rect = MapImageToControl(imageRect);
        using var pen = new Pen(Color.DeepSkyBlue, 1) { DashPattern = new[] { 4f, 4f } };
        previewGraphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
    }

    private void OnRoiChanged()
    {
        RoiListChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }

    public void RemoveSelected()
    {
        if (SelectedRoi == null)
        {
            return;
        }

        Rois.Remove(SelectedRoi);
        SelectRoi(null);
        RoiListChanged?.Invoke(this, EventArgs.Empty);
        Invalidate();
    }
}
