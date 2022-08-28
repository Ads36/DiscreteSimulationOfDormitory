//Discrete Simulation of Dormitory
//Adam Červenka, 1. ročník, Informatika, MFF UK
//letní semestr 2022
//Programování 2 NPRG031

using System;
using System.Collections.Generic;
using System.IO;
namespace DiscreteSimulationOfDormitory
{
    public class Calendar
    {
        private Dormitory dormitory;
        public Calendar(Dormitory dorm)
        {
            dormitory = dorm;
        }
        public SortedSet<Event> calendar = new SortedSet<Event>();
        public void ScheduleEvent(Event e)
        {
            calendar.Add(e);
        }
        public void Run()
        {
            while (calendar.Count > 0)
            {
                Event currentEvent = calendar.Min;
                calendar.Remove(currentEvent);
                if (currentEvent.Time > dormitory.MaxTime)
                {
                    Console.WriteLine("Time exceeded, end of simulation");
                    break;
                }
                currentEvent.Invoke(dormitory);
            }
        }
    }
    public class Dormitory
    {
        private Random random = new Random();
        private List<Elevator> Elevators = new();
        private Queue<Student> GatekeeperQueue = new();
        public Calendar calendar;
        private List<Student> StudentsInGym = new();
        private List<Student> StudentsInStudyRoom = new();
        private List<Student> StudentsInMusicRoom = new();
        private List<Student> StudentsInWashingMachinesRoom = new();
        private List<Student> Students = new();
        private List<string> GatekeeperNames = new();
        private int CurrentGatekeeperIndex;
        private int NumberOfStudents;
        public int MaxTime { get; private set; }
        private int GatekeeperServingTime;
        private int GatekeeperWorkTime;
        public int WashingMachinesRoomFloor { get; private set; }
        public int NumberOfFloors { get;}
        private int NumberOfElevators;
        private int NumberOfGatekeepers;
        private Students student;
        private Elevators elevator;
        public string ConvertToTime(int time)
        {
            TimeSpan TimeConverter = TimeSpan.FromSeconds(time);
            return TimeConverter.ToString(@"dd\:hh\:mm\:ss");
        }
        public Dormitory(Dorms dorms, Elevators elev, Students stud)
        {
            calendar = new(this);
            NumberOfFloors = (int)dorms.NumberOfFloors;
            NumberOfGatekeepers = (int)dorms.NumberOfGatekeepers;
            NumberOfStudents = (int)dorms.NumberOfStudents;
            NumberOfElevators = (int)dorms.NumberOfElevators;
            MaxTime = (int)dorms.MaxTime;
            GatekeeperServingTime = (int)dorms.GatekeeperServingTime;
            GatekeeperWorkTime = (int)dorms.GatekeeperWorkDuration;
            WashingMachinesRoomFloor = (int)dorms.WashingMachinesRoomFloor;
            elevator = elev;
            student = stud;
            Inititalize();
        }
        private void Inititalize()
        {
            for (int i = 0; i < NumberOfGatekeepers; i++)
            {
                GatekeeperNames.Add($"{(char)(i + 65)}");
            }
            InitializeElevators();
            InitializeStudents();
            calendar.ScheduleEvent(new OpeningDorms(0));
            for (int i = 1; i < MaxTime/GatekeeperWorkTime; i++)
            {
                ScheduleEvent(new ChangingGatekeepers(i * GatekeeperWorkTime));
            }
        }
        public void ScheduleEvent(Event even) {
            calendar.ScheduleEvent(even);
        }
        private void InitializeElevators()
        {
            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elev = new Elevator(this, elevator);
                Elevators.Add(elev);
            }
        }
        private void InitializeStudents()
        {
            for (int i = 0; i < NumberOfStudents; i++)
            {
                Student stud = new Student(this, student);
                Students.Add(stud);
                if (stud.CurrentPlace == Student.Place.InRoom)
                {
                    ScheduleEvent(new StudentWantsSomething(i * 10, stud, stud.Number));
                }
                else
                {
                    bool wantsSomething = (stud.Request != Student.WhatHeWants.Nothing);
                    ScheduleEvent(new EnteringDormitory(i * 100, stud, stud.Number, wantsSomething));
                }
            }
        }
        public Elevator RandomElevator()
        {
            int whichElevator = random.Next(0, NumberOfElevators);
            return Elevators[whichElevator];
        }
        public void AddToElevatorQueue(Student stud, int destination, int currentFloor, Elevator elevator, int time) 
        {
            stud.CurrentPlace = Student.Place.Waiting;
            Elevator el = elevator;
            Transfer tranferer = new Transfer(stud, destination);
            elevator.EnqueueOnFloor(currentFloor, tranferer);
            elevator.FloorsToStop.Add(destination);
            elevator.FloorsToStop.Add(currentFloor);
            //could be uncommented, it had debugging purposes but it also brings some information value about what is happening
            //Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is pressing elevator {elevator.Number} button at floor {currentFloor} heading to floor {destination}");
            if ((el.CurrentFloor < el.FloorsToStop.Max) && (el.CurrentState == Elevator.State.Stop))
            {
                ScheduleEvent(new ElevatorMovingUp(time + 1, el, el.Number));
            }
            else if ((el.CurrentFloor > el.FloorsToStop.Min) && (el.CurrentState == Elevator.State.Stop))
            {
                ScheduleEvent(new ElevatorMovingDown(time + 1, el, el.Number));
            }
        }
        public void StudentWantsSomthingFromGatekeeper(Student stud, int time)
        {
            if (stud.CurrentPlace == Student.Place.InRoom)
            {
                ScheduleEvent(new AddToElevatorQueue(stud, 0, stud.HomeFloor, RandomElevator(), time));
            }
            else if (stud.CurrentPlace == Student.Place.Outside)
            {
                ScheduleEvent(new EnteringDormitory(time, stud, stud.Number, true));
            }
            else
            {
                ScheduleEvent(new StudentWantsSomething(time + stud.TimeBetweenEvents, stud, stud.Number)); 
            }
        }
        public void ArrivingToFirstFloor(Student stud, int time)
        {
            
            if (stud.Request == Student.WhatHeWants.Nothing)
            {
                ScheduleEvent(new LeavingDormitory(time, stud, stud.Number));
            }
            else
            {
                ScheduleEvent(new AddToQueue(time, stud, stud.Number));
            }
        }
        public void EnteringDormitory(Student stud, int time, bool wantsSomething)
        {
                Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is entering dormitory");
                //if he comes at the start, there isn't any other possibility to trigger this if statement
                if (wantsSomething)
                {
                    ScheduleEvent(new AddToQueue(time, stud, stud.Number));
                }
                else
                {
                    stud.Request = stud.RandomRequest();
                    int randomness = random.Next(0, 10);
                    if (randomness < 7)
                    {
                        ScheduleEvent(new AddToElevatorQueue(stud, stud.HomeFloor, 0, RandomElevator(), time));
                    }
                    else
                    {
                        ScheduleEvent(new AddToQueue(time, stud, stud.Number));
                    }
            }
        }
        public void LeavingDormitory(Student stud, int time)
        {
            Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is leaving dormitory");
            stud.CurrentPlace = Student.Place.Outside;
            ScheduleEvent(new EnteringDormitory(time + stud.TimeOut, stud, stud.Number, false));
        }
        public void Open(int time)
        {
            Console.WriteLine($"<{ConvertToTime(time)}> Opening dormitory");
        }
        public void ChangeGatekeepers(int time)
        {
            int oldPorterIndex = CurrentGatekeeperIndex;
            CurrentGatekeeperIndex = (CurrentGatekeeperIndex + 1) % GatekeeperNames.Count;
            Console.WriteLine($"<{ConvertToTime(time)}> Changing porters: {GatekeeperNames[oldPorterIndex]} for {GetCurrentGatekeeperName()}");
        }
        private string GetCurrentGatekeeperName()
        {
            return GatekeeperNames[CurrentGatekeeperIndex];
        }
        public void BorrowingGymKeys(int time, Student who)
        {
            StudentsInGym.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInGym, who, who.Number));
            who.Request = Student.WhatHeWants.ReturningGymKeys;
            who.CurrentPlace = Student.Place.InGym;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the gym");
        }
        public void ReturningGymKeys(int time, Student who)
        {
            StudentsInGym.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the gym");
            //decide if he wants to go home or outside - goes home
            who.Request = Student.WhatHeWants.Nothing;
            ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
        }
        public void BorrowingMusicRoomKeys(int time, Student who)
        {
            StudentsInMusicRoom.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInMusicRoom, who, who.Number));
            who.Request = Student.WhatHeWants.ReturningMusicRoomKeys;
            who.CurrentPlace = Student.Place.InMusicRoom;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the music room");
        }
        public void ReturningMusicRoomKeys(int time, Student who)
        {
            StudentsInMusicRoom.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the music room");
            //decide if he wants to go home or outside
            int randomness = random.Next(0, 10);
            if (randomness < 4)
            {
                ScheduleEvent(new LeavingDormitory(time, who, who.Number));
            }
            else
            {
                who.Request = Student.WhatHeWants.Nothing;
                ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
            }
        }
        public void BorrowingWashingMachineRoomKeys(int time, Student who)
        {
            StudentsInWashingMachinesRoom.Add(who);
            ScheduleEvent(new AddToElevatorQueue(who, WashingMachinesRoomFloor, who.CurrentFloor, RandomElevator(), time+1));
            who.Request = Student.WhatHeWants.ReturningWashingMachineKeys;
            who.CurrentPlace = Student.Place.InWashingMachineRoom;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the washing machine room");
        }
        public void ReturningWashingMachineRoomKeys(int time, Student who)
        {
            StudentsInWashingMachinesRoom.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the washing machine room");
            //decide if he wants to go home or outside - goes home
            who.Request = Student.WhatHeWants.Nothing;
            ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
        }
        public void BorrowingStudyRoomKeys(int time, Student who)
        {
            StudentsInStudyRoom.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInStudyRoom, who, who.Number));
            who.Request = Student.WhatHeWants.ReturningStudyRoomKeys;
            who.CurrentPlace = Student.Place.InStudyRoom;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the study room");
        }
        public void ReturningStudyRoomKeys(int time, Student who)
        {
            StudentsInStudyRoom.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the study room");
            //decide if he wants to go home or outside
            int randomness = random.Next(0, 10);
            if (randomness < 2)
            {
                ScheduleEvent(new LeavingDormitory(time, who, who.Number));
            }
            else
            {
                who.Request = Student.WhatHeWants.Nothing;
                ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
            }
        }
        public void AddToGatekeeperQueue(int time, Student who)
        {
            GatekeeperQueue.Enqueue(who);
            who.CurrentPlace = Student.Place.Waiting;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} enters the queue of gatekeeper {GetCurrentGatekeeperName()}");
            if (GatekeeperQueue.Count == 1)
            {
                ScheduleEvent(new NextWaiterInQueue(time + GatekeeperServingTime));
            }
        }
        public void RemoveFromQueue(int time)
        {
            Student stud = GatekeeperQueue.Dequeue();
            switch (stud.Request)
            {
                case Student.WhatHeWants.GymKeys:
                    ScheduleEvent(new BorrowingGymKeys(time, stud));
                    break;
                case Student.WhatHeWants.ReturningGymKeys:
                    ScheduleEvent(new ReturningGymKeys(time, stud));
                    break;
                case Student.WhatHeWants.WashingMachineKeys:
                    ScheduleEvent(new BorrowingWashingMachineRoomKeys(time, stud));
                    break;
                case Student.WhatHeWants.ReturningWashingMachineKeys:
                    ScheduleEvent(new ReturningWashingMachineRoomKeys(time, stud));
                    break;
                case Student.WhatHeWants.MusicRoomKeys:
                    ScheduleEvent(new BorrowingMusicRoomKeys(time, stud));
                    break;
                case Student.WhatHeWants.ReturningMusicRoomKeys:
                    ScheduleEvent(new ReturningMusicRoomKeys(time, stud));
                    break;
                case Student.WhatHeWants.StudyRoomKeys:
                    ScheduleEvent(new BorrowingStudyRoomKeys(time, stud));
                    break;
                case Student.WhatHeWants.ReturningStudyRoomKeys:
                    ScheduleEvent(new ReturningStudyRoomKeys(time, stud));
                    break;
                default:
                    break;
            }
            if (GatekeeperQueue.Count > 0)
            {
                ScheduleEvent(new NextWaiterInQueue(time + GatekeeperServingTime));
            }
        }
    }
    //struct for more convenient loading of files
    public struct Dorms
    {
        public uint NumberOfFloors;
        public uint NumberOfGatekeepers;
        public uint NumberOfStudents;
        public uint NumberOfElevators;
        public uint MaxTime;
        public uint GatekeeperServingTime;
        public uint GatekeeperWorkDuration;
        public uint WashingMachinesRoomFloor;
    }
    public struct Elevators
    {
        public uint Capacity;
        public uint SpeedBetweenFloors;
    }
    public struct Students
    {
        public uint TimeInGym;
        public uint TimeInMusicRoom;
        public uint TimeInStudyRoom;
        public uint TimeInWashingMachinesRoom;
        public uint TimeOut;
        public uint TimeInRoom;
        public uint TimeBetweenEvents;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" Welcome to Discrete simulation of Dormitory.");
            Console.WriteLine(" Be sure that you have file named \"input.txt\" in the directory where you run this program.");
            Console.WriteLine(" Correct format is found in documentation, this file must have 17 lines of integers.");
            Console.WriteLine(" Press enter to start simulation.");
            Console.ReadLine();
            Dorms dormitory = new();
            Students student = new();
            Elevators elevator = new();
            try
            {
                //reading from file input.txt
                using (var input = new StreamReader("input.txt"))
                {
                    dormitory.NumberOfFloors = uint.Parse(input.ReadLine());
                    dormitory.NumberOfGatekeepers = uint.Parse(input.ReadLine());
                    dormitory.NumberOfStudents = uint.Parse(input.ReadLine());
                    dormitory.NumberOfElevators = uint.Parse(input.ReadLine());
                    dormitory.MaxTime = uint.Parse(input.ReadLine());
                    dormitory.GatekeeperServingTime = uint.Parse(input.ReadLine());
                    dormitory.GatekeeperWorkDuration = uint.Parse(input.ReadLine());
                    dormitory.WashingMachinesRoomFloor = uint.Parse(input.ReadLine());
                    elevator.Capacity = uint.Parse(input.ReadLine());
                    elevator.SpeedBetweenFloors = uint.Parse(input.ReadLine());
                    student.TimeInGym = uint.Parse(input.ReadLine());
                    student.TimeInMusicRoom = uint.Parse(input.ReadLine());
                    student.TimeInStudyRoom = uint.Parse(input.ReadLine());
                    student.TimeInWashingMachinesRoom = uint.Parse(input.ReadLine());
                    student.TimeOut = uint.Parse(input.ReadLine());
                    student.TimeInRoom = uint.Parse(input.ReadLine());
                    student.TimeBetweenEvents = uint.Parse(input.ReadLine());
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Can't find input file. Have you named it correctly? (input.txt)");
                Console.WriteLine("Press enter to exit application");
                Console.ReadLine();
                Environment.Exit(0);
            }
            catch (OverflowException)
            {
                Console.WriteLine("A number in input file is either smaller than 0 or very big. Check input file data values.");
                Console.WriteLine("Press enter to exit application");
                Console.ReadLine();
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                Console.WriteLine("There is a typo in input file. Are all input lines integers?");
                Console.WriteLine("Press enter to exit application");
                Console.ReadLine();
                Environment.Exit(0);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("There aren't enough (17 is needed) integers in input file.");
                Console.WriteLine("Press enter to exit application");
                Console.ReadLine();
                Environment.Exit(0);
            }
            Dormitory dorm = new Dormitory(dormitory, elevator, student);
            dorm.calendar.Run();
        }
    }
}
