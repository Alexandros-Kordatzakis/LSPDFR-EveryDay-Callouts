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
        private Blip PetsBlip;
        bool hasArrived;
        bool IsSpe﻿echFinished = false;
        bool OfficerFoundPet = false;
        bool OfficerFoundPetandLeftScene = false;


        public override bool OnBeforeCalloutDisplayed() {

            OwnerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            PetsSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(345f));

            ShowCalloutAreaBlipBeforeAccepting(OwnerSpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, OwnerSpawnPoint);

            // Callout will be displayed now on screen.
            CalloutMessage = "Lost Dog(Testing)";
            CalloutPosition = OwnerSpawnPoint;

            //OfficerFoundPet = false;
            Game.LogTrivial("(LostDog): Callout Message Displayed");
            // Callout Displayed and all Functions (like Logs or Booleans) are in action.

            Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", OwnerSpawnPoint);


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivial("(LostDog): Callout Accepted.");

            Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio("PTT");
            hasArrived = false;

            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.", 5000);
            Game.DisplaySubtitle("Go on ~p~scene~w~ and try to find the ~p~lost~w~ pet from the owners info.");

            // Making new Ped and it's Blip.
            Owner = new Ped(OwnerSpawnPoint);
            Owner.BlockPermanentEvents = true;
            OwnersBlip = Owner.AttachBlip();
            OwnersBlip.Color = (System.Drawing.Color.Yellow);
            OwnersBlip.EnableRoute(Color.Green);


            // Making new Pet.
            Pet = new Ped("A_C_Chop", PetsSpawnPoint, 1f);
            Pet.BlockPermanentEvents = true;
            PetsBlip = Pet.AttachBlip();
            PetsBlip.Color = (System.Drawing.Color.Blue);


            OwnersBlip.IsFriendly = true;
            PetsBlip.IsFriendly = true;


            Game.LogTrivial("(LostDog): Owners And Pets actions loaded.");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            // Callout not accepted by User. Cleaning up everthing that spawned and Blips etc.   CleanUp() Function.
            Game.LogTrivial("(LostDog): Callout Not Accepted  (By User)");

            CleanUp();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            // If user presses END, the callout is canceled and performs the CleanUp() func.  (Logs are being captured.)
            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.LogTrivial("(LostDog): Callout Ended.  User Pressed END. ");

                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                End();
            }

            // If Player is <= 20m from the Owner and the hasArrived bool = flase, help is being Displayed and the Bool hasArrived will be  = true. 
            if (Game.LocalPlayer.Character.DistanceTo(OwnersBlip.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(LostDog): Officer Arrived At Scene.");
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Caller~w~ to talk with him.");
                Game.LogTrivial("(LostDog): Game Help Message Displayed.");

            }

            // Now it starts the Dialogue. If Player is < 8 meters from the Owner, Player can press "Y" and the dialogue will begin. 
            if (Game.LocalPlayer.Character.DistanceTo(Owner.Position) < 8f) {

                while (!Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    GameFiber.Yield();


                Owner.Tasks.Clear();
                Owner.Tasks.StandStill(30000);

                Game.DisplaySubtitle("~b~Officer~w~: Hello sir! What happened?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Hello Officer, I have lost my pet and I want your help to find it..", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Alright! Can I have some additional information?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Sure! It's a ~y~Brown Rottweiler~w~.", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Ok. Are you sure it's ~b~Brown~w~?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Yes Officer!", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Well, I will do my best to find your pet. I'm sure it will be ~b~easy~w~ to find it. ", 4000);
                GameFiber.Wait(4500);
                IsSpeechFinished = true;    


                GameFiber.Wait(2500);
                Game.DisplayNotification("Search on the ~b~area~w~ to find the lost pet.");
                GameFiber.Wait(4000);
                Game.DisplayHelp("For help, when you reach 20 meters close to ~b~Chop~w~, it's Blip will appear on your Radar.");

                // TODO:  Here add a "Game.Displayhelp("");" to write if the player need help to find the pet. If user presses e.g. "A"  key, the Dogs blip will appear on his radar!!!
                // TODO:  Or,  if the player go e.g. 10m near the per, to display him the blip.  DONE!
            }


            // If Officer finds the Pet (<= 5m. from it), the Bool OfficerFoundPet = true  (as it was false until now.) 
            // and will begin a short talk with Dispatcher. 
            if (IsSpeechFinished = true && Game.LocalPlayer.Character.DistanceTo(Pet.Position) <= 5f) {

                OfficerFoundPet = true;
                Game.LogTrivial("Officer found Pet.");

                Functions.PlayScannerAudio("PTT");
                GameFiber.Wait(1000);
                Game.DisplayNotification("Dispacth, I found the lost pet. Let the Owner know my location to come and take it.");
                GameFiber.Wait(1000);
                Functions.PlayScannerAudio("REPORT_RESPONSE_COPY");
                Game.DisplayHelp("You can leave the scene now. Dispatch will take care of everything else.");
            }

            // Assuming that the Officer left the scene, when he is > 20m. away from it, a help message
            // will appear to tell him to press END.  (So the CleanUp() func. will take place.)
            if (OfficerFoundPet = true && Game.LocalPlayer.Character.DistanceTo(Pet.Position) > 20f) {

                Game.DisplayHelp("You can press ~g~End~w~ now.");
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

        }

    }
}
