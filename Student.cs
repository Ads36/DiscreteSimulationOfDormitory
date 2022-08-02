using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{

	public class Student
	{
		public Random random = new Random();
		public int Number { get; }
		private static int currentNumber = 1;
		public int HomeFloor;
		public int TimeInGym = 2700;
		public int TimeInMusicRoom = 3600;
		public int TimeInStudyRoom = 5400;
		public int TimeInWashingMachinesRoom = 1800;

		public Place CurrentPlace;
		public enum Place
        {
			Outside,
			InRoom,
			WaitingForElevator,
			InElevator,
			InGym,
			InStudyRoom,
			WaitingInQueue
        }
		public Student()
        {
			HomeFloor = random.Next(1,+1);
			Number = currentNumber++;
        }
		public Place GetCurrentPlace()
        {
			return CurrentPlace;
        }
		public Place nextPlace;
		public void NextPlace()
        {
			CurrentPlace = nextPlace;
        }
	}

}