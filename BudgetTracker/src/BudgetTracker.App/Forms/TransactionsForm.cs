using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Core.Events;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Transactions form for managing income and expense transactions
/// Demo Step 2: Transaction entry and management
/// </summary>
public partial class TransactionsForm : Form
{
    private DataGridView dgvTransactions;
    private Button btnAddTransaction;
    private Button btnEditTransaction;
    private Button btnDeleteTransaction;
    private Button btnRefresh;
    private Label lblTitle;
    private Panel pnlTransactions;
    private Panel pnlActions;
    private Panel pnlFilters;
    private Label lblFilterType;
    private ComboBox cmbFilterType;
    private Label lblFilterCategory;
    private ComboBox cmbFilterCategory;
    private Label lblFilterDateFrom;
    private DateTimePicker dtpFilterFrom;
    private Label lblFilterDateTo;
    private DateTimePicker dtpFilterTo;
    private Button btnApplyFilter;
    private Button btnClearFilter;
    private Label lblSearch;
    private TextBox txtSearch;
    private Label lblTotalIncome;
    private Label lblTotalExpense;
    private Label lblBalance;

    public TransactionsForm()
    {
        InitializeComponent();
        LoadTransactions();
        LoadFilterCategories();
        UpdateSummary();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = "Budget Tracker - Transactions";
        this.Size = new Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1000, 600);

        // Title Label
        lblTitle = new Label
        {
            Text = "Transaction Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // Filters Panel
        pnlFilters = new Panel
        {
            Location = new Point(20, 60),
            Size = new Size(1140, 120),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Filter Type Label
        lblFilterType = new Label
        {
            Text = "Type:",
            Location = new Point(15, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Filter Type ComboBox
        cmbFilterType = new ComboBox
        {
            Location = new Point(15, 40),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFilterType.Items.AddRange(new object[] { "All", "Income", "Expense" });
        cmbFilterType.SelectedIndex = 0;

        // Filter Category Label
        lblFilterCategory = new Label
        {
            Text = "Category:",
            Location = new Point(185, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Filter Category ComboBox
        cmbFilterCategory = new ComboBox
        {
            Location = new Point(185, 40),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        // Filter Date From Label
        lblFilterDateFrom = new Label
        {
            Text = "From Date:",
            Location = new Point(405, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Filter Date From DateTimePicker
        dtpFilterFrom = new DateTimePicker
        {
            Location = new Point(405, 40),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Short
        };
        dtpFilterFrom.Value = DateTime.Today.AddMonths(-1);

        // Filter Date To Label
        lblFilterDateTo = new Label
        {
            Text = "To Date:",
            Location = new Point(575, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Filter Date To DateTimePicker
        dtpFilterTo = new DateTimePicker
        {
            Location = new Point(575, 40),
            Size = new Size(150, 25),
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Short
        };

        // Search Label
        lblSearch = new Label
        {
            Text = "Search:",
            Location = new Point(15, 75),
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Search TextBox
        txtSearch = new TextBox
        {
            Location = new Point(85, 73),
            Size = new Size(300, 25),
            Font = new Font("Segoe UI", 10)
        };
        txtSearch.TextChanged += TxtSearch_TextChanged;

        // Apply Filter Button
        btnApplyFilter = new Button
        {
            Text = "Apply Filter",
            Location = new Point(745, 38),
            Size = new Size(180, 30),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnApplyFilter.FlatAppearance.BorderSize = 0;
        btnApplyFilter.Click += BtnApplyFilter_Click;

        // Clear Filter Button
        btnClearFilter = new Button
        {
            Text = "Clear Filter",
            Location = new Point(935, 38),
            Size = new Size(180, 30),
            BackColor = Color.FromArgb(158, 158, 158),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnClearFilter.FlatAppearance.BorderSize = 0;
        btnClearFilter.Click += BtnClearFilter_Click;

        pnlFilters.Controls.AddRange(new Control[]
        {
            lblFilterType, cmbFilterType,
            lblFilterCategory, cmbFilterCategory,
            lblFilterDateFrom, dtpFilterFrom,
            lblFilterDateTo, dtpFilterTo,
            lblSearch, txtSearch,
            btnApplyFilter, btnClearFilter
        });

        // Summary Labels
        lblTotalIncome = new Label
        {
            Text = "Total Income: $0.00",
            Location = new Point(20, 190),
            Size = new Size(250, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(76, 175, 80)
        };

        lblTotalExpense = new Label
        {
            Text = "Total Expense: $0.00",
            Location = new Point(280, 190),
            Size = new Size(250, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(244, 67, 54)
        };

        lblBalance = new Label
        {
            Text = "Balance: $0.00",
            Location = new Point(540, 190),
            Size = new Size(250, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 150, 243)
        };

        // Transactions Panel
        pnlTransactions = new Panel
        {
            Location = new Point(20, 225),
            Size = new Size(1140, 450),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            BorderStyle = BorderStyle.FixedSingle
        };

        // DataGridView for Transactions
        dgvTransactions = new DataGridView
        {
            Location = new Point(0, 0),
            Size = new Size(1140, 450),
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        // Configure columns
        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Id",
            HeaderText = "ID",
            DataPropertyName = "Id",
            Width = 50,
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Type",
            HeaderText = "Type",
            Width = 80,
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Date",
            HeaderText = "Date",
            DataPropertyName = "Date",
            Width = 100,
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Description",
            HeaderText = "Description",
            DataPropertyName = "Description",
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Category",
            HeaderText = "Category",
            Width = 120,
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Amount",
            HeaderText = "Amount",
            Width = 100,
            ReadOnly = true
        });

        dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Account",
            HeaderText = "Account",
            DataPropertyName = "Account",
            Width = 120,
            ReadOnly = true
        });

        dgvTransactions.CellFormatting += DgvTransactions_CellFormatting;

        pnlTransactions.Controls.Add(dgvTransactions);

        // Actions Panel
        pnlActions = new Panel
        {
            Location = new Point(20, 685),
            Size = new Size(1140, 60),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        // Add Transaction Button
        btnAddTransaction = new Button
        {
            Text = "Add Transaction",
            Location = new Point(0, 10),
            Size = new Size(225, 40),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAddTransaction.FlatAppearance.BorderSize = 0;
        btnAddTransaction.Click += BtnAddTransaction_Click;

        // Edit Transaction Button
        btnEditTransaction = new Button
        {
            Text = "Edit Transaction",
            Location = new Point(235, 10),
            Size = new Size(225, 40),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnEditTransaction.FlatAppearance.BorderSize = 0;
        btnEditTransaction.Click += BtnEditTransaction_Click;

        // Delete Transaction Button
        btnDeleteTransaction = new Button
        {
            Text = "Delete Transaction",
            Location = new Point(470, 10),
            Size = new Size(225, 40),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnDeleteTransaction.FlatAppearance.BorderSize = 0;
        btnDeleteTransaction.Click += BtnDeleteTransaction_Click;

        // Refresh Button
        btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(945, 10),
            Size = new Size(195, 40),
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += BtnRefresh_Click;

        pnlActions.Controls.AddRange(new Control[]
        {
            btnAddTransaction,
            btnEditTransaction,
            btnDeleteTransaction,
            btnRefresh
        });

        // Add all controls to form
        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            pnlFilters,
            lblTotalIncome,
            lblTotalExpense,
            lblBalance,
            pnlTransactions,
            pnlActions
        });
    }

    private void LoadFilterCategories()
    {
        try
        {
            var categories = Program.CategoryRepository.GetAll()
                .Where(c => c.IsActive)
                .ToList();

            cmbFilterCategory.DisplayMember = "Name";
            cmbFilterCategory.ValueMember = "Id";

            var categoriesWithAll = new List<Category>
            {
                new Category("All", "", "", "") { Id = 0 }
            };
            categoriesWithAll.AddRange(categories);

            cmbFilterCategory.DataSource = categoriesWithAll;
            cmbFilterCategory.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadTransactions()
    {
        try
        {
            var transactions = Program.TransactionRepository.GetAll().ToList();
            DisplayTransactions(transactions);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading transactions: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisplayTransactions(IEnumerable<Transaction> transactions)
    {
        dgvTransactions.Rows.Clear();

        foreach (var transaction in transactions.OrderByDescending(t => t.Date))
        {
            var category = Program.CategoryRepository.GetById(transaction.CategoryId);
            var type = transaction is Income ? "Income" : "Expense";
            var amount = transaction.GetFormattedAmount();

            dgvTransactions.Rows.Add(
                transaction.Id,
                type,
                transaction.Date.ToShortDateString(),
                transaction.Description,
                category?.Name ?? "Unknown",
                amount,
                transaction.Account
            );
        }

        UpdateSummary();
    }

    private void DgvTransactions_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvTransactions.Columns[e.ColumnIndex].Name == "Type" && e.Value != null)
        {
            string type = e.Value.ToString() ?? "";
            if (type == "Income")
            {
                e.CellStyle.ForeColor = Color.FromArgb(76, 175, 80);
                e.CellStyle.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
            }
            else if (type == "Expense")
            {
                e.CellStyle.ForeColor = Color.FromArgb(244, 67, 54);
                e.CellStyle.Font = new Font(dgvTransactions.Font, FontStyle.Bold);
            }
        }
    }

    private void UpdateSummary()
    {
        try
        {
            var transactions = Program.TransactionRepository.GetAll();

            // Using LINQ to calculate totals
            var totalIncome = transactions
                .Where(t => t is Income)
                .Sum(t => t.Amount.Amount);

            var totalExpense = transactions
                .Where(t => t is Expense)
                .Sum(t => t.Amount.Amount);

            var balance = totalIncome - totalExpense;

            lblTotalIncome.Text = $"Total Income: ${totalIncome:N2}";
            lblTotalExpense.Text = $"Total Expense: ${totalExpense:N2}";
            lblBalance.Text = $"Balance: ${balance:N2}";
            lblBalance.ForeColor = balance >= 0
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(244, 67, 54);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating summary: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAddTransaction_Click(object? sender, EventArgs e)
    {
        var dialog = new TransactionDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                Transaction transaction;
                var money = new Money(dialog.Amount, "USD");

                if (dialog.IsIncome)
                {
                    transaction = new Income(
                        dialog.Description,
                        money,
                        dialog.TransactionDate,
                        dialog.CategoryId,
                        dialog.Notes,
                        dialog.Account,
                        dialog.Source
                    );
                }
                else
                {
                    transaction = new Expense(
                        dialog.Description,
                        money,
                        dialog.TransactionDate,
                        dialog.CategoryId,
                        dialog.Notes,
                        dialog.Account
                    );
                }

                Program.TransactionRepository.Add(transaction);
                Program.TransactionRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnTransactionAdded(transaction);

                LoadTransactions();

                MessageBox.Show("Transaction added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding transaction: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnEditTransaction_Click(object? sender, EventArgs e)
    {
        if (dgvTransactions.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a transaction to edit.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var transactionId = (int)dgvTransactions.SelectedRows[0].Cells["Id"].Value;
        var transaction = Program.TransactionRepository.GetById(transactionId);

        if (transaction == null)
        {
            MessageBox.Show("Transaction not found.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var dialog = new TransactionDialog(transaction);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var money = new Money(dialog.Amount, "USD");

                transaction.Update(
                    dialog.Description,
                    money,
                    dialog.TransactionDate,
                    dialog.CategoryId,
                    dialog.Notes,
                    dialog.Account
                );

                if (transaction is Income income && dialog.IsIncome)
                {
                    // Update source for income
                    var sourceProperty = typeof(Income).GetProperty("Source");
                    sourceProperty?.SetValue(income, dialog.Source);
                }

                Program.TransactionRepository.Update(transaction);
                Program.TransactionRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnTransactionUpdated(transaction);

                LoadTransactions();

                MessageBox.Show("Transaction updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating transaction: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnDeleteTransaction_Click(object? sender, EventArgs e)
    {
        if (dgvTransactions.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a transaction to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var transactionId = (int)dgvTransactions.SelectedRows[0].Cells["Id"].Value;
        var transaction = Program.TransactionRepository.GetById(transactionId);

        if (transaction == null)
        {
            MessageBox.Show("Transaction not found.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete this transaction?\n\n" +
            $"{transaction.Description} - {transaction.GetFormattedAmount()}",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                Program.TransactionRepository.Remove(transaction);
                Program.TransactionRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnTransactionDeleted(transaction);

                LoadTransactions();

                MessageBox.Show("Transaction deleted successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting transaction: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnApplyFilter_Click(object? sender, EventArgs e)
    {
        try
        {
            // Using LINQ and Lambda expressions for filtering
            var transactions = Program.TransactionRepository.GetAll();

            // Filter by type
            var typeFilter = cmbFilterType.SelectedItem?.ToString() ?? "All";
            if (typeFilter == "Income")
            {
                transactions = transactions.Where(t => t is Income);
            }
            else if (typeFilter == "Expense")
            {
                transactions = transactions.Where(t => t is Expense);
            }

            // Filter by category
            var categoryId = (int)(cmbFilterCategory.SelectedValue ?? 0);
            if (categoryId > 0)
            {
                transactions = transactions.Where(t => t.CategoryId == categoryId);
            }

            // Filter by date range
            var fromDate = dtpFilterFrom.Value.Date;
            var toDate = dtpFilterTo.Value.Date;
            transactions = transactions.Where(t => t.Date >= fromDate && t.Date <= toDate);

            DisplayTransactions(transactions.ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying filter: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnClearFilter_Click(object? sender, EventArgs e)
    {
        cmbFilterType.SelectedIndex = 0;
        cmbFilterCategory.SelectedIndex = 0;
        dtpFilterFrom.Value = DateTime.Today.AddMonths(-1);
        dtpFilterTo.Value = DateTime.Today;
        txtSearch.Clear();
        LoadTransactions();
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        try
        {
            var searchText = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadTransactions();
                return;
            }

            // Using LINQ to search transactions
            var transactions = Program.TransactionRepository.GetAll()
                .Where(t =>
                    t.Description.ToLower().Contains(searchText) ||
                    (t.Notes != null && t.Notes.ToLower().Contains(searchText)) ||
                    (t.Account != null && t.Account.ToLower().Contains(searchText))
                )
                .ToList();

            DisplayTransactions(transactions);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching transactions: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        LoadTransactions();
        MessageBox.Show("Transactions refreshed!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
