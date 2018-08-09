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

    [CalloutInfo("LostCow", CalloutProbability.VeryHigh)]


    class LostCow : Callout {

        private Ped Owner;
        private Ped Pet;
        private Vector3 OwnerSpawnPoint;
        private Vector3 PetsSpawnPoint;
        private Blip OwnersBlip;

        private Blip PetsBlip;
        bool helpForBlip;

        bool hasArrived;


        public override bool OnBeforeCalloutDisplayed() {

            OwnerSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            PetsSpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(400f));

            ShowCalloutAreaBlipBeforeAccepting(OwnerSpawnPoint, 30f);
            AddMinimumDistanceCheck(20f, OwnerSpawnPoint);

            CalloutMessage = "Lost Pet(Testing)";
            CalloutPosition = OwnerSpawnPoint;
            hasArrived = false;
            Game.LogTrivial("(LostCow): Callout Message Displayed.");


            Functions.PlayScannerAudioUsingPosition("IN_OR_ON_POSITION", OwnerSpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted() {

            Functions.PlayScannerAudio("PTT");
            Functions.PlayScannerAudio("RESPOND_CODE_2");
            Functions.PlayScannerAudio("PTT");

            Game.DisplayNotification("Respond ~b~Code 2~w~");
            Game.DisplayHelp("Press ~b~End~w~ to end the callout.");
            Game.DisplayNotification("Go on ~p~scene~w~ and try to find the ~p~lost~w~ pet from the owners info.");
            Game.LogTrivial("(LostCow): Callout Accepted.");

            Owner = new Ped(OwnerSpawnPoint);
            Owner.BlockPermanentEvents = true;
            OwnersBlip = Owner.AttachBlip();
            OwnersBlip.Color = (System.Drawing.Color.Green);


            Pet = new Ped("A_C_Cow", PetsSpawnPoint, 1f);
            Pet.BlockPermanentEvents = true;


            OwnersBlip.IsFriendly = true;

            OwnersBlip.EnableRoute(Color.Green);

            Game.LogTrivial("(LostCow): All Owners And Peds Actions Loaded!");

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

                Game.LogTrivial("(LostCow): Callout Ended.  User Pressed END. ");

                Game.DisplayNotification("~g~Code 4~w~, return to patrol.");
                Functions.PlayScannerAudio("ATTENTION_ALL_UNITS WE_ARE_CODE_4");
                End();
            }

            if (Game.LocalPlayer.Character.DistanceTo(OwnersBlip.Position) <= 20f && !hasArrived) {

                hasArrived = true;
                Game.LogTrivial("(LostCow): Officer Arrived At Scene.");
                Game.DisplayHelp("Press ~p~Y~w~ when you reach the ~y~Caller~w~ to talk with him.");
                Game.LogTrivial("(LostCow): Game Help Message Displayed.");
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
