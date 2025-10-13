using System;
using System.Windows.Forms;

namespace BudgetTracker.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            MessageBox.Show("Hello World!!");
        }
    }
}
