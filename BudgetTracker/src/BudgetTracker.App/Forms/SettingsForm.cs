using BudgetTracker.Domain.Entities;
using BudgetTracker.Core.Services;

namespace BudgetTracker.App.Forms;

/// <summary>
/// Settings form for managing categories and application settings
/// Demo Step 1: Initial setup and configuration
/// </summary>
public partial class SettingsForm : Form
{
    private DataGridView dgvCategories;
    private Button btnAddCategory;
    private Button btnEditCategory;
    private Button btnDeleteCategory;
    private Button btnImportCSV;
    private Button btnExportCSV;
    private Label lblTitle;
    private Panel pnlCategories;
    private Panel pnlActions;

    public SettingsForm()
    {
        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        // Form settings
        this.Text = "Budget Tracker - Settings";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(600, 400);

        // Title Label
        lblTitle = new Label
        {
            Text = "Category Management",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Location = new Point(20, 20),
            Size = new Size(300, 35),
            AutoSize = true
        };

        // Categories Panel
        pnlCategories = new Panel
        {
            Location = new Point(20, 70),
            Size = new Size(740, 400),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            BorderStyle = BorderStyle.FixedSingle
        };

        // DataGridView for Categories
        dgvCategories = new DataGridView
        {
            Location = new Point(0, 0),
            Size = new Size(740, 400),
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
        dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Id",
            HeaderText = "ID",
            DataPropertyName = "Id",
            Width = 50,
            ReadOnly = true
        });

        dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Name",
            HeaderText = "Name",
            DataPropertyName = "Name",
            ReadOnly = true
        });

        dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Description",
            HeaderText = "Description",
            DataPropertyName = "Description",
            ReadOnly = true
        });

        dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Color",
            HeaderText = "Color",
            DataPropertyName = "Color",
            Width = 80,
            ReadOnly = true
        });

        dgvCategories.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Icon",
            HeaderText = "Icon",
            DataPropertyName = "Icon",
            Width = 60,
            ReadOnly = true
        });

        dgvCategories.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "IsActive",
            HeaderText = "Active",
            DataPropertyName = "IsActive",
            Width = 60,
            ReadOnly = true
        });

        pnlCategories.Controls.Add(dgvCategories);

        // Actions Panel
        pnlActions = new Panel
        {
            Location = new Point(20, 480),
            Size = new Size(760, 110),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        // Add Category Button
        btnAddCategory = new Button
        {
            Text = "Add Category",
            Location = new Point(0, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnAddCategory.FlatAppearance.BorderSize = 0;
        btnAddCategory.Click += BtnAddCategory_Click;

        // Edit Category Button
        btnEditCategory = new Button
        {
            Text = "Edit Category",
            Location = new Point(190, 10),
            Size = new Size(180, 60),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnEditCategory.FlatAppearance.BorderSize = 0;
        btnEditCategory.Click += BtnEditCategory_Click;

        // Delete Category Button
        btnDeleteCategory = new Button
        {
            Text = "Delete Category",
            Location = new Point(380, 10),
            Size = new Size(195, 60),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnDeleteCategory.FlatAppearance.BorderSize = 0;
        btnDeleteCategory.Click += BtnDeleteCategory_Click;

        // Import CSV Button
        btnImportCSV = new Button
        {
            Text = "Import CSV",
            Location = new Point(585, 10),
            Size = new Size(165, 60),
            BackColor = Color.FromArgb(255, 152, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnImportCSV.FlatAppearance.BorderSize = 0;
        btnImportCSV.Click += BtnImportCSV_Click;

        // Export CSV Button
        btnExportCSV = new Button
        {
            Text = "Export CSV",
            Location = new Point(0, 80),
            Size = new Size(165, 60),
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnExportCSV.FlatAppearance.BorderSize = 0;
        btnExportCSV.Click += BtnExportCSV_Click;

        // Add buttons to actions panel
        pnlActions.Controls.AddRange(new Control[]
        {
            btnAddCategory,
            btnEditCategory,
            btnDeleteCategory,
            btnImportCSV,
            btnExportCSV
        });

        // Add all controls to form
        this.Controls.AddRange(new Control[]
        {
            lblTitle,
            pnlCategories,
            pnlActions
        });
    }

    private void LoadCategories()
    {
        try
        {
            var categories = Program.CategoryRepository.GetAll().ToList();
            dgvCategories.DataSource = null;
            dgvCategories.DataSource = categories;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAddCategory_Click(object? sender, EventArgs e)
    {
        var dialog = new CategoryDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                var category = new Category(
                    dialog.CategoryName,
                    dialog.CategoryDescription,
                    dialog.CategoryColor,
                    dialog.CategoryIcon
                );

                Program.CategoryRepository.Add(category);
                Program.CategoryRepository.SaveChanges();

                LoadCategories();

                MessageBox.Show("Category added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnEditCategory_Click(object? sender, EventArgs e)
    {
        if (dgvCategories.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a category to edit.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;

        var dialog = new CategoryDialog(selectedCategory);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                selectedCategory.Update(
                    dialog.CategoryName,
                    dialog.CategoryDescription,
                    dialog.CategoryColor,
                    dialog.CategoryIcon
                );

                Program.CategoryRepository.Update(selectedCategory);
                Program.CategoryRepository.SaveChanges();

                LoadCategories();

                MessageBox.Show("Category updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnDeleteCategory_Click(object? sender, EventArgs e)
    {
        if (dgvCategories.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a category to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedCategory = (Category)dgvCategories.SelectedRows[0].DataBoundItem;

        var result = MessageBox.Show(
            $"Are you sure you want to delete the category '{selectedCategory.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                // Soft delete - just deactivate
                selectedCategory.Deactivate();
                Program.CategoryRepository.Update(selectedCategory);
                Program.CategoryRepository.SaveChanges();

                LoadCategories();

                MessageBox.Show("Category deactivated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnImportCSV_Click(object? sender, EventArgs e)
    {
        try
        {
            // Create OpenFileDialog for CSV file selection
            var openDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "csv",
                Title = "Import Transactions from CSV",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                // Show confirmation message with import instructions
                var confirmResult = MessageBox.Show(
                    "CSV Import Instructions:\n\n" +
                    "Required CSV Format:\n" +
                    "Type,Description,Amount,Date,Category,Account,Notes,Source\n\n" +
                    "- Type: 'Income' or 'Expense'\n" +
                    "- Amount: Positive number (e.g., 100.50)\n" +
                    "- Date: MM/DD/YYYY or YYYY-MM-DD\n" +
                    "- Category: Must exist in your categories\n\n" +
                    "Do you want to continue with the import?",
                    "CSV Import Instructions",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult == DialogResult.Yes)
                {
                    // Create CSV import service
                    var csvService = new CsvImportService(
                        Program.TransactionRepository,
                        Program.CategoryRepository
                    );

                    // Import from CSV
                    var (successCount, errorCount, errors) = csvService.ImportFromCsv(openDialog.FileName);

                    // Build result message
                    var resultMessage = $"Import completed!\n\n" +
                                      $"Successfully imported: {successCount} transactions\n" +
                                      $"Failed: {errorCount} rows\n";

                    if (errors.Count > 0)
                    {
                        resultMessage += "\nErrors:\n";
                        // Show only first 10 errors to avoid overwhelming the user
                        var errorsToShow = errors.Take(10).ToList();
                        resultMessage += string.Join("\n", errorsToShow);

                        if (errors.Count > 10)
                        {
                            resultMessage += $"\n... and {errors.Count - 10} more errors";
                        }
                    }

                    // Show result with appropriate icon
                    var icon = errorCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning;
                    MessageBox.Show(resultMessage, "Import Results", MessageBoxButtons.OK, icon);

                    // Refresh the form if any transactions were imported
                    if (successCount > 0)
                    {
                        LoadCategories();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error importing CSV: {ex.Message}", "Import Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnExportCSV_Click(object? sender, EventArgs e)
    {
        try
        {
            // Ask user what to export
            var exportChoice = MessageBox.Show(
                "What would you like to export?\n\n" +
                "Yes = Export Transactions (CSV)\n" +
                "No = Export Sample CSV Template\n" +
                "Cancel = Cancel Export",
                "Export Options",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (exportChoice == DialogResult.Cancel)
                return;

            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "csv"
            };

            if (exportChoice == DialogResult.Yes)
            {
                // Export transactions
                saveDialog.FileName = $"transactions_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var csvService = new CsvImportService(
                        Program.TransactionRepository,
                        Program.CategoryRepository
                    );

                    var transactions = Program.TransactionRepository.GetAll();
                    csvService.ExportToCsv(saveDialog.FileName, transactions);

                    MessageBox.Show($"Transactions exported successfully to:\n{saveDialog.FileName}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                // Export sample template
                saveDialog.FileName = $"transactions_template_{DateTime.Now:yyyyMMdd}.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var sampleCsv = CsvImportService.GenerateSampleCsv();
                    File.WriteAllText(saveDialog.FileName, sampleCsv);

                    MessageBox.Show(
                        $"Sample CSV template exported successfully to:\n{saveDialog.FileName}\n\n" +
                        "You can use this template as a reference for importing transactions.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting CSV: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
