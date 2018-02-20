using System;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public class ResetRecordingCommand : AbstractRobotCommand
    {
        public ResetRecordingCommand(ICommandRecorder commandRecorder) : base(commandRecorder)
        {
        }

        public override void Execute(object parameter)
        {
            CommandRecorder.EnableRecording = false;
            base.Execute(parameter);
        }

        protected override void InnerExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            CommandRecorder.Reset();
        }

        public override bool CanExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            if (robotCommandEventArgs.RobotCommandList.Count != 1 || robotCommandEventArgs.RobotCommandList[0].ToUpper() != "S")
                throw new ArgumentException(String.Format("Reset command syntax not matched.{0}Syntax: S{0}Eg: S", Environment.NewLine));

            return true;
        }
    }
}
