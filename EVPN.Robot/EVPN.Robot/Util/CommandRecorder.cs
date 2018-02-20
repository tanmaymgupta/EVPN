using System.Collections.Generic;

namespace EVPN.Robot.Util
{
    public interface ICommandRecorder
    {
        void Record(List<string> command);
        List<List<string>> Replay();
        void Reset();
        bool EnableRecording { get; set; }
    }

    public class CommandRecorder : ICommandRecorder
    {
        private List<List<string>> _commands;

        public CommandRecorder()
        {
            Reset();
        }

        public void Record(List<string> command)
        {
            if (EnableRecording)
                _commands.Add(command);
        }

        public List<List<string>> Replay()
        {
            return _commands;
        }

        public void Reset()
        {
            _commands = new List<List<string>>();
            EnableRecording = true;
        }

        public bool EnableRecording { get; set; }
    }
}
