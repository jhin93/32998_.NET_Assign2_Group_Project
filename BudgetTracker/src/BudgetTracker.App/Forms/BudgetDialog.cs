using BudgetTracker.Domain.Entities;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Dialog for adding or editing budgets
/// </summary>
public partial class BudgetDialog : Form
{
    private Label lblName;
    private TextBox txtName;
    private Label lblAmount;
    private TextBox txtAmount;
    private Label lblCategory;
    private ComboBox cmbCategory;
    private Label lblPeriodType;
    private ComboBox cmbPeriodType;
    private Label lblStartDate;
    private DateTimePicker dtpStartDate;
    private Label lblEndDate;
    private DateTimePicker dtpEndDate;
    private Button btnOK;
    private Button btnCancel;
    private Panel pnlPeriod;

    private Budget? _budget;

    public string BudgetName => txtName.Text.Trim();
    public decimal Amount => decimal.TryParse(txtAmount.Text, out var amount) ? amount : 0;
    public int CategoryId => cmbCategory.SelectedValue is int id ? id : 0;
    public DateTime StartDate => dtpStartDate.Value.Date;
    public DateTime EndDate => dtpEndDate.Value.Date;

    public BudgetDialog(Budget? budget = null)
    {
        _budget = budget;
        InitializeComponent();
        LoadCategories();

        if (_budget != null)
        {
            LoadBudgetData();
        }
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = _budget == null ? "Add Budget" : "Edit Budget";
        this.Size = new Size(500, 520);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Name Label
        lblName = new Label
        {
            Text = "Budget Name:",
            Location = new Point(30, 30),
            Size = new Size(120, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Name TextBox
        txtName = new TextBox
        {
            Location = new Point(30, 55),
            Size = new Size(420, 25),
            Font = new Font("Segoe UI", 10)
        };

        // Amount Label
        lblAmount = new Label
        {
            Text = "Budget Amount:",
            Location = new Point(30, 95),
            Size = new Size(120, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Amount TextBox
        txtAmount = new TextBox
        {
            Location = new Point(30, 120),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10)
        };

        // Category Label
        lblCategory = new Label
        {
            Text = "Category:",
            Location = new Point(30, 160),
            Size = new Size(120, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Category ComboBox
        cmbCategory = new ComboBox
        {
            Location = new Point(30, 185),
            Size = new Size(420, 25),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // Period Type Label
        lblPeriodType = new Label
        {
            Text = "Period Type:",
            Location = new Point(30, 225),
            Size = new Size(120, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Period Type ComboBox
        cmbPeriodType = new ComboBox
        {
            Location = new Point(30, 250),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbPeriodType.Items.AddRange(new object[] { "Monthly", "Yearly", "Custom" });
        cmbPeriodType.SelectedIndex = 0;
        cmbPeriodType.SelectedIndexChanged += CmbPeriodType_SelectedIndexChanged;

        // Period Panel
        pnlPeriod = new Panel
        {
            Location = new Point(30, 290),
            Size = new Size(420, 90),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Start Date Label
        lblStartDate = new Label
        {
            Text = "Start Date:",
            Location = new Point(10, 10),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Start Date DateTimePicker
        dtpStartDate = new DateTimePicker
        {
            Location = new Point(10, 35),
            Size = new Size(180, 25),
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Short
        };
        dtpStartDate.ValueChanged += DtpStartDate_ValueChanged;

        // End Date Label
        lblEndDate = new Label
        {
            Text = "End Date:",
            Location = new Point(210, 10),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // End Date DateTimePicker
        dtpEndDate = new DateTimePicker
        {
            Location = new Point(210, 35),
            Size = new Size(180, 25),
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Short,
            Enabled = false  // Disabled by default for Monthly/Yearly
        };

        pnlPeriod.Controls.AddRange(new Control[]
        {
            lblStartDate, dtpStartDate,
            lblEndDate, dtpEndDate
        });

        // OK Button
        btnOK = new Button
        {
            Text = "OK",
            Location = new Point(250, 410),
            Size = new Size(100, 40),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.Click += BtnOK_Click;

        // Cancel Button
        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(360, 410),
            Size = new Size(100, 40),
            BackColor = Color.FromArgb(158, 158, 158),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            lblName, txtName,
            lblAmount, txtAmount,
            lblCategory, cmbCategory,
            lblPeriodType, cmbPeriodType,
            pnlPeriod,
            btnOK, btnCancel
        });
    }

    private void LoadCategories()
    {
        try
        {
            var categories = Program.CategoryRepository.GetAll()
                .Where(c => c.IsActive)
                .ToList();

            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = categories;

            if (categories.Any())
            {
                cmbCategory.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadBudgetData()
    {
        if (_budget == null) return;

        txtName.Text = _budget.Name;
        txtAmount.Text = _budget.Amount.Amount.ToString("F2");

        // Select the category
        cmbCategory.SelectedValue = _budget.CategoryId;

        // Set dates
        dtpStartDate.Value = _budget.StartDate;
        dtpEndDate.Value = _budget.EndDate;

        // Determine period type
        var duration = (_budget.EndDate - _budget.StartDate).Days;
        if (duration >= 28 && duration <= 31)
        {
            cmbPeriodType.SelectedIndex = 0; // Monthly
        }
        else if (duration >= 365 && duration <= 366)
        {
            cmbPeriodType.SelectedIndex = 1; // Yearly
        }
        else
        {
            cmbPeriodType.SelectedIndex = 2; // Custom
            dtpEndDate.Enabled = true;
        }
    }

    private void CmbPeriodType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var periodType = cmbPeriodType.SelectedItem?.ToString() ?? "Monthly";

        switch (periodType)
        {
            case "Monthly":
                dtpEndDate.Enabled = false;
                UpdateEndDateForMonthly();
                break;
            case "Yearly":
                dtpEndDate.Enabled = false;
                UpdateEndDateForYearly();
                break;
            case "Custom":
                dtpEndDate.Enabled = true;
                break;
        }
    }

    private void DtpStartDate_ValueChanged(object? sender, EventArgs e)
    {
        var periodType = cmbPeriodType.SelectedItem?.ToString() ?? "Monthly";

        if (periodType == "Monthly")
        {
            UpdateEndDateForMonthly();
        }
        else if (periodType == "Yearly")
        {
            UpdateEndDateForYearly();
        }
    }

    private void UpdateEndDateForMonthly()
    {
        dtpEndDate.Value = dtpStartDate.Value.AddMonths(1).AddDays(-1);
    }

    private void UpdateEndDateForYearly()
    {
        dtpEndDate.Value = dtpStartDate.Value.AddYears(1).AddDays(-1);
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        try
        {
            // Phase 6.20: Input Validation & Error Handling

            // Validate name - required field
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a budget name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Validate name length
            if (txtName.Text.Length > 100)
            {
                MessageBox.Show("Budget name cannot exceed 100 characters.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Validate amount - must be numeric
            if (!decimal.TryParse(txtAmount.Text, out var amount))
            {
                MessageBox.Show("Please enter a valid numeric amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            // Validate amount - must be positive
            if (amount <= 0)
            {
                MessageBox.Show("Budget amount must be greater than zero.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            // Validate amount - reasonable maximum
            if (amount > 1000000000)
            {
                MessageBox.Show("Budget amount cannot exceed $1,000,000,000.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            // Validate amount - reasonable minimum
            if (amount < 1)
            {
                MessageBox.Show("Budget amount should be at least $1.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            // Validate category
            if (cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            // Validate dates - end date must be after start date
            if (dtpEndDate.Value.Date <= dtpStartDate.Value.Date)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpEndDate.Focus();
                return;
            }

            // Validate date range - warn if very short
            var daysDifference = (dtpEndDate.Value.Date - dtpStartDate.Value.Date).Days;
            if (daysDifference < 7)
            {
                var result = MessageBox.Show(
                    "Budget period is less than 1 week. Do you want to continue?",
                    "Short Budget Period",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    dtpEndDate.Focus();
                    return;
                }
            }

            // Validate date range - warn if very long
            if (daysDifference > 730) // More than 2 years
            {
                var result = MessageBox.Show(
                    "Budget period is more than 2 years. Do you want to continue?",
                    "Long Budget Period",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    dtpEndDate.Focus();
                    return;
                }
            }

            // Validate start date - warn if in the past
            if (dtpStartDate.Value.Date < DateTime.Today.AddMonths(-1))
            {
                var result = MessageBox.Show(
                    "Budget start date is more than 1 month in the past. Do you want to continue?",
                    "Past Start Date",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    dtpStartDate.Focus();
                    return;
                }
            }

            // All validations passed
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while saving the budget:\n{ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
