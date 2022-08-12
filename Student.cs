﻿using System;
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
		public int TimeOut { get; private set; } = 8 * 3600;
		public int TimeInRoom { get; private set; } = 5 * 3600;
		public int CurrentFloor;
		public int TimeBetweenEvents { get; private set; } = 24 * 3600;
		public Place CurrentPlace { get; set; }
		public enum Place
		{
			Outside,
			InRoom,
			Waiting,
			InElevator,
			InGym,
			InMusicRoom,
			InStudyRoom,
			InWashingMachineRoom
		}
		public Student(Dormitory dorm)
		{
			HomeFloor = random.Next(1, dorm.NumberOfFloors);
			Number = currentNumber++;
			int rand = random.Next(0, 100);
            if (rand > 30)
            {
				CurrentPlace = Place.InRoom;
            }
			else
            {
				CurrentPlace = Place.Outside;
            }
			Request = RandomRequest();
		}
		public WhatHeWants Request { get; set; }
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
			int randomness = random.Next(0, 6);
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
				case 5:
					request = Student.WhatHeWants.Nothing;
					break;
				default:
					break;
			}
			return request;
        }
	}
}
