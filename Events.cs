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
			//PrimaryPriority = primaryPriority;
			SecondaryPriority = secondaryPriority;
        }
		public int CompareTo(Event other)
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
            
        }
    }
}