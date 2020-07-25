namespace GuxtEditor
{
    partial class FormStageEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStageEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.entitySpritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entityBoxesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.editModeTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tilesetPictureBox = new System.Windows.Forms.PictureBox();
            this.mapPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.entityPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.entityListView = new System.Windows.Forms.ListView();
            this.hScreenPreviewScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScreenPreviewScrollBar = new System.Windows.Forms.VScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mapLayeredPictureBox = new LayeredPictureBox.LayeredPictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.editModeTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilesetPictureBox)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteAllEntitiesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // deleteAllEntitiesToolStripMenuItem
            // 
            this.deleteAllEntitiesToolStripMenuItem.Name = "deleteAllEntitiesToolStripMenuItem";
            this.deleteAllEntitiesToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.deleteAllEntitiesToolStripMenuItem.Text = "Delete All Entities";
            this.deleteAllEntitiesToolStripMenuItem.Click += new System.EventHandler(this.deleteAllEntitiesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileTypesToolStripMenuItem,
            this.toolStripSeparator1,
            this.entitySpritesToolStripMenuItem,
            this.entityBoxesToolStripMenuItem,
            this.screenPreviewToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // tileTypesToolStripMenuItem
            // 
            this.tileTypesToolStripMenuItem.CheckOnClick = true;
            this.tileTypesToolStripMenuItem.Name = "tileTypesToolStripMenuItem";
            this.tileTypesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.tileTypesToolStripMenuItem.Text = "Tile Types";
            this.tileTypesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.tileTypesToolStripMenuItem_CheckedChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // entitySpritesToolStripMenuItem
            // 
            this.entitySpritesToolStripMenuItem.Checked = true;
            this.entitySpritesToolStripMenuItem.CheckOnClick = true;
            this.entitySpritesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.entitySpritesToolStripMenuItem.Name = "entitySpritesToolStripMenuItem";
            this.entitySpritesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.entitySpritesToolStripMenuItem.Text = "Entitiy Sprites";
            this.entitySpritesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.entitySpritesToolStripMenuItem_CheckedChanged);
            // 
            // entityBoxesToolStripMenuItem
            // 
            this.entityBoxesToolStripMenuItem.Checked = true;
            this.entityBoxesToolStripMenuItem.CheckOnClick = true;
            this.entityBoxesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.entityBoxesToolStripMenuItem.Name = "entityBoxesToolStripMenuItem";
            this.entityBoxesToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.entityBoxesToolStripMenuItem.Text = "Entity Boxes";
            this.entityBoxesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.entityBoxesToolStripMenuItem_CheckedChanged);
            // 
            // screenPreviewToolStripMenuItem
            // 
            this.screenPreviewToolStripMenuItem.CheckOnClick = true;
            this.screenPreviewToolStripMenuItem.Name = "screenPreviewToolStripMenuItem";
            this.screenPreviewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.screenPreviewToolStripMenuItem.Text = "Screen Preview";
            this.screenPreviewToolStripMenuItem.CheckedChanged += new System.EventHandler(this.screenPreviewToolStripMenuItem_CheckedChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.editModeTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.hScreenPreviewScrollBar);
            this.splitContainer1.Panel2.Controls.Add(this.vScreenPreviewScrollBar);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 426);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 1;
            // 
            // editModeTabControl
            // 
            this.editModeTabControl.Controls.Add(this.tabPage1);
            this.editModeTabControl.Controls.Add(this.tabPage2);
            this.editModeTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editModeTabControl.Location = new System.Drawing.Point(0, 0);
            this.editModeTabControl.Name = "editModeTabControl";
            this.editModeTabControl.SelectedIndex = 0;
            this.editModeTabControl.Size = new System.Drawing.Size(266, 426);
            this.editModeTabControl.TabIndex = 0;
            this.editModeTabControl.SelectedIndexChanged += new System.EventHandler(this.editModeTabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(258, 400);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Map";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tilesetPictureBox, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.mapPropertyGrid, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(252, 394);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tilesetPictureBox
            // 
            this.tilesetPictureBox.Location = new System.Drawing.Point(3, 200);
            this.tilesetPictureBox.Name = "tilesetPictureBox";
            this.tilesetPictureBox.Size = new System.Drawing.Size(246, 191);
            this.tilesetPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.tilesetPictureBox.TabIndex = 0;
            this.tilesetPictureBox.TabStop = false;
            this.tilesetPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tilesetPictureBox_MouseClick);
            // 
            // mapPropertyGrid
            // 
            this.mapPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.mapPropertyGrid.Name = "mapPropertyGrid";
            this.mapPropertyGrid.Size = new System.Drawing.Size(246, 191);
            this.mapPropertyGrid.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(258, 400);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Entity";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.entityPropertyGrid, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.entityListView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 394);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // entityPropertyGrid
            // 
            this.entityPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityPropertyGrid.Location = new System.Drawing.Point(3, 3);
            this.entityPropertyGrid.Name = "entityPropertyGrid";
            this.entityPropertyGrid.Size = new System.Drawing.Size(246, 191);
            this.entityPropertyGrid.TabIndex = 0;
            // 
            // entityListView
            // 
            this.entityListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityListView.HideSelection = false;
            this.entityListView.Location = new System.Drawing.Point(3, 200);
            this.entityListView.MultiSelect = false;
            this.entityListView.Name = "entityListView";
            this.entityListView.Size = new System.Drawing.Size(246, 191);
            this.entityListView.TabIndex = 1;
            this.entityListView.UseCompatibleStateImageBehavior = false;
            // 
            // hScreenPreviewScrollBar
            // 
            this.hScreenPreviewScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScreenPreviewScrollBar.Enabled = false;
            this.hScreenPreviewScrollBar.LargeChange = 1;
            this.hScreenPreviewScrollBar.Location = new System.Drawing.Point(0, 409);
            this.hScreenPreviewScrollBar.Maximum = 0;
            this.hScreenPreviewScrollBar.Name = "hScreenPreviewScrollBar";
            this.hScreenPreviewScrollBar.Size = new System.Drawing.Size(513, 17);
            this.hScreenPreviewScrollBar.TabIndex = 3;
            this.hScreenPreviewScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScreenPreviewScrollChanged);
            // 
            // vScreenPreviewScrollBar
            // 
            this.vScreenPreviewScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScreenPreviewScrollBar.Enabled = false;
            this.vScreenPreviewScrollBar.LargeChange = 2;
            this.vScreenPreviewScrollBar.Location = new System.Drawing.Point(513, 0);
            this.vScreenPreviewScrollBar.Maximum = 1;
            this.vScreenPreviewScrollBar.Name = "vScreenPreviewScrollBar";
            this.vScreenPreviewScrollBar.Size = new System.Drawing.Size(17, 426);
            this.vScreenPreviewScrollBar.TabIndex = 2;
            this.vScreenPreviewScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScreenPreviewScrollChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.mapLayeredPictureBox);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(513, 406);
            this.panel1.TabIndex = 1;
            // 
            // mapLayeredPictureBox
            // 
            this.mapLayeredPictureBox.AutoScroll = true;
            this.mapLayeredPictureBox.CanvasScale = 1;
            this.mapLayeredPictureBox.CanvasSizeLocked = false;
            this.mapLayeredPictureBox.CurrentCanvasHeight = 0;
            this.mapLayeredPictureBox.CurrentCanvasWidth = 0;
            this.mapLayeredPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mapLayeredPictureBox.MaxCanvasHeight = 0;
            this.mapLayeredPictureBox.MaxCanvasSize = new System.Drawing.Size(0, 0);
            this.mapLayeredPictureBox.MaxCanvasWidth = 0;
            this.mapLayeredPictureBox.Name = "mapLayeredPictureBox";
            this.mapLayeredPictureBox.Size = new System.Drawing.Size(200, 100);
            this.mapLayeredPictureBox.TabIndex = 1;
            this.mapLayeredPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPictureBox_Paint);
            this.mapLayeredPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseDown);
            this.mapLayeredPictureBox.MouseEnter += new System.EventHandler(this.mapLayeredPictureBox_MouseEnter);
            this.mapLayeredPictureBox.MouseLeave += new System.EventHandler(this.mapPictureBox_MouseLeave);
            this.mapLayeredPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseMove);
            this.mapLayeredPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseUp);
            // 
            // FormStageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormStageEditor";
            this.Text = "FormStageEditor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormStageEditor_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.editModeTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilesetPictureBox)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl editModeTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox tilesetPictureBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PropertyGrid entityPropertyGrid;
        private System.Windows.Forms.ListView entityListView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PropertyGrid mapPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem tileTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entitySpritesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllEntitiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entityBoxesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem screenPreviewToolStripMenuItem;
        private System.Windows.Forms.HScrollBar hScreenPreviewScrollBar;
        private System.Windows.Forms.VScrollBar vScreenPreviewScrollBar;
        private LayeredPictureBox.LayeredPictureBox mapLayeredPictureBox;
    }
}