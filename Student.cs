using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
	public class Student
	{
		private Random random = new Random();
		public int Number { get; }
		private static int currentNumber = 1;
		public int HomeFloor { get; }
		public int TimeInGym { get; private set; }
		public int TimeInMusicRoom { get; private set; }
		public int TimeInStudyRoom { get; private set; }
		public int TimeInWashingMachinesRoom { get; private set; }
		public int TimeOut { get; private set; }
		public int TimeInRoom { get; private set; }
		public int CurrentFloor;
		public int TimeBetweenEvents { get; private set; }
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
		public Student(Dormitory dorm, Students stud)
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
			TimeInGym = stud.TimeInGym;
			TimeInMusicRoom = stud.TimeInMusicRoom;
			TimeInRoom = stud.TimeInRoom;
			TimeInWashingMachinesRoom = stud.TimeInWashingMachinesRoom;
			TimeInStudyRoom = stud.TimeInStudyRoom;
			TimeOut = stud.TimeOut;
			TimeBetweenEvents = stud.TimeBetweenEvents;

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
		//generating random requests
		public WhatHeWants RandomRequest()
        {
			int randomness = random.Next(0, 6);
			WhatHeWants request = WhatHeWants.Nothing;
			switch (randomness)
			{
				case 0:
					request = Student.WhatHeWants.GymKeys;
					break;
				//most people go to the gym, so the probability that student is going to the gym is doubled
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
