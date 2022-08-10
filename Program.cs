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
            //InitializeElevators();
            while (calendar.Count > 0)
            {
                Event currentEvent = calendar.Min;
                calendar.Remove(calendar.Min);
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
        public int MaxTime = 24 * 3600 * 1 / 8;
        public int PorterServingTime = 30;
        public int PorterTime = 8 * 3600;
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
            for (int i = 1; i < 5 - 1; i++)
            {
                calendar.ScheduleEvent(new ChangingPorters(i * PorterTime));
            }
        }
        public void ScheduleEvent(Event even) {
            calendar.ScheduleEvent(even);
        }
        public void InitializeElevators()
        {
            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elevator = new Elevator(ElevatorCapacity, 0, NumberOfFloors, this);
                elevator.Number = i;
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
                calendar.ScheduleEvent(new StudentWantsSomething(i * 10, stud));
            }
        }
        public void AddToElevatorQueue(Student stud, int destination, int currentFloor, int whichElev, int time) 
        {
            Prevoznik prevoz = new Prevoznik(stud, destination);
            Elevators[whichElev].ElevatorQueues[currentFloor].Enqueue(prevoz);
            Elevators[whichElev].FloorsToStop.Add(destination);
            Elevators[whichElev].FloorsToStop.Add(currentFloor);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is pressing elevator {Elevators[whichElev].Number} button at floor {currentFloor} heading to floor {destination}");
            calendar.ScheduleEvent(new PressingButtonOfElevator(time, Elevators[whichElev]));
        }
        public void StudentWantsSmt(Student stud, int time)
        {
            if (stud.CurrentPlace == Student.Place.InRoom)
            {
                int whichElevator = random.Next(0, NumberOfElevators);
                //Prevoznik prevoz = new Prevoznik(stud, 0);
                AddToElevatorQueue(stud, 0, stud.HomeFloor, whichElevator, time);
            }
            else if (stud.CurrentPlace == Student.Place.Outside)
            {
                calendar.ScheduleEvent(new ComingInDormitory(time, stud));
                PorterQueue.Enqueue(stud);
                stud.CurrentPlace = Student.Place.WaitingInQueue;
            }
            else
            {
                //do nothing, student is doing something and can't do 2 things at the same time
            }
        }
        public void ArrivingToFirstFloor(Student stud, int time)
        {
            if (stud.pozadavek == Student.WhatHeWants.Nothing)
            {
                calendar.ScheduleEvent(new LeavingDormitory(time, stud));
            }
            else
            {
                calendar.ScheduleEvent(new AddToQueue(time, stud));
            }
        }
        public void EnteringDormitory(Student stud, int time)
        {
            if (stud.CurrentPlace == Student.Place.Outside)
            {
                Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is entering dormitory");
                stud.CurrentPlace = Student.Place.WaitingForElevator;
            }
        }
        public void LeavingDormitory(Student stud, int time)
        {
            Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is leaving dormitory");
            stud.CurrentPlace = Student.Place.Outside;
            calendar.ScheduleEvent(new ComingInDormitory(time + stud.TimeOut, stud));
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
        public void ReturningGymKeys(int time, Student who)
        {
            StudentsInGym.Remove(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} returns keys from the gym");
        }
        public void BorrowingGymKeys(int time, Student who)
        {
            StudentsInGym.Add(who);
            calendar.ScheduleEvent(new AddToQueue(time + who.TimeInGym, who));
            who.pozadavek = Student.WhatHeWants.ReturningGymKeys;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the gym");
        }
        public void AddToPorterQueue(int time, Student who)
        {
            PorterQueue.Enqueue(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} enters the queue");
            if (PorterQueue.Count == 1)
            {
                calendar.ScheduleEvent(new NextWaiterInQueue(time + PorterServingTime));
            }
            //ScheduleEvent(new NextWaiterInQueue(time + (PorterQueue.Count - 1) * PorterServingTime));
        }
        public void RemoveFromQueue(int time)
        {
            Student stud = PorterQueue.Dequeue();
            switch (stud.pozadavek)
            {
                case Student.WhatHeWants.GymKeys:
                    BorrowingGymKeys(time, stud);
                    break;
                case Student.WhatHeWants.ReturningGymKeys:
                    ReturningGymKeys(time, stud);
                    break;
                case Student.WhatHeWants.WashingMachineKeys:
                    break;
                case Student.WhatHeWants.ReturningWashingMachineKeys:
                    break;
                case Student.WhatHeWants.MusicRoomKeys:
                    break;
                case Student.WhatHeWants.ReturningMusicRoomKeys:
                    break;
                case Student.WhatHeWants.StudyRoomKeys:
                    break;
                case Student.WhatHeWants.ReturningStudyRoomKeys:
                    break;
                default:
                    break;
            }
            AddToElevatorQueue(stud, stud.HomeFloor, 0, random.Next(0, NumberOfElevators), time);
            if (PorterQueue.Count>0)
            {
                calendar.ScheduleEvent(new NextWaiterInQueue(time + PorterServingTime));
            }
        }
        
    }
    class Program
    {

        static void Main(string[] args)
        {
            Dormitory dorm = new Dormitory(20,5,20,1,50,5);
            dorm.Inititalize();
            //dorm.PorterNames.Add("Adam");
            //dorm.PorterNames.Add("Milan");
            Console.WriteLine("Hello World!");
            Student Adam = new Student(dorm);
            Adam.CurrentPlace = Student.Place.InGym;
            Console.WriteLine(Adam.GetCurrentPlace());
            dorm.calendar.Run();
            /*for (int i = 0; i < 20; i++)
            {
                Student asas = new Student();
            }*/
        }
    }
}
