using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using RoiManager.Models;
using RoiManager.Services;

namespace RoiManager;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<RoiEntry> _rois = new();
    private Bitmap? _bitmap;
    private Rect? _draftRect;
    private RoiEntry? _selected;
    private EditMode _editMode = EditMode.None;
    private Point _startPoint;
    private string _configPath = Path.Combine(AppContext.BaseDirectory, "roi_config.json");

    public MainWindow()
    {
        InitializeComponent();
        RoiList.Items = _rois;
        ResultSelector.SelectedIndex = 0;
        OutputPathBox.Text = Path.Combine(AppContext.BaseDirectory, "Output");

        LoadImageButton.Click += OnLoadImage;
        AddOrUpdateButton.Click += OnAddOrUpdate;
        DeleteButton.Click += OnDelete;
        SaveButton.Click += OnSave;
        RoiList.SelectionChanged += (_, _) => SelectRoi(RoiList.SelectedItem as RoiEntry);

        Overlay.PointerPressed += OnPointerPressed;
        Overlay.PointerMoved += OnPointerMoved;
        Overlay.PointerReleased += OnPointerReleased;
        Overlay.PointerLeave += (_, _) => { if (_editMode == EditMode.Creating) FinishDraft(); };

        TryLoadConfig();
    }

    private void TryLoadConfig()
    {
        if (!File.Exists(_configPath)) return;
        try
        {
            var json = File.ReadAllText(_configPath);
            var items = JsonSerializer.Deserialize<List<RoiEntry>>(json);
            if (items != null)
            {
                _rois.Clear();
                foreach (var item in items)
                {
                    _rois.Add(item);
                }
            }
        }
        catch
        {
            // ignore malformed config
        }
    }

    private async void OnLoadImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "이미지 선택",
            AllowMultiple = false
        });
        var file = files.FirstOrDefault();
        if (file == null) return;
        await using var stream = await file.OpenReadAsync();
        _bitmap = await Task.Run(() => Bitmap.DecodeToWidth(stream, 2000));
        ImageView.Source = _bitmap;
        _draftRect = null;
        RoiInfo.Text = "이미지를 불러왔습니다. 마우스로 ROI를 지정하세요.";
        InvalidateOverlay();
    }

    private void OnAddOrUpdate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_bitmap == null)
        {
            RoiInfo.Text = "이미지를 먼저 불러오세요.";
            return;
        }

        var name = RoiNameBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            RoiInfo.Text = "ROI 이름을 입력하세요.";
            return;
        }

        var rect = _selected?.Region ?? _draftRect;
        if (rect == null || rect.Value.Width < 1 || rect.Value.Height < 1)
        {
            RoiInfo.Text = "ROI 영역을 먼저 지정하세요.";
            return;
        }

        var isOk = (ResultSelector.SelectedItem as ComboBoxItem)?.Content?.ToString() == "OK";

        var existing = _rois.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            existing.Region = rect.Value;
            existing.IsOk = isOk;
            _selected = existing;
        }
        else
        {
            var entry = new RoiEntry
            {
                Name = name,
                Region = rect.Value,
                IsOk = isOk
            };
            _rois.Add(entry);
            _selected = entry;
        }

        RoiList.SelectedItem = _selected;
        _draftRect = null;
        RoiInfo.Text = "ROI가 등록되었습니다.";
        SaveConfig();
        InvalidateOverlay();
    }

    private void OnDelete(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_selected == null) return;
        _rois.Remove(_selected);
        _selected = null;
        RoiList.SelectedItem = null;
        RoiInfo.Text = "선택한 ROI가 삭제되었습니다.";
        SaveConfig();
        InvalidateOverlay();
    }

    private void OnSave(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_bitmap == null)
        {
            RoiInfo.Text = "이미지를 먼저 불러오세요.";
            return;
        }

        if (_rois.Count == 0)
        {
            RoiInfo.Text = "저장할 ROI가 없습니다.";
            return;
        }

        var basePath = OutputPathBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(basePath))
        {
            RoiInfo.Text = "저장 경로를 입력하세요.";
            return;
        }

        var saver = new RoiStorage(_bitmap, basePath);
        saver.SaveCrops(_rois.ToList());
        SaveConfig();
        RoiInfo.Text = "ROI 이미지와 설정이 저장되었습니다.";
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_bitmap == null) return;
        var point = GetImagePoint(e.GetPosition(Overlay));
        if (point == null) return;

        var hit = HitTest(point.Value);
        _startPoint = point.Value;

        if (hit.mode != EditMode.None && hit.target != null)
        {
            _selected = hit.target;
            _editMode = hit.mode;
            RoiList.SelectedItem = _selected;
        }
        else
        {
            _editMode = EditMode.Creating;
            _draftRect = new Rect(point.Value, new Size(1, 1));
            SelectRoi(null);
        }

        InvalidateOverlay();
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_bitmap == null) return;
        if (_editMode == EditMode.None) return;
        var point = GetImagePoint(e.GetPosition(Overlay));
        if (point == null) return;

        switch (_editMode)
        {
            case EditMode.Creating:
                _draftRect = Normalize(new Rect(_startPoint, point.Value));
                break;
            case EditMode.Moving:
                if (_selected != null)
                {
                    var delta = point.Value - _startPoint;
                    _selected.Region = ClampToImage(new Rect(_selected.Region.Position + delta, _selected.Region.Size));
                    _startPoint = point.Value;
                }
                break;
            case EditMode.ResizeTopLeft:
            case EditMode.ResizeTopRight:
            case EditMode.ResizeBottomLeft:
            case EditMode.ResizeBottomRight:
                if (_selected != null)
                {
                    _selected.Region = Normalize(ResizeWithAnchor(_selected.Region, _editMode, point.Value));
                }
                break;
        }

        InvalidateOverlay();
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        FinishDraft();
    }

    private void FinishDraft()
    {
        _editMode = EditMode.None;
        if (_draftRect.HasValue)
        {
            RoiInfo.Text = "ROI 영역이 설정되었습니다. 이름을 입력하고 등록하세요.";
        }
    }

    private void SelectRoi(RoiEntry? entry)
    {
        _selected = entry;
        if (entry != null)
        {
            RoiNameBox.Text = entry.Name;
            ResultSelector.SelectedIndex = entry.IsOk ? 0 : 1;
            RoiInfo.Text = entry.ToDisplayString();
        }
        else
        {
            RoiInfo.Text = "ROI를 선택하거나 새로 그려 등록하세요.";
        }

        InvalidateOverlay();
    }

    private (EditMode mode, RoiEntry? target) HitTest(Point p)
    {
        const double handleSize = 12;
        foreach (var roi in _rois)
        {
            var display = GetDisplayRect(roi.Region);
            var handles = GetHandles(display, handleSize);
            if (handles.TopLeft.Contains(p)) return (EditMode.ResizeTopLeft, roi);
            if (handles.TopRight.Contains(p)) return (EditMode.ResizeTopRight, roi);
            if (handles.BottomLeft.Contains(p)) return (EditMode.ResizeBottomLeft, roi);
            if (handles.BottomRight.Contains(p)) return (EditMode.ResizeBottomRight, roi);
            if (display.Contains(p)) return (EditMode.Moving, roi);
        }

        return (EditMode.None, null);
    }

    private void InvalidateOverlay()
    {
        Overlay.Children.Clear();
        if (_bitmap == null) return;

        foreach (var roi in _rois)
        {
            var rect = GetDisplayRect(roi.Region);
            Overlay.Children.Add(CreateRectangle(rect, roi == _selected, roi.IsOk));
            if (roi == _selected)
            {
                foreach (var handle in GetHandles(rect, 10).All)
                {
                    Overlay.Children.Add(new Border
                    {
                        Width = 10,
                        Height = 10,
                        Background = Brushes.White,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        [Canvas.LeftProperty] = handle.X,
                        [Canvas.TopProperty] = handle.Y
                    });
                }
            }
        }

        if (_draftRect.HasValue)
        {
            Overlay.Children.Add(CreateRectangle(GetDisplayRect(_draftRect.Value), true, true, dash: true));
        }
    }

    private Border CreateRectangle(Rect rect, bool isSelected, bool isOk, bool dash = false)
    {
        return new Border
        {
            [Canvas.LeftProperty] = rect.X,
            [Canvas.TopProperty] = rect.Y,
            Width = rect.Width,
            Height = rect.Height,
            BorderBrush = isOk ? Brushes.LimeGreen : Brushes.OrangeRed,
            BorderThickness = new Thickness(isSelected ? 3 : 2),
            Background = dash ? new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)) : null,
            CornerRadius = new CornerRadius(1)
        };
    }

    private Rect GetDisplayRect(Rect imageRect)
    {
        var transform = GetImageTransform();
        var scale = transform.scale;
        var offset = transform.offset;
        return new Rect(
            imageRect.X * scale + offset.X,
            imageRect.Y * scale + offset.Y,
            imageRect.Width * scale,
            imageRect.Height * scale);
    }

    private (double scale, Point offset) GetImageTransform()
    {
        if (_bitmap == null) return (1, new Point());
        var canvasSize = Overlay.Bounds.Size;
        var imageSize = new Size(_bitmap.PixelSize.Width, _bitmap.PixelSize.Height);
        var scale = Math.Min(canvasSize.Width / imageSize.Width, canvasSize.Height / imageSize.Height);
        if (double.IsInfinity(scale) || double.IsNaN(scale)) scale = 1;
        var displaySize = imageSize * scale;
        var offset = new Point((canvasSize.Width - displaySize.Width) / 2, (canvasSize.Height - displaySize.Height) / 2);
        return (scale, offset);
    }

    private Rect Normalize(Rect rect)
    {
        rect = ClampToImage(rect);
        return new Rect(
            Math.Min(rect.Left, rect.Right),
            Math.Min(rect.Top, rect.Bottom),
            Math.Abs(rect.Width),
            Math.Abs(rect.Height));
    }

    private Rect ClampToImage(Rect rect)
    {
        if (_bitmap == null) return rect;
        var width = _bitmap.PixelSize.Width;
        var height = _bitmap.PixelSize.Height;
        var x = Math.Clamp(rect.X, 0, width - 1);
        var y = Math.Clamp(rect.Y, 0, height - 1);
        var right = Math.Clamp(rect.Right, 0, width);
        var bottom = Math.Clamp(rect.Bottom, 0, height);
        return new Rect(x, y, right - x, bottom - y);
    }

    private (Rect TopLeft, Rect TopRight, Rect BottomLeft, Rect BottomRight, IEnumerable<Rect> All) GetHandles(Rect rect, double size)
    {
        var hs = size;
        var tl = new Rect(rect.X - hs / 2, rect.Y - hs / 2, hs, hs);
        var tr = new Rect(rect.Right - hs / 2, rect.Y - hs / 2, hs, hs);
        var bl = new Rect(rect.X - hs / 2, rect.Bottom - hs / 2, hs, hs);
        var br = new Rect(rect.Right - hs / 2, rect.Bottom - hs / 2, hs, hs);
        return (tl, tr, bl, br, new[] { tl, tr, bl, br });
    }

    private Rect ResizeWithAnchor(Rect rect, EditMode mode, Point point)
    {
        var x1 = rect.X;
        var y1 = rect.Y;
        var x2 = rect.Right;
        var y2 = rect.Bottom;

        switch (mode)
        {
            case EditMode.ResizeTopLeft:
                x1 = point.X;
                y1 = point.Y;
                break;
            case EditMode.ResizeTopRight:
                x2 = point.X;
                y1 = point.Y;
                break;
            case EditMode.ResizeBottomLeft:
                x1 = point.X;
                y2 = point.Y;
                break;
            case EditMode.ResizeBottomRight:
                x2 = point.X;
                y2 = point.Y;
                break;
        }

        return ClampToImage(new Rect(new Point(x1, y1), new Point(x2, y2)));
    }

    private Point? GetImagePoint(Point canvasPoint)
    {
        if (_bitmap == null) return null;
        var (scale, offset) = GetImageTransform();
        var local = canvasPoint - offset;
        var x = local.X / scale;
        var y = local.Y / scale;
        if (x < 0 || y < 0 || x > _bitmap.PixelSize.Width || y > _bitmap.PixelSize.Height)
        {
            return null;
        }

        return new Point(x, y);
    }

    private void SaveConfig()
    {
        try
        {
            var dir = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var json = JsonSerializer.Serialize(_rois, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch
        {
            // ignore
        }
    }
}

internal enum EditMode
{
    None,
    Creating,
    Moving,
    ResizeTopLeft,
    ResizeTopRight,
    ResizeBottomLeft,
    ResizeBottomRight
}
