using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{

	public class Student
	{
		public Random random = new Random();
		public int Number { get; }
		private static int currentNumber = 1;
		public int HomeFloor { get; }
		public int TimeInGym { get; private set; } = 2700;
		public int TimeInMusicRoom { get; private set; } = 3600;
		public int TimeInStudyRoom { get; private set; } = 5400;
		public int TimeInWashingMachinesRoom { get; private set; } = 1800;
		public int TimeOut { get; private set; } = 3600;
		public int TimeInRoom { get; private set; } = 3600;
		public int CurrentFloor;
		public int TimeBetweenEvents { get; private set; } = 8 * 3600;
		public Place CurrentPlace;
		public enum Place
		{
			Outside,
			InRoom,
			WaitingForElevator,
			InElevator,
			InGym,
			InMusicRoom,
			InStudyRoom,
			InWashingMachineRoom,
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
				CurrentPlace = Place.InRoom;
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
		public WhatHeWants RandomRequest()
        {
			int randomness = random.Next(0, 5);
			WhatHeWants request = WhatHeWants.Nothing;
			switch (randomness)
			{
				case 0:
					request = Student.WhatHeWants.GymKeys;
					break;
				case 1:
					request = Student.WhatHeWants.GymKeys;
					break;
				case 2:
					request = Student.WhatHeWants.MusicRoomKeys;
					break;
				case 3:
					request = Student.WhatHeWants.StudyRoomKeys;
					break;
				case 4:
					request = Student.WhatHeWants.WashingMachineKeys;
					break;
				default:
					break;
			}
			return request;
        }
	}
}
