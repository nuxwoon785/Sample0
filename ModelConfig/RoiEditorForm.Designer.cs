using System.Windows.Forms;
using ModelConfig.Controls;

namespace ModelConfig;

partial class RoiEditorForm
{
    private System.ComponentModel.IContainer components = null!;
    private TableLayoutPanel mainLayout;
    private FlowLayoutPanel toolbar;
    private Button btnLoadImage;
    private TextBox txtImagePath;
    private Label lblOutput;
    private TextBox txtOutputPath;
    private Button btnBrowseOutput;
    private Button btnExport;
    private TableLayoutPanel contentLayout;
    private RoiCanvas canvas;
    private GroupBox groupRoiList;
    private ListBox lstRois;
    private TableLayoutPanel detailsLayout;
    private Label lblName;
    private TextBox txtRoiName;
    private Label lblClassification;
    private ComboBox cmbClassification;
    private Button btnDeleteRoi;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        mainLayout = new TableLayoutPanel();
        toolbar = new FlowLayoutPanel();
        btnLoadImage = new Button();
        txtImagePath = new TextBox();
        lblOutput = new Label();
        txtOutputPath = new TextBox();
        btnBrowseOutput = new Button();
        btnExport = new Button();
        contentLayout = new TableLayoutPanel();
        canvas = new RoiCanvas();
        groupRoiList = new GroupBox();
        detailsLayout = new TableLayoutPanel();
        lstRois = new ListBox();
        lblName = new Label();
        txtRoiName = new TextBox();
        lblClassification = new Label();
        cmbClassification = new ComboBox();
        btnDeleteRoi = new Button();
        mainLayout.SuspendLayout();
        toolbar.SuspendLayout();
        contentLayout.SuspendLayout();
        groupRoiList.SuspendLayout();
        detailsLayout.SuspendLayout();
        SuspendLayout();
        //
        // mainLayout
        //
        mainLayout.ColumnCount = 1;
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainLayout.Dock = DockStyle.Fill;
        mainLayout.RowCount = 2;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainLayout.Controls.Add(toolbar, 0, 0);
        mainLayout.Controls.Add(contentLayout, 0, 1);
        //
        // toolbar
        //
        toolbar.AutoSize = true;
        toolbar.Dock = DockStyle.Fill;
        toolbar.Padding = new Padding(10, 10, 10, 5);
        toolbar.Controls.Add(btnLoadImage);
        toolbar.Controls.Add(txtImagePath);
        toolbar.Controls.Add(lblOutput);
        toolbar.Controls.Add(txtOutputPath);
        toolbar.Controls.Add(btnBrowseOutput);
        toolbar.Controls.Add(btnExport);
        //
        // btnLoadImage
        //
        btnLoadImage.Text = "이미지 불러오기";
        btnLoadImage.AutoSize = true;
        btnLoadImage.Click += btnLoadImage_Click;
        //
        // txtImagePath
        //
        txtImagePath.Width = 300;
        txtImagePath.ReadOnly = true;
        txtImagePath.Margin = new Padding(5, 3, 10, 3);
        //
        // lblOutput
        //
        lblOutput.Text = "출력 경로:";
        lblOutput.AutoSize = true;
        lblOutput.Margin = new Padding(0, 8, 5, 0);
        //
        // txtOutputPath
        //
        txtOutputPath.Width = 220;
        txtOutputPath.Margin = new Padding(0, 3, 5, 3);
        //
        // btnBrowseOutput
        //
        btnBrowseOutput.Text = "찾아보기";
        btnBrowseOutput.AutoSize = true;
        btnBrowseOutput.Click += btnBrowseOutput_Click;
        //
        // btnExport
        //
        btnExport.Text = "ROI 저장/내보내기";
        btnExport.AutoSize = true;
        btnExport.Margin = new Padding(10, 3, 0, 3);
        btnExport.Click += btnExport_Click;
        //
        // contentLayout
        //
        contentLayout.ColumnCount = 2;
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        contentLayout.Dock = DockStyle.Fill;
        contentLayout.RowCount = 1;
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        contentLayout.Controls.Add(canvas, 0, 0);
        contentLayout.Controls.Add(groupRoiList, 1, 0);
        //
        // canvas
        //
        canvas.Dock = DockStyle.Fill;
        canvas.BorderStyle = BorderStyle.FixedSingle;
        //
        // groupRoiList
        //
        groupRoiList.Text = "ROI 목록";
        groupRoiList.Dock = DockStyle.Fill;
        groupRoiList.Controls.Add(detailsLayout);
        //
        // detailsLayout
        //
        detailsLayout.ColumnCount = 2;
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        detailsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        detailsLayout.Dock = DockStyle.Fill;
        detailsLayout.Padding = new Padding(8);
        detailsLayout.RowCount = 4;
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        detailsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        detailsLayout.Controls.Add(lstRois, 0, 0);
        detailsLayout.SetColumnSpan(lstRois, 2);
        detailsLayout.Controls.Add(lblName, 0, 1);
        detailsLayout.Controls.Add(txtRoiName, 1, 1);
        detailsLayout.Controls.Add(lblClassification, 0, 2);
        detailsLayout.Controls.Add(cmbClassification, 1, 2);
        detailsLayout.Controls.Add(btnDeleteRoi, 1, 3);
        //
        // lstRois
        //
        lstRois.Dock = DockStyle.Fill;
        lstRois.Height = 200;
        lstRois.SelectedIndexChanged += lstRois_SelectedIndexChanged;
        //
        // lblName
        //
        lblName.Text = "이름";
        lblName.AutoSize = true;
        lblName.Margin = new Padding(0, 8, 5, 0);
        //
        // txtRoiName
        //
        txtRoiName.Dock = DockStyle.Fill;
        txtRoiName.TextChanged += txtRoiName_TextChanged;
        //
        // lblClassification
        //
        lblClassification.Text = "분류";
        lblClassification.AutoSize = true;
        lblClassification.Margin = new Padding(0, 8, 5, 0);
        //
        // cmbClassification
        //
        cmbClassification.Dock = DockStyle.Fill;
        cmbClassification.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbClassification.SelectedIndexChanged += cmbClassification_SelectedIndexChanged;
        //
        // btnDeleteRoi
        //
        btnDeleteRoi.Text = "선택 삭제";
        btnDeleteRoi.AutoSize = true;
        btnDeleteRoi.Anchor = AnchorStyles.Right;
        btnDeleteRoi.Margin = new Padding(0, 8, 0, 0);
        btnDeleteRoi.Click += btnDeleteRoi_Click;
        //
        // RoiEditorForm
        //
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1100, 700);
        Controls.Add(mainLayout);
        Text = "ROI 편집기";
        mainLayout.ResumeLayout(false);
        mainLayout.PerformLayout();
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        contentLayout.ResumeLayout(false);
        groupRoiList.ResumeLayout(false);
        detailsLayout.ResumeLayout(false);
        detailsLayout.PerformLayout();
        ResumeLayout(false);
    }
}
