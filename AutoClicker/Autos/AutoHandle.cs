using AutoClicker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AutoClicker.Models.AutoAction;

namespace AutoClicker
{
    public class AutoHandle
    {
        private Thread AutoThread;
        public List<AutoAction> Actions { get; set; }

        #region "Count"

        public enum CountType
        {
            Fixed,
            UntilStopped
        }

        private CountType countType;
        private int count;

        #endregion "Count"

        #region "Update storage"

        private bool buttonUpdated;
        private bool actionUpdated;
        private bool delayUpdated;

        public BindingList<AutoAction> tmpActions { get; set; }

        private bool countUpdated;
        private CountType tmpCountType;
        private int tmpCount;

        #endregion "Update storage"

        public class NextClickEventArgs : EventArgs
        {
            public int NextClick;
        }

        public event EventHandler<NextClickEventArgs> NextClick;

        public EventHandler<EventArgs> Finished;

        private Random rnd;

        private AutoClicker Clicker;

        private AutoKey Key;

        public AutoHandle()
        {
            Actions = new List<AutoAction>();
            tmpActions = new BindingList<AutoAction>();
            Clicker = new AutoClicker();
            Key = new AutoKey();
            rnd = new Random();
        }

        private void Run()
        {
            SyncSettings();
            int remaining = count;
            while (countType == CountType.UntilStopped || remaining > 0)
            {
                if (!IsAlive)
                    return;
                foreach (AutoAction action in Actions)
                {
                    SyncSettings();

                    //Delay
                    int nextDelay = action.delayType == DelayType.Fixed ?
                        action.delay : nextDelay = rnd.Next(action.delay, action.delayRange);

                    if(action.AutoActionType == ActionType.Click)
                    {
                        Clicker.Click(action);
                    }
                    else if (action.AutoActionType == ActionType.Type)
                    {

                    }

                    NextClick?.Invoke(this, new NextClickEventArgs { NextClick = nextDelay });
                    Thread.Sleep(nextDelay);//Sleep delay time
                }
                remaining--;
            }
            Finished?.Invoke(this, null);
        }

        public bool IsAlive
        {
            get
            {
                if (AutoThread == null)
                {
                    return false;
                }
                return AutoThread.IsAlive;
            }
        }

        public void Start()
        {
            AutoThread = new Thread(Run)
            {
                IsBackground = true
            };
            AutoThread.Start();
        }

        public void Stop()
        {
            if (AutoThread != null)
            {
                AutoThread.Abort();
            }
        }

        private void SyncSettings()
        {
            if (buttonUpdated)
            {
                Actions = tmpActions.ToList<AutoAction>();

                buttonUpdated = false;
            }

            if (actionUpdated)
            {
                Actions = tmpActions.ToList<AutoAction>();

                actionUpdated = false;
            }

            if (delayUpdated)
            {
                Actions = tmpActions.ToList<AutoAction>();

                delayUpdated = false;
            }

            if (countUpdated)
            {
                countType = tmpCountType;
                count = tmpCount;

                countUpdated = false;
            }
            //System.Diagnostics.Debug.Print("Settings synced");
        }


        public void UpdateButton(ButtonType ButtonType, bool DoubleClick, int Index)
        {
            if (tmpActions.Count > Index)
            {
                tmpActions[Index].doubleClick = DoubleClick;
                tmpActions[Index].buttonType = ButtonType;
                buttonUpdated = true;
            }
        }

        public void UpdateLocation(LocationType LocationType, int X, int Y, int Width, int Height, int Index)
        {
            if (tmpActions.Count > Index)
            {
                tmpActions[Index].locationType = LocationType;
                tmpActions[Index].x = X;
                tmpActions[Index].y = Y;
                tmpActions[Index].width = Width;
                tmpActions[Index].height = Height;
                actionUpdated = true;
            }
        }

        public void UpdateDelay(DelayType DelayType, int Delay, int DelayRange, int Index)
        {
            if (tmpActions.Count > Index)
            {
                tmpActions[Index].delayType = DelayType;
                tmpActions[Index].delay = Delay;
                tmpActions[Index].delayRange = DelayRange;
                actionUpdated = true;
            }
            delayUpdated = true;
        }

        public void UpdateCount(CountType CountType, int Count)
        {
            tmpCountType = CountType;
            tmpCount = Count;

            countUpdated = true;
        }
    }
}
