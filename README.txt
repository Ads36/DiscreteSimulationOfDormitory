Discrete Simulation of Dormitory

Introduction
My goal was to create a discrete simulation of the Dormitory 17. Listopadu written in C#. Discrete simulation (discrete-event simulation)
models a system using a discrete sequence of events. Each event happens at a particular instance in time and does some change in the system. 
After an event is done, simulation jumps on next event (an event mostly adds another event(s) to a priority queue).

Task
Simulation described above. Console application in C#, input from file and console, output to console. Output includes time of event, description
of event action and sometimes more information. The program isn't perfect simulation of the dormitory (I think that task would be impossible), 
but at least contains basic ideas and events that could happen in the dormitory. Some events are generated randomly, so each output should be unique.

Algorithm
I used discrete simulation.

Data structures
Priority queue for event calendar (SortedSet from System.Collections.Generic), lists, queues.

Program
//todo

Alternate program solutions
More in possible extensions.

Input data representation
//todo

Output data representation
Output data are written on separate lines in console.
Almost all output data are in this format: <{time}> Student {student number} ... and some other things.
Time is in format dd:hh:mm:ss, where d are days, h are hours, m are minutes and s are seconds. Time begins at 0, all events have time relative to the start of simulation.
Each student has unique number, so identification is easy. Some other things includes elevators, each of them also has unique number.
Some output data are in format <{time}> Changing porters ..., this happens when porters need to change duties every 8 hours.


Possible extensions
Other events could be added, for example borrowing vacuum cleaners, selling baguettes and pouring water from Filtermac.
Another extension might be to add another dormitory building (which is independent on the other one) but runs in the same time frame.

How was the work done
I started this project in the beggining of July 2022 and finally finished in the middle of August 2022.
Altogether, I worked on this project for about a week and a half of pure work, but I procrastinated a lot and worked on some other projects.


What hasn't been done
Closing the reception event is not necessary, because the reception never closes. There is only a change of gatekeepers, which I've included in the simulation.

This is the second discrete simulation that I have made and I am not sure if I am going to do another one.