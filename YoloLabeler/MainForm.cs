using System.Drawing;
using System.Windows.Forms;
using YoloLabeler.Controls;
using YoloLabeler.Models;
using YoloLabeler.Services;

namespace YoloLabeler;

public partial class MainForm : Form
{
    private readonly AnnotationService _annotationService = new();
    private readonly ClassCatalog _classCatalog = new();
    private readonly List<string> _images = new();
    private string? _folder;
    private int _currentIndex;

    public MainForm()
    {
        InitializeComponent();
        KeyPreview = true;
        imageCanvas.SelectionChanged += ImageCanvasOnSelectionChanged;
        imageCanvas.DraftCompleted += ImageCanvasOnDraftCompleted;
    }

    private void ImageCanvasOnDraftCompleted(object? sender, EventArgs e)
    {
        var classId = classSelector.SelectedIndex >= 0 ? classSelector.SelectedIndex : -1;
        imageCanvas.SetSelectionClass(classId);
        RefreshAnnotationList();
    }

    private void ImageCanvasOnSelectionChanged(object? sender, Annotation? e)
    {
        if (e != null && e.ClassId >= 0 && e.ClassId < classSelector.Items.Count)
        {
            classSelector.SelectedIndex = e.ClassId;
        }
        else
        {
            classSelector.SelectedIndex = classSelector.Items.Count > 0 ? 0 : -1;
        }

        RefreshAnnotationList();
    }

    private void addClassButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(classNameInput.Text) || string.IsNullOrWhiteSpace(_folder))
        {
            return;
        }

        var classId = _classCatalog.EnsureClass(_folder, classNameInput.Text.Trim());
        UpdateClassSelector();
        classSelector.SelectedIndex = classId;
        classNameInput.Clear();
    }

    private void openFolderButton_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            LoadFolder(dialog.SelectedPath);
        }
    }

    private void LoadFolder(string path)
    {
        _folder = path;
        _images.Clear();
        _images.AddRange(Directory.GetFiles(path)
            .Where(f => IsImage(f))
            .OrderBy(f => f));

        _classCatalog.Load(path);
        UpdateClassSelector();

        _currentIndex = 0;
        LoadCurrentImage();
    }

    private void UpdateClassSelector()
    {
        classSelector.Items.Clear();
        foreach (var c in _classCatalog.Classes)
        {
            classSelector.Items.Add(c);
        }

        if (classSelector.Items.Count > 0)
        {
            classSelector.SelectedIndex = 0;
        }
    }

    private void LoadCurrentImage()
    {
        if (_images.Count == 0 || string.IsNullOrEmpty(_folder))
        {
            imageCanvas.CanvasImage = null;
            annotationList.Items.Clear();
            fileLabel.Text = "No images loaded";
            return;
        }

        _currentIndex = Math.Clamp(_currentIndex, 0, _images.Count - 1);
        var path = _images[_currentIndex];
        fileLabel.Text = $"{Path.GetFileName(path)} ({_currentIndex + 1}/{_images.Count})";
        using var stream = File.OpenRead(path);
        imageCanvas.CanvasImage = Image.FromStream(stream);

        var annotations = _annotationService.LoadAnnotations(path, imageCanvas.CanvasImage.Width, imageCanvas.CanvasImage.Height);
        imageCanvas.SetAnnotations(annotations);
        RefreshAnnotationList();
    }

    private void RefreshAnnotationList()
    {
        annotationList.Items.Clear();
        foreach (var annotation in imageCanvas.Annotations)
        {
            annotationList.Items.Add(DisplayText(annotation));
        }
    }

    private string DisplayText(Annotation annotation)
    {
        var cls = annotation.ClassId >= 0 ? _classCatalog.GetClass(annotation.ClassId) : "Unassigned";
        return $"{cls} @ ({annotation.Bounds.X:0}, {annotation.Bounds.Y:0}) {annotation.Bounds.Width:0}x{annotation.Bounds.Height:0}";
    }

    private bool IsImage(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".png" or ".jpg" or ".jpeg" or ".bmp";
    }

    private void previousButton_Click(object sender, EventArgs e)
    {
        _currentIndex--;
        LoadCurrentImage();
    }

    private void nextButton_Click(object sender, EventArgs e)
    {
        _currentIndex++;
        LoadCurrentImage();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        if (_images.Count == 0)
        {
            return;
        }

        var path = _images[_currentIndex];
        if (imageCanvas.CanvasImage != null)
        {
            _annotationService.SaveAnnotations(path, imageCanvas.CanvasImage.Width, imageCanvas.CanvasImage.Height, imageCanvas.Annotations);
        }
    }

    private void classSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (classSelector.SelectedIndex >= 0)
        {
            imageCanvas.SetSelectionClass(classSelector.SelectedIndex);
            RefreshAnnotationList();
        }
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete)
        {
            imageCanvas.DeleteSelected();
            RefreshAnnotationList();
        }
    }

    private void resetViewButton_Click(object sender, EventArgs e)
    {
        imageCanvas.ResetView();
    }
}
