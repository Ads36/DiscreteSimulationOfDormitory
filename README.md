# Discrete Simulation of Dormitory

## Introduction
My goal was to create a discrete simulation of the Dormitory 17. Listopadu written in C#. Discrete simulation (discrete-event simulation)
models a system using a discrete sequence of events. Each event happens at a particular instance in time and does some change in the system. 
After an event is done, simulation jumps on next event (an event mostly adds another event(s) to a priority queue).

## Task
Simulation described above. Console application in C#, input from file and console, output to console. Output includes time of event, description
of event action and sometimes more information. The program isn't perfect simulation of the dormitory (I think that task would be impossible), 
but at least contains basic ideas and events that could happen in the dormitory. Some events are generated randomly, so each output should be unique.

## Algorithm
I used discrete simulation.

## Data structures
Priority queue for event calendar (SortedSet from System.Collections.Generic), lists, queues.

## Program
Program is divided into 5 major classes:

### Class Dormitory
Class that holds everything together. It has instances of students, elevators, and calendar.
Methods:
Initialize, InitializeStudents, InitializeElevators – creates data structures, instances and schedules some events
All methods below are called when an event happens
StudentWantsSomethingFromGatekeeper – schedules events according to student current place
ArrivingToFirstFloor – student either joins queue or goes outside
EnteringDormitory, LeavingDormitory – called when student enters or leaves dormitory, schedules either EnteringDormitory or AddToQueue or sends student to his room
Open – opens dormitory
ChangeGatekeepers – changes gatekeepers
BorrowingThings and ReturningThings – 8 method, one of them called (borrow) after student leaves queue and another (return) called after he finishes an action
AddToGatekeeperQueue – enqueues student to gatekeeper queue
RemoveFromQueue – dequeues student from gatekeeper queue and also schedules event (BorrowingThings or ReturningThings) according to student's request
AddToElevatorQueue – adds student to queue and also starts an elevator if it is not moving

### Class Calendar
Class which is responsible for scheduling and invoking events.
Methods:
ScheduleEvent – schedules add event to SortedSet.
Run – invokes all events in a while loop.

### Class Elevator
This class provides everything about elevators. 
Methods:
MoveDown, MoveUp – moves elevator in corresponding direction.
Stop – stops the elevator.
GetOnElevator – adds student to the elevator. Also checks patience (student can't withstand many full elevators in his direction of travel) 
of other students in queue.
GetOffElevator – removes student from the elevator and assigns new request for him.
GettingOfElevator – removes all students on demanded floor (calls GetOffElevator for each of them).
IsFull checks capacity.
DoesSomeoneGetOff – returns true if there is someone who wants to get off at current floor.
GetNewPassanger – checks if there are any students in queue in current floor, if yes, they GetOnElevator.

### Abstract class Event
Each derived class must implement Action and have unique PrimaryPriority. Instances are compared to each other, so Event implements IComparable.
There is also static int Time – each event happens at a specific time.
Most events call 1 method of Dormitory instance, which is responsible for all things happening in simulation. Events have the same name as Dormitory methods, which are described above.
Derived classes:
OpeningDorms
ChangingGatekeepers
AddToQueue
AddToElevatorQueue
NextWaiterInQueue
StudentWantsSomething
ArrivingToFirstFloorByFoot
EnteringDormitory – called when student enters dormitory
LeavingDormitory – called when student is leaving dormitory
ElevatorMovingUp – called when elevator can move up (there is a floor to stop in a floor above current elevator floor)
ElevatorMovingDown – same as ElevatorMovingUp, but in the opposite direction
BorrowingAndReturningThings (and classes derived from this class) – all things (keys) student can borrow and than return have 2 classes derived
from this class, each is called when that event happens

### Class Student
This class holds information about a student.

## Alternate program solutions
More in possible extensions.

## Input data representation
Input data is read from both console and a file. Input from console is basic (just pressing enter), input from file is more complicated.
The file with input data must be called **"input.txt"** and must be in the same directory where you run this program (exe file).

The file must have exactly **17 lines, each line contains just one integer**. Format of lines is described below.
**All time values must be entered in seconds**
I do not account for any malfunction caused by bad input format.

Format of lines of input data in "input.txt":
Number (count) of floors in dormitory
Number of gatekeepers
Number of students
Number of elevators
Time when the simulation ends
Gatekeeper serving a student time
Work time of gatekeeper
Number of floor where the washing machines are
Capacity of elevator
Elevator time between floors (speed, smaller value means faster elevator)
Student time in gym
Student time in music room
Student time in study room
Student time in washing machines room
Student time outside
Student time in room
Student time between 2 attendance of gatekeeper can occur (time between events)

## Output data representation
Output data are written on separate lines in console.
Almost all output data are in this format: <{time}> Student {student number} ... and some other things.
Time is in format dd:hh:mm:ss, where d are days, h are hours, m are minutes and s are seconds. Time begins at 0, all events have time relative to the start of simulation.
Each student has unique number, so identification is easy. Some other things includes elevators, each of them also has unique number.
Some output data are in format <{time}> Changing porters ..., this happens when porters need to change duties every 8 hours.

## Possible extensions
Other events could be added, for example borrowing vacuum cleaners, selling baguettes and pouring water from Filtermac.
Another extension might be to add another dormitory building (which is independent on the other one) but runs in the same time frame.
Gatekeeper names might added to commentary and they can also be loaded from a file.
More randomness could be added to the duration of student events.

## How was the work done
I started this project in the beggining of July 2022 and finally finished in the middle of August 2022.
Altogether, I worked on this project for about a week and a half of pure work, but I procrastinated a lot and worked on some other projects.
I do not include any tests because of the randomness, but the program was completely tested using console.

## What hasn't been done
Closing the reception event is not necessary, because the reception never closes. There is only a change of gatekeepers, which I've included in the simulation.

This is the second discrete simulation that I have made and I am not sure if I am going to do another one.