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
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrambleImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unscrambleImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.stagesListBox = new System.Windows.Forms.ListBox();
            this.imagesListBox = new System.Windows.Forms.ListBox();
            this.attributesListBox = new System.Windows.Forms.ListBox();
            this.projectListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.modPropertyGrid = new System.Windows.Forms.PropertyGrid();
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
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00001F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00001F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00001F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.99999F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.stagesListBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.imagesListBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.attributesListBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.projectListBox, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.modPropertyGrid, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 426);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // mapsListBox
            // 
            this.stagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stagesListBox.FormattingEnabled = true;
            this.stagesListBox.Location = new System.Drawing.Point(3, 29);
            this.stagesListBox.Name = "mapsListBox";
            this.tableLayoutPanel1.SetRowSpan(this.stagesListBox, 2);
            this.stagesListBox.Size = new System.Drawing.Size(154, 394);
            this.stagesListBox.TabIndex = 0;
            this.stagesListBox.DoubleClick += new System.EventHandler(this.StagesListBox_DoubleClick);
            // 
            // imagesListBox
            // 
            this.imagesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagesListBox.FormattingEnabled = true;
            this.imagesListBox.Location = new System.Drawing.Point(163, 29);
            this.imagesListBox.Name = "imagesListBox";
            this.tableLayoutPanel1.SetRowSpan(this.imagesListBox, 2);
            this.imagesListBox.Size = new System.Drawing.Size(154, 394);
            this.imagesListBox.TabIndex = 2;
            this.imagesListBox.DoubleClick += new System.EventHandler(this.ImagesListBox_DoubleClick);
            // 
            // attributesListBox
            // 
            this.attributesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributesListBox.FormattingEnabled = true;
            this.attributesListBox.Location = new System.Drawing.Point(323, 29);
            this.attributesListBox.Name = "attributesListBox";
            this.attributesListBox.Size = new System.Drawing.Size(154, 194);
            this.attributesListBox.TabIndex = 3;
            this.attributesListBox.DoubleClick += new System.EventHandler(this.AttributesListBox_DoubleClick);
            // 
            // projectListBox
            // 
            this.projectListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectListBox.FormattingEnabled = true;
            this.projectListBox.Location = new System.Drawing.Point(323, 229);
            this.projectListBox.Name = "projectListBox";
            this.projectListBox.Size = new System.Drawing.Size(154, 194);
            this.projectListBox.TabIndex = 4;
            this.projectListBox.DoubleClick += new System.EventHandler(this.ProjectListBox_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Stages";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(163, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 26);
            this.label3.TabIndex = 7;
            this.label3.Text = "Images";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(323, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 26);
            this.label4.TabIndex = 8;
            this.label4.Text = "Map Attributes | Project Files";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // modPropertyGrid
            // 
            this.modPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modPropertyGrid.Location = new System.Drawing.Point(483, 3);
            this.modPropertyGrid.Name = "modPropertyGrid";
            this.tableLayoutPanel1.SetRowSpan(this.modPropertyGrid, 3);
            this.modPropertyGrid.Size = new System.Drawing.Size(314, 420);
            this.modPropertyGrid.TabIndex = 9;
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
            this.tableLayoutPanel1.PerformLayout();
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
        private System.Windows.Forms.ListBox stagesListBox;
        private System.Windows.Forms.ListBox imagesListBox;
        private System.Windows.Forms.ListBox attributesListBox;
        private System.Windows.Forms.ListBox projectListBox;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrambleImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unscrambleImageToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PropertyGrid modPropertyGrid;
    }
}

