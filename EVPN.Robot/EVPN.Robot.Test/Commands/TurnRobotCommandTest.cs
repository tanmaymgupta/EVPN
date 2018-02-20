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
    public class TurnRobotCommandTest
    {
        private IRobot _robot;
        private ICommand _turnRobotCommand;
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _turnRobotCommand = new TurnRobotCommand(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _turnRobotCommandAsListPositiveTestCases =
        {
            new object[] {new List<string> {"T","10.9"}, 10.9},
            new object[] {new List<string> {"t", "100.11"}, 100.11}
        };

        [Test, TestCaseSource(nameof(_turnRobotCommandAsListPositiveTestCases))]
        public void TurnCommandRunTest(List<string> command, double distance)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            _turnRobotCommand.Execute(robotCommandEventArgs);
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(dist => dist.Equals(distance))));
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
        }

        private static object[] _turnRobotCommandAsListNegativeTestCases =
        {
            new object[] {new List<string> {"T"}},
            new object[] {new List<string> {"T","M"}},
            new object[] {new List<string> {"M","10"}},
            new object[] {new List<string> {"T","10","10"}},
            new object[] {new List<string> {" T ","10"}}
        };

        [Test, TestCaseSource(nameof(_turnRobotCommandAsListNegativeTestCases))]
        public void TurnCommandRunNegativeCommandTest(List<string> command)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            var ex = Assert.Throws(typeof(ArgumentException), () => _turnRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, String.Format("Turn command syntax not matched.{0}Syntax: T <Angle in Decimal number>{0}Eg: T 45.8", Environment.NewLine));
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunTurnCommandWithNullRobotTest()
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(null, new List<string>{"T","10"});
            var ex = Assert.Throws(typeof(ArgumentException), () => _turnRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, "Robot has not been initialized. Cannot send command to it.");
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunTurnCommandWithWrongObjectTest()
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _turnRobotCommand.Execute("Object"));
            Assert.AreEqual(ex.Message, "Parameter passed is not of type RobotCommandEventArgs");
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }
    }
}
