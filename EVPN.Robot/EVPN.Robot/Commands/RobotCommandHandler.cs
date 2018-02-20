using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public class RobotCommandHandler
    {
        private const string CannotIdentityCommandError = "Cannot Identify the command '{0}'";
        private const string BlankCommandError = "Invalid command. Command cannot be blank.";
        private readonly Dictionary<RobotCommandType, ICommand> _robotCommandDictionary;
        public ICommandRecorder CommandRecorder { get; }
        
        public RobotCommandHandler(ICommandRecorder commandRecorder)
        {
            CommandRecorder = commandRecorder;
            _robotCommandDictionary = new Dictionary<RobotCommandType, ICommand>
            {
                {RobotCommandType.Move, new MoveRobotCommand(CommandRecorder)},
                {RobotCommandType.Turn, new TurnRobotCommand(CommandRecorder)},
                {RobotCommandType.Beep, new BeepRobotCommand(CommandRecorder)},
                {RobotCommandType.Replay, new ReplayRobotCommand(this)},
                {RobotCommandType.Reset, new ResetRecordingCommand(CommandRecorder)},
                {RobotCommandType.Quit, null}
            };
        }

        public RobotCommandType Run(IRobot robot, string command)
        {
            var commandList = GetCommandAsList(command);
            return Run(robot, commandList);
        }

        public RobotCommandType Run(IRobot robot, List<string> commandList)
        {
            if (commandList == null || commandList.Count == 0)
                throw new ArgumentException(BlankCommandError);

            if (!char.TryParse(commandList[0].ToUpper(), out var commandChar))
                throw new ArgumentException(string.Format(CannotIdentityCommandError, commandList[0]));

            if (!_robotCommandDictionary.ContainsKey((RobotCommandType)commandChar))
                throw new ArgumentException(string.Format(CannotIdentityCommandError, commandChar));

            var robotCommandType = (RobotCommandType) commandChar;
            if (robotCommandType == RobotCommandType.Quit) return robotCommandType;

            var robotCommandEventArgs = new RobotCommandEventArgs(robot, commandList);
            _robotCommandDictionary[(RobotCommandType) commandChar].Execute(robotCommandEventArgs);
            return robotCommandType;
        }

        public static List<string> GetCommandAsList(string commandStr)
        {
            if (string.IsNullOrWhiteSpace(commandStr))
                throw new ArgumentException(BlankCommandError);
            commandStr = commandStr.Trim();
            return commandStr.Split(' ').Where(str => str != "").ToList();
        }
    }

    public enum RobotCommandType
    {
        Move = 'M',
        Turn = 'T',
        Beep = 'B',
        Replay = 'R',
        Reset = 'S',
        Quit = 'Q',
        None = 'N'
    }
}
