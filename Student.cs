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
		public int TimeInGym { get; private set; } = 2700;
		public int TimeInMusicRoom { get; private set; } = 3600;
		public int TimeInStudyRoom { get; private set; } = 5400;
		public int TimeInWashingMachinesRoom { get; private set; } = 1800;
		public int TimeOut { get; private set; } = 3600;
		public int CurrentFloor;
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
		public Student(Dormitory dorm)
		{
			HomeFloor = random.Next(1, dorm.NumberOfFloors);
			Number = currentNumber++;
			int rand = random.Next(0, 100);
            if (rand > 50)
            {
				CurrentPlace = Place.InRoom;
            }
			else
            {
				CurrentPlace = Place.Outside;
            }
			pozadavek = WhatHeWants.GymKeys;
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

		public WhatHeWants pozadavek;
		public enum WhatHeWants
		{
			GymKeys,
			ReturningGymKeys,
			WashingMachineKeys,
			ReturningWashingMachineKeys,
			MusicRoomKeys,
			ReturningMusicRoomKeys,
			StudyRoomKeys,
			ReturningStudyRoomKeys, 
			Nothing
		}
		public bool IsReadyToGoQueue()
        {
            if (CurrentPlace == Place.InRoom || CurrentPlace == Place.Outside)
            {
				return true;
			}
			return false;
			
        }
	}
}
