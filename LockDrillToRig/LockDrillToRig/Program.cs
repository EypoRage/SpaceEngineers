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
using ParallelTasks;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set RuntimeInfo.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {

           


            // merge blocks and piston for piston unit
            IMyShipMergeBlock mbDrillBase = GridTerminalSystem.GetBlockWithName("mb") as IMyShipMergeBlock;
            IMyShipMergeBlock mbDrillBaseFrame = GridTerminalSystem.GetBlockWithName("mb") as IMyShipMergeBlock;
            IMyPistonBase piDrillBase = GridTerminalSystem.GetBlockWithName("pi") as IMyPistonBase;

            // merge Blocks and piston for drill head
            IMyShipMergeBlock mbDrillHead = GridTerminalSystem.GetBlockWithName("mb") as IMyShipMergeBlock;
            IMyShipMergeBlock mbDrillHeadFrame = GridTerminalSystem.GetBlockWithName("mb") as IMyShipMergeBlock;
            IMyPistonBase piDrillHead = GridTerminalSystem.GetBlockWithName("pi") as IMyPistonBase;


            if (IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame))
            {
                // piston unit already locked


            }
            else if (!(IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame))) {

                // piston unit is unlocked
                mergeBlocks(mbDrillBase, mbDrillBaseFrame, piDrillBase);

            }
            else
            {

                // only one merge block is merged --- This is unexpected behaviour

            }





            if (IsMerged(mbDrillHead) && IsMerged(mbDrillHeadFrame))
            {
                // piston unit already locked

            }
            else if (!(IsMerged(mbDrillHead) && IsMerged(mbDrillHeadFrame)))
            {

                // piston unit is unlocked
                mergeBlocks(mbDrillHead, mbDrillHeadFrame, piDrillHead);

            }
            else
            {

                // only one merge block is merged --- This is unexpected behaviour

            }


        }




        bool IsMerged(IMyShipMergeBlock mrg1)
        {
            //Find direction that block merges to
            Matrix mat;
            mrg1.Orientation.GetMatrix(out mat);
            Vector3I right1 = new Vector3I(mat.Right);

            //Check if there is a block in front of merge face
            IMySlimBlock sb = mrg1.CubeGrid.GetCubeBlock(mrg1.Position + right1);
            if (sb == null) return false;

            //Check if the other block is actually a merge block
            IMyShipMergeBlock mrg2 = sb.FatBlock as IMyShipMergeBlock;
            if (mrg2 == null) return false;

            //Check that other block is correctly oriented
            mrg2.Orientation.GetMatrix(out mat);
            Vector3I right2 = new Vector3I(mat.Right);
            return right2 == -right1;
        }


        void mergeBlocks(IMyShipMergeBlock mbMovable, IMyShipMergeBlock mbFrame, IMyPistonBase pi) {

            // enable needed blocks (override any state)
            mbMovable.Enabled = true;
            mbFrame.Enabled = true;
            pi.Enabled = true;

            // set piston velocity, configure other properties in game
            pi.Velocity = 0.1f;

            //check for merge 
            while (!(IsMerged(mbMovable) && IsMerged(mbFrame))) {

                // wait....

            }

            // stop piston
            pi.Velocity = 0f;


        }



    }
}