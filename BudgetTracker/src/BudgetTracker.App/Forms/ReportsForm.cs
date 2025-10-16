using BudgetTracker.Domain.Entities;
using BudgetTracker.Core.Services;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Reports form for analytics and insights
/// Phase 5.14: ReportsForm Implementation
/// Demo Step 5: Reports & Analysis
/// </summary>
public partial class ReportsForm : Form
{
    private Label lblTitle;
    private TabControl tabReports;

    // Spending Report Tab
    private TabPage tabSpending;
    private Panel pnlSpendingFilters;
    private Label lblSpendingDateFrom;
    private DateTimePicker dtpSpendingFrom;
    private Label lblSpendingDateTo;
    private DateTimePicker dtpSpendingTo;
    private Button btnGenerateSpending;
    private DataGridView dgvSpendingReport;

    // Income Report Tab
    private TabPage tabIncome;
    private Panel pnlIncomeFilters;
    private Label lblIncomeDateFrom;
    private DateTimePicker dtpIncomeFrom;
    private Label lblIncomeDateTo;
    private DateTimePicker dtpIncomeTo;
    private Button btnGenerateIncome;
    private DataGridView dgvIncomeReport;

    // Budget vs Actual Tab
    private TabPage tabBudgetComparison;
    private Panel pnlBudgetFilters;
    private ComboBox cmbBudgetPeriod;
    private Label lblBudgetPeriod;
    private Button btnGenerateBudgetComparison;
    private DataGridView dgvBudgetComparison;

    // Trend Analysis Tab
    private TabPage tabTrend;
    private Panel pnlTrendFilters;
    private Label lblTrendDateFrom;
    private DateTimePicker dtpTrendFrom;
    private Label lblTrendDateTo;
    private DateTimePicker dtpTrendTo;
    private ComboBox cmbTrendGroupBy;
    private Label lblTrendGroupBy;
    private Button btnGenerateTrend;
    private DataGridView dgvTrendReport;

    // Action Buttons
    private Button btnExportCSV;
    private Button btnClose;

    public ReportsForm()
    {
        InitializeComponent();
        InitializeFilters();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = "Budget Tracker - Reports & Analytics";
        this.Size = new Size(1200, 900);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1000, 700);

        // Title
        lblTitle = new Label
        {
            Text = "Reports & Analytics",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // Tab Control
        tabReports = new TabControl
        {
            Location = new Point(20, 65),
            Size = new Size(1140, 750),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        // Initialize Tabs
        InitializeSpendingTab();
        InitializeIncomeTab();
        InitializeBudgetComparisonTab();
        InitializeTrendTab();

        tabReports.TabPages.AddRange(new TabPage[]
        {
            tabSpending,
            tabIncome,
            tabBudgetComparison,
            tabTrend
        });

        // Export CSV Button
        btnExportCSV = new Button
        {
            Text = "Export to CSV",
            Location = new Point(20, 830),
            Size = new Size(150, 40),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnExportCSV.FlatAppearance.BorderSize = 0;
        btnExportCSV.Click += BtnExportCSV_Click;

        // Close Button
        btnClose = new Button
        {
            Text = "Close",
            Location = new Point(180, 830),
            Size = new Size(120, 40),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackColor = Color.FromArgb(158, 158, 158),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (s, e) => this.Close();

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            tabReports,
            btnExportCSV,
            btnClose
        });
    }

    private void InitializeSpendingTab()
    {
        tabSpending = new TabPage("Spending by Category");

        // Filter Panel
        pnlSpendingFilters = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(1100, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(240, 248, 255)
        };

        lblSpendingDateFrom = new Label
        {
            Text = "From:",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpSpendingFrom = new DateTimePicker
        {
            Location = new Point(70, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        lblSpendingDateTo = new Label
        {
            Text = "To:",
            Location = new Point(290, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpSpendingTo = new DateTimePicker
        {
            Location = new Point(325, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        btnGenerateSpending = new Button
        {
            Text = "Generate Report",
            Location = new Point(550, 18),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerateSpending.FlatAppearance.BorderSize = 0;
        btnGenerateSpending.Click += BtnGenerateSpending_Click;

        pnlSpendingFilters.Controls.AddRange(new Control[]
        {
            lblSpendingDateFrom, dtpSpendingFrom,
            lblSpendingDateTo, dtpSpendingTo,
            btnGenerateSpending
        });

        // DataGridView
        dgvSpendingReport = new DataGridView
        {
            Location = new Point(10, 100),
            Size = new Size(1100, 600),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvSpendingReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 });
        dgvSpendingReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "TransactionCount", HeaderText = "# Transactions", Width = 120 });
        dgvSpendingReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total Amount", Width = 150 });
        dgvSpendingReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "AverageAmount", HeaderText = "Avg Amount", Width = 150 });
        dgvSpendingReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Percentage", HeaderText = "% of Total", Width = 120 });

        tabSpending.Controls.AddRange(new Control[] { pnlSpendingFilters, dgvSpendingReport });
    }

    private void InitializeIncomeTab()
    {
        tabIncome = new TabPage("Income Report");

        // Filter Panel
        pnlIncomeFilters = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(1100, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(240, 248, 255)
        };

        lblIncomeDateFrom = new Label
        {
            Text = "From:",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpIncomeFrom = new DateTimePicker
        {
            Location = new Point(70, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        lblIncomeDateTo = new Label
        {
            Text = "To:",
            Location = new Point(290, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpIncomeTo = new DateTimePicker
        {
            Location = new Point(325, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        btnGenerateIncome = new Button
        {
            Text = "Generate Report",
            Location = new Point(550, 18),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerateIncome.FlatAppearance.BorderSize = 0;
        btnGenerateIncome.Click += BtnGenerateIncome_Click;

        pnlIncomeFilters.Controls.AddRange(new Control[]
        {
            lblIncomeDateFrom, dtpIncomeFrom,
            lblIncomeDateTo, dtpIncomeTo,
            btnGenerateIncome
        });

        // DataGridView
        dgvIncomeReport = new DataGridView
        {
            Location = new Point(10, 100),
            Size = new Size(1100, 600),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvIncomeReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 200 });
        dgvIncomeReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "TransactionCount", HeaderText = "# Transactions", Width = 120 });
        dgvIncomeReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Total Amount", Width = 150 });
        dgvIncomeReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "AverageAmount", HeaderText = "Avg Amount", Width = 150 });
        dgvIncomeReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Percentage", HeaderText = "% of Total", Width = 120 });

        tabIncome.Controls.AddRange(new Control[] { pnlIncomeFilters, dgvIncomeReport });
    }

    private void InitializeBudgetComparisonTab()
    {
        tabBudgetComparison = new TabPage("Budget vs Actual");

        // Filter Panel
        pnlBudgetFilters = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(1100, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(240, 248, 255)
        };

        lblBudgetPeriod = new Label
        {
            Text = "Period:",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        cmbBudgetPeriod = new ComboBox
        {
            Location = new Point(80, 22),
            Size = new Size(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbBudgetPeriod.Items.AddRange(new object[] { "Current Month", "Last Month", "Current Year", "All Active" });
        cmbBudgetPeriod.SelectedIndex = 0;

        btnGenerateBudgetComparison = new Button
        {
            Text = "Generate Report",
            Location = new Point(300, 18),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerateBudgetComparison.FlatAppearance.BorderSize = 0;
        btnGenerateBudgetComparison.Click += BtnGenerateBudgetComparison_Click;

        pnlBudgetFilters.Controls.AddRange(new Control[]
        {
            lblBudgetPeriod, cmbBudgetPeriod,
            btnGenerateBudgetComparison
        });

        // DataGridView
        dgvBudgetComparison = new DataGridView
        {
            Location = new Point(10, 100),
            Size = new Size(1100, 600),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudgetName", HeaderText = "Budget", Width = 200 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", Width = 150 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "BudgetAmount", HeaderText = "Budgeted", Width = 120 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "ActualAmount", HeaderText = "Actual", Width = 120 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "Difference", HeaderText = "Difference", Width = 120 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "Percentage", HeaderText = "% Used", Width = 100 });
        dgvBudgetComparison.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 });

        tabBudgetComparison.Controls.AddRange(new Control[] { pnlBudgetFilters, dgvBudgetComparison });
    }

    private void InitializeTrendTab()
    {
        tabTrend = new TabPage("Trend Analysis");

        // Filter Panel
        pnlTrendFilters = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(1100, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.FromArgb(240, 248, 255)
        };

        lblTrendDateFrom = new Label
        {
            Text = "From:",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpTrendFrom = new DateTimePicker
        {
            Location = new Point(70, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        lblTrendDateTo = new Label
        {
            Text = "To:",
            Location = new Point(290, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        dtpTrendTo = new DateTimePicker
        {
            Location = new Point(325, 22),
            Size = new Size(200, 25),
            Format = DateTimePickerFormat.Short
        };

        lblTrendGroupBy = new Label
        {
            Text = "Group By:",
            Location = new Point(545, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };

        cmbTrendGroupBy = new ComboBox
        {
            Location = new Point(620, 22),
            Size = new Size(120, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbTrendGroupBy.Items.AddRange(new object[] { "Daily", "Weekly", "Monthly" });
        cmbTrendGroupBy.SelectedIndex = 2;

        btnGenerateTrend = new Button
        {
            Text = "Generate Report",
            Location = new Point(760, 18),
            Size = new Size(150, 35),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGenerateTrend.FlatAppearance.BorderSize = 0;
        btnGenerateTrend.Click += BtnGenerateTrend_Click;

        pnlTrendFilters.Controls.AddRange(new Control[]
        {
            lblTrendDateFrom, dtpTrendFrom,
            lblTrendDateTo, dtpTrendTo,
            lblTrendGroupBy, cmbTrendGroupBy,
            btnGenerateTrend
        });

        // DataGridView
        dgvTrendReport = new DataGridView
        {
            Location = new Point(10, 100),
            Size = new Size(1100, 600),
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        dgvTrendReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Period", HeaderText = "Period", Width = 150 });
        dgvTrendReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Income", HeaderText = "Income", Width = 150 });
        dgvTrendReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "Expense", HeaderText = "Expense", Width = 150 });
        dgvTrendReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "NetAmount", HeaderText = "Net Amount", Width = 150 });
        dgvTrendReport.Columns.Add(new DataGridViewTextBoxColumn { Name = "TransactionCount", HeaderText = "# Transactions", Width = 120 });

        tabTrend.Controls.AddRange(new Control[] { pnlTrendFilters, dgvTrendReport });
    }

    private void InitializeFilters()
    {
        // Set default date ranges
        var today = DateTime.Today;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        dtpSpendingFrom.Value = firstDayOfMonth;
        dtpSpendingTo.Value = today;

        dtpIncomeFrom.Value = firstDayOfMonth;
        dtpIncomeTo.Value = today;

        dtpTrendFrom.Value = firstDayOfMonth.AddMonths(-2);
        dtpTrendTo.Value = today;
    }

    private void BtnGenerateSpending_Click(object? sender, EventArgs e)
    {
        try
        {
            dgvSpendingReport.Rows.Clear();

            var startDate = dtpSpendingFrom.Value.Date;
            var endDate = dtpSpendingTo.Value.Date;

            // LINQ + Lambda: Spending by category report
            var expenses = Program.TransactionRepository.GetAll()
                .Where(t => t is Expense)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            if (!expenses.Any())
            {
                MessageBox.Show("No expense data found for the selected period.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var totalSpending = expenses.Sum(t => t.Amount.Amount);

            // LINQ GroupBy with aggregation
            var categoryReport = expenses
                .GroupBy(t => t.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = Program.CategoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount.Amount),
                    AverageAmount = g.Average(t => t.Amount.Amount),
                    Percentage = (g.Sum(t => t.Amount.Amount) / totalSpending) * 100
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            foreach (var category in categoryReport)
            {
                dgvSpendingReport.Rows.Add(
                    category.CategoryName,
                    category.TransactionCount,
                    $"${category.TotalAmount:N2}",
                    $"${category.AverageAmount:N2}",
                    $"{category.Percentage:F1}%"
                );
            }

            // Add total row
            var totalRowIndex = dgvSpendingReport.Rows.Add(
                "TOTAL",
                expenses.Count,
                $"${totalSpending:N2}",
                $"${expenses.Average(t => t.Amount.Amount):N2}",
                "100.0%"
            );

            dgvSpendingReport.Rows[totalRowIndex].DefaultCellStyle.Font =
                new Font(dgvSpendingReport.Font, FontStyle.Bold);
            dgvSpendingReport.Rows[totalRowIndex].DefaultCellStyle.BackColor =
                Color.FromArgb(240, 240, 240);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating spending report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGenerateIncome_Click(object? sender, EventArgs e)
    {
        try
        {
            dgvIncomeReport.Rows.Clear();

            var startDate = dtpIncomeFrom.Value.Date;
            var endDate = dtpIncomeTo.Value.Date;

            // LINQ + Lambda: Income report
            var incomes = Program.TransactionRepository.GetAll()
                .Where(t => t is Income)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            if (!incomes.Any())
            {
                MessageBox.Show("No income data found for the selected period.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var totalIncome = incomes.Sum(t => t.Amount.Amount);

            // LINQ GroupBy for income by category
            var categoryReport = incomes
                .GroupBy(t => t.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = Program.CategoryRepository.GetById(g.Key)?.Name ?? "Unknown",
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount.Amount),
                    AverageAmount = g.Average(t => t.Amount.Amount),
                    Percentage = (g.Sum(t => t.Amount.Amount) / totalIncome) * 100
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            foreach (var category in categoryReport)
            {
                dgvIncomeReport.Rows.Add(
                    category.CategoryName,
                    category.TransactionCount,
                    $"${category.TotalAmount:N2}",
                    $"${category.AverageAmount:N2}",
                    $"{category.Percentage:F1}%"
                );
            }

            // Add total row
            var totalRowIndex = dgvIncomeReport.Rows.Add(
                "TOTAL",
                incomes.Count,
                $"${totalIncome:N2}",
                $"${incomes.Average(t => t.Amount.Amount):N2}",
                "100.0%"
            );

            dgvIncomeReport.Rows[totalRowIndex].DefaultCellStyle.Font =
                new Font(dgvIncomeReport.Font, FontStyle.Bold);
            dgvIncomeReport.Rows[totalRowIndex].DefaultCellStyle.BackColor =
                Color.FromArgb(240, 240, 240);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating income report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGenerateBudgetComparison_Click(object? sender, EventArgs e)
    {
        try
        {
            dgvBudgetComparison.Rows.Clear();

            var budgetService = new BudgetService(
                Program.BudgetRepository,
                Program.TransactionRepository,
                Program.CategoryRepository
            );

            var budgets = budgetService.GetAllActiveBudgetsWithSpending()
                .OrderByDescending(b => b.PercentageUsed)
                .ToList();

            if (!budgets.Any())
            {
                MessageBox.Show("No active budgets found.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var budget in budgets)
            {
                var difference = budget.BudgetAmount - budget.ActualSpending;
                var status = budget.IsExceeded ? "Exceeded" : budget.IsWarning ? "Warning" : "On Track";
                var statusColor = budget.IsExceeded ? Color.FromArgb(244, 67, 54) :
                                 budget.IsWarning ? Color.FromArgb(255, 152, 0) :
                                 Color.FromArgb(76, 175, 80);

                var rowIndex = dgvBudgetComparison.Rows.Add(
                    budget.BudgetName,
                    budget.CategoryName,
                    $"${budget.BudgetAmount:N2}",
                    $"${budget.ActualSpending:N2}",
                    $"${difference:N2}",
                    $"{budget.PercentageUsed:F1}%",
                    status
                );

                dgvBudgetComparison.Rows[rowIndex].Cells["Status"].Style.ForeColor = statusColor;
                dgvBudgetComparison.Rows[rowIndex].Cells["Status"].Style.Font =
                    new Font(dgvBudgetComparison.Font, FontStyle.Bold);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating budget comparison report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnGenerateTrend_Click(object? sender, EventArgs e)
    {
        try
        {
            dgvTrendReport.Rows.Clear();

            var startDate = dtpTrendFrom.Value.Date;
            var endDate = dtpTrendTo.Value.Date;
            var groupBy = cmbTrendGroupBy.SelectedItem?.ToString() ?? "Monthly";

            var transactions = Program.TransactionRepository.GetAll()
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList();

            if (!transactions.Any())
            {
                MessageBox.Show("No transaction data found for the selected period.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // LINQ GroupBy for trend analysis
            IEnumerable<IGrouping<string, Transaction>> groupedTransactions;

            if (groupBy == "Daily")
            {
                groupedTransactions = transactions
                    .GroupBy(t => t.Date.ToString("yyyy-MM-dd"));
            }
            else if (groupBy == "Weekly")
            {
                groupedTransactions = transactions
                    .GroupBy(t => $"{t.Date.Year}-W{GetWeekNumber(t.Date):00}");
            }
            else // Monthly
            {
                groupedTransactions = transactions
                    .GroupBy(t => t.Date.ToString("yyyy-MM"));
            }

            var trendReport = groupedTransactions
                .Select(g => new
                {
                    Period = g.Key,
                    Income = g.Where(t => t is Income).Sum(t => t.Amount.Amount),
                    Expense = g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                    NetAmount = g.Where(t => t is Income).Sum(t => t.Amount.Amount) -
                               g.Where(t => t is Expense).Sum(t => t.Amount.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.Period)
                .ToList();

            foreach (var period in trendReport)
            {
                var rowIndex = dgvTrendReport.Rows.Add(
                    period.Period,
                    $"${period.Income:N2}",
                    $"${period.Expense:N2}",
                    $"${period.NetAmount:N2}",
                    period.TransactionCount
                );

                // Color code net amount
                var netColor = period.NetAmount >= 0 ? Color.FromArgb(76, 175, 80) : Color.FromArgb(244, 67, 54);
                dgvTrendReport.Rows[rowIndex].Cells["NetAmount"].Style.ForeColor = netColor;
                dgvTrendReport.Rows[rowIndex].Cells["NetAmount"].Style.Font =
                    new Font(dgvTrendReport.Font, FontStyle.Bold);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating trend report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private int GetWeekNumber(DateTime date)
    {
        var currentCulture = System.Globalization.CultureInfo.CurrentCulture;
        var weekNum = currentCulture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        return weekNum;
    }

    private void BtnExportCSV_Click(object? sender, EventArgs e)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Export Report to CSV",
                FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // Determine which tab is active and export that data
                DataGridView? currentGrid = null;

                if (tabReports.SelectedTab == tabSpending)
                    currentGrid = dgvSpendingReport;
                else if (tabReports.SelectedTab == tabIncome)
                    currentGrid = dgvIncomeReport;
                else if (tabReports.SelectedTab == tabBudgetComparison)
                    currentGrid = dgvBudgetComparison;
                else if (tabReports.SelectedTab == tabTrend)
                    currentGrid = dgvTrendReport;

                if (currentGrid == null || currentGrid.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export. Please generate a report first.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ExportGridToCSV(currentGrid, saveDialog.FileName);

                MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting report: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportGridToCSV(DataGridView grid, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            // Write headers
            var headers = new List<string>();
            foreach (DataGridViewColumn column in grid.Columns)
            {
                headers.Add(column.HeaderText);
            }
            writer.WriteLine(string.Join(",", headers));

            // Write data rows
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;

                var values = new List<string>();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    var value = cell.Value?.ToString() ?? "";
                    // Escape commas and quotes
                    if (value.Contains(",") || value.Contains("\""))
                    {
                        value = $"\"{value.Replace("\"", "\"\"")}\"";
                    }
                    values.Add(value);
                }
                writer.WriteLine(string.Join(",", values));
            }
        }
    }
}
