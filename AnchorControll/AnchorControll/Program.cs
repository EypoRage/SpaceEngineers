using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        IMyBlockGroup groupLandingGears;
        List<IMyLandingGear> landingGears;

        IMyBlockGroup groupAnchorPistons;
        List<IMyPistonBase> anchorPistons;
        

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            // only fetch the objects on start to reduce processing complexity
            groupLandingGears = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Anker Fuesse") as IMyBlockGroup;
            landingGears = new List<IMyLandingGear>();
            groupLandingGears.GetBlocksOfType(landingGears);

            groupAnchorPistons = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Anker Kolben") as IMyBlockGroup;
            anchorPistons = new List<IMyPistonBase>();
            groupAnchorPistons.GetBlocksOfType(anchorPistons);
        }

        public void Save()
        {
        
        }

        public void Main(string argument, UpdateType updateSource)
        {

            if (argument.Equals("lockToGround")) {
                lockToGround();

            } else if (argument.Equals("unlockFromGround")) {
                unlockFromGround();

            } else {
                Runtime.UpdateFrequency = UpdateFrequency.None;

            }

            


            
        }


        void lockToGround() {

            bool isFullyLocked = true;

            for (int i = 0; i < landingGears.Count; i++)
            {

                if (!landingGears[i].IsLocked)
                {

                    landingGears[i].AutoLock = true;
                    anchorPistons[i].MaxLimit = 15f;
                    anchorPistons[i].Velocity = 0.1f;
                    isFullyLocked = false;


                }
                else if (landingGears[i].IsLocked)
                {
                    anchorPistons[i].Velocity = 0.0f;
                }

            }

            if (isFullyLocked)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }

        }





        void unlockFromGround()
        {

            bool isFullyUnlocked = true;

            for (int i = 0; i < landingGears.Count; i++)
            {

                if (!landingGears[i].IsLocked)
                {


                }
                else if (landingGears[i].IsLocked)
                {
                    landingGears[i].AutoLock = false;
                    landingGears[i].Unlock();
                    anchorPistons[i].MinLimit = 0f;
                    anchorPistons[i].Velocity = -0.1f;
                    isFullyUnlocked = false;
                }

            }

            if (isFullyUnlocked)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }

        }



    }
}