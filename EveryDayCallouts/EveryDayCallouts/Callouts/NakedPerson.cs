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
        bool IsSpe﻿echFinished;
        private bool PursuitCreated = false;


        public override bool OnBeforeCalloutDisplayed() {

            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(550f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Naked Person(Testing)";
            CalloutPosition = SpawnPoint;
            hasArrived = false;
            Game.LogTrivial("(NakedPerson): Callout Message Displayed");

//          Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", SpawnPoint);
//          Functions.PlayScannerAudio("END_3DPRT_PTT");


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivial("(NakedPerson): Callout Accepted");

//          Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
//          Functions.PlayScannerAudio("END_3DPRT_PTT");

//            Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayNotification("Respond ~b~Code 2~w~");

//          Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayHelp("Press ~b~End~w~ to forcefully end the callout.", 5000);

//          Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayNotification("Go on ~p~scene~w~ and try to ~g~speak~w~ with the suspect.");
//          Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayHelp("If it's a ~r~fake call~w~, just press ~b~End~w~ ");

            Suspect = new Ped(SpawnPoint);
            Suspect.BlockPermanentEvents = true;
            SuspectsBlip = Suspect.AttachBlip();
            SuspectsBlip.Color = (System.Drawing.Color.Green);
            Game.LogTrivial("(NakedPerson): All Ped's actions loaded.");
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

                Game.LogTrivial("(Naked Person): Callout ENDED. User pressed END.");

//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
//              Functions.PlayScannerAudio("PTT");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
//              Functions.PlayScannerAudio("END_3DPRT_PTT");
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(Suspect.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(NakedPerson): Officer Arrived At Scene");
//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Suspect~w~ to talk with him.");

            }

            if (!IsSpe﻿echFinished && Game.LocalPlayer.Character.DistanceTo(Suspect.Position) < 8f) {

                while (!Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    GameFiber.Yield();


                Suspect.Tasks.Clear();
                Suspect.Tasks.StandStill(30000);

                Game.DisplaySubtitle("~b~Officer~w~: Hello! What's going on?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: I don't know anything officer. What's happening?", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: We got a call for an incident exposure on this exact location. Have you seen anyone? ", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: No officer, I'm sorry. As you can see, no one here is ~r~naked~w~.. It must be a fake call.", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Yes. That's what I think too. ", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~b~Officer~w~: Alright. Thank you for your time. Have a nice day!", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: Thank you ~b~Officer~w~! Take care!", 3500);
                GameFiber.Wait(4000);
                IsSpeechFinished = true;

                GameFiber.Wait(4500);
                Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("You can now press ~b~END~w~ to be ~g~Code 4~w~.");
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
            if (calloutArea.Exists()) {
                calloutArea.Delete();
            }

        }

    }
}