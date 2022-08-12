using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
    public class Transfer
    {
        public int DestinationFloor { get; set; }
        public Student TransferredStudent;
        public int Patience { get; set; }
        public int MaxPatience { get; } = 3;
        public Transfer(Student stud, int floor)
        {
            DestinationFloor = floor;
            TransferredStudent = stud;
            Patience = 0;
        }
        public Student ReturnStudent()
        {
            return TransferredStudent;
        }
    }
    public class Elevator
    {
        public List<List<Transfer>> ElevatorQueues = new List<List<Transfer>>();
        public int CurrentFloor;
        public int Capacity;
        public int MaxFloor;
        public int SpeedBetweenFloors = 5;
        public int Number { get; private set; }
        public SortedSet<int> FloorsToStop = new();
        public List<Transfer> StudentsIn = new();

        public Elevator(int capacity, int floor, int maxFloor, int number)
        {
            Capacity = capacity;
            CurrentFloor = floor;
            MaxFloor = maxFloor;
            for (int i = 0; i < maxFloor + 1; i++)
            {
                List<Transfer> queue = new();
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
            if (CurrentFloor < 0)
            {
                CurrentFloor = 0;
            }
            //Console.WriteLine($"Jedu dolu a jsem v {CurrentFloor}");
            //Console.WriteLine($"jeste tolik zastavek {FloorsToStop.Count}");
            //Console.WriteLine($"Mam v sobe {StudentsIn.Count}");
        }
        public void MoveUp()
        {
            CurrentState = State.Up;
            CurrentFloor++;
            if (CurrentFloor>MaxFloor)
            {
                CurrentFloor = MaxFloor;
            }
            //Console.WriteLine($"Jedu nahoru a jsem v {CurrentFloor}");
            //Console.WriteLine($"jeste tolik zastavek {FloorsToStop.Count}");
            //Console.WriteLine($"Mam v sobe {StudentsIn.Count}");
        }
        public void Stop()
        {
            CurrentState = State.Stop;
        }
        public void GetOnElevator(int time, Dormitory dorm)
        {
            Transfer stud = ElevatorQueues[CurrentFloor][0];
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
                ElevatorQueues[CurrentFloor].RemoveAt(0);
                Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {stud.ReturnStudent().Number} is entering elevator {Number} at floor {CurrentFloor} and heading to floor {stud.DestinationFloor}");
                StudentsIn.Add(stud);
                stud.TransferredStudent.CurrentPlace = Student.Place.InElevator;
                FloorsToStop.Add(stud.DestinationFloor);
            }
        }
        public void GetOffElevator(Transfer stud, Dormitory dorm, int time)
        {
            StudentsIn.Remove(stud);
            Student student = stud.ReturnStudent();
            student.CurrentFloor = CurrentFloor;
            Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {stud.ReturnStudent().Number} is getting off elevator {Number} at floor {CurrentFloor}");
            if (CurrentFloor == student.HomeFloor)
            {
                student.CurrentPlace = Student.Place.InRoom;
                if (student.Request != Student.WhatHeWants.Nothing)
                {
                    student.Request = student.RandomRequest();
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom, student, student.Number));
                }
                else
                {
                    student.Request = student.RandomRequest();
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom * 2, student, student.Number));
                }
            }
            if (CurrentFloor == 0)
            {
                dorm.ArrivingToFirstFloor(student, time);
            }
            if (student.Request == Student.WhatHeWants.ReturningWashingMachineKeys && CurrentFloor == dorm.WashingMachinesRoomFloor)
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