using System;
using System.Collections.Generic;
namespace DiscreteSimulationOfDormitory
{
    public class Elevator
    {

        public enum State
        {
            Up,
            Down,
            Still
        }
        public State CurrentState;
        public void Move()
        {
            CurrentState = State.Down;
            if (CurrentState == State.Down)
            {

            }
        }
    }
}