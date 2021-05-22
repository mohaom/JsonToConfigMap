using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace JsonToConfigMap
{
    internal sealed class ApplyCustomTool
    {
        private const int _commandId = 0x0100;
        private static readonly Guid _commandSet = new Guid("4aaf93c0-70ae-4a4b-9fb6-1ad3997a9adf");
        private static DTE _dte;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE;

            var commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as IMenuCommandService;
            var cmdId = new CommandID(_commandSet, _commandId);

            var cmd = new OleMenuCommand(OnExecute, cmdId)
            {
                // This will defer visibility control to the VisibilityConstraints section in the .vsct file
                Supported = false
            };

            commandService.AddCommand(cmd);
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            ProjectItem item = _dte.SelectedItems.Item(1).ProjectItem;

            DialogResult dialogResult = MessageBox.Show("Do you want to tokenize Values ?", "Confirm", MessageBoxButtons.YesNo);



            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "Yaml Files (*.yaml)|*.yaml";
            diag.DefaultExt = "yaml";
            diag.AddExtension = true;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                if (dialogResult == DialogResult.Yes)
                {
                    ConfigMapUtility.ConvertToCM(item.Document.FullName, diag.FileName, item.ContainingProject.Name, true);
                }
                else
                {
                    ConfigMapUtility.ConvertToCM(item.Document.FullName, diag.FileName, item.ContainingProject.Name, false);
                }

            }
        }
    }
}
