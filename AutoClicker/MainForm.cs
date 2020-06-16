using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using AutoClicker.Models;

namespace AutoClicker
{
    public partial class MainForm : Form
    {
        private Keys hotkey;
        private Win32.fsModifiers hotkeyNodifiers;

        private Thread countdownThread;

        private AutoHandle autoHandle { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            autoHandle = new AutoHandle();
            LoadLocationsGrid(listBox1.SelectedIndex);
            //LoadSettings();
            ClickTypeHandler(null, null);
            LocationHandler(null, null);
            DelayHandler(null, null);
            CountHandler(null, null);

            autoHandle.NextClick += HandleNextClick;
            autoHandle.Finished += HandleFinished;
        }

        private void HandleNextClick(object sender, AutoHandle.NextClickEventArgs e)
        {
            countdownThread = new Thread(() => CountDown(e.NextClick));
            countdownThread.Start();
        }

        private void HandleFinished(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void CountDown(int Milliseconds)
        {
            for (int i = 0; i < Milliseconds; i += 10)
            {
                tslStatus.Text = string.Format("Next click: {0}ms", Milliseconds - i);
                Thread.Sleep(9);
            }
        }

        private void ClickTypeHandler(object sender, EventArgs e)
        {
            AutoAction.ButtonType buttonType;
            bool doubleClick = false;

            if (rdbClickSingleLeft.Checked || rdbClickDoubleLeft.Checked)
            {
                buttonType = AutoAction.ButtonType.Left;
            }
            else if (rdbClickSingleMiddle.Checked || rdbClickDoubleMiddle.Checked)
            {
                buttonType = AutoAction.ButtonType.Middle;
            }
            else
            {
                buttonType = AutoAction.ButtonType.Right;
            }

            if (rdbClickDoubleLeft.Checked || rdbClickDoubleMiddle.Checked || rdbClickDoubleRight.Checked)
            {
                doubleClick = true;
            }

            if (listBox1.SelectedIndex != -1)
            {
                autoHandle.UpdateButton(buttonType, doubleClick, listBox1.SelectedIndex);
                LoadLocationsGrid(listBox1.SelectedIndex);
            }
        }

        private void LocationHandler(object sender, EventArgs e)
        {
            AutoAction.LocationType locationType;
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;

            if (rdbLocationFixed.Checked)
            {
                locationType = AutoAction.LocationType.Fixed;
                x = (int)numFixedX.Value;
                y = (int)numFixedY.Value;
            }
            else if (rdbLocationMouse.Checked)
            {
                locationType = AutoAction.LocationType.Cursor;
            }
            else if (rdbLocationRandom.Checked)
            {
                locationType = AutoAction.LocationType.Random;
            }
            else
            {
                locationType = AutoAction.LocationType.RandomRange;
                x = (int)numRandomX.Value;
                y = (int)numRandomY.Value;
                width = (int)numRandomWidth.Value;
                height = (int)numRandomHeight.Value;
            }

            // Toggle visibility of controls.
            if (locationType == AutoAction.LocationType.Fixed)
            {
                numFixedX.Enabled = true;
                numFixedY.Enabled = true;
            }
            else
            {
                numFixedX.Enabled = false;
                numFixedY.Enabled = false;
            }

            if (locationType == AutoAction.LocationType.RandomRange)
            {
                numRandomX.Enabled = true;
                numRandomY.Enabled = true;
                numRandomWidth.Enabled = true;
                numRandomHeight.Enabled = true;
                btnSelect.Enabled = true;
            }
            else
            {
                numRandomX.Enabled = false;
                numRandomY.Enabled = false;
                numRandomWidth.Enabled = false;
                numRandomHeight.Enabled = false;
                btnSelect.Enabled = false;
            }

            if (listBox1.SelectedIndex != -1)
            {
                autoHandle.UpdateLocation(locationType, x, y, width, height, listBox1.SelectedIndex);
                LoadLocationsGrid(listBox1.SelectedIndex);
            }
        }

        private void DelayHandler(object sender, EventArgs e)
        {
            AutoAction.DelayType delayType;
            int delay = 0;
            int delayRange = 0;

            if (rdbDelayFixed.Checked)
            {
                delayType = AutoAction.DelayType.Fixed;
                delay = (int)numDelayFixed.Value;
            }
            else
            {
                delayType = AutoAction.DelayType.Range;
                delay = (int)numDelayRangeMin.Value;
                delayRange = (int)numDelayRangeMax.Value;
            }

            // Toggle visibility of controls.
            if (delayType == AutoAction.DelayType.Fixed)
            {
                numDelayFixed.Enabled = true;
                numDelayRangeMax.Enabled = false;
                numDelayRangeMin.Enabled = false;
            }
            else
            {
                numDelayFixed.Enabled = false;
                numDelayRangeMax.Enabled = true;
                numDelayRangeMin.Enabled = true;
            }
            if (listBox1.SelectedItems.Count > 0)
            {
                autoHandle.UpdateDelay(delayType, delay, delayRange, listBox1.SelectedIndex);
                LoadLocationsGrid(listBox1.SelectedIndex);
            }
        }

        private void CountHandler(object sender, EventArgs e)
        {
            AutoHandle.CountType countType;
            int count = 0;

            if (rdbCount.Checked)
            {
                countType = AutoHandle.CountType.Fixed;
                count = (int)numCount.Value;
            }
            else
            {
                countType = AutoHandle.CountType.UntilStopped;
            }

            // Toggle visibility of controls.
            if (countType == AutoHandle.CountType.Fixed)
            {
                numCount.Enabled = true;
            }
            else
            {
                numCount.Enabled = false;
            }

            autoHandle.UpdateCount(countType, count);
        }

        private void btnHotkeyRemove_Click(object sender, EventArgs e)
        {
            UnsetHotkey();
        }

        private void btnToggle_Click(object sender, EventArgs e)
        {
            if (!autoHandle.IsAlive)
            {
                if(String.IsNullOrEmpty(txtSelectedProcess.Text))
                {
                    MessageBox.Show("Please select a process.");
                    return;
                }
                autoHandle.Start();
                DisableControls();
            }
            else
            {
                autoHandle.Stop();
                EnableControls();
            }
        }

        private delegate void SetEnabledCallback(Control Control, bool Enabled);

        private void SetEnabled(Control Control, bool Enabled)
        {
            if (Control.InvokeRequired)
            {
                var d = new SetEnabledCallback(SetEnabled);
                this.Invoke(d, Control, Enabled);
            }
            else
            {
                Control.Enabled = Enabled;
            }
        }

        private delegate void SetButtonTextCallback(Button Control, string Text);

        private void SetButtonText(Button Control, string Text)
        {
            if (Control.InvokeRequired)
            {
                var d = new SetButtonTextCallback(SetButtonText);
                this.Invoke(d, Control, Text);
            }
            else
            {
                Control.Text = Text;
            }
        }

        private void EnableControls()
        {
            tslStatus.Text = "Not currently doing much helpful here to be honest";
            SetEnabled(grpClickType, true);
            SetEnabled(grpLocation, true);
            SetEnabled(grpDelay, true);
            SetEnabled(grpCount, true);
            SetButtonText(btnToggle, "Start");
            //grpClickType.Enabled = true;
            //grpLocation.Enabled = true;
            //grpDelay.Enabled = true;
            //grpCount.Enabled = true;
            //btnToggle.Text = "Start";
        }

        private void DisableControls()
        {
            SetEnabled(grpClickType, false);
            SetEnabled(grpLocation, false);
            SetEnabled(grpDelay, false);
            SetEnabled(grpCount, false);
            SetButtonText(btnToggle, "Stop");
            //grpClickType.Enabled = false;
            //grpLocation.Enabled = false;
            //grpDelay.Enabled = false;
            //grpCount.Enabled = false;
            //btnToggle.Text = "Stop";
        }

        private void SetLocation(AutoAction click)
        {
            if (click.doubleClick)
            {
                switch (click.buttonType)
                {
                    case AutoAction.ButtonType.Left:
                        rdbClickDoubleLeft.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickDoubleLeft.Checked = true;
                        rdbClickDoubleLeft.CheckedChanged += this.ClickTypeHandler;
                        break;

                    case AutoAction.ButtonType.Middle:
                        rdbClickDoubleMiddle.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickDoubleMiddle.Checked = true;
                        rdbClickDoubleMiddle.CheckedChanged += this.ClickTypeHandler;
                        break;

                    case AutoAction.ButtonType.Right:
                        rdbClickDoubleRight.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickDoubleRight.Checked = true;
                        rdbClickDoubleRight.CheckedChanged += this.ClickTypeHandler;
                        break;
                }
            }
            else
            {
                switch (click.buttonType)
                {
                    case AutoAction.ButtonType.Left:
                        rdbClickSingleLeft.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickSingleLeft.Checked = true;
                        rdbClickSingleLeft.CheckedChanged += this.ClickTypeHandler;
                        break;

                    case AutoAction.ButtonType.Middle:
                        rdbClickSingleMiddle.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickSingleMiddle.Checked = true;
                        rdbClickSingleMiddle.CheckedChanged += this.ClickTypeHandler;
                        break;

                    case AutoAction.ButtonType.Right:
                        rdbClickSingleRight.CheckedChanged -= this.ClickTypeHandler;
                        rdbClickSingleRight.Checked = true;
                        rdbClickSingleRight.CheckedChanged += this.ClickTypeHandler;
                        break;
                }
            }
            switch (click.locationType)
            {
                case AutoAction.LocationType.Fixed:
                    rdbLocationFixed.CheckedChanged -= this.ClickTypeHandler;
                    rdbLocationFixed.Checked = true;
                    rdbLocationFixed.CheckedChanged += this.ClickTypeHandler;
                    break;

                case AutoAction.LocationType.Cursor:
                    rdbLocationMouse.CheckedChanged -= this.ClickTypeHandler;
                    rdbLocationMouse.Checked = true;
                    rdbLocationMouse.CheckedChanged += this.ClickTypeHandler;
                    break;

                case AutoAction.LocationType.Random:
                    rdbLocationRandom.CheckedChanged -= this.ClickTypeHandler;
                    rdbLocationRandom.Checked = true;
                    rdbLocationRandom.CheckedChanged += this.ClickTypeHandler;
                    break;

                case AutoAction.LocationType.RandomRange:
                    rdbLocationRandomArea.CheckedChanged -= this.ClickTypeHandler;
                    rdbLocationRandomArea.Checked = true;
                    rdbLocationRandomArea.CheckedChanged += this.ClickTypeHandler;
                    break;
            }

            numFixedX.ValueChanged -= LocationHandler;
            numFixedX.Value = click.x;
            numFixedX.ValueChanged += LocationHandler;

            numFixedY.ValueChanged -= LocationHandler;
            numFixedY.Value = click.y;
            numFixedY.ValueChanged += LocationHandler;

            numRandomX.ValueChanged -= LocationHandler;
            numRandomX.Value = click.x;
            numRandomX.ValueChanged += LocationHandler;

            numRandomY.ValueChanged -= LocationHandler;
            numRandomY.Value = click.y;
            numRandomY.ValueChanged += LocationHandler;

            numRandomWidth.ValueChanged -= LocationHandler;
            numRandomWidth.Value = click.width;
            numRandomWidth.ValueChanged += LocationHandler;

            numRandomHeight.ValueChanged -= LocationHandler;
            numRandomHeight.Value = click.height;
            numRandomHeight.ValueChanged += LocationHandler;

            switch (click.delayType)
            {
                case AutoAction.DelayType.Fixed:
                    rdbDelayFixed.CheckedChanged -= DelayHandler;
                    rdbDelayFixed.Checked = true;
                    rdbDelayFixed.CheckedChanged -= DelayHandler;
                    break;

                case AutoAction.DelayType.Range:
                    rdbDelayRange.CheckedChanged -= DelayHandler;
                    rdbDelayRange.Checked = true;
                    rdbDelayRange.CheckedChanged -= DelayHandler;
                    break;
            }
            
            numDelayFixed.ValueChanged -= DelayHandler;
            numDelayFixed.Value = click.delay;
            numDelayFixed.ValueChanged += DelayHandler;

            numDelayRangeMin.ValueChanged -= DelayHandler;
            numDelayRangeMin.Value = click.delay;
            numDelayRangeMin.ValueChanged += DelayHandler;

            numDelayRangeMax.ValueChanged -= DelayHandler;
            numDelayRangeMax.Value = click.delay + click.delayRange;
            numDelayRangeMax.ValueChanged += DelayHandler;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == Win32.WM_HOTKEY)
            {
                // Ignore the hotkey if the user is editing it.
                if (txtHotkey.Focused)
                {
                    return;
                }

                Win32.fsModifiers modifiers = (Win32.fsModifiers)((int)m.LParam & 0xFFFF);
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                if (key == (hotkey & Keys.KeyCode) && modifiers == hotkeyNodifiers)
                {
                    btnToggle_Click(null, null);
                }
            }
        }

        private void txtHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            // Don't want to do anything if only a modifier key is pressed.
            //     Modifiers                                 Asian keys (kana, hanja, kanji etc)       IME related keys (convert etc)           Korean alt (process)  Windows keys
            if (!((e.KeyValue >= 16 && e.KeyValue <= 18) || (e.KeyValue >= 21 && e.KeyValue <= 25) || (e.KeyValue >= 28 && e.KeyValue <= 31) || e.KeyValue == 229 || (e.KeyValue >= 91 && e.KeyValue <= 92)))
            {
                Win32.UnregisterHotKey(this.Handle, (int)hotkey);
                hotkey = e.KeyData;
                // Extract modifiers
                hotkeyNodifiers = 0;
                if ((e.Modifiers & Keys.Shift) != 0)
                {
                    hotkeyNodifiers |= Win32.fsModifiers.Shift;
                }
                if ((e.Modifiers & Keys.Control) != 0)
                {
                    hotkeyNodifiers |= Win32.fsModifiers.Control;
                }
                if ((e.Modifiers & Keys.Alt) != 0)
                {
                    hotkeyNodifiers |= Win32.fsModifiers.Alt;
                }

                SetHotkey();
            }
        }

        private void SetHotkey()
        {
            txtHotkey.Text = KeysConverter.Convert(hotkey);
            Win32.RegisterHotKey(this.Handle, (int)hotkey, (uint)hotkeyNodifiers, (uint)(hotkey & Keys.KeyCode));
            btnHotkeyRemove.Enabled = true;
        }

        private void UnsetHotkey()
        {
            Win32.UnregisterHotKey(this.Handle, (int)hotkey);
            btnHotkeyRemove.Enabled = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SaveSettings();
        }

        public void SendRectangle(int X, int Y, int Width, int Height)
        {
            numRandomX.Value = X;
            numRandomY.Value = Y;
            numRandomWidth.Value = Width;
            numRandomHeight.Value = Height;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var form = new SelectionForm(this);
            form.Show();
        }

        private void LoadLocationsGrid(Int32? Index)
        {
            listBox1.Items.Clear();
            foreach (var item in autoHandle.tmpActions)
            {
                string clickType = string.Empty;
                switch (item.buttonType)
                {
                    case AutoAction.ButtonType.Left:
                        clickType = "Left";
                        break;

                    case AutoAction.ButtonType.Middle:
                        clickType = "Middle";
                        break;

                    case AutoAction.ButtonType.Right:
                        clickType = "Right";
                        break;
                }
                listBox1.Items.Add("X: " + item.x + ", Y: " + item.y + ", Delay: " + item.delay + ", Delay Range: " + item.delayRange + ", Click Type: " + clickType);
            }
            if (Index.HasValue)
            {
                listBox1.SelectedIndexChanged -= listBox1_SelectedIndexChanged;
                listBox1.SelectedIndex = Index.Value;
                listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            }
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            AutoAction location = new AutoAction
            {
                delay = 0,
                x = 0,
                y = 0,
                delayRange = 0,
                delayType = AutoAction.DelayType.Fixed,
                height = 0,
                width = 0,
                locationType = AutoAction.LocationType.Fixed,
                AutoActionType = AutoAction.ActionType.Click,
                buttonType = AutoAction.ButtonType.Left,
                doubleClick = false
            };
            autoHandle.tmpActions.Add(location);
            LoadLocationsGrid(autoHandle.tmpActions.Count - 1);
            SetLocation(location);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                autoHandle.tmpActions.RemoveAt(listBox1.SelectedIndex);
                if (autoHandle.tmpActions.Count >= listBox1.SelectedIndex + 1)
                {
                    LoadLocationsGrid(listBox1.SelectedIndex + 1);
                    SetLocation(autoHandle.tmpActions[listBox1.SelectedIndex]);
                }
                else if (listBox1.SelectedIndex - 1 > -1)
                {
                    LoadLocationsGrid(listBox1.SelectedIndex - 1);
                    SetLocation(autoHandle.tmpActions[listBox1.SelectedIndex]);
                }
                else
                    LoadLocationsGrid(null);
            }
            else
                MessageBox.Show("Please select a location to remove");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
                SetLocation(autoHandle.tmpActions[listBox1.SelectedIndex]);
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                autoHandle.tmpActions.Insert(listBox1.SelectedIndex + 1, autoHandle.tmpActions[listBox1.SelectedIndex]);
                LoadLocationsGrid(listBox1.SelectedIndex);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            autoHandle.tmpActions.Move(listBox1.SelectedIndex, MoveDirection.Down);
            LoadLocationsGrid(listBox1.SelectedIndex);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            autoHandle.tmpActions.Move(listBox1.SelectedIndex, MoveDirection.Up);
            LoadLocationsGrid(listBox1.SelectedIndex);
        }

        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog
            {
                FileName = "data.dat",
                Filter = "Data files (*.dat)|*.*"
            };

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                DataHandler.SaveList<AutoAction>(savefile.FileName, autoHandle.tmpActions.ToList());
            }
        }

        private void loadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog savefile = new OpenFileDialog
            {
                FileName = "data.dat",
                Filter = "Data files (*.dat)|*.*",
                RestoreDirectory = true
            };

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                autoHandle.tmpActions = new System.ComponentModel.BindingList<AutoAction>(DataHandler.LoadList<AutoAction>(savefile.FileName));
                LoadLocationsGrid(null);
            }
        }

        private void btnSelectProcess_Click(object sender, EventArgs e)
        {
            String process = null;
            ProcessForm processForm = new ProcessForm();
            processForm.FormClosing += delegate {
                process = processForm.SelectedProcess;
            };
            processForm.ShowDialog();
            if (!String.IsNullOrEmpty(process))
            {
                txtSelectedProcess.Text = process;
            }
        }

        private void txtSelectedProcess_Leave(object sender, EventArgs e)
        {

        }
    }
}

public static class ListExtensions
{
    public static void Move<T>(this IList<T> list, int IndexToMove, MoveDirection direction)
    {
        if (direction == MoveDirection.Up)
        {
            var old = list[IndexToMove - 1];
            list[IndexToMove - 1] = list[IndexToMove];
            list[IndexToMove] = old;
        }
        else
        {
            var old = list[IndexToMove + 1];
            list[IndexToMove + 1] = list[IndexToMove];
            list[IndexToMove] = old;
        }
    }
}

public enum MoveDirection
{
    Up,
    Down
}