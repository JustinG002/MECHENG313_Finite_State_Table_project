using System;

public class MECHENG313_Assignment_2_Task_1
{

    public delegate void Action();

    class FiniteStateTable
    {

        private cell_FST[,] FST;    //first index is input index, second is state index
        private string[] Inputs;    //array of event string that could trigger a state transition, in string format
        private string[] States;    //array of state of the FST
        private string initialState;    //the initiate state of the FST, default to 0,0 of the FST if not specified
        private bool isEmpty_StatesNInputs = true;  

        private cell_FST[,] empty_FST = new cell_FST[1, 1]; //empty FSM which doesnt have any state transition

        struct cell_FST
        {
            public bool isEmpty_nextState;
            public bool isEmpty_action;
            public bool isTransitionCondition_state;
            public bool isTransitionCondition_NULL;
            public string nextState;  //next state on the FST
            public Action action;  //any action associate with the transition
        }


        //Constructor for the FiniteStateTable class
        //arguments
        //Inputs: an array of string which consist of various events that could trigger an state transition
        //States: an array of string which consist of all the possible state of a FSM/FST
        //initalState: the inital state of the FSM/FST
        public FiniteStateTable(string[] Inputs, string[] States, string initialState)
        {
            //create a 2D array to store the FST
            this.FST = new cell_FST[Inputs.Length, States.Length];

            this.SetIsEmpty();
            isEmpty_StatesNInputs = false;
            //store the string array of various state and event
            this.Inputs = Inputs;
            this.States = States;

            SetInitialState(initialState); //set the inital state of teh FST
        }

        //set isEmpty and isTransitionCondition variables in of cell_FST struct to true to avoid index out of range
        private void SetIsEmpty()
        {
            for (int i = 0; i < FST.GetLength(0); i++)
            {
                for (int j = 0; j < FST.GetLength(1); j++)
                {
                    FST[i, j].isEmpty_nextState = true;
                    FST[i, j].isEmpty_action = true;
                    FST[i, j].isTransitionCondition_state = false;
                    FST[i, j].isTransitionCondition_NULL = false;
                }
            }
            empty_FST[0, 0].isEmpty_action = true;
            empty_FST[0, 0].isEmpty_nextState = true;
            empty_FST[0, 0].isTransitionCondition_NULL = false;
            empty_FST[0, 0].isTransitionCondition_state = false;
            empty_FST[0, 0].action = Action_NULL;
        }

        //return cell_FST of the Finite state table
        //argument:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        private cell_FST GetCell(int stateIndex, int inputIndex)
        {
            if ((stateIndex < 0) || (inputIndex < 0))
            {
                return empty_FST[0, 0];
            }
            else
            {
                return this.FST[inputIndex, stateIndex];
            }
        }
        //return cell_FST of the Finite state table using string inpuit
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        private cell_FST GetCell(string state, string input)
        {
            if (this.isEmpty_StatesNInputs)
            {
                //Console.WriteLine("GetCell method error, trying to use state&input find without a valid state and input list");
                return this.empty_FST[0, 0];
            }
            else
            {
                //get the index of the particular cell
                int stateIndex = GetStateIndex(state);
                int inputIndex = GetInputIndex(input);

                //check if the input input is value, return -1 if not
                if (stateIndex == -1 && inputIndex == -1)
                {
                    //Console.WriteLine("GetCell method error, trying to access not valid cell");
                    return this.empty_FST[0, 0];
                }
                else
                {
                    return this.FST[inputIndex, stateIndex];
                }
            }
        }



        //set the nextState variable in cell_FST of the Finite state table using index of FST 2D array
        //argument:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        //nextStateUbdex: index of the next state in teh States array
        private void SetNextState(int stateIndex, int inputIndex, int nextStateIndex)
        {
            if ((stateIndex < 0) || (inputIndex < 0) || (nextStateIndex < 0)) { }
            else
            {
                this.FST[inputIndex, stateIndex].isEmpty_nextState = false;
                this.FST[inputIndex, stateIndex].nextState = States[nextStateIndex];
            }
            
        }
        //set the nextState variable in cell_FST of the Finite state table using string
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        //nextState: state in States array, string
        public void SetNextState(string state, string input, string nextState)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);
            int nextStateIndex = GetStateIndex(nextState);

            if ((stateIndex >= 0) && (inputIndex >= 0) && (nextStateIndex >= 0))
            {
                SetNextState(stateIndex, inputIndex, nextStateIndex);
                for (int i = 0; i < States.Length; i++)
                {
                    if (this.States[i] == input)
                    {
                        this.FST[inputIndex, stateIndex].isTransitionCondition_state = true;
                    }
                }
            }
        }
        //return the next state as a string of a cell in the FST using index of FST 2D array
        //argument:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        private string GetNextState(int stateIndex, int inputIndex)
        {
            if ((stateIndex < 0) || (inputIndex < 0))
            {
                return States[stateIndex]; //return the current state if the input is out of index
            }
            else if (GetCell(stateIndex, inputIndex).isEmpty_nextState)
            {
                return States[stateIndex]; //return the current state if there isnt a next state
            }
            else
            {
                return GetCell(stateIndex, inputIndex).nextState;
            }

        }
        //return the next state as a string of a cell in the FST using string
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        public string GetNextState(string state, string input)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);
            int stateTransitionIndex = GetStateTransitionIndex(state);

            //account for when state transition can occur regardless of the input (eg. state based transition)
            if (GetCell(stateIndex, inputIndex).isEmpty_nextState)
            {
                if (GetCell(stateIndex, stateTransitionIndex).isTransitionCondition_state)
                {
                    return GetNextState(stateIndex, stateTransitionIndex);
                }
                else
                {
                    return state; //return current state
                }

            }
            else
            {
                return GetNextState(stateIndex, inputIndex);
            }
        }

        //set the action associate with a transition of a cell in FST using index of FST 2D array
        //argument:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        //action: delegate to be called during state transition, Action delegate
        private void SetAction(int stateIndex, int inputIndex, Action action)
        {
            if ((stateIndex < 0) || (inputIndex < 0)) { }
            else
            {
                this.FST[inputIndex, stateIndex].isEmpty_action = false;
                this.FST[inputIndex, stateIndex].action = action;
            }
        }
        //set the action associate with a transition of a cell in FST using string
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        //action: delegate to be called during state transition, Action delegate
        public void SetAction(string state, string input, Action action)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);

            if ((stateIndex >= 0) && (inputIndex >= 0))
            {
                SetAction(stateIndex, inputIndex, action);
            }
            else
            {
                //Console.WriteLine("SetAction method, trying to set a non-value state");
            }
        }
        //return the Action delegate associate with a state transition of a cell in FST using index of FST 2D array
        //argument:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        private Action GetAction(int stateIndex, int inputIndex)
        {
            if ((stateIndex < 0) || (inputIndex < 0))
            {
                return Action_NULL;
            }
            else if (GetCell(stateIndex, inputIndex).isEmpty_action)
            {
                //Console.WriteLine("GetNextState method error, accessing an empty cell");
                return Action_NULL;
            }
            else
            {
                return GetCell(stateIndex, inputIndex).action;
            }
        }
        //return the Action delegate associate with a state transition of a cell in FST using string
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        public Action GetAction(string state, string input)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);
            int stateTransitionIndex = GetStateTransitionIndex(state);

            //account for when state transition can occur regardless of the input (eg. state based transition)
            if (GetCell(stateIndex, inputIndex).isEmpty_action)
            {
                if (GetCell(stateIndex, stateTransitionIndex).isTransitionCondition_state)
                {
                    return GetAction(stateIndex, stateTransitionIndex);
                }
                else
                {
                    return Action_NULL;
                }

            }
            else
            {
                return GetAction(stateIndex, inputIndex);
            }

        }

        //set the initial state of the FST
        //argument:
        //state: state in States array, string
        public void SetInitialState(string state)
        {
            if (GetStateIndex(state) >= 0)
            {
                this.initialState = state;
            }
            else
            {
                //Console.WriteLine("setting InitalState failed, default to first element in the States arrat");
                this.initialState = States[0];
            }

        }
        //return the initial state of the FST
        public string GetInitalState()
        {
            return this.initialState;

        }


        //return the index of the event in the Inputs array from a event string
        //argument:
        //input: event in Inputs array, string
        private int GetInputIndex(string input)
        {
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (Inputs[i].Equals(input))
                {
                    return i;
                }
            }
            //Console.WriteLine("GetInputIndex method error, not match found");
            return -1;
        }

        //return the index of the state in the States array from a state string
        //argument:
        //state: state in States array, string
        private int GetStateIndex(string state)
        {
            for (int i = 0; i < States.Length; i++)
            {
                if (States[i].Equals(state))
                {
                    return i;
                }
            }
            //Console.WriteLine("GetStateIndex method error, not match found");
            return -1;
        }
        //return the index of an event for a state based trnsaction from a state string
        //argument:
        //state: state in States array, string
        private int GetStateTransitionIndex(string state)
        {
            //account for a whole stirng
            for (int i = 0; i < Inputs.Length; i++)
            {
                if (Inputs[i].Equals(state))
                {
                    return i;
                }
            }

            //acount for substring (eg only account for two FSM, could implment more but will complicate the code)
            //create a array of each state in the input string, assume they start with S
            int e = 0;
            int firstSInd, nextSInd;
            string currentState;
            firstSInd = state.IndexOf('S');

            do
            {

                //find all the S in current State

                nextSInd = state.IndexOf('S', (firstSInd + 1));

                //while there are more "S" in the current state

                int stateLength = nextSInd - firstSInd;

                if (stateLength <= 0)
                {
                    currentState = state.Substring(firstSInd);
                }
                else
                {
                    currentState = state.Substring(firstSInd, state.Length - 1 - firstSInd);
                }

                //check if the state is one of the "event" that will result in a state transition
                for (int i = 0; i < Inputs.Length; i++)
                {
                    if (Inputs[i].Equals(currentState))
                    {
                        return i;
                    }
                }

                e++;
                firstSInd = nextSInd;

            } while (nextSInd != -1);


            //if no match found
            return -1;
        }

        //return a bool variable based on if a particular cell of the FST have a next state associate with it
        public bool IsValidCell(string state, string input)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);

            if (stateIndex >= 0 && inputIndex >= 0)
            {
                return !(GetCell(stateIndex, inputIndex).isEmpty_nextState);
            }
            else
            {
                return false;
            }

        }

        //Null action, default for transation that doesn't have an event doesnt associate with a transition
        private void Action_NULL()
        {
        }
    }
}

