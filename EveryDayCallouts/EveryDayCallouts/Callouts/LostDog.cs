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

    [CalloutInfo("LostDog", CalloutProbability.VeryHigh)]


    class LostDog : Callout {

        private Ped Owner;
        private Ped Pet;
        private Vector3 OwnerSpawnPoint;
        private Vector3 PetsSpawnPoint;
        private Blip OwnersBlip;
        bool hasArrived;
        bool IsSpe﻿echFinished;


        public override bool OnBeforeCalloutDisplayed() {

            OwnerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            PetsSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(400f));

            ShowCalloutAreaBlipBeforeAccepting(OwnerSpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, OwnerSpawnPoint);

            CalloutMessage = "Lost Dog(Testing)";
            CalloutPosition = OwnerSpawnPoint;
            hasArrived = false;
            Game.LogTrivial("(LostDog): Callout Message Displayed");

            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", OwnerSpawnPoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivial("(LostDog): Callout Accepted.");

            Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio("PTT");

            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.");
            Game.DisplayNotification("Go on ~p~scene~w~ and try to find the ~p~lost~w~ pet from the owners info.");

            Owner = new Ped(OwnerSpawnPoint);
            Owner.BlockPermanentEvents = true;
            OwnersBlip = Owner.AttachBlip();
            OwnersBlip.Color = (System.Drawing.Color.Yellow);


            Pet = new Ped("A_C_Chop", PetsSpawnPoint, 1f);
            Pet.BlockPermanentEvents = true;


            OwnersBlip.IsFriendly = true;

            OwnersBlip.EnableRoute(Color.Green);


            Game.LogTrivial("(LostDog): Owners And Pets actions loaded.");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(LostDog): Callout Not Accepted  (By User)");

            CleanUp();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.LogTrivial("(LostDog): Callout Ended.  User Pressed END. ");

                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(OwnersBlip.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(LostDog): Officer Arrived At Scene.");
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Caller~w~ to talk with him.");
                Game.LogTrivial("(LostDog): Game Help Message Displayed.");

            }

            if (!IsSpe﻿echFinished &&  Game.LocalPlayer.Character.DistanceTo(Owner.Position) < 8f) {

                while (!Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    GameFiber.Yield();


                Owner.Tasks.Clear();
                Owner.Tasks.StandStill(30000);

                Game.DisplaySubtitle("~b~Officer~w~: Hello sir! What happened?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Hello Officer, I have lost my pet and I want your help to find it..", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Alright! Can I have some additional information please?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Sure! It's a ~y~Brown Dog~w~", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Ok. Are you sure it's ~b~Brown~w~?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Yes Officer!", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Well, I will do my best to find your pet. I'm sure it will be ~b~easy~w~ to find it. ", 4000);
                GameFiber.Wait(4500);
                IsSpeechFinished = true;    
                GameFiber.Hibernate();

                GameFiber.Wait(2500);
                Game.DisplayNotification("Search on the ~b~area~w~ to find the lost pet.");

                // TODO:  Here add a "Game.Displayhelp("");" to write if the player need halp to find the pet. If user presses e.g. "A"  key, the Dogs blip will appear on his radar!!!
            }

        }

        public override void End() {

            CleanUp();

            base.End();
        }

        public void CleanUp() {

            if (Owner.Exists()) {
                Owner.Dismiss();
            }
            if (Pet.Exists()) {
                Pet.Dismiss();
            }
            if (OwnersBlip.Exists()) {
                OwnersBlip.Delete();
            }

            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
            Game.DisplayNotification("All units, we are ~g~Code 4~w~");
        }

    }
}
