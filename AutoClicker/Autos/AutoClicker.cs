using AutoClicker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using static AutoClicker.Models.AutoAction;
using static AutoClicker.Models.AutoAction;

namespace AutoClicker
{
    [Serializable]
    internal class AutoClicker
    {
        private Random rnd;

        public AutoClicker()
        {
            rnd = new Random();
        }

        public static IntPtr handle;

        public void Click(AutoAction action)
        {
            List<ClickOnPointTool.INPUT> inputs = new List<ClickOnPointTool.INPUT>();
            var chosenPoint = new Win32.POINT();
            // Move the mouse if required.
            if (action.locationType == LocationType.Fixed)
            {
                chosenPoint.x = action.x;
                chosenPoint.y = action.y;
            }
            else if (action.locationType == LocationType.Random)
            {
                chosenPoint.x = rnd.Next(65536);
                chosenPoint.y = rnd.Next(65536);
            }
            else if (action.locationType == LocationType.RandomRange)
            {
                chosenPoint.x = rnd.Next(action.x, action.x + action.width);
                chosenPoint.y = rnd.Next(action.y, action.y + action.height);
            }
            //Click Type
            for (int i = 0; i < (action.doubleClick ? 2 : 1); i++)
            {
                // Add a delay if it's a double click
                if (i == 1)
                {
                    Thread.Sleep(50);
                }

                if (action.buttonType == ButtonType.Left)
                {
                    ClickOnPointTool.INPUT inputDown = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.LeftDown
                            }
                        }
                    };
                    inputs.Add(inputDown);
                    ClickOnPointTool.INPUT inputUp = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.LeftUp
                            }
                        }
                    };
                    inputs.Add(inputUp);
                }

                if (action.buttonType == ButtonType.Middle)
                {
                    ClickOnPointTool.INPUT inputDown = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.MiddleDown
                            }
                        }
                    };
                    inputs.Add(inputDown);
                    ClickOnPointTool.INPUT inputUp = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.MiddleUp
                            }
                        }
                    };
                    inputs.Add(inputUp);
                }

                if (action.buttonType == ButtonType.Right)
                {
                    ClickOnPointTool.INPUT inputDown = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.RightUp
                            }
                        }
                    };
                    inputs.Add(inputDown);
                    ClickOnPointTool.INPUT inputUp = new ClickOnPointTool.INPUT
                    {
                        Type = (uint)Win32.SendInputEventType.InputMouse,
                        Data = new ClickOnPointTool.MOUSEKEYBDHARDWAREINPUT()
                        {
                            Mouse = new ClickOnPointTool.MOUSEINPUT
                            {
                                Flags = (uint)Win32.MouseEventFlags.RightDown
                            }
                        }
                    };
                    inputs.Add(inputUp);
                }
            }

            if (inputs.Count > 0)
                AutoCursor.MoveMouse(chosenPoint.x, chosenPoint.y, 0, 0);

            Point lpPoint = new Point() { X = chosenPoint.x, Y = chosenPoint.y };
            //ClickOnPointTool.ClientToScreen(handle, ref lpPoint);

            //Another attempt, only supports left
            ClickOnPointTool.ClickOnPoint(handle, new System.Drawing.Point() { X = chosenPoint.x, Y = chosenPoint.y }, inputs.ToArray());
        }
    }
}