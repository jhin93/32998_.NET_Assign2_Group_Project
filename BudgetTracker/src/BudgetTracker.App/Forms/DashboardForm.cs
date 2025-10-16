using BudgetTracker.Domain.Entities;
using BudgetTracker.Core.Events;
using BudgetTracker.Core.Services;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Dashboard form showing overview of finances
/// Phase 4.12: DashboardForm Implementation
/// Phase 4.13: Implements event-driven auto-refresh
/// Demo Step 4: Dashboard review
/// </summary>
public partial class DashboardForm : Form
{
    private Label lblTitle;
    private Panel pnlSummary;
    private Label lblTotalBalance;
    private Label lblTotalBalanceValue;
    private Label lblTotalIncome;
    private Label lblTotalIncomeValue;
    private Label lblTotalExpense;
    private Label lblTotalExpenseValue;

    private Panel pnlRecentTransactions;
    private Label lblRecentTitle;
    private DataGridView dgvRecentTransactions;

    private Panel pnlBudgetStatus;
    private Label lblBudgetTitle;
    private FlowLayoutPanel flowBudgetStatus;

    private Panel pnlSpendingSummary;
    private Label lblSpendingSummaryTitle;
    private DataGridView dgvSpendingSummary;

    private Button btnRefresh;
    private Button btnViewTransactions;
    private Button btnViewBudgets;

    public DashboardForm()
    {
        InitializeComponent();
        LoadDashboardData();
        SubscribeToEvents();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = "Budget Tracker - Dashboard";
        this.Size = new Size(1200, 1600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1000, 1400);

        // Title
        lblTitle = new Label
        {
            Text = "Dashboard",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // Summary Panel
        pnlSummary = new Panel
        {
            Location = new Point(20, 65),
            Size = new Size(1140, 100),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(240, 248, 255)
        };

        // Total Balance
        lblTotalBalance = new Label
        {
            Text = "Total Balance",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(30, 15),
            Size = new Size(300, 25),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        lblTotalBalanceValue = new Label
        {
            Text = "$0.00",
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            Location = new Point(30, 40),
            Size = new Size(300, 40),
            ForeColor = Color.FromArgb(33, 150, 243)
        };

        // Total Income
        lblTotalIncome = new Label
        {
            Text = "Total Income",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(380, 15),
            Size = new Size(300, 25),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        lblTotalIncomeValue = new Label
        {
            Text = "$0.00",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(380, 45),
            Size = new Size(300, 35),
            ForeColor = Color.FromArgb(76, 175, 80)
        };

        // Total Expense
        lblTotalExpense = new Label
        {
            Text = "Total Expense",
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Location = new Point(730, 15),
            Size = new Size(300, 25),
            ForeColor = Color.FromArgb(70, 70, 70)
        };

        lblTotalExpenseValue = new Label
        {
            Text = "$0.00",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(730, 45),
            Size = new Size(300, 35),
            ForeColor = Color.FromArgb(244, 67, 54)
        };

        pnlSummary.Controls.AddRange(new Control[]
        {
            lblTotalBalance, lblTotalBalanceValue,
            lblTotalIncome, lblTotalIncomeValue,
            lblTotalExpense, lblTotalExpenseValue
        });

        // Recent Transactions Panel
        pnlRecentTransactions = new Panel
        {
            Location = new Point(20, 180),
            Size = new Size(560, 840),
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblRecentTitle = new Label
        {
            Text = "Recent Transactions",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(10, 10),
            AutoSize = true
        };

        dgvRecentTransactions = new DataGridView
        {
            Location = new Point(10, 45),
            Size = new Size(535, 780),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Date",
            HeaderText = "Date",
            Width = 80
        });
        dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Description",
            HeaderText = "Description"
        });
        dgvRecentTransactions.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Amount",
            HeaderText = "Amount",
            Width = 100
        });

        pnlRecentTransactions.Controls.AddRange(new Control[] { lblRecentTitle, dgvRecentTransactions });

        // Budget Status Panel
        pnlBudgetStatus = new Panel
        {
            Location = new Point(600, 180),
            Size = new Size(560, 840),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        lblBudgetTitle = new Label
        {
            Text = "Budget Status",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(10, 10),
            AutoSize = true
        };

        flowBudgetStatus = new FlowLayoutPanel
        {
            Location = new Point(10, 45),
            Size = new Size(530, 780),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };

        pnlBudgetStatus.Controls.AddRange(new Control[] { lblBudgetTitle, flowBudgetStatus });

        // Spending Summary Panel
        pnlSpendingSummary = new Panel
        {
            Location = new Point(20, 1040),
            Size = new Size(1140, 450),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblSpendingSummaryTitle = new Label
        {
            Text = "Spending by Category (Last 30 Days)",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Location = new Point(10, 10),
            AutoSize = true
        };

        dgvSpendingSummary = new DataGridView
        {
            Location = new Point(10, 45),
            Size = new Size(1115, 390),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvSpendingSummary.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Category",
            HeaderText = "Category",
            Width = 200
        });
        dgvSpendingSummary.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "TransactionCount",
            HeaderText = "Transactions",
            Width = 120
        });
        dgvSpendingSummary.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "TotalAmount",
            HeaderText = "Total Amount",
            Width = 150
        });
        dgvSpendingSummary.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Percentage",
            HeaderText = "% of Total",
            Width = 120
        });

        pnlSpendingSummary.Controls.AddRange(new Control[] { lblSpendingSummaryTitle, dgvSpendingSummary });

        // Action Buttons
        btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(20, 1510),
            Size = new Size(150, 40),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += BtnRefresh_Click;

        btnViewTransactions = new Button
        {
            Text = "View All Transactions",
            Location = new Point(180, 1510),
            Size = new Size(180, 40),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnViewTransactions.FlatAppearance.BorderSize = 0;
        btnViewTransactions.Click += BtnViewTransactions_Click;

        btnViewBudgets = new Button
        {
            Text = "View Budgets",
            Location = new Point(370, 1510),
            Size = new Size(150, 40),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnViewBudgets.FlatAppearance.BorderSize = 0;
        btnViewBudgets.Click += BtnViewBudgets_Click;

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            pnlSummary,
            pnlRecentTransactions,
            pnlBudgetStatus,
            pnlSpendingSummary,
            btnRefresh,
            btnViewTransactions,
            btnViewBudgets
        });
    }

    /// <summary>
    /// Subscribe to events for auto-refresh
    /// Demonstrates Delegates & Events requirement
    /// </summary>
    private void SubscribeToEvents()
    {
        EventManager.TransactionAdded += OnDataChanged;
        EventManager.TransactionUpdated += OnDataChanged;
        EventManager.TransactionDeleted += OnDataChanged;
        EventManager.BudgetAdded += OnBudgetChanged;
        EventManager.BudgetUpdated += OnBudgetChanged;
        EventManager.BudgetDeleted += OnBudgetChanged;
        EventManager.CategoryAdded += OnCategoryChanged;
        EventManager.CategoryUpdated += OnCategoryChanged;
    }

    /// <summary>
    /// Unsubscribe from events when form closes
    /// </summary>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        EventManager.TransactionAdded -= OnDataChanged;
        EventManager.TransactionUpdated -= OnDataChanged;
        EventManager.TransactionDeleted -= OnDataChanged;
        EventManager.BudgetAdded -= OnBudgetChanged;
        EventManager.BudgetUpdated -= OnBudgetChanged;
        EventManager.BudgetDeleted -= OnBudgetChanged;
        EventManager.CategoryAdded -= OnCategoryChanged;
        EventManager.CategoryUpdated -= OnCategoryChanged;
        base.OnFormClosed(e);
    }

    private void OnDataChanged(object? sender, TransactionEventArgs e)
    {
        // Auto-refresh dashboard when transaction changes
        if (InvokeRequired)
        {
            Invoke(new Action(() => LoadDashboardData()));
        }
        else
        {
            LoadDashboardData();
        }
    }

    private void OnBudgetChanged(object? sender, BudgetEventArgs e)
    {
        // Auto-refresh dashboard when budget changes
        if (InvokeRequired)
        {
            Invoke(new Action(() => LoadDashboardData()));
        }
        else
        {
            LoadDashboardData();
        }
    }

    private void OnCategoryChanged(object? sender, CategoryEventArgs e)
    {
        // Refresh dashboard when category changes
        if (InvokeRequired)
        {
            Invoke(new Action(() => LoadDashboardData()));
        }
        else
        {
            LoadDashboardData();
        }
    }

    private void LoadDashboardData()
    {
        LoadSummary();
        LoadRecentTransactions();
        LoadBudgetStatus();
        LoadSpendingSummary();
    }

    private void LoadSummary()
    {
        try
        {
            var transactions = Program.TransactionRepository.GetAll();

            var totalIncome = transactions
                .Where(t => t is Income)
                .Sum(t => t.Amount.Amount);

            var totalExpense = transactions
                .Where(t => t is Expense)
                .Sum(t => t.Amount.Amount);

            var balance = totalIncome - totalExpense;

            lblTotalIncomeValue.Text = $"${totalIncome:N2}";
            lblTotalExpenseValue.Text = $"${totalExpense:N2}";
            lblTotalBalanceValue.Text = $"${balance:N2}";
            lblTotalBalanceValue.ForeColor = balance >= 0
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(244, 67, 54);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading summary: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRecentTransactions()
    {
        try
        {
            dgvRecentTransactions.Rows.Clear();

            var recentTransactions = Program.TransactionRepository.GetAll()
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .Take(10)
                .ToList();

            foreach (var transaction in recentTransactions)
            {
                var type = transaction is Income ? "+" : "-";
                var color = transaction is Income
                    ? Color.FromArgb(76, 175, 80)
                    : Color.FromArgb(244, 67, 54);

                var rowIndex = dgvRecentTransactions.Rows.Add(
                    transaction.Date.ToShortDateString(),
                    transaction.Description,
                    $"{type}${transaction.Amount.Amount:N2}"
                );

                dgvRecentTransactions.Rows[rowIndex].Cells["Amount"].Style.ForeColor = color;
                dgvRecentTransactions.Rows[rowIndex].Cells["Amount"].Style.Font =
                    new Font(dgvRecentTransactions.Font, FontStyle.Bold);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading recent transactions: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadBudgetStatus()
    {
        try
        {
            flowBudgetStatus.Controls.Clear();

            var budgetService = new BudgetService(
                Program.BudgetRepository,
                Program.TransactionRepository,
                Program.CategoryRepository
            );

            var budgets = budgetService.GetAllActiveBudgetsWithSpending()
                .OrderByDescending(b => b.PercentageUsed)
                .Take(5)
                .ToList();

            if (!budgets.Any())
            {
                var noDataLabel = new Label
                {
                    Text = "No active budgets",
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                flowBudgetStatus.Controls.Add(noDataLabel);
                return;
            }

            foreach (var budget in budgets)
            {
                var budgetCard = CreateBudgetStatusCard(budget);
                flowBudgetStatus.Controls.Add(budgetCard);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading budget status: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Panel CreateBudgetStatusCard(BudgetSummary budget)
    {
        var card = new Panel
        {
            Size = new Size(510, 100),
            Margin = new Padding(5, 10, 5, 10),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            Padding = new Padding(10)
        };

        var lblName = new Label
        {
            Text = budget.BudgetName,
            Location = new Point(10, 10),
            Size = new Size(350, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            AutoEllipsis = true
        };

        var lblAmount = new Label
        {
            Text = $"${budget.ActualSpending:N2} / ${budget.BudgetAmount:N2}",
            Location = new Point(10, 40),
            Size = new Size(480, 22),
            Font = new Font("Segoe UI", 10),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var progressBar = new ProgressBar
        {
            Location = new Point(10, 68),
            Size = new Size(480, 22),
            Minimum = 0,
            Maximum = 100,
            Value = Math.Min((int)budget.PercentageUsed, 100)
        };

        if (budget.IsExceeded)
        {
            progressBar.ForeColor = Color.FromArgb(244, 67, 54);
        }
        else if (budget.IsWarning)
        {
            progressBar.ForeColor = Color.FromArgb(255, 152, 0);
        }
        else
        {
            progressBar.ForeColor = Color.FromArgb(76, 175, 80);
        }

        card.Controls.AddRange(new Control[] { lblName, lblAmount, progressBar });

        return card;
    }

    private void LoadSpendingSummary()
    {
        try
        {
            dgvSpendingSummary.Rows.Clear();

            // Get transactions for last 30 days
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-29);

            var expenses = Program.TransactionRepository.GetAll()
                .Where(t => t is Expense)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            if (!expenses.Any())
            {
                var rowIndex = dgvSpendingSummary.Rows.Add(
                    "No spending data",
                    "-",
                    "-",
                    "-"
                );
                dgvSpendingSummary.Rows[rowIndex].DefaultCellStyle.Font =
                    new Font(dgvSpendingSummary.Font, FontStyle.Italic);
                dgvSpendingSummary.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                return;
            }

            var totalSpending = expenses.Sum(t => t.Amount.Amount);

            // Group by category and calculate spending
            var spendingByCategory = expenses
                .GroupBy(t => t.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = Program.CategoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount.Amount),
                    Percentage = (g.Sum(t => t.Amount.Amount) / totalSpending) * 100
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            foreach (var category in spendingByCategory)
            {
                dgvSpendingSummary.Rows.Add(
                    category.CategoryName,
                    category.TransactionCount.ToString(),
                    $"${category.TotalAmount:N2}",
                    $"{category.Percentage:F1}%"
                );
            }

            // Add total row
            var totalRowIndex = dgvSpendingSummary.Rows.Add(
                "TOTAL",
                expenses.Count.ToString(),
                $"${totalSpending:N2}",
                "100.0%"
            );

            dgvSpendingSummary.Rows[totalRowIndex].DefaultCellStyle.Font =
                new Font(dgvSpendingSummary.Font, FontStyle.Bold);
            dgvSpendingSummary.Rows[totalRowIndex].DefaultCellStyle.BackColor =
                Color.FromArgb(240, 240, 240);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading spending summary: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        LoadDashboardData();
        MessageBox.Show("Dashboard refreshed!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnViewTransactions_Click(object? sender, EventArgs e)
    {
        var transactionsForm = new TransactionsForm();
        transactionsForm.ShowDialog();
    }

    private void BtnViewBudgets_Click(object? sender, EventArgs e)
    {
        var budgetsForm = new BudgetsForm();
        budgetsForm.ShowDialog();
    }
}
