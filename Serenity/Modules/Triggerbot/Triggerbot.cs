﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Serenity.Helpers;
using Serenity.Objects;

using static Serenity.Helpers.PrettyLog;

namespace Serenity.Modules.Triggerbot
{
    internal class Triggerbot : IModule
    {
        /// <summary>
        /// Contains all FOVs.
        /// </summary>
        public List<Fov> Fovs { get; set; }

        private Fov MyFov;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Triggerbot()
        {
            // Initialize Fovs.
            Fovs = new List<Fov>
            {
                new Fov { Resolution = new Point(1920, 1080), FieldOfView = new Rectangle(960, 400, 1, 165) }
            };

            MyFov = Fovs.FirstOrDefault(x => x.Resolution == new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));

            // Run the aimbot.
            var thread = new Thread(Run);
            if (MyFov != null)
            {
                thread.Start();
                LogInfo("Triggerbot initialized");
            }
            else
            {
                LogError("Could not initialize Triggerbot as screen does not match available resolutions.\n" +
                         "This will be fixed later, for now make your screen resolution 1920x1080.\n");
            }
        }

        public void LoadModule()
        {
        }

        /// <summary>
        /// Main trigger thread.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                if (MouseHelper.GetAsyncKeyState(SettingsManager.Triggerbot.AimKey) < 0)
                {
                    // Get the screen capture.
                    var screenCapture = ScreenHelper.GetScreenCapture(MyFov.FieldOfView);

                    // Search for a target.
                    var coordinates = SearchHelper.SearchColor(ref screenCapture, SettingsManager.Triggerbot.TargetColor, 100);

                    if (coordinates.X != 0 || coordinates.Y != 0)
                    {
                        MouseHelper.Click();
                    }

                    // Destroy the bitmap.
                    screenCapture.Dispose();
                    screenCapture = null;
                }

                Thread.Sleep(1);
            }
        }

        public void HandleCommand(IEnumerable<string> args)
        {
            throw new System.NotImplementedException();
        }
    }
}
