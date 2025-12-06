using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using ModelConfig.Models;
using ModelConfig.Services;

namespace ModelConfig;

public partial class MainForm : Form
{
    private readonly ConfigStorage _storage = new();
    private ProjectConfiguration _configuration = new();
    private readonly string _defaultConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");
    private readonly string _backupDirectory = Path.Combine(AppContext.BaseDirectory, "backups");

    public MainForm()
    {
        InitializeComponent();
        txtConfigPath.Text = _defaultConfigPath;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await LoadConfigurationAsync();
    }

    private async Task LoadConfigurationAsync()
    {
        try
        {
            _configuration = await _storage.LoadAsync(CurrentConfigPath);
            BindMasters();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"설정 파일을 불러오는 중 오류가 발생했습니다.\n{ex.Message}", "로드 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SaveConfigurationAsync()
    {
        try
        {
            await _storage.SaveAsync(_configuration, CurrentConfigPath);
            MessageBox.Show("설정을 저장했습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"설정 파일을 저장하는 중 오류가 발생했습니다.\n{ex.Message}", "저장 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string CurrentConfigPath => string.IsNullOrWhiteSpace(txtConfigPath.Text)
        ? _defaultConfigPath
        : txtConfigPath.Text;

    private MasterConfiguration? SelectedMaster => lstMasters.SelectedItem as MasterConfiguration;

    private CameraConfiguration? SelectedCamera => lstCameras.SelectedItem as CameraConfiguration;

    private InspectionConfiguration? SelectedInspection => lstInspections.SelectedItem as InspectionConfiguration;

    private void BindMasters()
    {
        lstMasters.DataSource = null;
        lstMasters.DataSource = _configuration.Masters;
        lstMasters.DisplayMember = nameof(MasterConfiguration.Name);
        DisplayMasterDetails();
    }

    private void BindCameras(MasterConfiguration? master)
    {
        lstCameras.DataSource = null;
        if (master != null)
        {
            lstCameras.DataSource = master.Cameras;
        }

        lstCameras.DisplayMember = nameof(CameraConfiguration.Name);
        DisplayCameraDetails();
    }

    private void BindInspections(CameraConfiguration? camera)
    {
        lstInspections.DataSource = null;
        if (camera != null)
        {
            lstInspections.DataSource = camera.Inspections;
        }

        lstInspections.DisplayMember = nameof(InspectionConfiguration.Name);
        DisplayInspectionDetails();
    }

    private void DisplayMasterDetails()
    {
        var master = SelectedMaster;
        if (master == null)
        {
            txtMasterName.Text = string.Empty;
            txtMasterCode.Text = string.Empty;
            txtMasterDescription.Text = string.Empty;
            BindCameras(null);
            return;
        }

        txtMasterName.Text = master.Name;
        txtMasterCode.Text = master.ModelCode;
        txtMasterDescription.Text = master.Description;
        BindCameras(master);
    }

    private void DisplayCameraDetails()
    {
        var camera = SelectedCamera;
        if (camera == null)
        {
            txtCameraName.Text = string.Empty;
            txtCameraLocation.Text = string.Empty;
            BindInspections(null);
            return;
        }

        txtCameraName.Text = camera.Name;
        txtCameraLocation.Text = camera.Location;
        BindInspections(camera);
    }

    private void DisplayInspectionDetails()
    {
        var inspection = SelectedInspection;
        if (inspection == null)
        {
            txtInspectionName.Text = string.Empty;
            txtInspectionType.Text = string.Empty;
            txtInspectionNotes.Text = string.Empty;
            return;
        }

        txtInspectionName.Text = inspection.Name;
        txtInspectionType.Text = inspection.InspectionType;
        txtInspectionNotes.Text = inspection.Notes;
    }

    private void btnAddMaster_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMasterName.Text))
        {
            MessageBox.Show("마스터 이름을 입력해 주세요.");
            return;
        }

        var master = new MasterConfiguration
        {
            Name = txtMasterName.Text.Trim(),
            ModelCode = txtMasterCode.Text.Trim(),
            Description = txtMasterDescription.Text.Trim()
        };

        _configuration.Masters.Add(master);
        BindMasters();
        lstMasters.SelectedItem = master;
    }

    private void btnUpdateMaster_Click(object sender, EventArgs e)
    {
        var master = SelectedMaster;
        if (master == null)
        {
            MessageBox.Show("수정할 마스터를 선택해 주세요.");
            return;
        }

        master.Name = txtMasterName.Text.Trim();
        master.ModelCode = txtMasterCode.Text.Trim();
        master.Description = txtMasterDescription.Text.Trim();
        BindMasters();
        lstMasters.SelectedItem = master;
    }

    private void btnDeleteMaster_Click(object sender, EventArgs e)
    {
        var master = SelectedMaster;
        if (master == null)
        {
            MessageBox.Show("삭제할 마스터를 선택해 주세요.");
            return;
        }

        _configuration.Masters.Remove(master);
        BindMasters();
    }

    private void btnCopyMaster_Click(object sender, EventArgs e)
    {
        var master = SelectedMaster;
        if (master == null)
        {
            MessageBox.Show("복사할 마스터를 선택해 주세요.");
            return;
        }

        var newModelCode = Interaction.InputBox("신규 기종 코드(검사유형)를 입력하세요.", "마스터 복사", master.ModelCode);
        if (string.IsNullOrWhiteSpace(newModelCode))
        {
            return;
        }

        var newName = $"{master.Name} ({newModelCode})";
        var clone = master.CloneWithNewModel(newModelCode.Trim(), newName);
        _configuration.Masters.Add(clone);
        BindMasters();
        lstMasters.SelectedItem = clone;
    }

    private void btnAddCamera_Click(object sender, EventArgs e)
    {
        var master = SelectedMaster;
        if (master == null)
        {
            MessageBox.Show("카메라를 추가할 마스터를 선택해 주세요.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtCameraName.Text))
        {
            MessageBox.Show("카메라 이름을 입력해 주세요.");
            return;
        }

        var camera = new CameraConfiguration
        {
            Name = txtCameraName.Text.Trim(),
            Location = txtCameraLocation.Text.Trim()
        };

        master.Cameras.Add(camera);
        BindCameras(master);
        lstCameras.SelectedItem = camera;
    }

    private void btnUpdateCamera_Click(object sender, EventArgs e)
    {
        var camera = SelectedCamera;
        var master = SelectedMaster;
        if (master == null || camera == null)
        {
            MessageBox.Show("수정할 카메라를 선택해 주세요.");
            return;
        }

        camera.Name = txtCameraName.Text.Trim();
        camera.Location = txtCameraLocation.Text.Trim();
        BindCameras(master);
        lstCameras.SelectedItem = camera;
    }

    private void btnDeleteCamera_Click(object sender, EventArgs e)
    {
        var master = SelectedMaster;
        var camera = SelectedCamera;
        if (master == null || camera == null)
        {
            MessageBox.Show("삭제할 카메라를 선택해 주세요.");
            return;
        }

        master.Cameras.Remove(camera);
        BindCameras(master);
    }

    private void btnAddInspection_Click(object sender, EventArgs e)
    {
        var camera = SelectedCamera;
        if (camera == null)
        {
            MessageBox.Show("검사를 추가할 카메라를 선택해 주세요.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtInspectionName.Text))
        {
            MessageBox.Show("검사명을 입력해 주세요.");
            return;
        }

        var inspection = new InspectionConfiguration
        {
            Name = txtInspectionName.Text.Trim(),
            InspectionType = txtInspectionType.Text.Trim(),
            Notes = txtInspectionNotes.Text.Trim()
        };

        camera.Inspections.Add(inspection);
        BindInspections(camera);
        lstInspections.SelectedItem = inspection;
    }

    private void btnUpdateInspection_Click(object sender, EventArgs e)
    {
        var inspection = SelectedInspection;
        var camera = SelectedCamera;
        if (camera == null || inspection == null)
        {
            MessageBox.Show("수정할 검사를 선택해 주세요.");
            return;
        }

        inspection.Name = txtInspectionName.Text.Trim();
        inspection.InspectionType = txtInspectionType.Text.Trim();
        inspection.Notes = txtInspectionNotes.Text.Trim();
        BindInspections(camera);
        lstInspections.SelectedItem = inspection;
    }

    private void btnDeleteInspection_Click(object sender, EventArgs e)
    {
        var inspection = SelectedInspection;
        var camera = SelectedCamera;
        if (camera == null || inspection == null)
        {
            MessageBox.Show("삭제할 검사를 선택해 주세요.");
            return;
        }

        camera.Inspections.Remove(inspection);
        BindInspections(camera);
    }

    private async void btnSave_Click(object sender, EventArgs e)
    {
        await SaveConfigurationAsync();
    }

    private async void btnLoad_Click(object sender, EventArgs e)
    {
        await LoadConfigurationAsync();
    }

    private async void btnBackup_Click(object sender, EventArgs e)
    {
        try
        {
            var backupPath = await _storage.BackupAsync(CurrentConfigPath, _backupDirectory);
            MessageBox.Show($"백업을 생성했습니다:\n{backupPath}", "백업 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"백업 중 오류가 발생했습니다.\n{ex.Message}", "백업 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void lstMasters_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisplayMasterDetails();
    }

    private void lstCameras_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisplayCameraDetails();
    }

    private void lstInspections_SelectedIndexChanged(object sender, EventArgs e)
    {
        DisplayInspectionDetails();
    }
}
