using System;

namespace EVPN.Robot.Commands
{
    public class ReplayRobotCommand : AbstractRobotCommand
    {
        private RobotCommandHandler _robotCommandHandler;
        
        public ReplayRobotCommand(RobotCommandHandler robotCommandHandler) : base(robotCommandHandler.CommandRecorder)
        {
            _robotCommandHandler = robotCommandHandler;
        }

        public override void Execute(object parameter)
        {
            var isRecordingEnabled = CommandRecorder.EnableRecording;
            CommandRecorder.EnableRecording = false;
            base.Execute(parameter);
            CommandRecorder.EnableRecording = isRecordingEnabled;
        }

        protected override void InnerExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            

            var replayCommands = CommandRecorder.Replay();
            foreach (var command in replayCommands)
            {
                try
                {
                    _robotCommandHandler.Run(robotCommandEventArgs.Robot, command);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override bool CanExecute(RobotCommandEventArgs robotCommandEventArgs)
        {
            if (robotCommandEventArgs.Robot == null)
                throw new ArgumentException("Robot has not been initialized. Cannot send command to it.");

            if (robotCommandEventArgs.RobotCommandList.Count != 1 || robotCommandEventArgs.RobotCommandList[0].ToUpper() != "R")
                throw new ArgumentException(String.Format("Replay command syntax not matched.{0}Syntax: R{0}Eg: R", Environment.NewLine));

            return true;
        }
    }
}
