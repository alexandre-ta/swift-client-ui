using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOpenStackUI
{
    /// <summary>
    /// This class represents a form dialog for creating a new folder or a new container
    /// </summary>
    public partial class CreateFolderDialog : Form
    {
        /// <summary>
        /// Main view
        /// </summary>
        private OpenStackView mainView;

        /// <summary>
        /// If isFolder is false, then this windows is called to create a container
        /// </summary>
        Boolean isFolder = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="main"></param>
        /// <param name="urlMain"></param>
        /// <param name="isFolder"></param>
        public CreateFolderDialog(OpenStackView main, String urlMain, Boolean isFolder)
        {
            this.mainView = main;
            this.isFolder = isFolder;
            InitializeComponent();
            if (isFolder)
            {
                this.Text = "Create Folder";
                this.lblUrlMain.Text = urlMain;
            }
            else
            {
                this.Text = "Create Container";
                this.lblUrlMain.Text = "/";
            }
        }

        /// <summary>
        /// Click on close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelCreateFolder_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Check the new container name or the new folder name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidateCreateFolder_Click(object sender, EventArgs e)
        {
            if (!tbUrlFolderToCreate.Text.Equals(""))
            {
                mainView.OnClickCreateFolder(tbUrlFolderToCreate.Text, isFolder);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("The new folder is not well formed.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }

        }

    }
}
