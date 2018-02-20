using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using EVPN.Robot.Util;

namespace EVPN.Robot.Commands
{
    public abstract class AbstractRobotCommand : ICommand
    {
        protected ICommandRecorder CommandRecorder;
        
        [ImportingConstructor]
        public AbstractRobotCommand(ICommandRecorder commandRecorder)
        {
            CommandRecorder = commandRecorder;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            if (!(parameter is RobotCommandEventArgs robotCommandEventArgs))
                throw new ArgumentException("Parameter passed is not of type RobotCommandEventArgs");
            if (CanExecute(robotCommandEventArgs))
            {
                CommandRecorder.Record(robotCommandEventArgs.RobotCommandList);
                InnerExecute(robotCommandEventArgs);
            }
        }

        protected abstract void InnerExecute(RobotCommandEventArgs robotCommandEventArgs);

        public abstract bool CanExecute(RobotCommandEventArgs robotCommandEventArgs);

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RobotCommandEventArgs
    {
        public List<string> RobotCommandList { get; }
        public IRobot Robot { get; set; }
        public RobotCommandEventArgs(IRobot robot, List<string> robotCommandList)
        {
            Robot = robot;
            RobotCommandList = robotCommandList;
        }
    }
}
