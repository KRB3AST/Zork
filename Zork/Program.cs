using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace Zork
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void IsTrue(bool expression, string message = null)
        {
            if (expression == false)
            {
                throw new Exception(message);
            }
        }
    }
    internal class Program
    {

        private static Room CurrentRoom
        {
            get
            {
                return Rooms[Location.Row, Location.Column];
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            const string roomDescriptionsFilename = "Rooms.txt";
            InitializeRoomDescriptions(roomDescriptionsFilename);


            Room previousRoom = null;
            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT)
            {
                Console.WriteLine(CurrentRoom.Name);
                            
                if (previousRoom != CurrentRoom)
                {
                    Console.WriteLine(CurrentRoom.Description);
                    previousRoom = CurrentRoom;
                }

                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());
                switch (command)
                {
                    case Commands.QUIT:
                        Console.WriteLine("Thank you for playing!");
                        break;

                    case Commands.LOOK:
                        Console.WriteLine(CurrentRoom.Description);
                        break;

                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:             
                    case Commands.WEST:
                        if (Move(command) == false)
                        {
                            Console.WriteLine("The way is shut!");
                        }
                        break;
                       
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
        }

        private static bool Move(Commands command)
        {
            Assert.IsTrue(IsDirection(command), "Invalid direction.");

            bool isValidMove = true;
            switch (command)
            {
                case Commands.NORTH when Location.Row < Rooms.GetLength(0) - 1:
                    Location.Row++;
                    break;

                case Commands.SOUTH when Location.Row > 0:
                    Location.Row--;
                    break;

                case Commands.EAST when Location.Column < Rooms.GetLength(1) - 1:              
                        Location.Column++;
                        break;

                case Commands.WEST when Location.Column > 0:                   
                    Location.Column--;
                    break;

                default:
                    isValidMove = false;
                    break;
            }

            return isValidMove;
        }

        

        private static Commands ToCommand(string commandString) =>
             Enum.TryParse(commandString, true, out Commands result) ?result : Commands.UNKNOWN;
           
        private static bool IsDirection(Commands command) => Directions.Contains(command);

        private static void InitializeRoomDescriptions(string roomsDescriptionsFilename)
        {
            var roomMap = new Dictionary<string, Room>();
            foreach (Room room in Rooms)
            {
                roomMap.Add(room.Name, room);
                roomMap[room.Name] = room;
            }

            string[] lines = File.ReadAllLines(roomsDescriptionsFilename);
            foreach (string line in lines)
            {
                const string delimiter = "##";
                const int expectedFieldCount = 2;

               string[] fields = line.Split(delimiter);
               Assert.IsTrue(fields.Length == expectedFieldCount, "Invalid record.");

                (string name, string description) = (fields[(int)Fields.Name], fields[(int)Fields.Description]);
                roomMap[name].Description = description;
            }                                       
        }         

        private static readonly Room[,] Rooms =
        {
            { new Room("Rocky Trail"), new Room("South of House"), new Room("Canyon View") },
            { new Room("Forest"),      new Room("West of House"),   new Room("Behind House") },
            { new Room("Dense Woods"), new Room("North of House"), new Room("Clearing") }

        };

        private static readonly List<Commands> Directions = new List<Commands>
        { Commands.NORTH,
          Commands.SOUTH,
          Commands.EAST,
          Commands.WEST
        };

        private enum Fields
        {
            Name = 0,
            Description = 1
        }

        private static (int Row, int Column) Location = (1, 1);
    }
}
