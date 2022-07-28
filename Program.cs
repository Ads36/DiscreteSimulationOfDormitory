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
        public List<string> PorterNames = new();
        public int CurrentPorterIndex;
        public int NumberOfStudents;
        public int MaxTime;
        public int NumberOfFloors { get;}
        public int NumberOfElevators;
        public int NumberOfPorters;
        public int ElevatorCapacity;

        public void Run()
        {
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
        public Dormitory()
        {
            NumberOfFloors = 20;
            NumberOfPorters = 5;
            NumberOfStudents = 1000;
            NumberOfElevators = 4;
            MaxTime = 3 * 24 * 60;
            ElevatorCapacity = 4;
        }
        public void Initialize()
        {
            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elevator = new Elevator(ElevatorCapacity, 0, NumberOfFloors);
                Elevators.Add(elevator);
            }

        }
        public void AddToElevatorQueue(Student stud, int destination, int floor) 
        {
            Prevoznik prevoz = new Prevoznik(stud, destination);
            Elevator.ElevatorQueues[floor].Enqueue(prevoz);
        }
        public void Open(int time)
        {
            Console.WriteLine($"<{time}> Opening dormitory");
        }
        public void ChangePorters(int time)
        {
            int oldPorterIndex = CurrentPorterIndex;
            CurrentPorterIndex = (CurrentPorterIndex + 1) % PorterNames.Count;
            Console.WriteLine($"<{time}> Changing porters: {PorterNames[oldPorterIndex]} for {GetCurrentPorterName()}");
        }
        public string GetCurrentPorterName()
        {
            return PorterNames[CurrentPorterIndex];
        }
        
    }
    class Program
    {
        static void Main(string[] args)
        {
            Dormitory dorm = new Dormitory();
            dorm.Initialize();
            Console.WriteLine("Hello World!");
            Student Adam = new Student();
            Adam.CurrentPlace = Student.Place.InGym;
            Console.WriteLine(Adam.GetCurrentPlace());
            /*for (int i = 0; i < 20; i++)
            {
                Student asas = new Student();
            }*/
        }
    }
}
