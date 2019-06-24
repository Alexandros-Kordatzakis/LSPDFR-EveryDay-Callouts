using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Reflection;


namespace EveryDayCallouts {

    public class Main : Plugin {

        public override void Initialize() {

            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Plugin EveryDay Callouts" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
            Game.LogTrivial("Go on duty to fully load EveryDay Callouts.");

        }

        public override void Finally() {

            Game.LogTrivial("EveryDay_Calouts has been cleaned up.");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty) {

            if (OnDuty) {

                RegisterCallouts();
                Game.DisplayNotification("~r~EveryDay~w~ ~p~Callouts~w~ ~y~v0.1.8.1~w~ loaded successfully! ~g~Alexander K.~w~");
            }
        }

        private static void RegisterCallouts() {

            Functions.RegisterCallout(typeof(Callouts.LostDog));
            Functions.RegisterCallout(typeof(Callouts.RandomCheck));
            Functions.RegisterCallout(typeof(Callouts.LostCow));
            Functions.RegisterCallout(typeof(Callouts.NakedPerson));
        }

        //   Albo's  Stuff   From Here:

        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null) {

            foreach (Assembly assembly in Functions.GetAllUserPlugins()) {

                AssemblyName an = assembly.GetName(); if (an.Name.ToLower() == Plugin.ToLower()){

                    if (minversion == null || an.Version.CompareTo(minversion) >= 0) {
                      return true;
                    }
                }
            }
            return false;
        }

        //  To here.

    }
}
