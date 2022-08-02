using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
	public abstract class Event : IComparable<Event>
	{
        public int Time { get; }
		protected abstract int PrimaryPriority { get; }

		public Event(int time, int secondaryPriority = 1)
        {
			Time = time;
			//PrimaryPriority = primaryPriority;

        }
		public int CompareTo(Event other)
        {
			if (Time.CompareTo(other.Time) != 0)
				return Time.CompareTo(other.Time);
			if (PrimaryPriority.CompareTo(other.PrimaryPriority) == 0)
            {
				return 1;
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
		public BorrowingGymKeys(int time, Student who, int priority) : base(time, who, priority)
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
		public ReturningGymKeys(int time, Student who, int priority) : base(time, who, priority)
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