using System;
using System.Windows.Forms;

namespace MainfreightProject;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainfreightForm());
    }
}