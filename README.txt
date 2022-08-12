Discrete Simulation of Dormitory

Introduction
My goal was to create a discrete simulation of the Dormitory 17. Listopadu written in C#. Discrete simulation (discrete-event simulation)
models a system using a discrete sequence of events. Each event happens at a particular instance in time and does some change in the system. 
After an event is done, simulation jumps on next event (an event mostly adds another event(s) to a priority queue).

Task
Simulation described above. Console application in C#, input from file and console, output to console. Output includes time of event, description
of event action and sometimes more information. The program isn't perfect simulation of the dormitory (I think that task would be impossible), 
but at least contains basic ideas and events that could happen in the dormitory.