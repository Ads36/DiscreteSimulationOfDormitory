using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
	public abstract class Event : IComparable<Event>
	{
        public int Time { get; }
		protected abstract int PrimaryPriority { get; }
		protected int SecondaryPriority { get; set; }
		public Event(int time, int secondaryPriority = 1)
        {
			Time = time;
			SecondaryPriority = secondaryPriority;

        }
		public int CompareTo(Event other)
        {
			if (Time.CompareTo(other.Time) != 0)
				return Time.CompareTo(other.Time);
			if (PrimaryPriority.CompareTo(other.PrimaryPriority) == 0)
            {
				return SecondaryPriority.CompareTo(other.SecondaryPriority);
            }
			return PrimaryPriority.CompareTo(other.PrimaryPriority);
		}
		protected abstract void Action(Dormitory dorm);
		public void Invoke(Dormitory dorm)
        {
			Action(dorm);
        }
	}
	//events: opening/closing, changeofvratny, borrowing keys - gym, music room, washing machines, elevators, gym, study rooms, entering and leaving
	class ElevatorMovingUp : Event
    {
		private Elevator el;
		public ElevatorMovingUp(int time, Elevator elev, int secondaryPriority) : base(time)
		{
			el = elev;
		}
		protected override int PrimaryPriority => 8;
		protected override void Action(Dormitory dorm)
		{
			if (el.StudentsIn.Count > 1)
            {
				el.WhoGetsOff(dorm, Time);
			}
			el.GetNewPassanger(dorm, Time);
			el.MoveUp();
			if (el.FloorsToStop.Count > 0)
			{
				if (el.CurrentFloor < el.FloorsToStop.Max && el.CurrentState == Elevator.State.Up)
				{
					int pocet = dorm.calendar.calendar.Count;
					dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors, el, el.Number));
					int i = 1;
					while (pocet == dorm.calendar.calendar.Count)
                    {
						dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors + i, el, el.Number));
						i++;
					}
				}
				else
				{
					int pocet = dorm.calendar.calendar.Count;
					dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors, el, el.Number));
					int i = 1;
					while (pocet == dorm.calendar.calendar.Count)
                    {
						dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors + i, el, el.Number));
						i++;
					}
				}
			}
			else
            {
				el.MoveDown();
				el.Stop();
            }
		}
	}
	class ElevatorMovingDown : Event
	{
		private Elevator el;
		public ElevatorMovingDown(int time, Elevator elev, int secondaryPriority) : base(time)
		{
			el = elev;
		}
		protected override int PrimaryPriority => 9;
		protected override void Action(Dormitory dorm)
		{
			if (el.StudentsIn.Count > 0)
            {
				el.WhoGetsOff(dorm, Time);
			}
			el.GetNewPassanger(dorm, Time);
			el.MoveDown();
			if (el.FloorsToStop.Count > 0)
			{
				if (el.CurrentFloor > el.FloorsToStop.Min && el.CurrentState == Elevator.State.Down)
				{
					int number = dorm.calendar.calendar.Count;
					dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors, el, el.Number));
					int i = 1;
					while (number == dorm.calendar.calendar.Count)
					{
						dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors + i, el, el.Number));
						i++;
					}
				}
				else
				{
					int number = dorm.calendar.calendar.Count;
					dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors, el, el.Number));
					int i = 1;
					while (number == dorm.calendar.calendar.Count)
					{
						dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors + i, el, el.Number));
						i++;
					}
				}
			}
			else
            {
				el.MoveUp();
				el.Stop();
            }
		}
	}
	class PressingButtonOfElevator : Event
    {
		private Elevator el;
		public PressingButtonOfElevator(int time, Elevator elev, int secondaryPriority) : base(time)
		{
			el = elev;
		}
		protected override int PrimaryPriority => 20;
		protected override void Action(Dormitory dorm)
		{
            if ((el.CurrentFloor < el.FloorsToStop.Max) && (el.CurrentState == Elevator.State.Stop))
            {
				dorm.ScheduleEvent(new ElevatorMovingUp(Time+1, el, el.Number));
			}
            else if ((el.CurrentFloor > el.FloorsToStop.Min) && (el.CurrentState == Elevator.State.Stop))
            {
				dorm.ScheduleEvent(new ElevatorMovingDown(Time+1, el, el.Number));
            }
		}
	}
	class AddToElevatorQueue : Event
    {
		private Student student;
		private int destination;
		private int currentFloor;
		private Elevator elevator;
		public AddToElevatorQueue(Student stud, int destinationFloor, int currentFloor1, Elevator elev, int time) : base(time)
        {
			student = stud;
			destination = destinationFloor;
			currentFloor = currentFloor1;
			elevator = elev;
        }
		protected override int PrimaryPriority => 2;
        protected override void Action(Dormitory dorm)
        {
			dorm.AddToElevatorQueue(student, destination, currentFloor, elevator, Time+1);
        }
    }
	class StudentWantsSomething : Event
    {
		private Student student;
		public StudentWantsSomething(int time, Student stud, int secondaryPriority) : base(time)
        {
			student = stud;
        }
		protected override int PrimaryPriority => 8;
        protected override void Action(Dormitory dorm)
        {
			dorm.StudentWantsSmt(student, Time);
        }
    }
	class ArrivingToFirstFloorByFoot : Event
    {
		private Student student;
		public ArrivingToFirstFloorByFoot(int time, Student stud, int secondaryPriority) : base(time)
		{
			student = stud;
		}
		protected override int PrimaryPriority => 12;
		protected override void Action(Dormitory dorm)
		{
            if (student.CurrentFloor != 0)
            {
				dorm.ArrivingToFirstFloor(student, Time);
			}
		}
	}
	class OpeningDorms : Event
    {
		public OpeningDorms(int time) : base(time)
        {
        }
		protected override int PrimaryPriority => 1;
		protected override void Action(Dormitory dorm)
        {
			dorm.Open(Time);
        }
    }
	class ChangingPorters : Event
    {
		public ChangingPorters(int time) : base(time)
        {
        }
		protected override int PrimaryPriority => 2;
        protected override void Action(Dormitory dorm)
        {
			dorm.ChangePorters(Time);
        }
    }
	class AddToQueue : Event
    {
		private Student student;
		public AddToQueue(int time, Student stud, int secondaryPriority) : base(time)
		{
			student = stud;
			SecondaryPriority = secondaryPriority;
		}
		protected override int PrimaryPriority => 4;
		protected override void Action(Dormitory dorm)
		{
			dorm.AddToPorterQueue(Time, student);
		}
	}
	class ComingInDormitory : Event
    {
		private Student student;
		private bool WantsSomething;
		public ComingInDormitory(int time, Student stud, int secondaryPriority, bool wantsSomething) : base(time)
		{
			student = stud;
			WantsSomething = wantsSomething;
		}
		protected override int PrimaryPriority => 12;
		protected override void Action(Dormitory dorm)
		{
			dorm.EnteringDormitory(student, Time, WantsSomething);
		}
	}
	class LeavingDormitory : Event
    {
		private Student student;
		public LeavingDormitory(int time, Student stud, int secondaryPriority) : base(time)
		{
			student = stud;
		}
		protected override int PrimaryPriority => 13;
		protected override void Action(Dormitory dorm)
		{
			dorm.LeavingDormitory(student, Time);
		}
	}
	class NextWaiterInQueue : Event
    {
		public NextWaiterInQueue(int time) : base(time)
        {
        }
		protected override int PrimaryPriority => 5;
        protected override void Action(Dormitory dorm)
        {
			dorm.RemoveFromQueue(Time);
        }
    }
	public abstract class BorrowingAndReturningThings : Event
    {
		private Student student;
		protected enum ThingsToBorrow
        {
			GymKeys,
			WashingMachineKeys,
			StudyRoomKeys,
			VacuumCleaner,
			MusicRoomKeys
        }
		protected new abstract int SecondaryPriority { get;}
		public BorrowingAndReturningThings(int time, Student who, int priority = 1) : base(time)
        {
			student = who;
		}
		public int CompareTo(BorrowingAndReturningThings other)
        {
			if (Time.CompareTo(other.Time) != 0)
				return Time.CompareTo(other.Time);
			if (PrimaryPriority.CompareTo(other.PrimaryPriority) == 0)
			{
				if (SecondaryPriority.CompareTo(other.SecondaryPriority) == 0)
				{
					return student.Number.CompareTo(other.student.Number);
				}
				return SecondaryPriority.CompareTo(other.SecondaryPriority);
			}
			return PrimaryPriority.CompareTo(other.PrimaryPriority);
		}
		protected override int PrimaryPriority => 1;
    }
	class BorrowingGymKeys : BorrowingAndReturningThings
    {
		private Student student;
		public BorrowingGymKeys(int time, Student who) : base(time, who)
        {
			student = who;
        }
		protected override int SecondaryPriority => 1;
		protected override void Action(Dormitory dorm)
        {
			dorm.BorrowingGymKeys(Time, student);
        }
    }
	class ReturningGymKeys : BorrowingAndReturningThings
	{
		private Student student;
		public ReturningGymKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 2;
		protected override void Action(Dormitory dorm)
		{
			dorm.ReturningGymKeys(Time, student);
		}
	}
	class BorrowingWashingMachineRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public BorrowingWashingMachineRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 3;
		protected override void Action(Dormitory dorm)
		{
			dorm.BorrowingWashingMachineRoomKeys(Time, student);
		}
	}
	class ReturningWashingMachineRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public ReturningWashingMachineRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 4;
		protected override void Action(Dormitory dorm)
		{
			dorm.ReturningWashingMachineRoomKeys(Time, student);
		}
	}
	class BorrowingMusicRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public BorrowingMusicRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 5;
		protected override void Action(Dormitory dorm)
		{
			dorm.BorrowingMusicRoomKeys(Time, student);
		}
	}
	class ReturningMusicRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public ReturningMusicRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 6;
		protected override void Action(Dormitory dorm)
		{
			dorm.ReturningMusicRoomKeys(Time, student);
		}
	}
	class BorrowingStudyRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public BorrowingStudyRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 5;
		protected override void Action(Dormitory dorm)
		{
			dorm.BorrowingStudyRoomKeys(Time, student);
		}
	}
	class ReturningStudyRoomKeys : BorrowingAndReturningThings
	{
		private Student student;
		public ReturningStudyRoomKeys(int time, Student who) : base(time, who)
		{
			student = who;
		}
		protected override int SecondaryPriority => 8;
		protected override void Action(Dormitory dorm)
		{
			dorm.ReturningStudyRoomKeys(Time, student);
		}
	}
}