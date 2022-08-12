using System;
using System.Collections.Generic;
using CodEx;
using System.IO;
namespace DiscreteSimulationOfDormitory
{
    public class Calendar
    {
        private Dormitory dormitory;
        public Calendar(Dormitory dorm){
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
        private int ElevatorCapacity;
        private Students student;
        private Elevators elevator;
        public string ConvertToTime(int time)
        {
            TimeSpan TimeConverter = TimeSpan.FromSeconds(time);
            return TimeConverter.ToString(@"dd\:hh\:mm\:ss");
        }
        public Dormitory(Dorms dorms, Elevators elev, Students stud)
        {
            NumberOfFloors = dorms.NumberOfFloors;
            NumberOfGatekeepers = dorms.NumberOfGatekeepers;
            NumberOfStudents = dorms.NumberOfStudents;
            NumberOfElevators = dorms.NumberOfElevators;
            MaxTime = dorms.MaxTime;
            GatekeeperServingTime = dorms.GatekeeperServingTime;
            GatekeeperWorkTime = dorms.GatekeeperWorkDuration;
            ElevatorCapacity = elev.Capacity;
            WashingMachinesRoomFloor = dorms.WashingMachinesRoomFloor;
            elevator = elev;
            student = stud;
            Inititalize();
        }
        public void Inititalize()
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
        public void InitializeElevators()
        {
            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elevator = new Elevator(ElevatorCapacity, 0, NumberOfFloors, i + 1);
                elevator.CurrentState = Elevator.State.Stop;
                Elevators.Add(elevator);
            }
        }
        public void InitializeStudents()
        {
            for (int i = 0; i < NumberOfStudents; i++)
            {
                Student stud = new Student(this);
                Students.Add(stud);
                if (stud.CurrentPlace == Student.Place.InRoom)
                {
                    calendar.ScheduleEvent(new StudentWantsSomething(i * 10, stud, stud.Number));
                }
                else
                {
                    bool wantsSomething = (stud.Request != Student.WhatHeWants.Nothing);
                    calendar.ScheduleEvent(new ComingInDormitory(i * 100, stud, stud.Number, wantsSomething));
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
            Transfer prevoz = new Transfer(stud, destination);
            elevator.ElevatorQueues[currentFloor].Add(prevoz);
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
                ScheduleEvent(new ComingInDormitory(time, stud, stud.Number, true));
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
            ScheduleEvent(new ComingInDormitory(time + stud.TimeOut, stud, stud.Number, false));
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
        public string GetCurrentGatekeeperName()
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
        public void AddToPorterQueue(int time, Student who)
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
            if (GatekeeperQueue.Count>0)
            {
                ScheduleEvent(new NextWaiterInQueue(time + GatekeeperServingTime));
            }
        }
    }
    public struct Dorms
    {
        public int NumberOfFloors;
        public int NumberOfGatekeepers;
        public int NumberOfStudents;
        public int NumberOfElevators;
        public int MaxTime;
        public int GatekeeperServingTime;
        public int GatekeeperWorkDuration;
        public int WashingMachinesRoomFloor;
    }
    public struct Elevators
    {
        public int Capacity;
        public int SpeedBetweenFloors;
    }
    public struct Students
    {
        public int TimeInGym;
        public int TimeInMusicRoom;
        public int TimeInStudyRoom;
        public int TimeInWashingMachinesRoom;
        public int TimeOut;
        public int TimeInRoom;
        public int TimeBetweenEvents;
    }
    class Program
    {

        static void Main(string[] args)
        {
            Dorms dormitory = new();
            Students student = new();
            Elevators elevator = new();
            try
            {
                using (var input = new StreamReader("input.txt"))
                {
                    dormitory.NumberOfFloors = int.Parse(input.ReadLine());
                    dormitory.NumberOfGatekeepers = int.Parse(input.ReadLine());
                    dormitory.NumberOfStudents = int.Parse(input.ReadLine());
                    dormitory.NumberOfElevators = int.Parse(input.ReadLine());
                    dormitory.MaxTime = int.Parse(input.ReadLine());
                    dormitory.GatekeeperServingTime = int.Parse(input.ReadLine());
                    dormitory.GatekeeperWorkDuration = int.Parse(input.ReadLine());
                    dormitory.WashingMachinesRoomFloor = int.Parse(input.ReadLine());
                    elevator.Capacity = int.Parse(input.ReadLine());
                    elevator.SpeedBetweenFloors = int.Parse(input.ReadLine());
                    student.TimeInGym = int.Parse(input.ReadLine());
                    student.TimeInMusicRoom = int.Parse(input.ReadLine());
                    student.TimeInStudyRoom = int.Parse(input.ReadLine());
                    student.TimeInWashingMachinesRoom = int.Parse(input.ReadLine());
                    student.TimeOut = int.Parse(input.ReadLine());
                    student.TimeInRoom = int.Parse(input.ReadLine());
                    student.TimeBetweenEvents = int.Parse(input.ReadLine());
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Can't find input file. Have you named it correctly? (input.txt)");
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
                Console.WriteLine("There aren't enough integers in input file");
                Console.WriteLine("Press enter to exit application");
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            Dormitory dorm = new Dormitory(dormitory, elevator, student);
            dorm.calendar.Run();
        }
    }
}
