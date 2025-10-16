using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Dialog for adding or editing a transaction (Income or Expense)
/// </summary>
public class TransactionDialog : Form
{
    private RadioButton rbIncome;
    private RadioButton rbExpense;
    private TextBox txtDescription;
    private TextBox txtAmount;
    private ComboBox cmbCategory;
    private DateTimePicker dtpDate;
    private TextBox txtPaymentMethod;
    private TextBox txtNotes;
    private TextBox txtSource; // For Income only
    private Label lblTransactionType;
    private Label lblDescription;
    private Label lblAmount;
    private Label lblCategory;
    private Label lblDate;
    private Label lblPaymentMethod;
    private Label lblNotes;
    private Label lblSource;
    private Button btnOK;
    private Button btnCancel;
    private Panel pnlTransactionType;

    public bool IsIncome => rbIncome.Checked;
    public string Description => txtDescription.Text;
    public decimal Amount => decimal.TryParse(txtAmount.Text, out var amount) ? amount : 0;
    public int CategoryId => cmbCategory.SelectedValue != null ? (int)cmbCategory.SelectedValue : 0;
    public DateTime TransactionDate => dtpDate.Value;
    public string Account => txtPaymentMethod.Text;
    public string Notes => txtNotes.Text;
    public string Source => txtSource.Text;

    private readonly Transaction? _existingTransaction;

    public TransactionDialog(Transaction? existingTransaction = null)
    {
        _existingTransaction = existingTransaction;
        InitializeComponent();
        LoadCategories();
        LoadExistingData();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = _existingTransaction == null ? "Add Transaction" : "Edit Transaction";
        this.Size = new Size(700, 680);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Transaction Type Label
        lblTransactionType = new Label
        {
            Text = "Transaction Type:",
            Location = new Point(30, 30),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Transaction Type Panel
        pnlTransactionType = new Panel
        {
            Location = new Point(200, 25),
            Size = new Size(450, 35),
            BorderStyle = BorderStyle.FixedSingle
        };

        // Income RadioButton
        rbIncome = new RadioButton
        {
            Text = "Income",
            Location = new Point(20, 7),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 11),
            Checked = true
        };
        rbIncome.CheckedChanged += RbTransactionType_CheckedChanged;

        // Expense RadioButton
        rbExpense = new RadioButton
        {
            Text = "Expense",
            Location = new Point(150, 7),
            Size = new Size(100, 20),
            Font = new Font("Segoe UI", 11)
        };
        rbExpense.CheckedChanged += RbTransactionType_CheckedChanged;

        pnlTransactionType.Controls.Add(rbIncome);
        pnlTransactionType.Controls.Add(rbExpense);

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
            Location = new Point(200, 83),
            Size = new Size(450, 30),
            Font = new Font("Segoe UI", 11)
        };

        // Amount Label
        lblAmount = new Label
        {
            Text = "Amount ($):",
            Location = new Point(30, 135),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Amount TextBox
        txtAmount = new TextBox
        {
            Location = new Point(200, 133),
            Size = new Size(200, 30),
            Font = new Font("Segoe UI", 11)
        };

        // Category Label
        lblCategory = new Label
        {
            Text = "Category:",
            Location = new Point(30, 185),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Category ComboBox
        cmbCategory = new ComboBox
        {
            Location = new Point(200, 183),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // Date Label
        lblDate = new Label
        {
            Text = "Date:",
            Location = new Point(30, 235),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Date DateTimePicker
        dtpDate = new DateTimePicker
        {
            Location = new Point(200, 233),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            Format = DateTimePickerFormat.Short
        };

        // Payment Method Label
        lblPaymentMethod = new Label
        {
            Text = "Payment Method:",
            Location = new Point(30, 285),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Payment Method TextBox
        txtPaymentMethod = new TextBox
        {
            Location = new Point(200, 283),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            Text = "Cash"
        };

        // Source Label (Income only)
        lblSource = new Label
        {
            Text = "Source:",
            Location = new Point(30, 335),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Visible = true
        };

        // Source TextBox (Income only)
        txtSource = new TextBox
        {
            Location = new Point(200, 333),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            Visible = true
        };

        // Notes Label
        lblNotes = new Label
        {
            Text = "Notes:",
            Location = new Point(30, 385),
            AutoSize = true,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };

        // Notes TextBox
        txtNotes = new TextBox
        {
            Location = new Point(200, 383),
            Size = new Size(450, 120),
            Font = new Font("Segoe UI", 11),
            Multiline = true
        };

        // OK Button
        btnOK = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(420, 560),
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
            Location = new Point(540, 560),
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
            lblTransactionType, pnlTransactionType,
            lblDescription, txtDescription,
            lblAmount, txtAmount,
            lblCategory, cmbCategory,
            lblDate, dtpDate,
            lblPaymentMethod, txtPaymentMethod,
            lblSource, txtSource,
            lblNotes, txtNotes,
            btnOK, btnCancel
        });

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
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
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadExistingData()
    {
        if (_existingTransaction != null)
        {
            txtDescription.Text = _existingTransaction.Description;
            txtAmount.Text = _existingTransaction.Amount.Amount.ToString("F2");
            cmbCategory.SelectedValue = _existingTransaction.CategoryId;
            dtpDate.Value = _existingTransaction.Date;
            txtPaymentMethod.Text = _existingTransaction.Account ?? "Cash";
            txtNotes.Text = _existingTransaction.Notes ?? string.Empty;

            if (_existingTransaction is Income income)
            {
                rbIncome.Checked = true;
                txtSource.Text = income.Source ?? string.Empty;
            }
            else
            {
                rbExpense.Checked = true;
            }
        }
    }

    private void RbTransactionType_CheckedChanged(object? sender, EventArgs e)
    {
        // Show/hide Source field based on transaction type
        bool isIncome = rbIncome.Checked;
        lblSource.Visible = isIncome;
        txtSource.Visible = isIncome;
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        // Validate Description
        if (string.IsNullOrWhiteSpace(txtDescription.Text))
        {
            MessageBox.Show("Description is required.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDescription.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }

        // Validate Amount
        if (!decimal.TryParse(txtAmount.Text, out var amount) || amount <= 0)
        {
            MessageBox.Show("Please enter a valid positive amount.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAmount.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }

        // Validate Category
        if (cmbCategory.SelectedValue == null)
        {
            MessageBox.Show("Please select a category.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbCategory.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }

        // Validate Payment Method
        if (string.IsNullOrWhiteSpace(txtPaymentMethod.Text))
        {
            MessageBox.Show("Payment method is required.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPaymentMethod.Focus();
            this.DialogResult = DialogResult.None;
            return;
        }
    }
}
