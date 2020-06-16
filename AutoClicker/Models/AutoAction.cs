using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoClicker.AutoClicker;

namespace AutoClicker.Models
{
    //Josh list adding
    [Serializable]
    public class AutoAction
    {
        #region "Click"

        public ButtonType buttonType { get; set; }
        public bool doubleClick { get; set; }

        public enum ButtonType
        {
            Left,
            Middle,
            Right
        }

        #region "Location"

        public LocationType locationType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public enum LocationType
        {
            Cursor,
            Fixed,
            Random,
            RandomRange
        }

        #endregion "Location"

        #endregion "Button"

        #region "Delay"

        public DelayType delayType;
        public int delay;
        public int delayRange;

        public enum DelayType
        {
            Fixed,
            Range
        }

        #endregion "Delay"

        #region Type

        String typeText;
        


        #endregion

        public ActionType AutoActionType { get; set; }

        public enum ActionType
        {
            Click, Type
        }
    }
}
