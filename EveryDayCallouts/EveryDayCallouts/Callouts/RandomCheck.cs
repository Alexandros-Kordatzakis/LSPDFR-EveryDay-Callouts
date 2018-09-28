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
        private Ped GangMemb6;
        private Ped GangMemb7;
        private Ped GangMemb8;
        private Vehicle SuspectsVehicle;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip1;
        private Blip SuspectBlip2;
        private Blip SuspectBlip3;
        private Blip SuspectBlip4;
        private Blip SuspectBlip5;
        private Blip SuspectBlip6;
        private Blip SuspectBlip7;
        private Blip SuspectBlip8;
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

            Functions.PlayScannerAudio("PTT");
            GameFiber.Wait(1000);
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            GameFiber.Wait(500);
            Functions.PlayScannerAudio("END_3DPRT_PTT");

            GameFiber.Wait(1000);
            Functions.PlayScannerAudio("NOTIF_SOUND");
            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.", 5000);


            SuspectsVehicle = new Vehicle("BURRITO", SpawnPoint, 1f);
            SuspectsVehicle.IsPersistent = true;

            GangMemb1 = new Ped("CSB_BallasOG", SpawnPoint, 1f);
            GangMemb1.BlockPermanentEvents = true;
            SuspectBlip1 = GangMemb1.AttachBlip();

            GangMemb2 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb2.BlockPermanentEvents = true;
            SuspectBlip2 = GangMemb2.AttachBlip();

            GangMemb3 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb3.BlockPermanentEvents = true;
            SuspectBlip3 = GangMemb3.AttachBlip();

            GangMemb4 = new Ped("CSB_Ramp_gang", SpawnPoint, 1f);
            GangMemb4.BlockPermanentEvents = true;
            SuspectBlip4 = GangMemb4.AttachBlip();

            GangMemb5 = new Ped("IG_RAMP_GANG", SpawnPoint, 1f);
            GangMemb5.BlockPermanentEvents = true;
            SuspectBlip5 = GangMemb5.AttachBlip();

            GangMemb6 = new Ped("CSB_BallasOG", SpawnPoint, 3f);
            GangMemb6.BlockPermanentEvents = true;
            SuspectBlip6 = GangMemb6.AttachBlip();

            GangMemb7 = new Ped("CSB_BallasOG", SpawnPoint, 4f);
            GangMemb7.BlockPermanentEvents = true;
            SuspectBlip7 = GangMemb7.AttachBlip();

            GangMemb8 = new Ped("CSB_Ramp_gang", SpawnPoint, 8f);
            GangMemb8.BlockPermanentEvents = true;
            SuspectBlip8 = GangMemb8.AttachBlip();


            SuspectBlip1.IsFriendly = false;
            SuspectBlip2.IsFriendly = false;
            SuspectBlip3.IsFriendly = false;
            SuspectBlip4.IsFriendly = false;
            SuspectBlip5.IsFriendly = false;
            SuspectBlip6.IsFriendly = false;
            SuspectBlip7.IsFriendly = false;
            SuspectBlip8.IsFriendly = false;

            calloutArea = new Blip(SpawnPoint, 40f);
            calloutArea.Color = (System.Drawing.Color.Red);
            calloutArea.Alpha = 0.5f;
            calloutArea.EnableRoute(System.Drawing.Color.Blue);


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

                Functions.PlayScannerAudio("NOTIF_SOUND");
                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                GameFiber.Wait(500);
                Functions.PlayScannerAudio("PTT");
                GameFiber.Wait(500);
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                GameFiber.Wait(500);
                Functions.PlayScannerAudio("END_3DPRT_PTT");
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(SuspectBlip2.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(RandomCheck): Officer Arrived At Scene.");
            }

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(GangMemb1.Position) <= 15f) {


                GangMemb2.PlayAmbientSpeech("Run! It's the police!");
                Game.DisplaySubtitle("~r~Gang Member:~w~ Run! It's the police!", 2000);
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, GangMemb1);
                GangMemb1.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb2);
                GangMemb2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb3);
                GangMemb3.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb4);
                GangMemb4.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb5);
                GangMemb5.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb6);
                GangMemb6.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Functions.AddPedToPursuit(Pursuit, GangMemb7);
                GangMemb7.Tasks.AimWeaponAt(Game.LocalPlayer.Character, 10000);
                Functions.AddPedToPursuit(Pursuit, GangMemb8);
                GangMemb8.Tasks.FightAgainst(Game.LocalPlayer.Character);


                if (Functions.IsPedArrested(GangMemb1)) {
                    GangMemb1.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb1)) {
                    GangMemb1.Tasks.ClearImmediately(); 
                }
                
                if (Functions.IsPedArrested(GangMemb2)) {
                    GangMemb2.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb2)) {
                    GangMemb2.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb3)) {
                    GangMemb3.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb3)) {
                    GangMemb3.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb4)) {
                    GangMemb4.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb4)) {
                    GangMemb4.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb5)) {
                    GangMemb5.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb5)) {
                    GangMemb5.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb6)) {
                    GangMemb6.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb6)) {
                    GangMemb6.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb7)) {
                    GangMemb7.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb7)) {
                    GangMemb7.Tasks.ClearImmediately();
                }

                if (Functions.IsPedArrested(GangMemb8)) {
                    GangMemb8.Tasks.ClearImmediately();
                }
                if (Functions.IsPedStoppedByPlayer(GangMemb8)) {
                    GangMemb8.Tasks.ClearImmediately();
                }


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
/*
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
            if (GangMemb6.Exists()) {
                GangMemb6.Dismiss();
            }
            if (GangMemb7.Exists()) {
                GangMemb7.Dismiss();
            }
            if (GangMemb8.Exists()) {
                GangMemb8.Dismiss();
            }

            if (SuspectsVehicle.Exists()) {
                SuspectsVehicle.Dismiss();         ///// Not mandatory.
            }  */
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
            if (SuspectBlip6.Exists())
            {
                SuspectBlip6.Delete();
            }
            if (SuspectBlip7.Exists())
            {
                SuspectBlip7.Delete();
            }
            if (SuspectBlip8.Exists())
            {
                SuspectBlip8.Delete();
            }
            if (calloutArea.Exists()) {
                calloutArea.Delete();
            }

        }

    }
}






                


