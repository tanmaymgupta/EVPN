using System;
using EVPN.Robot.Commands;
using EVPN.Robot.Util;

namespace EVPN.Robot.App
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommandRecorder commandRecorder = new CommandRecorder();
            RobotCommandHandler robotCammandHandler = new RobotCommandHandler(commandRecorder);
            RobotCommandType drawingCommand = RobotCommandType.None;

            Console.WriteLine("Welcome to the Mock Robot Interface:");
            Console.WriteLine("'Move' command will move Robot A. Syntax: M <Distance in Decimal number> Example: M 10.3");
            Console.WriteLine("'Turn' command will Turn Robot A. Syntax: T <Angle in Decimal number> Eg: T 45.8");
            Console.WriteLine("'Beep' command will Beep Robot A. Syntax: B");
            Console.WriteLine("'Replay' command will replay all the past commands sent to Robot A on Robot B. Syntax: R");
            Console.WriteLine("'Quit' command syntax not matched. Syntax: Q");
            do
            {
                IRobot robotA = new MockRobotA();
                IRobot robotB = new MockRobotB();
                IRobot robot;
                try
                {
                    Console.Write("Enter Command: ");
                    var commandStr = Console.ReadLine();
                    if (commandStr != null && commandStr.Trim().ToUpper() == "R")
                        robot = robotB;
                    else
                        robot = robotA;

                    drawingCommand = RobotCommandType.None;
                    try
                    {
                        drawingCommand = robotCammandHandler.Run(robot, commandStr);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (drawingCommand != RobotCommandType.Quit);
            Console.WriteLine("Press a key to Quit");
            Console.ReadKey();
        }

        public class MockRobotA : IRobot
        {
            public void Move(double distance)
            {
                Console.WriteLine("Robot 'A' Moves " + distance);
            }

            public void Turn(double angle)
            {
                Console.WriteLine("Robot 'A' turns " + angle);
            }

            public void Beep()
            {
                Console.WriteLine("Robot 'A' Beeps");
            }
        }

        public class MockRobotB : IRobot
        {
            public void Move(double distance)
            {
                Console.WriteLine("Robot 'B' Moves " + distance);
            }

            public void Turn(double angle)
            {
                Console.WriteLine("Robot 'B' turns " + angle);
            }

            public void Beep()
            {
                Console.WriteLine("Robot 'B' Beeps");
            }
        }
    }
}
