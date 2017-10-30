using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOpenStackUI
{
    public partial class OpenStackView : Form
    {
        /// <summary>
        /// Controller
        /// </summary>
        OpenStackController controller;

        /// <summary>
        /// Get the row selected for the right click
        /// </summary>
        int indexRowSelected;

        /// <summary>
        /// Create folder dialog form
        /// </summary>
        CreateFolderDialog cr;

        /// <summary>
        /// Thread for loading form
        /// </summary>
        private LoadingDialog loading;

        private volatile bool _shouldStop;

        /// <summary>
        /// Constructor
        /// </summary>
        public OpenStackView()
        {
            InitializeComponent();
            // Initialize datagrid

            DataGridViewImageColumn imageCol = new DataGridViewImageColumn();
            dgBrowser.Columns.Add(imageCol);

            dgBrowser.ColumnCount = 4;
            dgBrowser.Columns[0].Name = "";
            dgBrowser.Columns[0].Width = 30;

            dgBrowser.Columns[1].Name = "Name";
            dgBrowser.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgBrowser.Columns[2].Name = "Size";
            dgBrowser.Columns[2].Width = 100;

            dgBrowser.Columns[3].Name = "Last Modified";
            dgBrowser.Columns[3].Width = 150;
        }

        /// <summary>
        /// Add a listener to this view
        /// </summary>
        /// <param name="controller"></param>
        public void AddListener(OpenStackController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Event for upload button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StartLoading();
                controller.UploadFile(openFileDialog1.FileName);
                StopLoading();
            }
        }

        /// <summary>
        /// Event for download button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            OpenDialogDownload();
        }

        /// <summary>
        /// Open download dialog form 
        /// </summary>
        public void OpenDialogDownload()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Please select your directory for storing your downloaded files";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DataGridViewCell cell = dgBrowser.CurrentCell;
                int index = 0;
                if (cell != null)
                {
                    index = cell.RowIndex;
                    StartLoading();
                    controller.Download(index, fbd.SelectedPath);
                    StopLoading();
                }
            }
        }

        /// <summary>
        /// Event for create folder button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateFolder_Click(object sender, EventArgs e)
        {
            OpenDialogCreateFolder(true);
        }

        /// <summary>
        /// Open create folder dialog form
        /// </summary>
        /// <param name="isFolder"></param>
        public void OpenDialogCreateFolder(Boolean isFolder)
        {
            cr = new CreateFolderDialog(this, controller.GetUrl(), isFolder);
            cr.ShowDialog();
        }

        /// <summary>
        /// Event for open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OnClickSelectedRow();
        }

        /// <summary>
        /// Event for delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        /// <summary>
        /// Open dialog form to ask user to confirm deleting file
        /// </summary>
        public void DeleteFile()
        {
            if (MessageBox.Show("Really delete?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DataGridViewCell cell = dgBrowser.CurrentCell;
                int index = 0;
                if (cell != null)
                {
                    index = cell.RowIndex;
                    StartLoading();
                    controller.Delete(index);
                    StopLoading();
                }
            }
        }

        /// <summary>
        /// Event for connection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnection_Click(object sender, EventArgs e)
        {
            StartLoading();
            controller.Connect(tbTenantName.Text, tbUsername.Text, tbPassword.Text);
            StopLoading();
        }

        /// <summary>
        /// Update list of containers
        /// </summary>
        /// <param name="containers"></param>
        public void UpdateContainers(FilesModel containers)
        {
        
            lbContainers.Items.Clear();
            if (containers != null)
            {
                List<FileModel> listes = containers.GetFiles();
                for (int i = 0; i < listes.Count(); i++)
                {
                    lbContainers.Items.Add(listes.ElementAt(i).Name.TrimEnd('/'));
                }
            }
        }

        /// <summary>
        /// Updates the list of files
        /// </summary>
        /// <param name="root"></param>
        public void UpdateTree(NodeModel root)
        {
            dgBrowser.Rows.Clear();
            dgBrowser.ScrollBars = ScrollBars.None;

            NodeModel node = root;

            if (root != null)
            {
                if (node.Children.Count() != 0)
                {
                    btnOpen.Enabled = (root.Children.ElementAt(0).Value.IsDirectory) ? true : false;
                }
               
                for (int i = 0; i < node.Children.Count(); i++)
                {
                    FileModel child = node.Children.ElementAt(i).Value;
                    Bitmap imgFolder = new Bitmap("img/dossier-table-16.png");
                    Bitmap imgFile = new Bitmap("img/file-table-16.png");
                    Bitmap tmp;

                    tmp = (child.IsDirectory) ? imgFolder : imgFile;
                    Object[] row = new Object[] { tmp, child.Name, child.Size, child.Last_modified };
                    dgBrowser.Rows.Add(row);
                }
            }

            dgBrowser.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// Event for go button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGo_Click(object sender, EventArgs e)
        {
            controller.GoToUrl(tbUrl.Text);
        }

        /// <summary>
        /// Event for back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            controller.GoToParent();
        }

        /// <summary>
        /// Event double click for table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgBrowser_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            controller.GoToFile(index);
        }

        /// <summary>
        /// Event for a simple click (left or right) into the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgBrowser_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridViewCell cell = dgBrowser.CurrentCell;
            int index = 0;
            if (cell != null)
            {
                index = cell.RowIndex;
                if (controller.IsNodeDirectory(index))
                {
                    btnOpen.Enabled = true;
                }
                else
                {
                    btnOpen.Enabled = false;
                }
            }
            // Open a context menu by a right click
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();

                indexRowSelected = dgBrowser.HitTest(e.X, e.Y).RowIndex;

                if (indexRowSelected != -1)
                {
                    dgBrowser.Rows[indexRowSelected].Selected = true;

                    if (controller.IsNodeDirectory(indexRowSelected))
                    {
                        MenuItem menuOpen = new MenuItem("Open");
                        menuOpen.Click += new EventHandler(MenuItemOpen_Click);
                        m.MenuItems.Add(menuOpen);
                        m.MenuItems.Add("-");

                        MenuItem menuDownload = new MenuItem("Download Folder");
                        menuDownload.Click += new EventHandler(MenuItemDownload_Click);
                        m.MenuItems.Add(menuDownload);

                        MenuItem menuRemove = new MenuItem("Delete Folder");
                        menuRemove.Click += new EventHandler(MenuItemRemove_Click);
                        m.MenuItems.Add(menuRemove);
                    }
                    else
                    {
                        MenuItem menuDownload = new MenuItem("Download");
                        menuDownload.Click += new EventHandler(MenuItemDownload_Click);
                        m.MenuItems.Add(menuDownload);

                        MenuItem menuRemove = new MenuItem("Delete");
                        menuRemove.Click += new EventHandler(MenuItemRemove_Click);
                        m.MenuItems.Add(menuRemove);
                    }
                }

                m.Show(dgBrowser, new Point(e.X, e.Y));
            }
        }

        /// <summary>
        /// Event for open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemOpen_Click(object sender, EventArgs e)
        {
            OnClickSelectedRow();
        }

        /// <summary>
        /// Go to file with index row selected
        /// </summary>
        public void OnClickSelectedRow()
        {
            DataGridViewCell cell = dgBrowser.CurrentCell;
            int index = 0;
            if(cell != null)
            {
                index = cell.RowIndex;
                controller.GoToFile(index);
            }
        }

        /// <summary>
        /// Event for download button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemDownload_Click(object sender, EventArgs e)
        {
            OpenDialogDownload();
        }

        /// <summary>
        /// Event for delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemRemove_Click(object sender, EventArgs e)
        {
            DeleteFile();
        }

        /// <summary>
        /// Create folder or container
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isFolder"></param>
        public void OnClickCreateFolder(String url, Boolean isFolder)
        {
            if (!controller.CreateFolder(url, isFolder))
            {
                MessageBox.Show("The container name already exists.\nPlease choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event for double click into containers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbContainers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.lbContainers.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                StartLoading();
                controller.SelectContainer(index);
                StopLoading();
            }
        }

        /// <summary>
        /// Key press into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbUrl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                controller.GoToUrl(tbUrl.Text);
            }
        }

        /// <summary>
        /// Event for create container button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateContainer_Click(object sender, EventArgs e)
        {
            OpenDialogCreateFolder(false);
        }

        /// <summary>
        /// Event for delete container button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteContainer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Really delete?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int index = lbContainers.SelectedIndex;
                if (index != -1)
                {
                    StartLoading();
                    controller.SelectContainer(index);
                    controller.DeleteContainer();
                    StopLoading();
                }

            }
        }

        /// <summary>
        /// Event for quit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        #region LoadingThread members

        /// <summary>
        /// Start thread
        /// </summary>
        public void StartLoading()
        {
            _shouldStop = false;
            // Do some time consuming work in separate thread
            Thread t = new Thread(new ThreadStart(DoSomeWork));
            t.Start();
        }

        /// <summary>
        /// Time consuming method
        /// </summary>
        private void DoSomeWork()
        {
            // Show the splash form
            loading = new LoadingDialog();
            loading.Show();
            while (!_shouldStop)
            {
                // Do nothing
            }
            loading.Dispose();
            loading = null;
        }

        /// <summary>
        /// Abort thread
        /// </summary>
        public void StopLoading()
        {
            _shouldStop = true;
        }

        #endregion

        #region OpenStackView Members

        public String Information
        {
            get
            {
                return tfInformation.Text;
            }
            set
            {
                tfInformation.AppendText(value);
            }
        }

        public void AddInformation()
        {

        }

        public void EnableLogin(Boolean enable)
        {
            if (enable)
            {
                gbLogin.Enabled = true;
            }
            else
            {
                gbLogin.Enabled = false;
            }
        }

        public void EnableBrowser(Boolean enable)
        {
            gbBrowser.Enabled = enable;
        }

        public void EnableButtonsObjects(Boolean enable)
        {
            panelButtonsObjects.Enabled = enable;
        }

        public String Url
        {
            get
            {
                return tbUrl.Text;
            }
            set
            {
                tbUrl.Text = value;
            }
        }

        #endregion

    }
}
