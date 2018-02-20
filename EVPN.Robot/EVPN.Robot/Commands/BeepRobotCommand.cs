using System;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public class BeepRobotCommand : AbstractRobotCommand
    {
        public BeepRobotCommand(ICommandRecorder commandRecorder) : base(commandRecorder)
        {
        }

        protected override void InnerExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            robotCommandEventArgs.Robot.Beep();
        }

        public override bool CanExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            if (robotCommandEventArgs.Robot == null)
                throw new ArgumentException("Robot has not been initialized. Cannot send command to it.");

            if (robotCommandEventArgs.RobotCommandList.Count != 1 || robotCommandEventArgs.RobotCommandList[0].ToUpper() != "B")
                throw new ArgumentException(String.Format("Beep command syntax not matched.{0}Syntax: B{0}Eg: B", Environment.NewLine));

            return true;
        }
    }
}
