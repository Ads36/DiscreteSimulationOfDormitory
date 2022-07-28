using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
    public class Prevoznik
    {
        public int DestinationFloor;
        public Student Prevazejici;
        public Prevoznik(Student stud, int floor)
        {
            DestinationFloor = floor;
            Prevazejici = stud;
        }
    }
    public class Elevator
    {
        public static List<Queue<Prevoznik>> ElevatorQueues = new();
        public int CurrentFloor;
        public int Capacity;
        public int SpeedBetweenFloors;
        public class Comp : IComparer<int>
        {
            Elevator ele;
            public Comp(Elevator el)
            {
                ele = el;
            }
            public int Compare(int a, int b)
            {
                return 1;
                if (ele.CurrentState==State.Up)
                {

                }
            }
        }

        public SortedSet<int> FloorsToStop;
        public List<Prevoznik> StudentsIn;

        public Elevator(int capacity, int floor, int maxFloor)
        {
            Capacity = capacity;
            CurrentFloor = floor;
            Queue<Prevoznik> queue = new();
            for (int i = 0; i < maxFloor + 1; i++)
            {
                ElevatorQueues.Add(queue);
            }
            //FloorsToStop.Comparer = 
        }
        public enum State
        {
            Up,
            Down,
            Stop
        }
        public State CurrentState { get; set; }
        public void MoveDown()
        {
            CurrentState = State.Down;
            CurrentFloor--;
        }
        public void MoveUp()
        {
            CurrentState = State.Up;
            CurrentFloor++;
        }
        public void Stop()
        {
            CurrentState = State.Stop;
        }
        public void GetOnElevator(Prevoznik stud)
        {
            if (StudentsIn.Count == Capacity)
            {
                Console.WriteLine("sorry, elevator is full");
                //press the button again, wait for next
            }
            else
            {
                StudentsIn.Add(stud);
                stud.Prevazejici.CurrentPlace = Student.Place.InElevator;
                FloorsToStop.Add(stud.DestinationFloor);
            }
        }
        public void GetOffElevator(Prevoznik stud)
        {
            StudentsIn.Remove(stud);
            FloorsToStop.Remove(CurrentFloor);
            stud.Prevazejici.NextPlace();
        }
        public void WhoGetsOff()
        {
            foreach (var stud in StudentsIn)
            {
                if (stud.DestinationFloor == CurrentFloor)
                {
                    GetOffElevator(stud);
                }
            }
            GetNewPassanger();
            DecidingDirectionAfterGettingOff();
        }
        public bool DoesSomeoneGetOff()
        {
            foreach (var stud in StudentsIn)
            {
                if (stud.DestinationFloor == CurrentFloor)
                {
                    return true;
                }
            }
            return false;
        }
        public void DecidingDirectionAfterGettingOff()
        {
            if (StudentsIn.Count == 0)
            {
                if (FloorsToStop.Count > 0)
                {
                    if (FloorsToStop.Max > CurrentFloor)
                    {
                        MoveUp();
                    }
                    else if (FloorsToStop.Min < CurrentFloor)
                    {
                        MoveDown();
                    }
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                if (CurrentState == State.Up && FloorsToStop.Max > CurrentFloor)
                {
                    MoveUp();
                }
                if (CurrentState == State.Down && FloorsToStop.Min < CurrentFloor)
                {
                    MoveDown();
                }
            }
        }
        public void ContinueInDirection()
        {
            if (DoesSomeoneGetOff())
            {
                WhoGetsOff();
            }
            else
            {
                if (CurrentState == State.Down)
                {
                    MoveDown();
                }
                if (CurrentState == State.Up)
                {
                    MoveUp();
                }
            }
        }
        public void GetNewPassanger()
        {
            if (ElevatorQueues[CurrentFloor].Count > 0)
            {
                for (int i = 0; i < ElevatorQueues[CurrentFloor].Count; i++)
                {
                    Prevoznik stud = ElevatorQueues[CurrentFloor].Dequeue();
                    GetOnElevator(stud);
                    if (StudentsIn.Count == Capacity)
                    {
                        break;
                    }
                }
            }
        }

    }
}