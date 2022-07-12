using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
	public class Student
	{
		public int Number { get; }
		private static int currentNumber = 1;
		public Place CurrentPlace;
		public enum Place
        {
			Outside,
			InRoom,
			WaitingForElevator,
			InElevator,
			InGym,
			InStudyRoom,
			WaitingInQueue,
        }
		public Student()
        {
			Number = currentNumber++;
        }
		public Place GetCurrentPlace()
        {
			return CurrentPlace;
        }
	}
}