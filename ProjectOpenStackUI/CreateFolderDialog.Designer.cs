namespace ProjectOpenStackUI
{
    partial class CreateFolderDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateFolderDialog));
            this.tbUrlFolderToCreate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUrlMain = new System.Windows.Forms.Label();
            this.btnCancelCreateFolder = new System.Windows.Forms.Button();
            this.btnValidateCreateFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbUrlFolderToCreate
            // 
            this.tbUrlFolderToCreate.Location = new System.Drawing.Point(86, 14);
            this.tbUrlFolderToCreate.Name = "tbUrlFolderToCreate";
            this.tbUrlFolderToCreate.Size = new System.Drawing.Size(307, 20);
            this.tbUrlFolderToCreate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Directory : ";
            // 
            // lblUrlMain
            // 
            this.lblUrlMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrlMain.Location = new System.Drawing.Point(86, 43);
            this.lblUrlMain.Name = "lblUrlMain";
            this.lblUrlMain.Size = new System.Drawing.Size(307, 13);
            this.lblUrlMain.TabIndex = 5;
            this.lblUrlMain.Text = "..";
            this.lblUrlMain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancelCreateFolder
            // 
            this.btnCancelCreateFolder.Image = global::ProjectOpenStackUI.Properties.Resources.annuler_16;
            this.btnCancelCreateFolder.Location = new System.Drawing.Point(214, 65);
            this.btnCancelCreateFolder.Name = "btnCancelCreateFolder";
            this.btnCancelCreateFolder.Size = new System.Drawing.Size(75, 23);
            this.btnCancelCreateFolder.TabIndex = 3;
            this.btnCancelCreateFolder.Text = "Cancel";
            this.btnCancelCreateFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelCreateFolder.UseVisualStyleBackColor = true;
            this.btnCancelCreateFolder.Click += new System.EventHandler(this.btnCancelCreateFolder_Click);
            // 
            // btnValidateCreateFolder
            // 
            this.btnValidateCreateFolder.Image = global::ProjectOpenStackUI.Properties.Resources.ok_16;
            this.btnValidateCreateFolder.Location = new System.Drawing.Point(133, 65);
            this.btnValidateCreateFolder.Name = "btnValidateCreateFolder";
            this.btnValidateCreateFolder.Size = new System.Drawing.Size(75, 23);
            this.btnValidateCreateFolder.TabIndex = 2;
            this.btnValidateCreateFolder.Text = "Create";
            this.btnValidateCreateFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnValidateCreateFolder.UseVisualStyleBackColor = true;
            this.btnValidateCreateFolder.Click += new System.EventHandler(this.btnValidateCreateFolder_Click);
            // 
            // CreateFolderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 101);
            this.Controls.Add(this.lblUrlMain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancelCreateFolder);
            this.Controls.Add(this.btnValidateCreateFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbUrlFolderToCreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateFolderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbUrlFolderToCreate;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnValidateCreateFolder;
        public System.Windows.Forms.Button btnCancelCreateFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblUrlMain;
    }
}