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
    public class ResetRobotCommandTest
    {
        private IRobot _robot;
        private ICommand _resetRobotCommand;
        private ICommand _moveRobotCommand;
        private ICommand _beepRobotCommand;
        private ICommand _turnRobotCommand;
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _resetRobotCommand = new ResetRecordingCommand(_commandRecorder);
            _beepRobotCommand = new BeepRobotCommand(_commandRecorder);
            _moveRobotCommand = new MoveRobotCommand(_commandRecorder);
            _turnRobotCommand = new TurnRobotCommand(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _ResetRobotCommandAsListPositiveTestCases =
        {
            new object[] {new List<string> {"S"}},
            new object[] {new List<string> {"s"}}
        };

        [Test, TestCaseSource(nameof(_ResetRobotCommandAsListPositiveTestCases))]
        public void ResetCommandRunTest(List<string> command)
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
            _resetRobotCommand.Execute(robotCommandEventArgs);
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        private static object[] _ResetRobotCommandAsListNegativeTestCases =
        {
            new object[] {new List<string> {"S "}},
            new object[] {new List<string> {"R"}},
            new object[] {new List<string> {"S","10"}},
            new object[] {new List<string> {"s","10","10"}}
        };

        [Test, TestCaseSource(nameof(_ResetRobotCommandAsListNegativeTestCases))]
        public void ResetCommandRunNegativeCommandTest(List<string> command)
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
            var ex = Assert.Throws(typeof(ArgumentException), () => _resetRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, String.Format("Reset command syntax not matched.{0}Syntax: S{0}Eg: S", Environment.NewLine));
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
        }

        [Test]
        public void RunResetCommandWithNullRobotTest()
        {
            _moveRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "m", "10" }));
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _turnRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "t", "100" }));
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _beepRobotCommand.Execute(new RobotCommandEventArgs(_robot, new List<string> { "b" }));
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandEventArgs = new RobotCommandEventArgs(null, new List<string>{"S"});
            Assert.DoesNotThrow(() => _resetRobotCommand.Execute(robotCommandEventArgs));
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunResetCommandWithWrongObjectTest()
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
