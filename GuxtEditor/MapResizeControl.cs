using GuxtModdingFramework.Maps;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GuxtEditor
{
    partial class MapResizeControl : UserControl
    {
        short CurrentWidth, CurrentHeight;
        short NewWidth { get => (short)newWidthNumericUpDown.Value; set => newWidthNumericUpDown.Value = value; }
        short NewHeight { get => (short)newHeightNumericUpDown.Value; set => newHeightNumericUpDown.Value = value; }
        ResizeModes ResizeMode { get => (ResizeModes)resizeModeComboBox.SelectedItem; set => resizeModeComboBox.SelectedItem = value; }
        bool ShrinkBuffer { get => shrinkBufferCheckBox.Checked; set => shrinkBufferCheckBox.Checked = value; }

        public MapResizeControl()
        {
            InitializeComponent();

            editControls = new List<Control>()
            {
                newWidthNumericUpDown,
                newHeightNumericUpDown,
                resizeModeComboBox
            };

            resizeModeComboBox.DataSource = Enum.GetValues(typeof(ResizeModes));
            resizeModeComboBox.SelectedItem = ResizeModes.Logical;
            UpdateShrinkBufferEnabled();
        }
        public MapResizeControl(short width, short height) : this()
        {
            InitSize(width, height);
        }
        public void InitSize(short width, short height)
        {
            SetCurrentSize(width, height);
            NewWidth = width;
            NewHeight = height;
        }
        public void SetCurrentSize(short width, short height)
        {
            CurrentWidth = width;
            CurrentHeight = height;

            currentWidthLabel.Text = CurrentWidth.ToString();
            currentHeightLabel.Text = CurrentHeight.ToString();

            UpdateResizeButtonEnabled();
        }
        void UpdateShrinkBufferEnabled()
        {
            shrinkBufferCheckBox.Enabled = (ResizeMode == ResizeModes.Buffer);
        }
        void UpdateResizeButtonEnabled()
        {
            resizeButton.Enabled = (NewWidth != CurrentWidth || NewHeight != CurrentHeight);
        }

        readonly List<Control> editControls;
        public bool IsBeingEdited => editControls.Contains(ActiveControl);

        private void resizeModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateShrinkBufferEnabled();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateResizeButtonEnabled();
        }

        public event EventHandler<MapResizeInitiatedEventArgs>? MapResizeInitialized;

        private void resizeButton_Click(object sender, EventArgs e)
        {
            if (MapResizeInitialized != null)
            {
                MapResizeInitialized.Invoke(this, new MapResizeInitiatedEventArgs(NewWidth, NewHeight, ResizeMode, ShrinkBuffer));
                SetCurrentSize(NewWidth, NewHeight);
            }
        }
    }

    class MapResizeInitiatedEventArgs : EventArgs
    {
        public short Width { get; }
        public short Height { get; }
        public ResizeModes ResizeMode { get; }
        public bool ShrinkBuffer { get; }

        public MapResizeInitiatedEventArgs(short width, short height, ResizeModes resizeMode, bool shrinkBuffer)
        {
            Width = width;
            Height = height;
            ResizeMode = resizeMode;
            ShrinkBuffer = shrinkBuffer;
        }
    }
}
