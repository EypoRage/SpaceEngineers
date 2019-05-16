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
        IMyTextPanel statusLCD;

        IMyBlockGroup groupLandingGears;
        List<IMyLandingGear> landingGears;

        IMyBlockGroup groupPistonDrill;
        List<IMyPistonBase> pistons;

        IMyShipMergeBlock mbDrillBase;
        IMyShipMergeBlock mbDrillHead;

        IMyBlockGroup groupDrills;
        List<IMyShipDrill> drills;

        IMyMotorAdvancedStator rotor;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            statusLCD = GridTerminalSystem.GetBlockWithName("Bohrinsel Bohrer Status LCD") as IMyTextPanel;

            groupLandingGears = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Anker") as IMyBlockGroup;
            landingGears = new List<IMyLandingGear>();
            groupLandingGears.GetBlocksOfType(landingGears);

            groupPistonDrill = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer Kolben") as IMyBlockGroup;
            pistons = new List<IMyPistonBase>();
            groupPistonDrill.GetBlocksOfType(pistons);

            mbDrillBase = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Basis 1") as IMyShipMergeBlock;
            mbDrillHead = GridTerminalSystem.GetBlockWithName("Bohrinsel Sperre Verbinder Kopf 1") as IMyShipMergeBlock;

            groupDrills = GridTerminalSystem.GetBlockGroupWithName("Bohrinsel Bohrer") as IMyBlockGroup;
            drills = new List<IMyShipDrill>();
            groupDrills.GetBlocksOfType(drills);

            rotor = GridTerminalSystem.GetBlockWithName("Bohrinsel Advanced Rotor") as IMyMotorAdvancedStator;

        }

        public void Save()
        {
           
        }

        public void Main(string argument, UpdateType updateSource)
        {

            statusLCD.WriteText("BOHRER STATUS:      \n");

            statusLCD.WriteText("Rig verankert:      " + isAnchoredToGround() + "\n");

            statusLCD.WriteText("Bohrer gesperrt:    " + isLocked() + "\n");

            statusLCD.WriteText("Bohrer Parkpos.:    " + isParking() + "\n");

            statusLCD.WriteText("Bohrer Startpos.:   " + isStartPos() + "\n");

            statusLCD.WriteText("Bohrer aktiv:       " + isDrilling() + "\n");

            statusLCD.WriteText("Kolbenposition:     " + pistonDistance() + "\n");

            statusLCD.WriteText("Rotor°:             " + rotorRAD() + "\n");

            statusLCD.WriteText("Rotor Beschl.:      " + rotorTorque() + "\n");

            statusLCD.WriteText("Rotor Bremskraft:   " + rotorBrakingTorque() + "\n");

            statusLCD.WriteText("Rotor RPM:          " + rotorRPM() + "\n");


        }


        public string isAnchoredToGround()
        {
            string result = "NEIN";


            foreach (IMyLandingGear landingGear in landingGears)
            {

                if (landingGear.IsLocked)
                {
                    result = "JA";
                }
                else {
                    result = "NEIN";
                }

            }

            return result;

        }


        public string pistonDistance()
        {
            double pistonDistance = 0;



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

            return pistonDistance.ToString("n2");

        }



        public string isParking()
        {
            bool isParking = true;



            foreach (IMyPistonBase piston in pistons)
            {
                var pistonInf = piston.DetailedInfo;


                //splits the string into an array by separating by the ':' character
                string[] pistInfArr = (pistonInf.Split(':'));

                // splits the resulting 0.0m into an array with single index of "0.0" by again splitting by character "m"
                string[] pistonDist = (pistInfArr[1].Split('m'));

                //uses double.Parse method to parse the "0.0" into a usable double of value 0.0
                double pistonDistD = double.Parse(pistonDist[0]);

                if (pistonDistD > 0.5f)
                {

                    isParking = false;
                }


            }

            if (isParking)
            {
                return "JA";
            }
            else {
                return "NEIN";
            }

        }


        public string isStartPos()
        {
            bool isStartPos = false;


            foreach (IMyPistonBase piston in pistons)
            {
                var pistonInf = piston.DetailedInfo;


                //splits the string into an array by separating by the ':' character
                string[] pistInfArr = (pistonInf.Split(':'));

                // splits the resulting 0.0m into an array with single index of "0.0" by again splitting by character "m"
                string[] pistonDist = (pistInfArr[1].Split('m'));

                //uses double.Parse method to parse the "0.0" into a usable double of value 0.0
                double pistonDistD = double.Parse(pistonDist[0]);

                if (pistonDistD > 2.5f && pistonDistD < 5f)
                {

                    isStartPos = true;
                }


            }

            if (isStartPos)
            {
                return "JA";
            }
            else
            {
                return "NEIN";
            }

        }



        public string isLocked()
        {
            if (IsMerged(mbDrillBase) && IsMerged(mbDrillHead))
            {

                return "JA";

            }
            else
            {
                return "NEIN";
            }


        }


        public string isDrilling()
        {

            bool isDrilling = false;

            foreach (IMyShipDrill drill in drills)
            {
                if (drill.Enabled)
                {
                    isDrilling = true;
                }
            }

            if (isDrilling) {
                return "JA";
            } else {
                return "NEIN";
            }
        }



        string rotorRAD() {

            return rotor.Angle.ToString("n2");

        }

        string rotorTorque()
        {

            return rotor.Torque.ToString("n2");

        }

        string rotorBrakingTorque()
        {

            return rotor.BrakingTorque.ToString("n2");

        }

        string rotorRPM()
        {
            return rotor.TargetVelocityRPM.ToString("n2");

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

    }
}