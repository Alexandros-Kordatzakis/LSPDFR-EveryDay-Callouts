using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;
using Timer = System.Timers.Timer;


namespace EveryDayCallouts.Callouts {

    [CalloutInfo("Vandalism", CalloutProbability.VeryHigh)]

    class Vandalism : Callout {


        public override bool OnBeforeCalloutDisplayed() {

            // SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(550f));            ------  Change Vector3 to an X,Y,Z Location
            //ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            //AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Vandalism(Testing)";
            //CalloutPosition = SpawnPoint;
            //hasArrived = false;
            Game.LogTrivial("(Vandalism): Callout Message Displayed");

            //          Functions.PlayScannerAudio("PTT");
            //Functions.PlayScannerAudioUsingPosition("WE_HAVE  IN_OR_ON_POSITION", SpawnPoint);
            //          Functions.PlayScannerAudio("END_3DPRT_PTT");


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivial("(Vandalism): Callout Accepted");


            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(Vandalism): Callout Not Accepted.");

            End();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.LogTrivial("(Vandalism): Callout ENDED. User pressed END.");

                //              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                //              Functions.PlayScannerAudio("PTT");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                //              Functions.PlayScannerAudio("END_3DPRT_PTT");
                End();
            }


        }

    }

    /*public override void End() {

        CleanUp();

        base.End();
    }

    public void CleanUp() {


    } */

}
