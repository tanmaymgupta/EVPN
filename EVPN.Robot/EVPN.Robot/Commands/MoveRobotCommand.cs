using System;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public class MoveRobotCommand : AbstractRobotCommand
    {
        public MoveRobotCommand(ICommandRecorder commandRecorder) : base(commandRecorder)
        {
        }

        protected override void InnerExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            var distance = double.Parse(robotCommandEventArgs.RobotCommandList[1]);
            robotCommandEventArgs.Robot.Move(distance);
        }

        public override bool CanExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            if (robotCommandEventArgs.Robot == null)
                throw new ArgumentException("Robot has not been initialized. Cannot send command to it.");

            if (robotCommandEventArgs.RobotCommandList.Count != 2 || robotCommandEventArgs.RobotCommandList[0].ToUpper() != "M" || !double.TryParse(robotCommandEventArgs.RobotCommandList[1], out var distance))
                throw new ArgumentException(String.Format("Move command syntax not matched.{0}Syntax: M <Distance in Decimal number>{0}Eg: M 10.3", Environment.NewLine));

            return true;
        }
    }
}
