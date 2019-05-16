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
    partial class Program : MyGridProgram{
        // merge blocks and piston for piston unit
        IMyShipMergeBlock mbDrillBase;
        IMyShipMergeBlock mbDrillBaseFrame;
        IMyPistonBase piDrillBase;

        // merge Blocks and piston for drill head
        IMyShipMergeBlock mbDrillHead;
        IMyShipMergeBlock mbDrillHeadFrame;

        //STATUS LCD
        IMyTextPanel statusLcd;

        public Program(){

            // merge blocks and piston for piston unit
            mbDrillBase = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Basis 1") as IMyShipMergeBlock;
            mbDrillBaseFrame = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Basis 2") as IMyShipMergeBlock;
            piDrillBase = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Kolben Basis") as IMyPistonBase;

            // merge Blocks and piston for drill head
            mbDrillHead = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Kopf 1") as IMyShipMergeBlock;
            mbDrillHeadFrame = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Kopf 2") as IMyShipMergeBlock;

            // status lcd
            statusLcd = GridTerminalSystem.GetBlockWithName("Bohrinsel Bohrer Status LCD") as IMyTextPanel;

        }

        public void Save(){
            // nothing <3
        }

        public void Main(string argument, UpdateType updateSource){

            statusLcd.WriteText("");

            switch (argument){

                case "extendBelowRig":
                    extendBelowRig();
                    break;

                case "retractOverRig":
                    retractOverRig();
                    break;

                case "drill":
                    drill();
                    break;

                case "stopDrill":
                    stopDrill();
                    break;

                case "retractDrill":
                    retractDrill();
                    break;

                case "fullyRetractDrill":
                    fullRetractDrill();
                    break;

                case "unlock":
                    unlock();
                    break;

                case "lock":
                    mergelock();
                    break;

                default:
                    break;
            }

        }



        public void extendBelowRig()
        {

            // nur wenn: der bohrer nicht bohrt und der bohrer abgedockt ist und der rotor nicht dreht und die pistons zwischen 0 und 3m lang sind und nur wenn das rig verankert ist

            if (!isDrilling() && !isLocked() && !isRotating() && isParking() && isAnchoredToGround())
            {

                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();
                // Fetch the blocks from that group. Note that the type designation is implicit
                // here, it's defined by the list above - this is why you don't need to specify
                // the type in <>.
                //To be clear there is no additional permutation needed beacause the list that ist the sorting parametar alreadis IS permutated
                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    // set the mal lenght of the piston to 1m so the drill head is below the rig
                    piston.MaxLimit = 3.0f;
                    // set the velocity crazy low because there are 6 pistons that will add up (6 * 0.1 = 0.6m/s) may be to fast
                    piston.Velocity = 0.1f;
                }

            }
            else
            {
                statusLcd.WriteText("Bohrsation nicht bereit", true);
            }


        }


        public void retractOverRig()
        {

            // nur wenn: der bohrer nicht bohrt und der bohrer abgedockt ist und der rotor nicht dreht und die pistons zwischen 0 und 1m lang sind und nur wenn das rig verankert ist

            if (!isDrilling() && !isLocked() && !isRotating() && isParking() && isAnchoredToGround())
            {

                IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;
                setRotorAngle(rotor, 220);

                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();
                // Fetch the blocks from that group. Note that the type designation is implicit
                // here, it's defined by the list above - this is why you don't need to specify
                // the type in <>.
                //To be clear there is no additional permutation needed beacause the list that ist the sorting parametar alreadis IS permutated
                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    // set the mal lenght of the piston to 1m so the drill head is below the rig
                    piston.MaxLimit = 3.0f;
                    piston.MinLimit = 0.0f;
                    // set the velocity crazy low because there are 6 pistons that will add up (6 * 0.1 = 0.6m/s) may be to fast
                    piston.Velocity = -0.1f;
                }

            }


        }

        public void drill()
        {

            if (pistonDistance() >= 18)
            {

                IMyBlockGroup groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;

                List<IMyShipDrill> drills = new List<IMyShipDrill>();
                groupDrills.GetBlocksOfType(drills);

                foreach (IMyShipDrill drill in drills)
                {
                    drill.Enabled = true;
                }


                IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;
                rotor.RotorLock = false;
                rotor.TargetVelocityRPM = 3;


                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();

                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    piston.MaxLimit = 999;
                    piston.Velocity = 0.01f;

                }


            }
        }


        public void stopDrill()
        {

            if (pistonDistance() >= 18)
            {

                IMyBlockGroup groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;

                List<IMyShipDrill> drills = new List<IMyShipDrill>();
                groupDrills.GetBlocksOfType(drills);

                foreach (IMyShipDrill drill in drills)
                {
                    drill.Enabled = false;
                }


                IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;
                rotor.RotorLock = false;
                rotor.TargetVelocityRPM = 0;


                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();

                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    piston.MaxLimit = 999;
                    piston.Velocity = 0.00f;

                }


            }
        }


        public void retractDrill()
        {

            if (pistonDistance() >= 18)
            {

                IMyBlockGroup groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;

                List<IMyShipDrill> drills = new List<IMyShipDrill>();
                groupDrills.GetBlocksOfType(drills);

                foreach (IMyShipDrill drill in drills)
                {
                    drill.Enabled = false;
                }

                IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;
                rotor.RotorLock = false;
                rotor.TargetVelocityRPM = 0;

                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();

                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    piston.MinLimit = 2f;
                    piston.MaxLimit = 999.00f;
                    piston.Velocity = -0.10f;

                }
            }
        }



        public void fullRetractDrill()
        {

            if (pistonDistance() >= 18)
            {

                IMyBlockGroup groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;

                List<IMyShipDrill> drills = new List<IMyShipDrill>();
                groupDrills.GetBlocksOfType(drills);

                foreach (IMyShipDrill drill in drills)
                {
                    drill.Enabled = false;
                }


                IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

                List<IMyPistonBase> pistons = new List<IMyPistonBase>();

                groupPistonDrill.GetBlocksOfType(pistons);

                foreach (IMyPistonBase piston in pistons)
                {
                    piston.MinLimit = 0f;
                    piston.MaxLimit = 999.00f;
                    piston.Velocity = -0.10f;

                }

                IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;

                setRotorAngle(rotor, 220);


            }
        }


        //TODO: this will be a pain in the ass :(

        public void anchorRig()
        {

            IMyBlockGroup groupLandingGears = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Anker") as IMyBlockGroup;

            List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
            groupLandingGears.GetBlocksOfType(landingGears);

            foreach (IMyLandingGear landingGear in landingGears) {

                landingGear.AutoLock = true;

            }

            IMyBlockGroup groupPistonsLG = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Kolben Anker") as IMyBlockGroup;

            List<IMyPistonBase> pistonsLG = new List<IMyPistonBase>();
            groupPistonsLG.GetBlocksOfType(pistonsLG);

            foreach (IMyPistonBase pistonLG in pistonsLG)
            {
                

            }




        }

        public void unAnchorRig()
        {

        }






        public bool isDrilling()
        {

            bool isDrilling = false;

            IMyBlockGroup groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;

            List<IMyShipDrill> drills = new List<IMyShipDrill>();
            groupDrills.GetBlocksOfType(drills);

            foreach (IMyShipDrill drill in drills)
            {
                if (drill.Enabled)
                {
                    isDrilling = true;
                }
            }

            statusLcd.WriteText("Is Drilling: " + isDrilling, true);
            return isDrilling;
        }


        public bool isLocked()
        {
            if (IsMerged(mbDrillBase) && IsMerged(mbDrillHead))
            {

                return true;

            }
            else {
                return false;
            }


        }

        public bool isRotating()
        {
            bool isRotating = true;

            IMyMotorAdvancedStator rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;

            //rotor might be moving ever so slightly so i implemented a buffer zone of 0.3 rpm
            if (rotor.TargetVelocityRPM < 0.3f)
            {
                isRotating = false;
            }

            statusLcd.WriteText("Is Rotating: " + isRotating, true);

            return isRotating;
        }

        public bool isParking()
        {
            bool isParking = true;

            IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

            List<IMyPistonBase> pistons = new List<IMyPistonBase>();

            groupPistonDrill.GetBlocksOfType(pistons);

            foreach (IMyPistonBase piston in pistons)
            {
                var pistonInf = piston.DetailedInfo;


                //splits the string into an array by separating by the ':' character
                string[] pistInfArr = (pistonInf.Split(':'));

                // splits the resulting 0.0m into an array with single index of "0.0" by again splitting by character "m"
                string[] pistonDist = (pistInfArr[1].Split('m'));

                //uses double.Parse method to parse the "0.0" into a usable double of value 0.0
                double pistonDistD = double.Parse(pistonDist[0]);

                if (pistonDistD > 4)
                {

                    isParking = false;
                }


            }

            statusLcd.WriteText("Is Parking: " + isParking, true);
            return isParking;

        }







        public double pistonDistance()
        {
            double pistonDistance = 0;

            IMyBlockGroup groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;

            List<IMyPistonBase> pistons = new List<IMyPistonBase>();

            groupPistonDrill.GetBlocksOfType(pistons);

            foreach (IMyPistonBase piston in pistons)
            {
                var pistonInf = piston.DetailedInfo;


                //splits the string into an array by separating by the ':' character
                string[] pistInfArr = (pistonInf.Split(':'));

                // splits the resulting 0.0m into an array with single index of "0.0" by again splitting by character "m"
                string[] pistonDist = (pistInfArr[1].Split('m'));

                //uses double.Parse method to parse the "0.0" into a usable double of value 0.0
                double pistonDistD = double.Parse(pistonDist[0]);

                pistonDistance = pistonDistance + pistonDistD;


            }

            return pistonDistance;

        }







        //TODO:  this will be a pain in the ass :(
        public bool isAnchoredToGround()
        {
            

            IMyBlockGroup groupLandingGears = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Anker") as IMyBlockGroup;

            List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
            groupLandingGears.GetBlocksOfType(landingGears);

            foreach (IMyLandingGear landingGear in landingGears)
            {

                if (landingGear.IsLocked) {
                    return true;
                }

            }

            //DEBUG!!
            return true;
        }




        public void setRotorAngle(IMyMotorAdvancedStator thisRotor, float angle)
        {

            float currentAngle = thisRotor.Angle / (float)Math.PI * 180f;

            Echo("Current Angle: " + currentAngle.ToString());
            Echo("New Angle: " + angle.ToString());

            if (angle > currentAngle)
            {
                thisRotor.SetValue<float>("UpperLimit", angle);
                thisRotor.SetValue<float>("LowerLimit", -361f);
                thisRotor.SetValue<float>("Velocity", 3f);
            }
            else
            {
                thisRotor.SetValue<float>("LowerLimit", angle);
                thisRotor.SetValue<float>("UpperLimit", 361f);
                thisRotor.SetValue<float>("Velocity", -3f);
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


        void unmergeBlocks(IMyShipMergeBlock mbMovable, IMyShipMergeBlock mbFrame, IMyPistonBase pi)
        {

            // enable needed blocks (override any state)
            mbMovable.Enabled = false;
            mbFrame.Enabled = false;
            pi.Enabled = true;

            // set piston velocity, configure other properties in game
            pi.Velocity = -0.1f;

        }


        void mergeBlocks(IMyShipMergeBlock mbMovable, IMyShipMergeBlock mbFrame)
        {

            // enable needed blocks (override any state)
            mbMovable.Enabled = true;
            mbFrame.Enabled = true;

        }


        void mergeBlocks(IMyShipMergeBlock mbMovable, IMyShipMergeBlock mbFrame, IMyPistonBase pi)
        {

            // enable needed blocks (override any state)
            mbMovable.Enabled = true;
            mbFrame.Enabled = true;
            pi.Enabled = true;

            // set piston velocity, configure other properties in game
            pi.Velocity = 0.1f;



        }

        void unmergeBlocks(IMyShipMergeBlock mbMovable, IMyShipMergeBlock mbFrame)
        {

            // enable needed blocks (override any state)
            mbMovable.Enabled = false;
            mbFrame.Enabled = false;

        }


        void unlock()
        {




            if (IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame))
            {
                // piston unit already locked
                unmergeBlocks(mbDrillBase, mbDrillBaseFrame, piDrillBase);

            }
            else if (!(IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame)))
            {

                // piston unit is unlocked


            }
            else
            {

                // only one merge block is merged --- This is unexpected behaviour

            }


            if (IsMerged(mbDrillHead) && IsMerged(mbDrillHeadFrame))
            {
                // piston unit already locked
                unmergeBlocks(mbDrillHead, mbDrillHeadFrame);

            }
            else if (!(IsMerged(mbDrillHead) && IsMerged(mbDrillHeadFrame)))
            {

                // piston unit is unlocked


            }
            else
            {

                // only one merge block is merged --- This is unexpected behaviour

            }

        }


        void mergelock()
        {

            if (IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame))
            {
                // piston unit already locked


            }
            else if (!(IsMerged(mbDrillBase) && IsMerged(mbDrillBaseFrame)))
            {

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
                mergeBlocks(mbDrillHead, mbDrillHeadFrame);

            }
            else
            {

                // only one merge block is merged --- This is unexpected behaviour

            }

        }

    }
}