using System.Collections.Generic;
using EVPN.Robot.Util;
using NUnit.Framework;

namespace EVPN.Robot.Test.Util
{
    [TestFixture]
    public class CommandRecorderTest
    {
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
        }

        [Test]
        public void ResetTest()
        {
            _commandRecorder.Record(new List<string> { "Test1" });
            _commandRecorder.Record(new List<string> { "Test2" });
            _commandRecorder.Record(new List<string> { "Test3" });
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
            _commandRecorder.Reset();
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void ResetSetsEnableRecordToTrueTest()
        {
            _commandRecorder.EnableRecording = false;
            Assert.IsFalse(_commandRecorder.EnableRecording);
            _commandRecorder.Reset();
            Assert.IsTrue(_commandRecorder.EnableRecording);
        }

        [Test]
        public void RecordTest()
        {
            var replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 0);
            _commandRecorder.Record(new List<string> { "Test1" });
            _commandRecorder.Record(new List<string> { "Test2" });
            _commandRecorder.Record(new List<string> { "Test3" });
            replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 3);
            Assert.AreEqual(replay[0][0], "Test1");
            Assert.AreEqual(replay[1][0], "Test2");
            Assert.AreEqual(replay[2][0], "Test3");
        }

        [Test]
        public void RecordTestWithRecordingDisabled()
        {
            var replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 0);
            _commandRecorder.EnableRecording = false;
            _commandRecorder.Record(new List<string> { "Test1" });
            _commandRecorder.Record(new List<string> { "Test2" });
            _commandRecorder.Record(new List<string> { "Test3" });
            replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 0);
        }

        [Test]
        public void ReplayTest()
        {
            var replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 0);
            _commandRecorder.Record(new List<string> { "Test1" });
            _commandRecorder.Record(new List<string> { "Test2" });
            _commandRecorder.Record(new List<string> { "Test3" });
            replay = _commandRecorder.Replay();
            Assert.AreEqual(replay.Count, 3);
            Assert.AreEqual(replay[0][0], "Test1");
            Assert.AreEqual(replay[1][0], "Test2");
            Assert.AreEqual(replay[2][0], "Test3");
        }
    }
}
