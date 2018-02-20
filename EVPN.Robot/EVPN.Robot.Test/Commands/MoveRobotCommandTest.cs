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
    public class MoveRobotCommandTest
    {
        private IRobot _robot;
        private ICommand _moveRobotCommand;
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _moveRobotCommand = new MoveRobotCommand(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _moveRobotCommandAsListPositiveTestCases =
        {
            new object[] {new List<string> {"M","10.9"}, 10.9},
            new object[] {new List<string> {"m", "100.11"}, 100.11}
        };

        [Test, TestCaseSource(nameof(_moveRobotCommandAsListPositiveTestCases))]
        public void MoveCommandRunTest(List<string> command, double distance)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            _moveRobotCommand.Execute(robotCommandEventArgs);
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(distance))));
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
        }

        private static object[] _moveRobotCommandAsListNegativeTestCases =
        {
            new object[] {new List<string> {"M"}},
            new object[] {new List<string> {"M","G"}},
            new object[] {new List<string> {"B","10"}},
            new object[] {new List<string> {"M","10","10"}},
            new object[] {new List<string> {" M ","10"}}
        };

        [Test, TestCaseSource(nameof(_moveRobotCommandAsListNegativeTestCases))]
        public void MoveCommandRunNegativeCommandTest(List<string> command)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            var ex = Assert.Throws(typeof(ArgumentException), () => _moveRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, String.Format("Move command syntax not matched.{0}Syntax: M <Distance in Decimal number>{0}Eg: M 10.3", Environment.NewLine));
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunMoveCommandWithNullRobotTest()
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(null, new List<string>{"M","10"});
            var ex = Assert.Throws(typeof(ArgumentException), () => _moveRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, "Robot has not been initialized. Cannot send command to it.");
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunMoveCommandWithWrongObjectTest()
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _moveRobotCommand.Execute("Object"));
            Assert.AreEqual(ex.Message, "Parameter passed is not of type RobotCommandEventArgs");
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }
    }
}
