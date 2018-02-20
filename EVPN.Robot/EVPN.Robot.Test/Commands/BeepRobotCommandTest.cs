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
    public class BeepRobotCommandTest
    {
        private IRobot _robot;
        private ICommand _beepRobotCommand;
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _beepRobotCommand = new BeepRobotCommand(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _beepRobotCommandAsListPositiveTestCases =
        {
            new object[] {new List<string> {"B"}},
            new object[] {new List<string> {"b"}}
        };

        [Test, TestCaseSource(nameof(_beepRobotCommandAsListPositiveTestCases))]
        public void BeepCommandRunTest(List<string> command)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            _beepRobotCommand.Execute(robotCommandEventArgs);
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
        }

        private static object[] _beepRobotCommandAsListNegativeTestCases =
        {
            new object[] {new List<string> {"S"}},
            new object[] {new List<string> {"B","10"}},
            new object[] {new List<string> {"B","10","10"}}
        };

        [Test, TestCaseSource(nameof(_beepRobotCommandAsListNegativeTestCases))]
        public void BeepCommandRunNegativeCommandTest(List<string> command)
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(_robot, command);
            var ex = Assert.Throws(typeof(ArgumentException), () => _beepRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, String.Format("Beep command syntax not matched.{0}Syntax: B{0}Eg: B", Environment.NewLine));
            _robot.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunBeepCommandWithNullRobotTest()
        {
            var robotCommandEventArgs = new RobotCommandEventArgs(null, new List<string>{"B"});
            var ex = Assert.Throws(typeof(ArgumentException), () => _beepRobotCommand.Execute(robotCommandEventArgs));
            Assert.AreEqual(ex.Message, "Robot has not been initialized. Cannot send command to it.");
            _robot.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }

        [Test]
        public void RunBeepCommandWithWrongObjectTest()
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _beepRobotCommand.Execute("Object"));
            Assert.AreEqual(ex.Message, "Parameter passed is not of type RobotCommandEventArgs");
            _robot.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
        }
    }
}
