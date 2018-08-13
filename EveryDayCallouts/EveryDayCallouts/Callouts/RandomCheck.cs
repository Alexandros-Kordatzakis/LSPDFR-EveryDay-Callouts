using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System.Drawing;




namespace EveryDayCallouts.Callouts {

    [CalloutInfo("RandomCheck", CalloutProbability.VeryHigh)]

    public class RandomCheck : Callout {

        private Ped GangMemb1;
        private Ped GangMemb2;
        private Ped GangMemb3;
        private Ped GangMemb4;
        private Ped GangMemb5;
        private Vehicle SuspectsVehicle;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip1;
        private Blip SuspectBlip2;
        private Blip SuspectBlip3;
        private Blip SuspectBlip4;
        private Blip SuspectBlip5;
        private Blip calloutArea;
        private LHandle Pursuit;
        bool hasArrived;
        private bool PursuitCreated = false;


        public override bool OnBeforeCalloutDisplayed() {

            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, SpawnPoint);

            CalloutMessage = "Too many Gang members together(Testing)";
            CalloutPosition = SpawnPoint;
            hasArrived = false;
            Game.LogTrivial("(RandomCheck): Callout Messsage Displayed");

            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", SpawnPoint);


            return base.OnBeforeCalloutDisplayed();
        }


        public override bool OnCalloutAccepted() {

//            Functions.PlayScannerAudio("PTT");
//            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio("PTT");
            Game.DisplayNotification("Respond ~b~Code 2~w~");

            SuspectsVehicle = new Vehicle("BURRITO", SpawnPoint, 1f);
            SuspectsVehicle.IsPersistent = true;

            GangMemb1 = new Ped("CSB_BallasOG", SpawnPoint, 1f);
            GangMemb1.BlockPermanentEvents = true;
            SuspectBlip1 = GangMemb1.AttachBlip();

            GangMemb2 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb2.BlockPermanentEvents = true;
            SuspectBlip2 = GangMemb3.AttachBlip();

            GangMemb3 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb3.BlockPermanentEvents = true;
            SuspectBlip3 = GangMemb3.AttachBlip();

            GangMemb4 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb4.BlockPermanentEvents = true;
            SuspectBlip4 = GangMemb4.AttachBlip();

            GangMemb5 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb5.BlockPermanentEvents = true;
            SuspectBlip5 = GangMemb5.AttachBlip();


            SuspectBlip1.IsFriendly = false;
            SuspectBlip2.IsFriendly = false;
            SuspectBlip3.IsFriendly = false;
            SuspectBlip4.IsFriendly = false;
            SuspectBlip5.IsFriendly = false;

            calloutArea = new Blip(SpawnPoint, 40f);
            calloutArea.Color = (System.Drawing.Color.Red);
            calloutArea.Alpha = 0.5f;
            calloutArea.EnableRoute(System.Drawing.Color.Yellow);


            Game.LogTrivial("(RandomCheck): All Peds' and Vehicles' actions loaded.");

            return base.OnCalloutAccepted();
        }


        public override void OnCalloutNotAccepted() {

            Game.LogTrivial("(RandomCheck): Callout Not Accepted");

            CleanUp();
            base.OnCalloutNotAccepted();
        }

        public override void Process() {

            base.Process();

            if (Game.IsKeyDown(System.Windows.Forms.Keys.End)) {

                Game.LogTrivial("(RandomCheck): If statement executed. User pressed END and canceled the callout.");

                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");

                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(SuspectBlip2.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(RandomCheck): Officer Arrived At Scene.");
            }

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(GangMemb1.Position) <= 15f) {


                GangMemb2.PlayAmbientSpeech("Run! Its the police!");
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, GangMemb1);
                Functions.AddPedToPursuit(Pursuit, GangMemb2);
                Functions.AddPedToPursuit(Pursuit, GangMemb3);
                Functions.AddPedToPursuit(Pursuit, GangMemb4);
                Functions.AddPedToPursuit(Pursuit, GangMemb5);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;

                Game.LogTrivial("(RandomCheck): Pursuit Created. All gang members are added to it. ");
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

            if (GangMemb1.Exists()) {
                GangMemb1.Dismiss();
            }
            if (GangMemb2.Exists()) {
                GangMemb2.Dismiss();
            }
            if (GangMemb3.Exists()) {
                GangMemb3.Dismiss();
            }
            if (GangMemb4.Exists()) {
                GangMemb4.Dismiss();
            }
            if (GangMemb5.Exists()) {
                GangMemb5.Dismiss();
            }

            //            if (SuspectsVehicle.Exists()) {
            //                SuspectsVehicle.Dismiss();         ///// Not mandatory.
            //            }

            if (SuspectBlip1.Exists()) {
                SuspectBlip1.Delete();
            }
            if (SuspectBlip2.Exists()) {
                SuspectBlip2.Delete();
            }
            if (SuspectBlip3.Exists()) {
                SuspectBlip3.Delete();
            }
            if (SuspectBlip4.Exists()) {
                SuspectBlip4.Delete();
            }
            if (SuspectBlip5.Exists()) {
                SuspectBlip5.Delete();
            }
            if (calloutArea.Exists()) {
                calloutArea.Delete();
            }

            Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
            Game.DisplayNotification("All units, we are ~g~Code 4~w~");
        }

    }
}


/* 
 TODO:  Add dialogues to the code in the files:  LostCow.cs, LostDog.cs, NakedPerson.cs

 FIXME:  Check if NoNameSet send me a message on LCPDFR.com with how to use this code.    Then, implement this code to the files mentioned in TODO. 
 
 private readonly List<string> dialogWithPed = new List<string>() {

    "Officer: Hello sir! What happened?",
    "Caller: Hello Officer, I have lost my pet and I want your help to find it.."
    "Officer: Alright! Can I have some additional information please?"
    "Caller: Sure! It's a ~y~White Cow~w~, with ~b~Brown~w~ marks."
    "Officer: A COW ?!"
    "Caller: Yes sir. A ~r~cow~w~."
    "Officer: Well, I don't know how you lost a ~y~cow~w~, but I'm sure it will be ~b~easy~w~ to find it."
}

private int dialogWithPedIndex;

if (!Game.IsKeyDown(Keys.Y)) return;

if (dialogWithPedIndex < dialogWithPed.Count) {

    Game.DisplaySubtitle(dialogWithPed[dialogWithPedIndex]);
    dialogWithPedIndex++;
}

if (dialogWithPedIndex == 7) {

}


*/ 