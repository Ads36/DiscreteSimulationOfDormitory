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
		public ElevatorMovingUp(int time, Elevator elev) : base(time)
		{
			el = elev;
		}
		protected override int PrimaryPriority => 5;
		protected override void Action(Dormitory dorm)
		{
			el.MoveUp();
			if (el.StudentsIn.Count > 1)
            {
				el.WhoGetsOff(dorm, Time);
			}
			
			el.GetNewPassanger(dorm);
			
			if (el.FloorsToStop.Count > 0)
			{

				if (el.CurrentFloor < el.FloorsToStop.Max && el.CurrentState == Elevator.State.Up && !el.IsFull())
				{
					dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors, el));
				}
				else if (el.CurrentFloor == el.FloorsToStop.Max)
				{
					dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors, el));
				}
				else
				{
					dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors, el));
				}
			}
			else
            {
				el.Stop();
            }
		}
	}
	class ElevatorMovingDown : Event
	{
		private Elevator el;
		public ElevatorMovingDown(int time, Elevator elev) : base(time)
		{
			el = elev;
		}
		protected override int PrimaryPriority => 5;
		protected override void Action(Dormitory dorm)
		{
			el.MoveDown();
			if (el.StudentsIn.Count > 1)
            {
				el.WhoGetsOff(dorm, Time);
			}
			
			el.GetNewPassanger(dorm);
			
			if (el.FloorsToStop.Count > 0)
			{
				if (el.CurrentFloor > el.FloorsToStop.Min && el.CurrentState == Elevator.State.Down)
				{
					dorm.ScheduleEvent(new ElevatorMovingDown(Time + el.SpeedBetweenFloors, el));
				}
				else if (el.CurrentFloor == el.FloorsToStop.Min)
				{
					dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors, el));
				}
				else
				{
					dorm.ScheduleEvent(new ElevatorMovingUp(Time + el.SpeedBetweenFloors, el));
				}
			}
			else
            {
				el.Stop();
            }
		}
	}
	class PressingButtonOfElevator : Event
    {
		private Elevator el;
		public PressingButtonOfElevator(int time, Elevator elev) : base(time)
		{
			el = elev;
			
		}
		protected override int PrimaryPriority => 6;
		protected override void Action(Dormitory dorm)
		{
            if (el.CurrentFloor < el.FloorsToStop.Max && el.CurrentState == Elevator.State.Stop)
            {
				Console.WriteLine("neco");
				dorm.ScheduleEvent(new ElevatorMovingUp(Time, el));
			}
            else if (el.CurrentFloor > el.FloorsToStop.Min && el.CurrentState == Elevator.State.Stop)
            {
				Console.WriteLine("neco");
				dorm.ScheduleEvent(new ElevatorMovingDown(Time, el));
            }
            
		}
	}
	class StudentWantsSomething : Event
    {
		private Student student;
		public StudentWantsSomething(int time, Student stud) : base(time)
        {
			student = stud;
        }
		protected override int PrimaryPriority => 10;
        protected override void Action(Dormitory dorm)
        {
			dorm.StudentWantsSmt(student, Time);
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
		public AddToQueue(int time, Student stud) : base(time)
		{
			student = stud;
		}
		protected override int PrimaryPriority => 4;
		protected override void Action(Dormitory dorm)
		{
			dorm.AddToPorterQueue(Time, student);
		}
	}
	class LeavingRoom : Event
    {
		public LeavingRoom(int time, Student stud) : base(time)
        {

        }
		protected override int PrimaryPriority => 4;
        protected override void Action(Dormitory dorm)
        {
            //dorm.
        }
    }
	class ComingInDormitory : Event
    {
		private Student student;
		public ComingInDormitory(int time, Student stud) : base(time)
		{
			student = stud;
		}
		protected override int PrimaryPriority => 4;
		protected override void Action(Dormitory dorm)
		{
			dorm.EnteringDormitory(student, Time);
		}
	}
	class LeavingDormitory : Event
    {
		private Student student;
		public LeavingDormitory(int time, Student stud) : base(time)
		{
			student = stud;
		}
		protected override int PrimaryPriority => 4;
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
		protected override int PrimaryPriority => 3;
        protected override void Action(Dormitory dorm)
        {
			dorm.RemoveFromQueue(Time);
        }
    }
	public abstract class BorrowingAndReturningThings : Event
    {
		protected enum ThingsToBorrow
        {
			GymKeys,
			WashingMachineKeys,
			StudyRoomKeys,
			VacuumCleaner,
			MusicRoomKeys
        }
		protected abstract int SecondaryPriority { get;}
		public BorrowingAndReturningThings(int time, Student who, int priority = 1) : base(time)
        {
			//SecondaryPriority = priority;
		}
		public int CompareTo(BorrowingAndReturningThings other)
        {
			if (Time.CompareTo(other.Time) != 0)
				return Time.CompareTo(other.Time);
			if (PrimaryPriority.CompareTo(other.PrimaryPriority) == 0)
			{
				if (SecondaryPriority.CompareTo(other.SecondaryPriority) == 0)
				{
					return 1;
				}
				return SecondaryPriority.CompareTo(other.SecondaryPriority);
			}
			return PrimaryPriority.CompareTo(other.PrimaryPriority);
		}
		protected override int PrimaryPriority => 3;
		
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
		protected override int SecondaryPriority => 1;
		protected override void Action(Dormitory dorm)
		{
			dorm.ReturningGymKeys(Time, student);
		}
	}
}