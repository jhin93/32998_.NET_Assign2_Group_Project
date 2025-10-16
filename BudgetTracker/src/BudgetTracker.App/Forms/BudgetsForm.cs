using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.ValueObjects;
using BudgetTracker.Core.Events;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Budgets form for managing budgets and tracking spending
/// Demo Step 3: Budget setup and monitoring
/// Phase 3.9: BudgetsForm Implementation
/// </summary>
public partial class BudgetsForm : Form
{
    private Label lblTitle;
    private Panel pnlBudgets;
    private FlowLayoutPanel flowBudgetCards;
    private Panel pnlActions;
    private Button btnAddBudget;
    private Button btnEditBudget;
    private Button btnDeleteBudget;
    private Button btnRefresh;
    private Button btnTemplates;
    private ComboBox cmbFilterStatus;
    private Label lblFilterStatus;

    private Budget? _selectedBudget;

    public BudgetsForm()
    {
        InitializeComponent();
        LoadBudgets();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = "Budget Tracker - Budgets";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 600);

        // Title Label
        lblTitle = new Label
        {
            Text = "Budget Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, 20),
            AutoSize = true
        };

        // Filter Status Label
        lblFilterStatus = new Label
        {
            Text = "Filter:",
            Location = new Point(700, 25),
            Size = new Size(50, 25),
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };

        // Filter Status ComboBox
        cmbFilterStatus = new ComboBox
        {
            Location = new Point(755, 22),
            Size = new Size(200, 25),
            Font = new Font("Segoe UI", 10),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFilterStatus.Items.AddRange(new object[] { "All", "Active", "Exceeded", "Warning" });
        cmbFilterStatus.SelectedIndex = 1; // Default to "Active"
        cmbFilterStatus.SelectedIndexChanged += CmbFilterStatus_SelectedIndexChanged;

        // Budgets Panel with FlowLayoutPanel for cards
        pnlBudgets = new Panel
        {
            Location = new Point(20, 60),
            Size = new Size(940, 490),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true
        };

        flowBudgetCards = new FlowLayoutPanel
        {
            Location = new Point(0, 0),
            Size = new Size(920, 470),
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(10)
        };

        pnlBudgets.Controls.Add(flowBudgetCards);

        // Actions Panel
        pnlActions = new Panel
        {
            Location = new Point(20, 560),
            Size = new Size(940, 80),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        // Add Budget Button
        btnAddBudget = new Button
        {
            Text = "Add Budget",
            Location = new Point(0, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAddBudget.FlatAppearance.BorderSize = 0;
        btnAddBudget.Click += BtnAddBudget_Click;

        // Edit Budget Button
        btnEditBudget = new Button
        {
            Text = "Edit Budget",
            Location = new Point(190, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnEditBudget.FlatAppearance.BorderSize = 0;
        btnEditBudget.Click += BtnEditBudget_Click;

        // Delete Budget Button
        btnDeleteBudget = new Button
        {
            Text = "Delete Budget",
            Location = new Point(380, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnDeleteBudget.FlatAppearance.BorderSize = 0;
        btnDeleteBudget.Click += BtnDeleteBudget_Click;

        // Templates Button
        btnTemplates = new Button
        {
            Text = "Budget Templates",
            Location = new Point(570, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnTemplates.FlatAppearance.BorderSize = 0;
        btnTemplates.Click += BtnTemplates_Click;

        // Refresh Button
        btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(760, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Click += BtnRefresh_Click;

        pnlActions.Controls.AddRange(new Control[]
        {
            btnAddBudget,
            btnEditBudget,
            btnDeleteBudget,
            btnTemplates,
            btnRefresh
        });

        // Add all controls to form
        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            lblFilterStatus,
            cmbFilterStatus,
            pnlBudgets,
            pnlActions
        });
    }

    private void LoadBudgets()
    {
        try
        {
            flowBudgetCards.Controls.Clear();

            var budgets = Program.BudgetRepository.GetAll().ToList();

            // Apply filter
            var filterStatus = cmbFilterStatus.SelectedItem?.ToString() ?? "Active";
            budgets = FilterBudgets(budgets, filterStatus);

            // Update spending for each budget using LINQ
            UpdateBudgetSpending(budgets);

            if (!budgets.Any())
            {
                var noDataLabel = new Label
                {
                    Text = "No budgets found. Click 'Add Budget' to create one.",
                    Font = new Font("Segoe UI", 12, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                flowBudgetCards.Controls.Add(noDataLabel);
                return;
            }

            // Create budget cards
            foreach (var budget in budgets.OrderByDescending(b => b.IsCurrentlyActive()).ThenBy(b => b.Name))
            {
                var card = CreateBudgetCard(budget);
                flowBudgetCards.Controls.Add(card);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading budgets: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private List<Budget> FilterBudgets(List<Budget> budgets, string filterStatus)
    {
        return filterStatus switch
        {
            "Active" => budgets.Where(b => b.IsCurrentlyActive()).ToList(),
            "Exceeded" => budgets.Where(b => b.IsExceeded).ToList(),
            "Warning" => budgets.Where(b => b.IsWarning).ToList(),
            _ => budgets
        };
    }

    private void UpdateBudgetSpending(List<Budget> budgets)
    {
        var transactions = Program.TransactionRepository.GetAll()
            .Where(t => t is Expense)
            .ToList();

        foreach (var budget in budgets)
        {
            // Reset current spent
            budget.Reset();

            // Calculate spending for this budget using LINQ
            var spending = transactions
                .Where(t => t.CategoryId == budget.CategoryId &&
                           t.Date >= budget.StartDate &&
                           t.Date <= budget.EndDate)
                .Sum(t => t.Amount.Amount);

            if (spending > 0)
            {
                budget.AddSpending(new Money(spending, "USD"));
            }
        }
    }

    private Panel CreateBudgetCard(Budget budget)
    {
        var category = Program.CategoryRepository.GetById(budget.CategoryId);

        var card = new Panel
        {
            Size = new Size(900, 140),
            Margin = new Padding(5),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            Cursor = Cursors.Hand
        };

        // Budget name and category
        var lblBudgetName = new Label
        {
            Text = budget.Name,
            Location = new Point(15, 15),
            Size = new Size(400, 25),
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };

        var lblCategoryName = new Label
        {
            Text = $"Category: {category?.Name ?? "Unknown"}",
            Location = new Point(15, 45),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.Gray
        };

        // Period
        var lblPeriod = new Label
        {
            Text = $"Period: {budget.StartDate:MMM dd, yyyy} - {budget.EndDate:MMM dd, yyyy}",
            Location = new Point(15, 70),
            Size = new Size(400, 20),
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Gray
        };

        // Amount info
        var lblAmount = new Label
        {
            Text = $"${budget.CurrentSpent.Amount:N2} / ${budget.Amount.Amount:N2}",
            Location = new Point(720, 15),
            Size = new Size(160, 25),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight
        };

        // Progress bar
        var progressBar = new ProgressBar
        {
            Location = new Point(15, 100),
            Size = new Size(770, 25),
            Minimum = 0,
            Maximum = 100,
            Value = Math.Min((int)budget.PercentageUsed, 100)
        };

        // Set progress bar color based on status
        if (budget.IsExceeded)
        {
            progressBar.ForeColor = Color.FromArgb(244, 67, 54); // Red
        }
        else if (budget.IsWarning)
        {
            progressBar.ForeColor = Color.FromArgb(255, 152, 0); // Orange
        }
        else
        {
            progressBar.ForeColor = Color.FromArgb(76, 175, 80); // Green
        }

        // Status label
        var lblStatus = new Label
        {
            Text = budget.GetStatusMessage(),
            Location = new Point(795, 100),
            Size = new Size(90, 25),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        if (budget.IsExceeded)
        {
            lblStatus.ForeColor = Color.FromArgb(244, 67, 54);
        }
        else if (budget.IsWarning)
        {
            lblStatus.ForeColor = Color.FromArgb(255, 152, 0);
        }
        else
        {
            lblStatus.ForeColor = Color.FromArgb(76, 175, 80);
        }

        // Add click event to select budget
        card.Click += (s, e) => SelectBudget(budget, card);
        foreach (Control ctrl in card.Controls)
        {
            ctrl.Click += (s, e) => SelectBudget(budget, card);
        }

        card.Controls.AddRange(new Control[]
        {
            lblBudgetName,
            lblCategoryName,
            lblPeriod,
            lblAmount,
            progressBar,
            lblStatus
        });

        // Store budget reference in Tag
        card.Tag = budget;

        return card;
    }

    private void SelectBudget(Budget budget, Panel card)
    {
        _selectedBudget = budget;

        // Highlight selected card
        foreach (Panel c in flowBudgetCards.Controls.OfType<Panel>())
        {
            c.BackColor = Color.White;
        }
        card.BackColor = Color.FromArgb(230, 240, 255);
    }

    private void CmbFilterStatus_SelectedIndexChanged(object? sender, EventArgs e)
    {
        LoadBudgets();
    }

    private void BtnAddBudget_Click(object? sender, EventArgs e)
    {
        var dialog = new BudgetDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var money = new Money(dialog.Amount, "USD");

                var budget = new Budget(
                    dialog.BudgetName,
                    money,
                    dialog.CategoryId,
                    dialog.StartDate,
                    dialog.EndDate
                );

                Program.BudgetRepository.Add(budget);
                Program.BudgetRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnBudgetAdded(budget);

                LoadBudgets();

                MessageBox.Show("Budget added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding budget: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnEditBudget_Click(object? sender, EventArgs e)
    {
        if (_selectedBudget == null)
        {
            MessageBox.Show("Please select a budget to edit.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var dialog = new BudgetDialog(_selectedBudget);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var money = new Money(dialog.Amount, "USD");

                // Update budget properties
                _selectedBudget.Name = dialog.BudgetName;
                _selectedBudget.UpdateAmount(money);
                _selectedBudget.UpdatePeriod(dialog.StartDate, dialog.EndDate);
                _selectedBudget.CategoryId = dialog.CategoryId;

                Program.BudgetRepository.Update(_selectedBudget);
                Program.BudgetRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnBudgetUpdated(_selectedBudget);

                LoadBudgets();

                MessageBox.Show("Budget updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating budget: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnDeleteBudget_Click(object? sender, EventArgs e)
    {
        if (_selectedBudget == null)
        {
            MessageBox.Show("Please select a budget to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete the budget '{_selectedBudget.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                var budgetToDelete = _selectedBudget;
                Program.BudgetRepository.Remove(_selectedBudget);
                Program.BudgetRepository.SaveChanges();

                // Raise event for inter-form communication
                EventManager.OnBudgetDeleted(budgetToDelete);

                _selectedBudget = null;
                LoadBudgets();

                MessageBox.Show("Budget deleted successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting budget: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnTemplates_Click(object? sender, EventArgs e)
    {
        var templateChoice = MessageBox.Show(
            "Create budget templates for common categories?\n\n" +
            "This will create monthly budgets for:\n" +
            "- Food: $500\n" +
            "- Transportation: $200\n" +
            "- Entertainment: $150\n" +
            "- Utilities: $300\n\n" +
            "Do you want to create these templates?",
            "Budget Templates",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (templateChoice == DialogResult.Yes)
        {
            try
            {
                var categories = Program.CategoryRepository.GetAll().ToList();
                var today = DateTime.Today;
                var templatesCreated = 0;

                // Template definitions
                var templates = new Dictionary<string, decimal>
                {
                    { "Food", 500m },
                    { "Transportation", 200m },
                    { "Entertainment", 150m },
                    { "Utilities", 300m }
                };

                foreach (var template in templates)
                {
                    var category = categories.FirstOrDefault(c =>
                        c.Name.Equals(template.Key, StringComparison.OrdinalIgnoreCase));

                    if (category != null)
                    {
                        // Check if budget already exists for this category
                        var existingBudget = Program.BudgetRepository.GetAll()
                            .FirstOrDefault(b => b.CategoryId == category.Id && b.IsCurrentlyActive());

                        if (existingBudget == null)
                        {
                            var budget = Budget.CreateMonthly(
                                $"{category.Name} - {today:MMM yyyy}",
                                template.Value,
                                category.Id,
                                today
                            );

                            Program.BudgetRepository.Add(budget);
                            templatesCreated++;
                        }
                    }
                }

                if (templatesCreated > 0)
                {
                    Program.BudgetRepository.SaveChanges();
                    LoadBudgets();

                    MessageBox.Show($"Created {templatesCreated} budget template(s) successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No templates were created. Budgets may already exist for these categories.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating templates: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        LoadBudgets();
        MessageBox.Show("Budgets refreshed!", "Success",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
