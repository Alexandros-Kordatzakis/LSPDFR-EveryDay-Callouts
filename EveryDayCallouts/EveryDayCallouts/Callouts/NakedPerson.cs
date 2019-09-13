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

    [CalloutInfo("NakedPerson", CalloutProbability.VeryHigh)]

    class NakedPerson : Callout {

        private Ped Suspect;

        private Vector3 SpawnPoint;

        private Blip SuspectsBlip;
        private Blip calloutArea;

        bool hasArrived;
        bool IsSpe﻿echFinished;

        private readonly string[] OnNotAcceptedAudio = { "AI_RESPOND_01", "AI_RESPOND_02", "AI_RESPOND_03", "AI_RESPOND_04", "AI_RESPOND_05" };
        private readonly string[] End3rdPRTAudio = { "END_3DPRT_PTT_01", "END_3DPRT_PTT_02", "END_3DPRT_PTT_03", "END_3DPRT_PTT_04", "END_3DPRT_PTT_05", "END_3DPRT_06" };
        private readonly string[] NotiffSound = { "NOTIF_SOUND_1", "NOTIF_SOUND_2" };
        private readonly string[] PTTAudio = { "PTT_1", "PTT_2", "PTT_3" };



        public override bool OnBeforeCalloutDisplayed() {

            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(550f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Naked Person(Testing)";
            CalloutPosition = SpawnPoint;
            Game.LogTrivial("(NakedPerson): Callout Message Displayed");

            Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", SpawnPoint);
            Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivial("(NakedPerson): Callout Accepted");
            hasArrived = false;

            Functions.PlayScannerAudio(MathHelper.Choose(PTTAudio));
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));

            Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
            Game.DisplayNotification("Respond ~b~Code 2~w~");

            Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
            Game.DisplayHelp("Press ~b~End~s~ to end the callout", 6500);

            Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
            Game.DisplayNotification("Get on ~p~scene~w~ and try to ~g~speak~w~ with the suspect.");
            Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
            Game.DisplayHelp("If it's a ~r~fake call~w~, press ~b~End~w~ ");


            Suspect = new Ped(SpawnPoint);
            Suspect.IsPersistent = true;
            SuspectsBlip = Suspect.AttachBlip();
            SuspectsBlip.Color = Color.Yellow;
            Game.LogTrivial("(NakedPerson): All Peds' actions loaded.");

            calloutArea = new Blip(SpawnPoint, 40f);
            calloutArea.Color = (System.Drawing.Color.Yellow);
            calloutArea.Alpha = 0.5f;
            calloutArea.EnableRoute(Color.Yellow);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(NakedPerson): Callout Not Accepted.");

            Functions.PlayScannerAudio(MathHelper.Choose(OnNotAcceptedAudio));

            End();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.LogTrivial("(Naked Person): Callout *ENDED*. User pressed END.");

                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio(MathHelper.Choose(PTTAudio));
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(Suspect.Position) < 20f && hasArrived == false) {

                Game.LogTrivial("(NakedPerson): Officer Arrived At Scene");
                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Suspect~w~ to talk with him.");
                hasArrived = true;
                IsSpe﻿echFinished = false;

            }

            if (IsSpe﻿echFinished == false && Game.LocalPlayer.Character.DistanceTo(Suspect.Position) < 5f) {

                while (!Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    GameFiber.Yield();


                Suspect.Tasks.Clear();
                Suspect.Tasks.StandStill(30000);

                Game.DisplaySubtitle("~b~Officer~w~: Hello! What's going on? Are you the caller?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: I don't know anything officer. What's happening?", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: We got a call for an incident exposure on this exact location. Have you seen anyone? ", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: Unfortunately no Officer. Sorry. As you can see, no one here is ~y~naked~w~.. It must have been a prank call...", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Yes. That's how it looks like.", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~b~Officer~w~: Alright. Thank you for your time. Have a nice day!", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Suspect~w~: Thank you ~b~Officer~w~! Take care!", 3500);
                IsSpeechFinished = true;
                GameFiber.Wait(4500);

                Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("You can now press ~b~END~w~ to become ~g~available~w~.");
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