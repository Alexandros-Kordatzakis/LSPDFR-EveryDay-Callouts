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


        private readonly string[] OnNotAcceptedAudio = { "AI_RESPOND_01", "AI_RESPOND_02", "AI_RESPOND_03", "AI_RESPOND_04", "AI_RESPOND_05" };
        private readonly string[] End3rdPRTAudio = { "END_3DPRT_PTT_01", "END_3DPRT_PTT_02", "END_3DPRT_PTT_03", "END_3DPRT_PTT_04", "END_3DPRT_PTT_05", "END_3DPRT_06" };
        private readonly string[] NotiffSound = { "NOTIF_SOUND_1", "NOTIF_SOUND_2" };
        private readonly string[] PTTAudio = { "PTT_1", "PTT_2", "PTT_3" };


        public override bool OnBeforeCalloutDisplayed() {

            OwnerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            PetsSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(315f));

//          ShowCalloutAreaBlipBeforeAccepting(OwnerSpawnPoint, 30f);

            // Callout will be displayed now on screen.
            CalloutMessage = "Lost Dog(Testing)";
            CalloutPosition = OwnerSpawnPoint;

            Game.LogTrivialDebug("(LostDog Debug): Callout Message Displayed");
            // Callout Displayed and all Functions (Logs or Booleans) are in action.

            Functions.PlayScannerAudio(MathHelper.Choose(PTTAudio));
            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", OwnerSpawnPoint);
            Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Game.LogTrivialDebug("(LostDog Debug): Callout Accepted.");

            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));
            hasArrived = false;
            OfficerFoundPet = false;

            Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplaySubtitle("Get on ~p~scene~w~ and try to find the ~p~lost~w~ pet from the owners info.", 8000);


            // Making new Ped and it's Blip.
            Owner = new Ped(OwnerSpawnPoint);
            Owner.IsPersistent = true;
            OwnersBlip = Owner.AttachBlip();
            OwnersBlip.Color = Color.Yellow;
            OwnersBlip.EnableRoute(System.Drawing.Color.Yellow);


            // Making new Pet.
            Pet = new Ped("A_C_Chop", PetsSpawnPoint, 1f);
            Pet.IsPersistent = true;
            PetsBlip = Pet.AttachBlip();
            PetsBlip.Color = Color.White;


            Game.LogTrivialDebug("(LostDog Debug): Owners and Pets actions loaded.");

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted() {

            // Callout not accepted by User. Cleaning up everthing that spawned and Blips etc.   CleanUp() Function.
            Game.LogTrivialDebug("(LostDog Debug): Callout Not Accepted  (By User)");

            Functions.PlayScannerAudio(MathHelper.Choose(OnNotAcceptedAudio));

            CleanUp();

            base.OnCalloutNotAccepted();
        }

        public override void Process() {
           

            // If user presses END, the callout is canceled and performs the CleanUp() func.  (Logs are being captured.)

            if (Game.IsKeyDown(Keys.End)) {

                Game.LogTrivialDebug("(LostDog Debug): Callout Ended.  User Pressed END.");

                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));

                CleanUp();
                End();
            }


            // If Player is <= 20m from the Owner and the hasArrived = flase, help is being Displayed and the Bool hasArrived will be  = true. 
            if (Game.LocalPlayer.Character.DistanceTo(OwnerSpawnPoint) < 15f) {

                hasArrived = true;

                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~b~Caller~w~ to talk with him.", 2500);
                OwnersBlip.DisableRoute();

            }

            // Now it starts the Dialogue. If Player is <8 meters from the Owner, Player can press "Y" and the dialogue will begin. 
            if (hasArrived = true && Game.LocalPlayer.Character.DistanceTo(OwnerSpawnPoint) < 5f) {

                while (!Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    GameFiber.Yield();


                Owner.Tasks.Clear();
                Owner.Tasks.StandStill(30000);

                Game.DisplaySubtitle("~b~Officer~w~: Hello sir! What happened?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Hello Officer, I have lost my pet and I want your help to find it..", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Alright. Can I have some additional information?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Sure! It's a ~y~Brown Rottweiler~w~.", 4000);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Ok. Are you sure it's ~b~Brown~w~?", 4000);
                GameFiber.Wait(4500);
                Game.DisplaySubtitle("~o~Owner~w~: Yes Officer!", 3500);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle("~b~Officer~w~: Well, I will do my best to find your pet! Hope it's somewhere here nearby... ", 4000);
                GameFiber.Wait(4500);

                IsSpeechFinished = true;
                Game.LogTrivial("(LostDog): IsSpeechFinished = true;");


                GameFiber.Wait(2500);
                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayNotification("Search on the ~b~area~w~ to find the lost pet.");
                GameFiber.Wait(4000);
                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayHelp("For help, ~b~Chop's~w~ Blip is on your Radar.");

            }

 
            if (IsSpeechFinished = true && Game.LocalPlayer.Character.DistanceTo(PetsBlip) < 4f) {

                OfficerFoundPet = true;
                Game.LogTrivialDebug("(LostDog Debug): Officer Found Pet.");


                Functions.PlayScannerAudio(MathHelper.Choose(PTTAudio));
                Game.DisplayNotification("Dispacth, I see the lost pet. Let the Owner know my location to come and take it.");
                Functions.PlayScannerAudio("REPORT_RESPONSE_COPY");
                Functions.PlayScannerAudio(MathHelper.Choose(End3rdPRTAudio));

                Functions.PlayScannerAudio(MathHelper.Choose(NotiffSound));
                Game.DisplayHelp("You can leave the scene now. Dispatch will take care of everything else.");
                
            }

            if (OfficerFoundPet = true && Game.LocalPlayer.Character.DistanceTo(PetsBlip) > 8f) {

                OfficerFoundPetandLeftScene = true;
                Game.LogTrivial("(LostDog Log): OfficerFoundPetandLeftScene = true;");

            } 


            if (OfficerFoundPetandLeftScene = true && Game.LocalPlayer.Character.DistanceTo(PetsBlip) > 15f) {

                Game.LogTrivialDebug("(LostDog Debug): Officer Left the Scene from the Pet.");
                Game.DisplayNotification("You can press ~b~End~w~ now.");

            }


            base.Process();

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

            OwnersBlip.DisableRoute();
        }
    }
}
