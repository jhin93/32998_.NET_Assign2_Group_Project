using BudgetTracker.Domain.Entities;

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
        MessageBox.Show("CSV Import feature will be implemented in Phase 2!", "Coming Soon",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnExportCSV_Click(object? sender, EventArgs e)
    {
        try
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"categories_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var categories = Program.CategoryRepository.GetAll();
                var csv = "Id,Name,Description,Color,Icon,IsActive\n";

                foreach (var category in categories)
                {
                    csv += $"{category.Id},{category.Name},{category.Description},{category.Color},{category.Icon},{category.IsActive}\n";
                }

                File.WriteAllText(saveDialog.FileName, csv);

                MessageBox.Show("Categories exported successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error exporting categories: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
