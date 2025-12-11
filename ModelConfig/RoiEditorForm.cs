using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ModelConfig.Controls;
using ModelConfig.Models;
using ModelConfig.Services;

namespace ModelConfig;

public partial class RoiEditorForm : Form
{
    private readonly BindingList<RoiAnnotation> _rois = new();
    private readonly RoiExportService _exportService = new();
    private string? _imagePath;

    public RoiEditorForm()
    {
        InitializeComponent();
        lstRois.DataSource = _rois;
        lstRois.DisplayMember = nameof(RoiAnnotation.Name);
        canvas.Rois = _rois;
        canvas.RoiListChanged += Canvas_RoiListChanged;
        canvas.SelectedRoiChanged += Canvas_SelectedRoiChanged;
        cmbClassification.DataSource = Enum.GetValues(typeof(RoiClassification));
    }

    private void Canvas_SelectedRoiChanged(object? sender, EventArgs e)
    {
        UpdateSelectionFields(canvas.SelectedRoi);
    }

    private void Canvas_RoiListChanged(object? sender, EventArgs e)
    {
        lstRois.Refresh();
        UpdateSelectionFields(canvas.SelectedRoi);
    }

    private void btnLoadImage_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _imagePath = dialog.FileName;
            txtImagePath.Text = _imagePath;
            using var bitmap = new Bitmap(_imagePath);
            canvas.ImageBitmap = new Bitmap(bitmap);
        }
    }

    private void lstRois_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstRois.SelectedItem is RoiAnnotation roi)
        {
            canvas.SelectRoi(roi);
        }
    }

    private void btnDeleteRoi_Click(object sender, EventArgs e)
    {
        canvas.RemoveSelected();
    }

    private async void btnExport_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_imagePath))
        {
            MessageBox.Show("먼저 이미지를 불러와 주세요.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
        {
            MessageBox.Show("출력 경로를 입력해 주세요.");
            return;
        }

        if (!_rois.Any())
        {
            MessageBox.Show("내보낼 ROI가 없습니다.");
            return;
        }

        try
        {
            await _exportService.ExportCropsAsync(_rois, _imagePath, txtOutputPath.Text);
            await _exportService.SaveConfigurationAsync(_rois, txtOutputPath.Text);
            MessageBox.Show("ROI를 저장하고 분류 폴더로 내보냈습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"저장 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnBrowseOutput_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtOutputPath.Text = dialog.SelectedPath;
        }
    }

    private void UpdateSelectionFields(RoiAnnotation? roi)
    {
        if (roi == null)
        {
            txtRoiName.Text = string.Empty;
            cmbClassification.SelectedIndex = -1;
            return;
        }

        txtRoiName.Text = roi.Name;
        cmbClassification.SelectedItem = roi.Classification;
    }

    private void txtRoiName_TextChanged(object sender, EventArgs e)
    {
        if (canvas.SelectedRoi != null && !string.IsNullOrWhiteSpace(txtRoiName.Text))
        {
            canvas.SelectedRoi.Name = txtRoiName.Text.Trim();
            lstRois.Refresh();
        }
    }

    private void cmbClassification_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (canvas.SelectedRoi != null && cmbClassification.SelectedItem is RoiClassification classification)
        {
            canvas.SelectedRoi.Classification = classification;
            canvas.Invalidate();
        }
    }
}
