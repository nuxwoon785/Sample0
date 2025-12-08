using System.Drawing;
using System.Windows.Forms;
using YoloLabeler.Controls;

namespace YoloLabeler;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null!;
    private ImageCanvas imageCanvas = null!;
    private Button openFolderButton = null!;
    private Button previousButton = null!;
    private Button nextButton = null!;
    private Button saveButton = null!;
    private Button generateButton = null!;
    private Button resetViewButton = null!;
    private ComboBox classSelector = null!;
    private TextBox classNameInput = null!;
    private Button addClassButton = null!;
    private ListBox annotationList = null!;
    private Label fileLabel = null!;

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
        imageCanvas = new ImageCanvas();
        openFolderButton = new Button();
        previousButton = new Button();
        nextButton = new Button();
        saveButton = new Button();
        generateButton = new Button();
        resetViewButton = new Button();
        classSelector = new ComboBox();
        classNameInput = new TextBox();
        addClassButton = new Button();
        annotationList = new ListBox();
        fileLabel = new Label();
        var layout = new TableLayoutPanel();
        var controlsPanel = new FlowLayoutPanel();
        var rightPanel = new TableLayoutPanel();

        SuspendLayout();

        layout.Dock = DockStyle.Fill;
        layout.ColumnCount = 2;
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        layout.RowCount = 2;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        controlsPanel.AutoSize = true;
        controlsPanel.Dock = DockStyle.Fill;
        controlsPanel.Controls.Add(openFolderButton);
        controlsPanel.Controls.Add(previousButton);
        controlsPanel.Controls.Add(nextButton);
        controlsPanel.Controls.Add(saveButton);
        controlsPanel.Controls.Add(generateButton);
        controlsPanel.Controls.Add(resetViewButton);
        controlsPanel.Controls.Add(fileLabel);
        controlsPanel.Controls.Add(new Label { Text = "Class:", AutoSize = true, Padding = new Padding(10, 5, 0, 0) });
        controlsPanel.Controls.Add(classSelector);
        controlsPanel.Controls.Add(classNameInput);
        controlsPanel.Controls.Add(addClassButton);

        layout.Controls.Add(controlsPanel, 0, 0);
        layout.SetColumnSpan(controlsPanel, 2);
        layout.Controls.Add(imageCanvas, 0, 1);
        layout.Controls.Add(rightPanel, 1, 1);

        imageCanvas.BackColor = System.Drawing.Color.Black;
        imageCanvas.Dock = DockStyle.Fill;
        imageCanvas.MinimumSize = new System.Drawing.Size(200, 200);

        openFolderButton.Text = "Open";
        openFolderButton.Click += openFolderButton_Click;

        previousButton.Text = "Prev";
        previousButton.Click += previousButton_Click;

        nextButton.Text = "Next";
        nextButton.Click += nextButton_Click;

        saveButton.Text = "Save";
        saveButton.Click += saveButton_Click;

        generateButton.Text = "Generate";
        generateButton.Click += generateButton_Click;

        resetViewButton.Text = "Reset View";
        resetViewButton.Click += resetViewButton_Click;

        fileLabel.AutoSize = true;
        fileLabel.Padding = new Padding(10, 7, 0, 0);

        classSelector.DropDownStyle = ComboBoxStyle.DropDownList;
        classSelector.Width = 120;
        classSelector.SelectedIndexChanged += classSelector_SelectedIndexChanged;

        classNameInput.Width = 120;
        classNameInput.PlaceholderText = "New class";

        addClassButton.Text = "Add";
        addClassButton.Click += addClassButton_Click;

        rightPanel.Dock = DockStyle.Fill;
        rightPanel.ColumnCount = 1;
        rightPanel.RowCount = 1;
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        rightPanel.Controls.Add(annotationList, 0, 0);

        annotationList.Dock = DockStyle.Fill;
        annotationList.HorizontalScrollbar = true;

        Controls.Add(layout);
        Text = "YOLO Labeling Tool";
        WindowState = FormWindowState.Maximized;
        KeyDown += MainForm_KeyDown;

        ResumeLayout(false);
    }
}
