using BudgetTracker.Domain.Entities;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Dialog for adding or editing a category
/// </summary>
public class CategoryDialog : Form
{
    private TextBox txtName;
    private TextBox txtDescription;
    private TextBox txtColor;
    private TextBox txtIcon;
    private Button btnOK;
    private Button btnCancel;
    private Label lblName;
    private Label lblDescription;
    private Label lblColor;
    private Label lblIcon;

    public string CategoryName => txtName.Text;
    public string CategoryDescription => txtDescription.Text;
    public string CategoryColor => txtColor.Text;
    public string CategoryIcon => txtIcon.Text;

    private readonly Category? _existingCategory;

    public CategoryDialog(Category? existingCategory = null)
    {
        _existingCategory = existingCategory;
        InitializeComponent();
        LoadExistingData();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = _existingCategory == null ? "Add Category" : "Edit Category";
        this.Size = new Size(720, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Name Label
        lblName = new Label
        {
            Text = "Name:",
            Location = new Point(30, 30),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Name TextBox
        txtName = new TextBox
        {
            Location = new Point(180, 28),
            Size = new Size(430, 30),
            Font = new Font("Segoe UI", 11)
        };

        // Description Label
        lblDescription = new Label
        {
            Text = "Description:",
            Location = new Point(30, 85),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Description TextBox
        txtDescription = new TextBox
        {
            Location = new Point(180, 83),
            Size = new Size(430, 100),
            Font = new Font("Segoe UI", 11),
            Multiline = true
        };

        // Color Label
        lblColor = new Label
        {
            Text = "Color (Hex):",
            Location = new Point(30, 215),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Color TextBox
        txtColor = new TextBox
        {
            Location = new Point(180, 213),
            Size = new Size(200, 30),
            Font = new Font("Segoe UI", 11),
            Text = "#4CAF50"
        };

        // Icon Label
        lblIcon = new Label
        {
            Text = "Icon (Emoji):",
            Location = new Point(30, 275),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Icon TextBox
        txtIcon = new TextBox
        {
            Location = new Point(180, 273),
            Size = new Size(120, 30),
            Font = new Font("Segoe UI", 11)
        };

        // OK Button
        btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(380, 370),
            Size = new Size(110, 40),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.Click += BtnOK_Click;

        // Cancel Button
        btnCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(500, 370),
            Size = new Size(110, 40),
            BackColor = Color.FromArgb(158, 158, 158),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        btnCancel.FlatAppearance.BorderSize = 0;

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            lblName, txtName,
            lblDescription, txtDescription,
            lblColor, txtColor,
            lblIcon, txtIcon,
            btnOK, btnCancel
        });

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }

    private void LoadExistingData()
    {
        if (_existingCategory != null)
        {
            txtName.Text = _existingCategory.Name;
            txtDescription.Text = _existingCategory.Description ?? string.Empty;
            txtColor.Text = _existingCategory.Color ?? "#4CAF50";
            txtIcon.Text = _existingCategory.Icon ?? string.Empty;
        }
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Category name is required.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtName.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }

        // Color validation (optional)
        if (!string.IsNullOrWhiteSpace(txtColor.Text) && !txtColor.Text.StartsWith("#"))
        {
            MessageBox.Show("Color must be in hex format (e.g., #4CAF50).", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtColor.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }
    }
}
