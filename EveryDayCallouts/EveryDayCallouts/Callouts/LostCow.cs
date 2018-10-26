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



namespace EveryDayCallouts.Callouts
{

    [CalloutInfo("LostCow", CalloutProbability.VeryHigh)]


    class LostCow : Callout
    {

        private Ped Owner;
        private Ped Pet;
        private Vector3 OwnerSpawnPoint;
        private Vector3 PetsSpawnPoint;
        private Blip OwnersBlip;

        private Blip PetsBlip;

//      bool hasArrived;
        bool IsSpeechFinished = false;
//      bool OfficerFoundPet = false;
//      bool OfficerFoundPetandLeftScene = false;


        public override bool OnBeforeCalloutDisplayed() {

            OwnerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            PetsSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(400f));

            ShowCalloutAreaBlipBeforeAccepting(OwnerSpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, OwnerSpawnPoint);

            CalloutMessage = "Lost Pet(Testing)";
            CalloutPosition = OwnerSpawnPoint;
            Game.LogTrivial("(LostCow): Callout Message Displayed.");


            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", OwnerSpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

//          Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
//          Functions.PlayScannerAudio("END_3DPRT_PTT");
//          hasArrived = false;

//          Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.", 5000);
            Game.DisplayNotification("Go on ~p~scene~w~ and try to find the ~p~lost~w~ pet from the owners info.");
            Game.LogTrivial("(LostCow): Callout Accepted.");


            //Peds and Pets Blips.
            Owner = new Ped(OwnerSpawnPoint);
            Owner.BlockPermanentEvents = true;
            OwnersBlip = Owner.AttachBlip();
            OwnersBlip.Color = (System.Drawing.Color.Green);
            OwnersBlip.EnableRoute(Color.Green);


            Pet = new Ped("A_C_Cow", PetsSpawnPoint, 1f);
            Pet.BlockPermanentEvents = true;
            PetsBlip = Pet.AttachBlip();
            PetsBlip.Color = (System.Drawing.Color.Blue);


            OwnersBlip.IsFriendly = true;
            PetsBlip.IsFriendly = true;

            OwnersBlip.EnableRoute(System.Drawing.Color.Purple);


            Game.LogTrivial("(LostCow): All Owners And Ped Actions Loaded!");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(LostCow): Callout Not Accepted. (From User)");

            CleanUp();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {
            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
//              Functions.PlayScannerAudio("PTT");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
//              Functions.PlayScannerAudio("END_3DPRT_PTT");
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(OwnersBlip.Position) <= 20f) {

//              hasArrived = true;
                Game.LogTrivial("(LostCow): Officer Arrived At Scene.");
//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Caller~w~ to talk with him.");
                Game.LogTrivial("(LostCow): Game Help Message Displayed.");
            }

            if (Game.LocalPlayer.Character.DistanceTo(Owner.Position) < 8f) {

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
                Game.DisplaySubtitle("~o~Owner~w~: Sure! It's a ~y~White Cow~w~, with ~b~Brown~w~ marks.", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: A COW ?!", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Yes sir! A cow.", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Ok... Are you sure it's not ~b~Brown~w~, with ~y~White~w~ marks?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Yes Officer!", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Well, I don't know how you lost a ~y~cow~w~, but I'm sure it will be ~b~easy~w~ to find it. ", 4000);
                GameFiber.Wait(4500);
                IsSpeechFinished = true;

                GameFiber.Wait(2500);
//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayNotification("Search on the ~b~area~w~ to find the lost pet.");
                GameFiber.Wait(4000);
  //            Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("For help, when you reach 20 meters close to ~b~Chop~w~, it's Blip will appear on your Radar.");
            }



            // If Officer finds the Pet (<= 5m. from it), the Bool OfficerFoundPet = true  (as it was false until now.) 
            // and will begin a short talk with Dispatcher. 
            if (IsSpeechFinished = true && Game.LocalPlayer.Character.DistanceTo(Pet.Position) <= 5f) {

//              OfficerFoundPet = true;
                Game.LogTrivial("(Lost Cow) Officer found pet = true");
                Game.LogTrivial("Officer found Pet.");

//              Functions.PlayScannerAudio("PTT");
                Game.DisplayNotification("Dispacth, I found the lost pet. Let the Owner know my location to come and take it.");
                Functions.PlayScannerAudio("REPORT_RESPONSE_COPY");
//              Functions.PlayScannerAudio("END_3DPRT_PTT");

//              Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayHelp("You can leave the scene now. Dispatch will take care of everything else.");
                Game.DisplayHelp("You can press ~b~END~w~ now. Good Job officer!");
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
            if (PetsBlip.Exists()) {
                PetsBlip.Delete();
            }

        }

    }
}