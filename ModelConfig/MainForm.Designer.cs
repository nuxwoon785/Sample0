using System.Windows.Forms;

namespace ModelConfig;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        mainLayout = new TableLayoutPanel();
        topBar = new FlowLayoutPanel();
        lblConfigPath = new Label();
        txtConfigPath = new TextBox();
        btnLoad = new Button();
        btnSave = new Button();
        btnBackup = new Button();
        contentLayout = new TableLayoutPanel();
        groupMasters = new GroupBox();
        masterLayout = new TableLayoutPanel();
        lstMasters = new ListBox();
        masterFields = new TableLayoutPanel();
        lblMasterName = new Label();
        txtMasterName = new TextBox();
        lblMasterCode = new Label();
        txtMasterCode = new TextBox();
        lblMasterDescription = new Label();
        txtMasterDescription = new TextBox();
        masterButtons = new FlowLayoutPanel();
        btnAddMaster = new Button();
        btnUpdateMaster = new Button();
        btnDeleteMaster = new Button();
        btnCopyMaster = new Button();
        groupCameras = new GroupBox();
        cameraLayout = new TableLayoutPanel();
        lstCameras = new ListBox();
        cameraFields = new TableLayoutPanel();
        lblCameraName = new Label();
        txtCameraName = new TextBox();
        lblCameraLocation = new Label();
        txtCameraLocation = new TextBox();
        cameraButtons = new FlowLayoutPanel();
        btnAddCamera = new Button();
        btnUpdateCamera = new Button();
        btnDeleteCamera = new Button();
        groupInspections = new GroupBox();
        inspectionLayout = new TableLayoutPanel();
        lstInspections = new ListBox();
        inspectionFields = new TableLayoutPanel();
        lblInspectionName = new Label();
        txtInspectionName = new TextBox();
        lblInspectionType = new Label();
        txtInspectionType = new TextBox();
        lblInspectionNotes = new Label();
        txtInspectionNotes = new TextBox();
        inspectionButtons = new FlowLayoutPanel();
        btnAddInspection = new Button();
        btnUpdateInspection = new Button();
        btnDeleteInspection = new Button();
        mainLayout.SuspendLayout();
        topBar.SuspendLayout();
        contentLayout.SuspendLayout();
        groupMasters.SuspendLayout();
        masterLayout.SuspendLayout();
        masterFields.SuspendLayout();
        masterButtons.SuspendLayout();
        groupCameras.SuspendLayout();
        cameraLayout.SuspendLayout();
        cameraFields.SuspendLayout();
        cameraButtons.SuspendLayout();
        groupInspections.SuspendLayout();
        inspectionLayout.SuspendLayout();
        inspectionFields.SuspendLayout();
        inspectionButtons.SuspendLayout();
        SuspendLayout();
        // 
        // mainLayout
        // 
        mainLayout.ColumnCount = 1;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Controls.Add(topBar, 0, 0);
        mainLayout.Controls.Add(contentLayout, 0, 1);
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.RowCount = 2;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.Padding = new Padding(10);
        // 
        // topBar
        // 
        topBar.AutoSize = true;
        topBar.Controls.Add(lblConfigPath);
        topBar.Controls.Add(txtConfigPath);
        topBar.Controls.Add(btnLoad);
        topBar.Controls.Add(btnSave);
        topBar.Controls.Add(btnBackup);
        topBar.Dock = DockStyle.Fill;
        topBar.Padding = new Padding(0, 0, 0, 5);
        // 
        // lblConfigPath
        // 
        lblConfigPath.Anchor = AnchorStyles.Left;
        lblConfigPath.AutoSize = true;
        lblConfigPath.Text = "설정 파일";
        lblConfigPath.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtConfigPath
        // 
        txtConfigPath.Width = 360;
        txtConfigPath.Anchor = AnchorStyles.Left;
        txtConfigPath.Margin = new Padding(0, 3, 10, 0);
        // 
        // btnLoad
        // 
        btnLoad.Text = "불러오기";
        btnLoad.Margin = new Padding(0, 3, 5, 0);
        btnLoad.Click += btnLoad_Click;
        // 
        // btnSave
        // 
        btnSave.Text = "저장";
        btnSave.Margin = new Padding(0, 3, 5, 0);
        btnSave.Click += btnSave_Click;
        // 
        // btnBackup
        // 
        btnBackup.Text = "백업";
        btnBackup.Margin = new Padding(0, 3, 5, 0);
        btnBackup.Click += btnBackup_Click;
        // 
        // contentLayout
        // 
        contentLayout.ColumnCount = 3;
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
        contentLayout.Controls.Add(groupMasters, 0, 0);
        contentLayout.Controls.Add(groupCameras, 1, 0);
        contentLayout.Controls.Add(groupInspections, 2, 0);
        contentLayout.Dock = DockStyle.Fill;
        contentLayout.RowCount = 1;
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        // 
        // groupMasters
        // 
        groupMasters.Controls.Add(masterLayout);
        groupMasters.Dock = DockStyle.Fill;
        groupMasters.Text = "마스터 관리";
        // 
        // masterLayout
        // 
        masterLayout.ColumnCount = 1;
        masterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        masterLayout.Controls.Add(lstMasters, 0, 0);
        masterLayout.Controls.Add(masterFields, 0, 1);
        masterLayout.Controls.Add(masterButtons, 0, 2);
        masterLayout.Dock = DockStyle.Fill;
        masterLayout.RowCount = 3;
        masterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        masterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        masterLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        masterLayout.Padding = new Padding(8);
        // 
        // lstMasters
        // 
        lstMasters.Dock = DockStyle.Fill;
        lstMasters.SelectedIndexChanged += lstMasters_SelectedIndexChanged;
        // 
        // masterFields
        // 
        masterFields.ColumnCount = 2;
        masterFields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        masterFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        masterFields.Controls.Add(lblMasterName, 0, 0);
        masterFields.Controls.Add(txtMasterName, 1, 0);
        masterFields.Controls.Add(lblMasterCode, 0, 1);
        masterFields.Controls.Add(txtMasterCode, 1, 1);
        masterFields.Controls.Add(lblMasterDescription, 0, 2);
        masterFields.Controls.Add(txtMasterDescription, 1, 2);
        masterFields.Dock = DockStyle.Fill;
        masterFields.RowCount = 3;
        masterFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        masterFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        masterFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        masterFields.Padding = new Padding(0, 8, 0, 4);
        masterFields.AutoSize = true;
        // 
        // lblMasterName
        // 
        lblMasterName.AutoSize = true;
        lblMasterName.Text = "마스터";
        lblMasterName.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtMasterName
        // 
        txtMasterName.Dock = DockStyle.Fill;
        txtMasterName.Margin = new Padding(0, 3, 0, 3);
        // 
        // lblMasterCode
        // 
        lblMasterCode.AutoSize = true;
        lblMasterCode.Text = "기종 코드";
        lblMasterCode.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtMasterCode
        // 
        txtMasterCode.Dock = DockStyle.Fill;
        txtMasterCode.Margin = new Padding(0, 3, 0, 3);
        // 
        // lblMasterDescription
        // 
        lblMasterDescription.AutoSize = true;
        lblMasterDescription.Text = "설명";
        lblMasterDescription.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtMasterDescription
        // 
        txtMasterDescription.Dock = DockStyle.Fill;
        txtMasterDescription.Margin = new Padding(0, 3, 0, 3);
        txtMasterDescription.Multiline = true;
        txtMasterDescription.Height = 60;
        // 
        // masterButtons
        // 
        masterButtons.AutoSize = true;
        masterButtons.Controls.Add(btnAddMaster);
        masterButtons.Controls.Add(btnUpdateMaster);
        masterButtons.Controls.Add(btnDeleteMaster);
        masterButtons.Controls.Add(btnCopyMaster);
        masterButtons.Dock = DockStyle.Fill;
        masterButtons.FlowDirection = FlowDirection.LeftToRight;
        masterButtons.Padding = new Padding(0, 4, 0, 0);
        // 
        // btnAddMaster
        // 
        btnAddMaster.Text = "추가";
        btnAddMaster.Margin = new Padding(0, 3, 5, 0);
        btnAddMaster.Click += btnAddMaster_Click;
        // 
        // btnUpdateMaster
        // 
        btnUpdateMaster.Text = "수정";
        btnUpdateMaster.Margin = new Padding(0, 3, 5, 0);
        btnUpdateMaster.Click += btnUpdateMaster_Click;
        // 
        // btnDeleteMaster
        // 
        btnDeleteMaster.Text = "삭제";
        btnDeleteMaster.Margin = new Padding(0, 3, 5, 0);
        btnDeleteMaster.Click += btnDeleteMaster_Click;
        // 
        // btnCopyMaster
        // 
        btnCopyMaster.Text = "복사";
        btnCopyMaster.Margin = new Padding(0, 3, 0, 0);
        btnCopyMaster.Click += btnCopyMaster_Click;
        // 
        // groupCameras
        // 
        groupCameras.Controls.Add(cameraLayout);
        groupCameras.Dock = DockStyle.Fill;
        groupCameras.Text = "카메라 관리";
        // 
        // cameraLayout
        // 
        cameraLayout.ColumnCount = 1;
        cameraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        cameraLayout.Controls.Add(lstCameras, 0, 0);
        cameraLayout.Controls.Add(cameraFields, 0, 1);
        cameraLayout.Controls.Add(cameraButtons, 0, 2);
        cameraLayout.Dock = DockStyle.Fill;
        cameraLayout.RowCount = 3;
        cameraLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        cameraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cameraLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cameraLayout.Padding = new Padding(8);
        // 
        // lstCameras
        // 
        lstCameras.Dock = DockStyle.Fill;
        lstCameras.SelectedIndexChanged += lstCameras_SelectedIndexChanged;
        // 
        // cameraFields
        // 
        cameraFields.ColumnCount = 2;
        cameraFields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        cameraFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        cameraFields.Controls.Add(lblCameraName, 0, 0);
        cameraFields.Controls.Add(txtCameraName, 1, 0);
        cameraFields.Controls.Add(lblCameraLocation, 0, 1);
        cameraFields.Controls.Add(txtCameraLocation, 1, 1);
        cameraFields.Dock = DockStyle.Fill;
        cameraFields.RowCount = 2;
        cameraFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cameraFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cameraFields.Padding = new Padding(0, 8, 0, 4);
        cameraFields.AutoSize = true;
        // 
        // lblCameraName
        // 
        lblCameraName.AutoSize = true;
        lblCameraName.Text = "카메라";
        lblCameraName.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtCameraName
        // 
        txtCameraName.Dock = DockStyle.Fill;
        txtCameraName.Margin = new Padding(0, 3, 0, 3);
        // 
        // lblCameraLocation
        // 
        lblCameraLocation.AutoSize = true;
        lblCameraLocation.Text = "위치/용도";
        lblCameraLocation.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtCameraLocation
        // 
        txtCameraLocation.Dock = DockStyle.Fill;
        txtCameraLocation.Margin = new Padding(0, 3, 0, 3);
        // 
        // cameraButtons
        // 
        cameraButtons.AutoSize = true;
        cameraButtons.Controls.Add(btnAddCamera);
        cameraButtons.Controls.Add(btnUpdateCamera);
        cameraButtons.Controls.Add(btnDeleteCamera);
        cameraButtons.Dock = DockStyle.Fill;
        cameraButtons.FlowDirection = FlowDirection.LeftToRight;
        cameraButtons.Padding = new Padding(0, 4, 0, 0);
        // 
        // btnAddCamera
        // 
        btnAddCamera.Text = "추가";
        btnAddCamera.Margin = new Padding(0, 3, 5, 0);
        btnAddCamera.Click += btnAddCamera_Click;
        // 
        // btnUpdateCamera
        // 
        btnUpdateCamera.Text = "수정";
        btnUpdateCamera.Margin = new Padding(0, 3, 5, 0);
        btnUpdateCamera.Click += btnUpdateCamera_Click;
        // 
        // btnDeleteCamera
        // 
        btnDeleteCamera.Text = "삭제";
        btnDeleteCamera.Margin = new Padding(0, 3, 0, 0);
        btnDeleteCamera.Click += btnDeleteCamera_Click;
        // 
        // groupInspections
        // 
        groupInspections.Controls.Add(inspectionLayout);
        groupInspections.Dock = DockStyle.Fill;
        groupInspections.Text = "검사 관리";
        // 
        // inspectionLayout
        // 
        inspectionLayout.ColumnCount = 1;
        inspectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        inspectionLayout.Controls.Add(lstInspections, 0, 0);
        inspectionLayout.Controls.Add(inspectionFields, 0, 1);
        inspectionLayout.Controls.Add(inspectionButtons, 0, 2);
        inspectionLayout.Dock = DockStyle.Fill;
        inspectionLayout.RowCount = 3;
        inspectionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        inspectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inspectionLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inspectionLayout.Padding = new Padding(8);
        // 
        // lstInspections
        // 
        lstInspections.Dock = DockStyle.Fill;
        lstInspections.SelectedIndexChanged += lstInspections_SelectedIndexChanged;
        // 
        // inspectionFields
        // 
        inspectionFields.ColumnCount = 2;
        inspectionFields.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inspectionFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        inspectionFields.Controls.Add(lblInspectionName, 0, 0);
        inspectionFields.Controls.Add(txtInspectionName, 1, 0);
        inspectionFields.Controls.Add(lblInspectionType, 0, 1);
        inspectionFields.Controls.Add(txtInspectionType, 1, 1);
        inspectionFields.Controls.Add(lblInspectionNotes, 0, 2);
        inspectionFields.Controls.Add(txtInspectionNotes, 1, 2);
        inspectionFields.Dock = DockStyle.Fill;
        inspectionFields.RowCount = 3;
        inspectionFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inspectionFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inspectionFields.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inspectionFields.Padding = new Padding(0, 8, 0, 4);
        inspectionFields.AutoSize = true;
        // 
        // lblInspectionName
        // 
        lblInspectionName.AutoSize = true;
        lblInspectionName.Text = "검사명";
        lblInspectionName.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtInspectionName
        // 
        txtInspectionName.Dock = DockStyle.Fill;
        txtInspectionName.Margin = new Padding(0, 3, 0, 3);
        // 
        // lblInspectionType
        // 
        lblInspectionType.AutoSize = true;
        lblInspectionType.Text = "검사유형/기종코드";
        lblInspectionType.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtInspectionType
        // 
        txtInspectionType.Dock = DockStyle.Fill;
        txtInspectionType.Margin = new Padding(0, 3, 0, 3);
        // 
        // lblInspectionNotes
        // 
        lblInspectionNotes.AutoSize = true;
        lblInspectionNotes.Text = "비고";
        lblInspectionNotes.Margin = new Padding(0, 6, 5, 0);
        // 
        // txtInspectionNotes
        // 
        txtInspectionNotes.Dock = DockStyle.Fill;
        txtInspectionNotes.Margin = new Padding(0, 3, 0, 3);
        txtInspectionNotes.Multiline = true;
        txtInspectionNotes.Height = 60;
        // 
        // inspectionButtons
        // 
        inspectionButtons.AutoSize = true;
        inspectionButtons.Controls.Add(btnAddInspection);
        inspectionButtons.Controls.Add(btnUpdateInspection);
        inspectionButtons.Controls.Add(btnDeleteInspection);
        inspectionButtons.Dock = DockStyle.Fill;
        inspectionButtons.FlowDirection = FlowDirection.LeftToRight;
        inspectionButtons.Padding = new Padding(0, 4, 0, 0);
        // 
        // btnAddInspection
        // 
        btnAddInspection.Text = "추가";
        btnAddInspection.Margin = new Padding(0, 3, 5, 0);
        btnAddInspection.Click += btnAddInspection_Click;
        // 
        // btnUpdateInspection
        // 
        btnUpdateInspection.Text = "수정";
        btnUpdateInspection.Margin = new Padding(0, 3, 5, 0);
        btnUpdateInspection.Click += btnUpdateInspection_Click;
        // 
        // btnDeleteInspection
        // 
        btnDeleteInspection.Text = "삭제";
        btnDeleteInspection.Margin = new Padding(0, 3, 0, 0);
        btnDeleteInspection.Click += btnDeleteInspection_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1200, 720);
        Controls.Add(mainLayout);
        MinimumSize = new System.Drawing.Size(1000, 600);
        Name = "MainForm";
        Text = "ModelConfig - 비전 설정";
        Load += MainForm_Load;
        mainLayout.ResumeLayout(false);
        mainLayout.PerformLayout();
        topBar.ResumeLayout(false);
        topBar.PerformLayout();
        contentLayout.ResumeLayout(false);
        groupMasters.ResumeLayout(false);
        masterLayout.ResumeLayout(false);
        masterLayout.PerformLayout();
        masterFields.ResumeLayout(false);
        masterFields.PerformLayout();
        masterButtons.ResumeLayout(false);
        masterButtons.PerformLayout();
        groupCameras.ResumeLayout(false);
        cameraLayout.ResumeLayout(false);
        cameraLayout.PerformLayout();
        cameraFields.ResumeLayout(false);
        cameraFields.PerformLayout();
        cameraButtons.ResumeLayout(false);
        cameraButtons.PerformLayout();
        groupInspections.ResumeLayout(false);
        inspectionLayout.ResumeLayout(false);
        inspectionLayout.PerformLayout();
        inspectionFields.ResumeLayout(false);
        inspectionFields.PerformLayout();
        inspectionButtons.ResumeLayout(false);
        inspectionButtons.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel mainLayout;
    private FlowLayoutPanel topBar;
    private Label lblConfigPath;
    private TextBox txtConfigPath;
    private Button btnLoad;
    private Button btnSave;
    private Button btnBackup;
    private TableLayoutPanel contentLayout;
    private GroupBox groupMasters;
    private TableLayoutPanel masterLayout;
    private ListBox lstMasters;
    private TableLayoutPanel masterFields;
    private Label lblMasterName;
    private TextBox txtMasterName;
    private Label lblMasterCode;
    private TextBox txtMasterCode;
    private Label lblMasterDescription;
    private TextBox txtMasterDescription;
    private FlowLayoutPanel masterButtons;
    private Button btnAddMaster;
    private Button btnUpdateMaster;
    private Button btnDeleteMaster;
    private Button btnCopyMaster;
    private GroupBox groupCameras;
    private TableLayoutPanel cameraLayout;
    private ListBox lstCameras;
    private TableLayoutPanel cameraFields;
    private Label lblCameraName;
    private TextBox txtCameraName;
    private Label lblCameraLocation;
    private TextBox txtCameraLocation;
    private FlowLayoutPanel cameraButtons;
    private Button btnAddCamera;
    private Button btnUpdateCamera;
    private Button btnDeleteCamera;
    private GroupBox groupInspections;
    private TableLayoutPanel inspectionLayout;
    private ListBox lstInspections;
    private TableLayoutPanel inspectionFields;
    private Label lblInspectionName;
    private TextBox txtInspectionName;
    private Label lblInspectionType;
    private TextBox txtInspectionType;
    private Label lblInspectionNotes;
    private TextBox txtInspectionNotes;
    private FlowLayoutPanel inspectionButtons;
    private Button btnAddInspection;
    private Button btnUpdateInspection;
    private Button btnDeleteInspection;
}
