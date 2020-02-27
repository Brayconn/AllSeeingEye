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
            this.entityPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.entityListView = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mapPictureBox = new System.Windows.Forms.PictureBox();
            this.hScreenPreviewScrollBar = new System.Windows.Forms.HScrollBar();
            this.vScreenPreviewScrollBar = new System.Windows.Forms.VScrollBar();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.None;
            this.mainPanel.Size = new System.Drawing.Size(510, 406);
            // 
            // splitContainer1
            // 
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.vScreenPreviewScrollBar);
            this.splitContainer1.Panel2.Controls.Add(this.hScreenPreviewScrollBar);
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.mapPictureBox);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(513, 406);
            this.panel1.TabIndex = 1;
            // 
            // mapPictureBox
            // 
            this.mapPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mapPictureBox.Name = "mapPictureBox";
            this.mapPictureBox.Size = new System.Drawing.Size(167, 133);
            this.mapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.mapPictureBox.TabIndex = 0;
            this.mapPictureBox.TabStop = false;
            this.mapPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPictureBox_Paint);
            this.mapPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseDown);
            this.mapPictureBox.MouseLeave += new System.EventHandler(this.mapPictureBox_MouseLeave);
            this.mapPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseMove);
            this.mapPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseUp);
            // 
            // hScreenPreviewScrollBar
            // 
            this.hScreenPreviewScrollBar.Location = new System.Drawing.Point(-1, 409);
            this.hScreenPreviewScrollBar.Name = "hScreenPreviewScrollBar";
            this.hScreenPreviewScrollBar.Size = new System.Drawing.Size(513, 17);
            this.hScreenPreviewScrollBar.TabIndex = 1;
            // 
            // vScreenPreviewScrollBar
            // 
            this.vScreenPreviewScrollBar.Location = new System.Drawing.Point(513, 3);
            this.vScreenPreviewScrollBar.Name = "vScreenPreviewScrollBar";
            this.vScreenPreviewScrollBar.Size = new System.Drawing.Size(17, 423);
            this.vScreenPreviewScrollBar.TabIndex = 2;
            // 
            // FormStageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormStageEditor";
            this.Text = "FormStageEditor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormStageEditor_KeyDown);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid entityPropertyGrid;
        private System.Windows.Forms.ListView entityListView;
        private System.Windows.Forms.PictureBox mapPictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.HScrollBar hScreenPreviewScrollBar;
        private System.Windows.Forms.VScrollBar vScreenPreviewScrollBar;
    }
}