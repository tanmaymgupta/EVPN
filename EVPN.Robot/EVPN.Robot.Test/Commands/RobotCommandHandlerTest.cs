using System;
using System.Collections.Generic;
using EVPN.Robot.Commands;
using EVPN.Robot.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace EVPN.Robot.Test.Commands
{
    [TestFixture]
    public class RobotCommandHandlerTest
    {
        private IRobot _robot;
        private RobotCommandHandler _robotCommandHandler;
        private ICommandRecorder _commandRecorder;
        [SetUp]
        public void Init()
        {
            _commandRecorder = new CommandRecorder();
            _robotCommandHandler = new RobotCommandHandler(_commandRecorder);
            _robot = MockRepository.GenerateStub<IRobot>();
        }

        private static object[] _getCommandAsListPositiveTestCases =
        {
            new object[] {"T 10.2", new List<string> {"T", "10.2"}},
            new object[] {"  T1 T2  10.2  ", new List<string> {"T1", "T2", "10.2"}},
            new object[] {"  T1 T2  10.2  11 ", new List<string> {"T1", "T2", "10.2", "11"}}
        };

        [Test, TestCaseSource(nameof(_getCommandAsListPositiveTestCases))]
        public void GetCommandAsListPositiveTest(string str, List<string> result)
        {
            Assert.AreEqual(RobotCommandHandler.GetCommandAsList(str), result);
        }

        private static object[] _getCommandAsListNegativeTestCases =
        {
            new object[] {""},
            new object[] {"    "},
            new object[] {null}
        };

        [Test, TestCaseSource(nameof(_getCommandAsListNegativeTestCases))]
        public void GetCommandAsListNegativeTest(string str)
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => RobotCommandHandler.GetCommandAsList(str));
            Assert.That(ex.Message, Is.EqualTo("Invalid command. Command cannot be blank."));
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void NullorEmptyCommandRunTest(string str)
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _robotCommandHandler.Run(_robot, str));
            Assert.AreEqual(ex.Message, "Invalid command. Command cannot be blank.");
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Beep());
        }

        [TestCase("10 10", "10")]
        [TestCase("test 1", "test")]
        [TestCase("$$", "$$")]
        public void FirstWordIsNotACharacterCommandRunTest(string str, string result)
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _robotCommandHandler.Run(_robot, str));
            Assert.AreEqual(ex.Message, $"Cannot Identify the command '{result}'");
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Beep());
        }

        [TestCase("D 10", 'D')]
        [TestCase("G", 'G')]
        [TestCase("$", '$')]
        [TestCase("5", '5')]
        public void FirstcharacterIsNotAnIdentifiableCommandRunTest(string str, char result)
        {
            var ex = Assert.Throws(typeof(ArgumentException), () => _robotCommandHandler.Run(_robot, str));
            Assert.AreEqual(ex.Message, $"Cannot Identify the command '{result}'");
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Beep());
        }
        
        [TestCase("M 10.0", 10)]
        [TestCase("m 10.1", 10.1)]
        [TestCase("M 10001.1", 10001.1)]
        public void MoveCommandRunTest(string command, double distance)
        {
            var robotCommandType = _robotCommandHandler.Run(_robot, command);
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(distance))));
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
            Assert.AreEqual(robotCommandType, RobotCommandType.Move);
        }

        [TestCase("T 10.0", 10)]
        [TestCase("t 10.1", 10.1)]
        [TestCase("T 10001.1", 10001.1)]
        public void TurnCommandRunTest(string command, double angle)
        {
            var robotCommandType = _robotCommandHandler.Run(_robot, command);
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(angle))));
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
            Assert.AreEqual(robotCommandType, RobotCommandType.Turn);
        }

        [TestCase("b")]
        [TestCase("B")]
        public void BeepCommandRunTest(string command)
        {
            var robotCommandType = _robotCommandHandler.Run(_robot, command);
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 1);
            Assert.AreEqual(robotCommandType, RobotCommandType.Beep);
        }

        [TestCase("r")]
        [TestCase("R")]
        public void ReplayCommandRunTest(string command)
        {
            _robotCommandHandler.Run(_robot, "m 10");
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _robotCommandHandler.Run(_robot, "t 100");
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _robotCommandHandler.Run(_robot, "b");
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandType = _robotCommandHandler.Run(robotB, command);
            robotB.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            robotB.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            robotB.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);
            Assert.AreEqual(robotCommandType, RobotCommandType.Replay);
        }

        [TestCase("s")]
        [TestCase("S")]
        public void ResetCommandRunTest(string command)
        {
            _robotCommandHandler.Run(_robot, "m 10");
            _robot.AssertWasCalled(x => x.Move(Arg<double>.Matches(dist => dist.Equals(10))));
            _robotCommandHandler.Run(_robot, "t 100");
            _robot.AssertWasCalled(x => x.Turn(Arg<double>.Matches(ang => ang.Equals(100))));
            _robotCommandHandler.Run(_robot, "b");
            _robot.AssertWasCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 3);

            var robotB = MockRepository.GenerateStub<IRobot>();
            var robotCommandType = _robotCommandHandler.Run(robotB, command);
            robotB.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            robotB.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
            Assert.AreEqual(robotCommandType, RobotCommandType.Reset);
        }

        [TestCase("q")]
        [TestCase("Q")]
        public void QuitCommandRunTest(string command)
        {
            var robotCommandType = _robotCommandHandler.Run(_robot, command);
            _robot.AssertWasNotCalled(x => x.Move(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Turn(Arg<double>.Is.Anything));
            _robot.AssertWasNotCalled(x => x.Beep());
            Assert.AreEqual(_commandRecorder.Replay().Count, 0);
            Assert.AreEqual(robotCommandType, RobotCommandType.Quit);
        }
    }
}
