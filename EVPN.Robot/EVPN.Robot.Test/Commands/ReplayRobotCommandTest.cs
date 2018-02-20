using System;
using System.Collections.Generic;
using System.Windows.Input;
using EVPN.Robot.Commands;
using EVPN.Robot.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace EVPN.Robot.Test.Commands
{
    [TestFixture]
    public class ReplayRobotCommandTest
    {
        private IRobot _robot;
        private ICommand _replayRobotCommand;
        private ICommand _moveRobotCommand;
        private ICommand _beepRobotCommand;
        private ICommand _turnRobotCommand;
        private ICommandRecorder _commandRecorder;
        private RobotCommandHandler _robotCommandHandler;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _robotCommandHandler = new RobotCommandHandler(_commandRecorder);
            _replayRobotCommand = new ReplayRobotCommand(_robotCommandHandler);
            _beepRobotCommand = new BeepRobotCommand(_commandRecorder);
            _moveRobotCommand = new MoveRobotCommand(_commandRecorder);
            _turnRobotCommand = new TurnRobotCommand(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _replayRobotCommandAsListPositiveTestCases =
        {
            new object[] {new List<string> {"R"}},
            new object[] {new List<string> {"r"}}
        };

        [Test, TestCaseSource(nameof(_replayRobotCommandAsListPositiveTestCases))]
        public void ReplayCommandRunTest(List<string> command)
        {
            _moveRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> {"m", "10"}));
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _turnRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> {"t", "100"}));
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _beepRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> {"b"}));
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandEventArgs = new RobotCommandEventArgs(robotB, command);
            _replayRobotCommand.Execute(robotCommandEventArgs);
            robotB.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            robotB.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            robotB.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
        }

        private static object[] _replayRobotCommandAsListNegativeTestCases =
        {
            new object[] {new List<string> {"S"}},
            new object[] {new List<string> {"r","10"}},
            new object[] {new List<string> {"R","10","10"}}
        };

        [Test, TestCaseSource(nameof(_replayRobotCommandAsListNegativeTestCases))]
        public void ReplayCommandRunNegativeCommandTest(List<string> command)
        {
            _moveRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "m", "10" }));
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _turnRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "t", "100" }));
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _beepRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "b" }));
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandEventArgs = new RobotCommandEventArgs(robotB, command);
            var ex = Assert.Throws(typeof(ArgumentException), () => _replayRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, String.Format("Replay command syntax not matched.{0}Syntax: R{0}Eg: R", Environment.NewLine));
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
        }

        [Test]
        public void RunReplayCommandWithNullRobotTest()
        {
            _moveRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "m", "10" }));
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _turnRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "t", "100" }));
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _beepRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "b" }));
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandEventArgs = new RobotCommandEventArgs(null, new List<string>{"R"});
            var ex = Assert.Throws(typeof(ArgumentException), () => _replayRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, "Robot has not been initialized. Cannot send command to it.");
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
        }

        [Test]
        public void RunReplayCommandWithWrongObjectTest()
        {
            _moveRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "m", "10" }));
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _turnRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "t", "100" }));
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _beepRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "b" }));
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
            var ex = Assert.Throws(typeof(ArgumentException), () => _moveRobotCommand.Execute("Object"));
            Assert.AreEqual(ex.Message, "Parameter passed is not of type RobotCommandEventArgs");
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
        }
    }
}
