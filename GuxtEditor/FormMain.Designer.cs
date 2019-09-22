namespace GuxtEditor
{
    partial class FormMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mapsListBox = new System.Windows.Forms.ListBox();
            this.entitiesListBox = new System.Windows.Forms.ListBox();
            this.imagesListBox = new System.Windows.Forms.ListBox();
            this.attributesListBox = new System.Windows.Forms.ListBox();
            this.projectListBox = new System.Windows.Forms.ListBox();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrambleImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unscrambleImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.mapsListBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.entitiesListBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.imagesListBox, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.attributesListBox, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.projectListBox, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 426);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // mapsListBox
            // 
            this.mapsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapsListBox.FormattingEnabled = true;
            this.mapsListBox.Location = new System.Drawing.Point(3, 3);
            this.mapsListBox.Name = "mapsListBox";
            this.mapsListBox.Size = new System.Drawing.Size(154, 420);
            this.mapsListBox.TabIndex = 0;
            this.mapsListBox.DoubleClick += new System.EventHandler(this.MapsListBox_DoubleClick);
            // 
            // entitiesListBox
            // 
            this.entitiesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entitiesListBox.FormattingEnabled = true;
            this.entitiesListBox.Location = new System.Drawing.Point(163, 3);
            this.entitiesListBox.Name = "entitiesListBox";
            this.entitiesListBox.Size = new System.Drawing.Size(154, 420);
            this.entitiesListBox.TabIndex = 1;
            this.entitiesListBox.DoubleClick += new System.EventHandler(this.EntitiesListBox_DoubleClick);
            // 
            // imagesListBox
            // 
            this.imagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagesListBox.FormattingEnabled = true;
            this.imagesListBox.Location = new System.Drawing.Point(323, 3);
            this.imagesListBox.Name = "imagesListBox";
            this.imagesListBox.Size = new System.Drawing.Size(154, 420);
            this.imagesListBox.TabIndex = 2;
            this.imagesListBox.DoubleClick += new System.EventHandler(this.ImagesListBox_DoubleClick);
            // 
            // attributesListBox
            // 
            this.attributesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributesListBox.FormattingEnabled = true;
            this.attributesListBox.Location = new System.Drawing.Point(483, 3);
            this.attributesListBox.Name = "attributesListBox";
            this.attributesListBox.Size = new System.Drawing.Size(154, 420);
            this.attributesListBox.TabIndex = 3;
            this.attributesListBox.DoubleClick += new System.EventHandler(this.AttributesListBox_DoubleClick);
            // 
            // projectListBox
            // 
            this.projectListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectListBox.FormattingEnabled = true;
            this.projectListBox.Location = new System.Drawing.Point(643, 3);
            this.projectListBox.Name = "projectListBox";
            this.projectListBox.Size = new System.Drawing.Size(154, 420);
            this.projectListBox.TabIndex = 4;
            this.projectListBox.DoubleClick += new System.EventHandler(this.ProjectListBox_DoubleClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scrambleImageToolStripMenuItem,
            this.unscrambleImageToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // scrambleImageToolStripMenuItem
            // 
            this.scrambleImageToolStripMenuItem.Name = "scrambleImageToolStripMenuItem";
            this.scrambleImageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.scrambleImageToolStripMenuItem.Text = "Scramble Image...";
            this.scrambleImageToolStripMenuItem.Click += new System.EventHandler(this.ScrambleImageToolStripMenuItem_Click);
            // 
            // unscrambleImageToolStripMenuItem
            // 
            this.unscrambleImageToolStripMenuItem.Name = "unscrambleImageToolStripMenuItem";
            this.unscrambleImageToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.unscrambleImageToolStripMenuItem.Text = "Unscramble Image...";
            this.unscrambleImageToolStripMenuItem.Click += new System.EventHandler(this.UnscrambleImageToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "All-Seeing Eye";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox mapsListBox;
        private System.Windows.Forms.ListBox entitiesListBox;
        private System.Windows.Forms.ListBox imagesListBox;
        private System.Windows.Forms.ListBox attributesListBox;
        private System.Windows.Forms.ListBox projectListBox;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrambleImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unscrambleImageToolStripMenuItem;
    }
}

