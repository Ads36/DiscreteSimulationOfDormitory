using System;
using System.Collections.Generic;
using CodEx;
namespace DiscreteSimulationOfDormitory
{

    public class Dormitory
    {
        public Random random = new Random();
        public List<Elevator> Elevators = new();

        public Queue<Student> PorterQueue = new();
        public SortedSet<Event> calendar = new();
        public List<Student> StudentsInGym = new();
        public List<Student> StudentsInStudyRoom = new();
        public List<Student> StudentsInMusicRoom = new();
        public List<Student> StudentsInWashingMachinesRoom = new();
        public List<Student> Students = new();
        public List<string> PorterNames = new();
        public int CurrentPorterIndex;
        public int NumberOfStudents;
        public int MaxTime;
        public int PorterServingTime = 30;
        public int NumberOfFloors { get;}
        public int NumberOfElevators;
        public int NumberOfPorters;
        public int ElevatorCapacity;
        public TimeSpan TimeConverter;
        public string ConvertToTime(int time)
        {
            TimeSpan TimeConverter = TimeSpan.FromSeconds(time);
            return TimeConverter.ToString(@"hh\:mm\:ss\:fff");
        }
        public void ScheduleEvent(Event e)
        {
            calendar.Add(e);
        }
        public void Run()
        {
            InitializeElevators();
            while(calendar.Count > 0)
            {
                Event currentEvent = calendar.Min;
                calendar.Remove(currentEvent);
                currentEvent.Invoke(this);
                if (currentEvent.Time > MaxTime)
                {
                    Console.WriteLine("Time exceeded, end of simulation");
                    break;
                }
            }
        }
        public Dormitory(int numberOfFloors, int numberOfPorters, int numberOfStudents, int numberOfElevators, int maxTime, int elevatorCapacity)
        {
            NumberOfFloors = 20;
            NumberOfPorters = 5;
            NumberOfStudents = 1000;
            NumberOfElevators = 4;
            MaxTime = 3 * 24 * 60;
            ElevatorCapacity = 4;
            for (int i = 0; i < numberOfPorters; i++)
            {
                PorterNames.Add($"{(char)(i+65)}");
            }
        }
        public void InitializeElevators()
        {
            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elevator = new Elevator(ElevatorCapacity, 0, NumberOfFloors, this);
                elevator.Number = i;
                Elevators.Add(elevator);
            }
        }
        public void InitializeStudents()
        {
            for (int i = 0; i < NumberOfStudents; i++)
            {
                Student stud = new Student();
                Students.Add(stud);
            }
        }
        public void AddToElevatorQueue(Student stud, int destination, int currentFloor, int whichElev, int time) 
        {
            Prevoznik prevoz = new Prevoznik(stud, destination);
            Elevators[whichElev].ElevatorQueues[currentFloor].Enqueue(prevoz);
            Elevators[whichElev].FloorsToStop.Add(destination);
            if (Elevators[whichElev].ElevatorQueues[currentFloor].Count == 1)
            {
                ScheduleEvent(new PressingButtonOfElevator(time, Elevators[whichElev]));
            }
            
        }
        public void GetOffElevator(Prevoznik stud)
        {

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
                PorterQueue.Enqueue(stud);
            }
            else
            {
                //do nothing, student is doing something and can't do 2 things at the same time
            }
        }
        public void LeavingDormitory(Student stud, int time)
        {
            Console.WriteLine($"<{ConvertToTime(time)}> Student {stud.Number} is leaving dormitory");
            ScheduleEvent(new ComingInDormitory(time + stud.TimeOut, stud));
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
            ScheduleEvent(new AddToQueue(time + who.TimeInGym, who));
            who.pozadavek = Student.WhatHeWants.ReturningGymKeys;
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} borrows keys from the gym");
        }
        public void AddToPorterQueue(int time, Student who)
        {
            PorterQueue.Enqueue(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who.Number} enters the queue");
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
                ScheduleEvent(new NextWaiterInQueue(time + PorterServingTime));
            }
        }
        
    }
    class Program
    {

        static void Main(string[] args)
        {
            Dormitory dorm = new Dormitory(1,5,1,1,1,1);
            
            Console.WriteLine("Hello World!");
            Student Adam = new Student();
            Adam.CurrentPlace = Student.Place.InGym;
            Console.WriteLine(Adam.GetCurrentPlace());
            dorm.Run();
            /*for (int i = 0; i < 20; i++)
            {
                Student asas = new Student();
            }*/
        }
    }
}
