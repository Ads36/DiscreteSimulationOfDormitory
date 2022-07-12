using System;
using System.Collections.Generic;
using CodEx;
namespace DiscreteSimulationOfDormitory
{
    class Dormitory
    {
        public int MaxTime;
        public int NumberOfFloors;
        public int NumberOfElevators;
        public int NumberOfPorters;
        public List<Elevator> Elevators = new();
        public List<Student> PorterQueue = new();
        public SortedSet<Event> calendar = new();
        public int NumberOfStudents;
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
        }
        public void Initialize()
        {

            for (int i = 0; i < NumberOfElevators; i++)
            {
                Elevator elevator = new Elevator();
                Elevators.Add(elevator);
            }
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
        }
    }
}
