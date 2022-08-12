using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
    //this class is used as class that holds student and his destination while in the elevator
    public class Transfer
    {
        public int DestinationFloor { get; set; }
        public Student TransferredStudent;
        public int Patience { get; set; }
        public int MaxPatience { get; private set; } = 3;
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
        private List<List<Transfer>> ElevatorQueues = new();
        private static int currentNumber = 1;
        public int CurrentFloor { get; private set; }
        private int Capacity;
        private int MaxFloor;
        public int SpeedBetweenFloors { get; private set; }
        public int Number { get; private set; }
        public SortedSet<int> FloorsToStop = new();
        public List<Transfer> StudentsIn = new();

        public Elevator(Dormitory dorm, Elevators elev)
        {
            Capacity = elev.Capacity;
            CurrentFloor = 0;
            MaxFloor = dorm.NumberOfFloors;
            SpeedBetweenFloors = elev.SpeedBetweenFloors;
            //make instances of queues
            for (int i = 0; i < MaxFloor + 1; i++)
            {
                List<Transfer> queue = new();
                ElevatorQueues.Add(queue);
            }
            CurrentState = State.Stop;
            Number = currentNumber++;
        }
        public enum State
        {
            Up,
            Down,
            Stop
        }
        public State CurrentState { get; set; }
        public void EnqueueOnFloor(int floor, Transfer tran)
        {
            ElevatorQueues[floor].Add(tran);
        }
        public void MoveDown()
        {
            CurrentState = State.Down;
            CurrentFloor--;
            if (CurrentFloor < 0)
            {
                CurrentFloor = 0;
            }
            /*this commentary is only for debugging purposes
            Console.WriteLine($"Jedu dolu a jsem v {CurrentFloor}");
            Console.WriteLine($"jeste tolik zastavek {FloorsToStop.Count}");
            Console.WriteLine($"Mam v sobe {StudentsIn.Count}");*/
        }
        public void MoveUp()
        {
            CurrentState = State.Up;
            CurrentFloor++;
            if (CurrentFloor>MaxFloor)
            {
                CurrentFloor = MaxFloor;
            }
            /*this commentary is only for debugging purposes
            Console.WriteLine($"Jedu nahoru a jsem v {CurrentFloor}");
            Console.WriteLine($"jeste tolik zastavek {FloorsToStop.Count}");
            Console.WriteLine($"Mam v sobe {StudentsIn.Count}");*/
        }
        public void Stop()
        {
            CurrentState = State.Stop;
        }
        private void GetOnElevator(int time, Dormitory dorm)
        {
            Transfer stud = ElevatorQueues[CurrentFloor][0];
            //can't enter elevator, because it is full
            if (StudentsIn.Count == Capacity)
            {
                Student student = stud.ReturnStudent();
                Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {student.Number} can't enter elevator {Number}, because it is full");
                stud.Patience++;
                //check if student lost his patience or not
                if (stud.Patience >= stud.MaxPatience)
                {
                    stud.Patience = 0;
                    ElevatorQueues[CurrentFloor].Remove(stud);
                    Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {student.Number} is losing hope and using stairs instead of elevators");
                    dorm.ScheduleEvent(new ArrivingToFirstFloorByFoot(time + student.CurrentFloor * 20, student, student.Number));
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
        //called when student is leaving elevator
        private void GetOffElevator(Transfer stud, Dormitory dorm, int time)
        {
            StudentsIn.Remove(stud);
            Student student = stud.ReturnStudent();
            student.CurrentFloor = CurrentFloor;
            Console.WriteLine($"<{dorm.ConvertToTime(time)}> Student {stud.ReturnStudent().Number} is getting off elevator {Number} at floor {CurrentFloor}");
            //if he arrived at his floor
            if (CurrentFloor == student.HomeFloor)
            {
                student.CurrentPlace = Student.Place.InRoom;
                if (student.Request != Student.WhatHeWants.Nothing)
                {
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom, student, student.Number));
                }
                else
                {
                    //new request, to make simulation neverending
                    student.Request = student.RandomRequest();
                    dorm.ScheduleEvent(new StudentWantsSomething(time + student.TimeInRoom * 2, student, student.Number));
                }
            }
            if (CurrentFloor == 0)
            {
                dorm.ArrivingToFirstFloor(student, time);
            }
            //check if he wants to wash his clothes and than return
            if (student.Request == Student.WhatHeWants.ReturningWashingMachineKeys && CurrentFloor == dorm.WashingMachinesRoomFloor)
            {
                dorm.ScheduleEvent(new AddToElevatorQueue(student, 0, dorm.WashingMachinesRoomFloor, this, time + student.TimeInWashingMachinesRoom));
            }
        }
        public void GettingOffElevator(Dormitory dorm, int time)
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
        //get new tranferers
        public void GetNewPassanger(Dormitory dorm, int time)
        {
            if (ElevatorQueues[CurrentFloor].Count > 0)
            {
                //only students in queue can get on elevator
                for (int i = 0; i < ElevatorQueues[CurrentFloor].Count; i++)
                {
                    if (StudentsIn.Count == Capacity)
                    {
                        break;
                    }
                    GetOnElevator(time, dorm);
                }
            }
            //no one else is waiting in queue, so this floor could be removed from stops
            if (ElevatorQueues[CurrentFloor].Count == 0)
            {
                FloorsToStop.Remove(CurrentFloor);
            }
        }
    }
}