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
        public List<List<Prevoznik>> ElevatorQueues = new List<List<Prevoznik>>();
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
            
            for (int i = 0; i < maxFloor + 1; i++)
            {
                List<Prevoznik> queue = new();
                ElevatorQueues.Add(queue);
            }
            Number = number;
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
            Console.WriteLine($"Jedu dolu a jsem v {CurrentFloor}");
            //Console.WriteLine($"Mam v sobe {StudentsIn.Count}");
        }
        public void MoveUp()
        {
            CurrentState = State.Up;
            CurrentFloor++;
            Console.WriteLine($"Jedu nahoru a jsem v {CurrentFloor}");
            //Console.WriteLine($"Mam v sobe {StudentsIn.Count}");
        }
        public void Stop()
        {
            CurrentState = State.Stop;
        }
        public void GetOnElevator(int time, Dormitory dorm)
        {
            Prevoznik stud = ElevatorQueues[CurrentFloor][0];
            ElevatorQueues[CurrentFloor].RemoveAt(0);
            if (StudentsIn.Count == Capacity)
            {
                Student student = stud.ReturnStudent();
                Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {student.Number} can't enter elevator {Number}, because it is full");
                stud.Patience++;
                ElevatorQueues[CurrentFloor].Add(stud);
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
            student.CurrentFloor = CurrentFloor;
            //stud.Prevazejici.NextPlace();
            Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {stud.ReturnStudent().Number} is getting off elevator {Number} at floor {CurrentFloor}");
            if (CurrentFloor == student.HomeFloor)
            {
                student.CurrentPlace = Student.Place.InRoom;
                if (student.pozadavek != Student.WhatHeWants.Nothing)
                {
                    student.pozadavek = student.RandomRequest();
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom, student, student.Number));
                }
                else
                {
                    student.pozadavek = student.RandomRequest();
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom * 2, student, student.Number));
                }
            }
            if (CurrentFloor == 0)
            {
                dorm.ArrivingToFirstFloor(student, time);
            }
            if (student.pozadavek == Student.WhatHeWants.ReturningWashingMachineKeys && CurrentFloor == dorm.WashingMachinesRoomFloor)
            {
                dorm.ScheduleEvent(new AddToElevatorQueue(student, 0, dorm.WashingMachinesRoomFloor, this, time + student.TimeInWashingMachinesRoom));
            }
        }
        public void WhoGetsOff(Dormitory dorm, int time)
        {
            if (DoesSomeoneGetOff())
            {
                int howMany = StudentsIn.Count;
                bool[] which = new bool[howMany];
                for (int i = StudentsIn.Count - 1; i>=0; i--)
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
            //Console.WriteLine(ElevatorQueues[CurrentFloor].Count);
            if (ElevatorQueues[CurrentFloor].Count > 0)
            {
                for (int i = 0; i < ElevatorQueues[CurrentFloor].Count; i++)
                {
                    if (StudentsIn.Count == Capacity)
                    {
                        break;
                    }
                    GetOnElevator(time, dorm);
                }
            }
            if (ElevatorQueues[CurrentFloor].Count == 0)
            {
                FloorsToStop.Remove(CurrentFloor);
            }
        }
    }
}