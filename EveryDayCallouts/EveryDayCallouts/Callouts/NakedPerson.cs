using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;


namespace EveryDayCallouts.Callouts {

    [CalloutInfo("NakedPerson", CalloutProbability.VeryHigh)]

    class NakedPerson : Callout {

        private Ped Suspect;
        private Vector3 SpawnPoint;
        private Blip SuspectsBlip;
        private Blip calloutArea;
        private LHandle Pursuit;
        bool hasArrived;
        private bool PursuitCreated = false;


        public override bool OnBeforeCalloutDisplayed() {

            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(550f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Naked Person(Testing)";
            CalloutPosition = SpawnPoint;
            hasArrived = false;
            Game.LogTrivial("(NakedPerson): Callout Message Displayed");        

            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {
            
            Game.LogTrivial("(NakedPerson): Callout Accepted");

            Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio("PTT");

            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.");
            Game.DisplayNotification("Go on ~p~scene~w~ and try to ~g~speak~w~ with the suspect.");
            Game.DisplayHelp("If it's a ~r~fake call~w~, just press ~b~End~w~ ");

            Suspect = new Ped(SpawnPoint);
            Suspect.BlockPermanentEvents = true;
            SuspectsBlip = Suspect.AttachBlip();
            SuspectsBlip.Color = (System.Drawing.Color.Green);
            Game.LogTrivial("(NakedPerson): All Ped's actions loaded.");     /// Game.LogTrivial  that needs to be commented for not using memory.

            SuspectsBlip.IsFriendly = false;

            calloutArea = new Blip(SpawnPoint, 40f);
            calloutArea.Color = (System.Drawing.Color.Yellow);
            calloutArea.Alpha = 0.5f;
            calloutArea.EnableRoute(System.Drawing.Color.Yellow);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(NakedPerson): Callout Not Accepted.");   

            End();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.DisplayNotification("~g~Code 4~w~, return to patrol.~b~(Back 10-8)~w~");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                Game.LogTrivial("(NakedPerson): Officer Pressed END button.  Callout canceled.");      
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(Suspect.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(NakedPerson): Officer Arrived At Scene");       
            }

            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit)) {

                End();
            }
        }

        public override void End() {

            CleanUp();

            base.End();
        }

        public void CleanUp() {

            if (Suspect.Exists()) {
                Suspect.Dismiss();
            }

            if (SuspectsBlip.Exists()) {
                SuspectsBlip.Delete();
            }
            if (calloutArea.Exists()){
                calloutArea.Delete();
            }

            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
            Game.DisplayNotification("All units, we are ~g~Code 4~w~");
        }

    }
}
