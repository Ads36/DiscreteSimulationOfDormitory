using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
	abstract class Event : IComparable<Event>
	{
        public int Time { get; }
		public int PrimaryPriority { get; }
		public int SecondaryPriority { get; }
		public Event(int time, int primaryPriority, int secondaryPriority)
        {
			Time = time;
			PrimaryPriority = primaryPriority;
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


}