using System;
using System.Collections.Generic;
using CodEx;
namespace DiscreteSimulationOfDormitory
{

    public class Dormitory
    {
        public List<Elevator> Elevators = new();
        public List<Student> PorterQueue = new();
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
                Elevator elevator = new Elevator(ElevatorCapacity, 0, NumberOfFloors);
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
        public void AddToElevatorQueue(Student stud, int destination, int floor) 
        {
            Prevoznik prevoz = new Prevoznik(stud, destination);
            Elevator.ElevatorQueues[floor].Enqueue(prevoz);
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
            
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who} returns keys from the gym");
        }
        public void BorrowingGymKeys(int time, Student who)
        {
            StudentsInGym.Add(who);
            Console.WriteLine($"<{ConvertToTime(time)}> Student {who} borrows keys from the gym");
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
