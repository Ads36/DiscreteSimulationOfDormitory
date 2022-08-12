using System;
using System.Collections.Generic;
using CodEx;
namespace DiscreteSimulationOfDormitory
{
    public class Calendar
    {
        public Dormitory dormitory;
        public Calendar(Dormitory dorm){
            dormitory = dorm;
        }
        public SortedSet<Event> calendar = new();
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
                currentEvent.Invoke(dormitory);
                if (currentEvent.Time > dormitory.MaxTime)
                {
                    Console.WriteLine("Time exceeded, end of simulation");
                    break;
                }
            }
        }
    }

    public class Dormitory
    {
        public Random random = new Random();
        public List<Elevator> Elevators = new();
        public Queue<Student> PorterQueue = new();
        public Calendar calendar;
        public List<Student> StudentsInGym = new();
        public List<Student> StudentsInStudyRoom = new();
        public List<Student> StudentsInMusicRoom = new();
        public List<Student> StudentsInWashingMachinesRoom = new();
        public List<Student> Students = new();
        public List<string> PorterNames = new();
        public int CurrentPorterIndex;
        public int NumberOfStudents;
        public int MaxTime = 24 * 3600 * 5;
        public int PorterServingTime = 30;
        public int PorterTime = 8 * 3600;
        public int WashingMachinesRoomFloor = 5;
        public int NumberOfFloors { get;}
        public int NumberOfElevators;
        public int NumberOfPorters;
        public int ElevatorCapacity;
        public TimeSpan TimeConverter;
        public string ConvertToTime(int time)
        {
            TimeSpan TimeConverter = TimeSpan.FromSeconds(time);
            return TimeConverter.ToString(@"dd\:hh\:mm\:ss");
        }
        public Dormitory(int numberOfFloors, int numberOfPorters, int numberOfStudents, int numberOfElevators, int maxTime, int elevatorCapacity)
        {
            NumberOfFloors = numberOfFloors;
            NumberOfPorters = numberOfPorters;
            NumberOfStudents = numberOfStudents;
            NumberOfElevators = numberOfElevators;
            calendar = new Calendar(this);
            //MaxTime = 3 * 24 * 60;
            ElevatorCapacity = elevatorCapacity;
            for (int i = 0; i < NumberOfPorters; i++)
            {
                PorterNames.Add($"{(char)(i+65)}");
            }
        }
        public void Inititalize()
        {
            InitializeElevators();
            InitializeStudents();

            calendar.ScheduleEvent(new OpeningDorms(0));
            //ScheduleEvent(new )MaxTime/PorterTime
            for (int i = 1; i < MaxTime/PorterTime; i++)
            {
                ScheduleEvent(new ChangingPorters(i * PorterTime));
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
                calendar.ScheduleEvent(new StudentWantsSomething(i * 10, stud, stud.Number));
            }
        }
        public Elevator RandomElevator()
        {
            int whichElevator = random.Next(0, NumberOfElevators);
            return Elevators[whichElevator];
        }
        public void AddToElevatorQueue(Student stud, int destination, int currentFloor, Elevator elevator, int time) 
        {
            stud.CurrentPlace = Student.Place.WaitingForElevator;
            Prevoznik prevoz = new Prevoznik(stud, destination);
            elevator.ElevatorQueues[currentFloor].Add(prevoz);
            elevator.FloorsToStop.Add(destination);
            elevator.FloorsToStop.Add(currentFloor);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is pressing elevator {elevator.Number} button at floor {currentFloor} heading to floor {destination}");
            ScheduleEvent(new PressingButtonOfElevator(time, elevator, elevator.Number));
        }
        public void StudentWantsSmt(Student stud, int time)
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
            //Console.WriteLine($"Student {stud.Number} arrived to first floor");
            if (stud.pozadavek == Student.WhatHeWants.Nothing)
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
            if (stud.CurrentPlace == Student.Place.Outside)
            {
                Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is entering dormitory");
                if (wantsSomething)
                {
                    AddToPorterQueue(time, stud);
                    //stud.CurrentPlace = Student.Place.WaitingInQueue;
                }
                else
                {
                    stud.pozadavek = stud.RandomRequest();
                    int randomness = random.Next(0, 10);
                    if (randomness < 7)
                    {
                        AddToElevatorQueue(stud, stud.HomeFloor, 0, RandomElevator(), time);
                        //ScheduleEvent(new StudentWantsSomething(time + stud.TimeBetweenEvents, stud, stud.Number));
                    }
                    else
                    {
                        AddToPorterQueue(time, stud);
                    }
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
        public void ChangePorters(int time)
        {
            int oldPorterIndex = CurrentPorterIndex;
            CurrentPorterIndex = (CurrentPorterIndex + 1) % PorterNames.Count;
            Console.WriteLine($"<{ConvertToTime(time)}> Changing porters: {PorterNames[oldPorterIndex]} for {GetCurrentPorterName()}");
        }
        public string GetCurrentPorterName()
        {
            return PorterNames[CurrentPorterIndex];
        }
        public void BorrowingGymKeys(int time, Student who)
        {
            StudentsInGym.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInGym, who, who.Number));
            who.pozadavek = Student.WhatHeWants.ReturningGymKeys;
            who.CurrentPlace = Student.Place.InGym;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the gym");
        }
        public void ReturningGymKeys(int time, Student who)
        {
            StudentsInGym.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the gym");
            //decide if he wants to go home or outside
            who.pozadavek = Student.WhatHeWants.Nothing;
            ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
        }
        public void BorrowingMusicRoomKeys(int time, Student who)
        {
            StudentsInMusicRoom.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInMusicRoom, who, who.Number));
            who.pozadavek = Student.WhatHeWants.ReturningMusicRoomKeys;
            who.CurrentPlace = Student.Place.InMusicRoom;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the music room");
        }
        public void ReturningMusicRoomKeys(int time, Student who)
        {
            StudentsInMusicRoom.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the music room");
            //decide if he wants to go home or outside
            who.pozadavek = Student.WhatHeWants.Nothing;
            ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
        }
        public void BorrowingWashingMachineRoomKeys(int time, Student who)
        {
            StudentsInWashingMachinesRoom.Add(who);
            ScheduleEvent(new AddToElevatorQueue(who, WashingMachinesRoomFloor, who.CurrentFloor, RandomElevator(), time+1));
            //ScheduleEvent(new AddToQueue(time + who.TimeInWashingMachinesRoom, who, who.Number));
            who.pozadavek = Student.WhatHeWants.ReturningWashingMachineKeys;
            who.CurrentPlace = Student.Place.InWashingMachineRoom;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the washing machine room");
        }
        public void ReturningWashingMachineRoomKeys(int time, Student who)
        {
            StudentsInWashingMachinesRoom.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the washing machine room");
            //decide if he wants to go home or outside
            who.pozadavek = Student.WhatHeWants.Nothing;
            ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
        }
        public void BorrowingStudyRoomKeys(int time, Student who)
        {
            StudentsInStudyRoom.Add(who);
            ScheduleEvent(new AddToQueue(time + who.TimeInStudyRoom, who, who.Number));
            who.pozadavek = Student.WhatHeWants.ReturningStudyRoomKeys;
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
                who.pozadavek = Student.WhatHeWants.Nothing;
                ScheduleEvent(new AddToElevatorQueue(who, who.HomeFloor, 0, RandomElevator(), time));
            }
        }
        public void AddToPorterQueue(int time, Student who)
        {
            PorterQueue.Enqueue(who);
            who.CurrentPlace = Student.Place.WaitingInQueue;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} enters the queue of porter {GetCurrentPorterName()}");
            if (PorterQueue.Count == 1)
            {
                ScheduleEvent(new NextWaiterInQueue(time + PorterServingTime));
            }
            //ScheduleEvent(new NextWaiterInQueue(time + (PorterQueue.Count - 1) * PorterServingTime));
        }
        public void RemoveFromQueue(int time)
        {
            Student stud = PorterQueue.Dequeue();
            switch (stud.pozadavek)
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
            if (PorterQueue.Count>0)
            {
                ScheduleEvent(new NextWaiterInQueue(time + PorterServingTime));
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Dormitory dorm = new Dormitory(20, 5, 100, 1, 50, 10);
            dorm.Inititalize();
            dorm.calendar.Run();
        }
    }
}
