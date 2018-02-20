using System;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public class TurnRobotCommand : AbstractRobotCommand
    {
        public TurnRobotCommand(ICommandRecorder commandRecorder) : base(commandRecorder)
        {
        }

        protected override void InnerExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            var angle = double.Parse(robotCommandEventArgs.RobotCommandList[1]);
            robotCommandEventArgs.Robot.Turn(angle);
        }

        public override bool CanExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            if(robotCommandEventArgs.Robot == null)
                throw new ArgumentException("Robot has not been initialized. Cannot send command to it.");

            if (robotCommandEventArgs.RobotCommandList.Count != 2 || robotCommandEventArgs.RobotCommandList[0].ToUpper() != "T" || !double.TryParse(robotCommandEventArgs.RobotCommandList[1], out var angle))
                throw new ArgumentException(String.Format("Turn command syntax not matched.{0}Syntax: T <Angle in Decimal number>{0}Eg: T 45.8", Environment.NewLine));

            return true;
        }
    }
}
