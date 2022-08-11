using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
    public class Prevoznik
    {
        public int DestinationFloor { get; set; }
        public Student Prevazejici;
        public int Patience { get; set; }
        public int MaxPatience { get; } = 3;
        public Prevoznik(Student stud, int floor)
        {
            DestinationFloor = floor;
            Prevazejici = stud;
            Patience = 0;
        }
        public Student ReturnStudent()
        {
            return Prevazejici;
        }
    }
    public class Elevator
    {
        public List<Queue<Prevoznik>> ElevatorQueues = new();
        public int CurrentFloor;
        public int Capacity;
        public int SpeedBetweenFloors = 5;
        public int Number { get; private set; }
        public SortedSet<int> FloorsToStop = new();
        public List<Prevoznik> StudentsIn = new();

        public Elevator(int capacity, int floor, int maxFloor, int number)
        {
            Capacity = capacity;
            CurrentFloor = floor;
            Queue<Prevoznik> queue = new();
            for (int i = 0; i < maxFloor + 1; i++)
            {
                ElevatorQueues.Add(queue);
            }
            Number = number;
            //CurrentState = State.Stop;
            
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
            //Console.WriteLine($"Jedu dolu a jsem v {CurrentFloor}");
        }
        public void MoveUp()
        {
            CurrentState = State.Up;
            CurrentFloor++;
            //Console.WriteLine($"Jedu nahoru a jsem v {CurrentFloor}");
        }
        public void Stop()
        {
            CurrentState = State.Stop;
        }
        public void GetOnElevator(Prevoznik stud, int time, Dormitory dorm)
        {
            if (StudentsIn.Count == Capacity)
            {
                Student student = stud.ReturnStudent();
                Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {student.Number} can't enter elevator {Number}, because it is full");
                stud.Patience++;
                if (stud.Patience >= stud.MaxPatience)
                {
                    stud.Patience = 0;
                    Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {student.Number} is losing hope and using stairs instead of elevators");
                    dorm.calendar.ScheduleEvent(new ArrivingToFirstFloorByFoot(time + student.CurrentFloor * 20, student, student.Number));
                }
            }
            else
            {
                StudentsIn.Add(stud);
                stud.Prevazejici.CurrentPlace = Student.Place.InElevator;
                FloorsToStop.Add(stud.DestinationFloor);
            }
        }
        public void GetOffElevator(Prevoznik stud, Dormitory dorm, int time)
        {
            StudentsIn.Remove(stud);
            
            Student student = stud.ReturnStudent();
            //stud.Prevazejici.NextPlace();
            if (CurrentFloor == student.HomeFloor)
            {
                student.CurrentPlace = Student.Place.InRoom;
            }
            if (CurrentFloor == 0)
            {
                dorm.ArrivingToFirstFloor(student, time);
            }
            
        }
        public void WhoGetsOff(Dormitory dorm, int time)
        {
            if (DoesSomeoneGetOff())
            {
                for (int i = 0; i <StudentsIn.Count; i++)
                {
                    if (StudentsIn[i].DestinationFloor == CurrentFloor)
                    {
                        GetOffElevator(StudentsIn[i], dorm, time);
                    }
                }
                FloorsToStop.Remove(CurrentFloor);
            }
            //GetNewPassanger(dorm);
            //DecidingDirectionAfterGettingOff(dorm, time);
        }
        public State WhereToMove()
        {
            if (FloorsToStop.Count > 0)
            {
                if (CurrentFloor < FloorsToStop.Max)
                {
                    return State.Up;
                }
                else if (CurrentFloor > FloorsToStop.Min)
                {
                    return State.Down;
                }
                return State.Stop;
            }
            else
            {
                return State.Stop;
            }
        }
        public bool IsFull()
        {
            if (StudentsIn.Count == Capacity)
            {
                return true;
            }
            return false;
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
        public void GetNewPassanger(Dormitory dorm, int time)
        {
            if (ElevatorQueues[CurrentFloor].Count > 0)
            {
                for (int i = 0; i < ElevatorQueues[CurrentFloor].Count; i++)
                {
                    Prevoznik stud = ElevatorQueues[CurrentFloor].Dequeue();
                    GetOnElevator(stud, time, dorm);
                    if (StudentsIn.Count == Capacity)
                    {
                        break;
                    }
                }
            }
            if (ElevatorQueues[CurrentFloor].Count == 0)
            {
                FloorsToStop.Remove(CurrentFloor);
            }
        }

    }
}