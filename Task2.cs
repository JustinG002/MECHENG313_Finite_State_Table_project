using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace MECHENG313_Assignment_2_Task_2
{

    public delegate void Action();

    class Program
    {
        
        //program entry point
        static void Main(string[] args)
        {
            List<string> LoggerString = new List<string>();

            var Logger = new ConsoleLogger(LoggerString);

            Task2();

            WriteToFile(LoggerString.ToArray());

        }
      


        //Runs the logic for the finite state machine describe in Task 2 of the assignment
        static void Task2()
        {
            //iniciate variables
            ConsoleKeyInfo input; //store the last user input
            var Task2FST = CreatFST_Task2();    //create the FST for Task 2

            string currentState = Task2FST.GetInitalState(); //the current state is the inital state of the FST
            Console.WriteLine("{1} The Inital State is {0}", currentState, DateTime.Now.ToString());

            do
            {
                //ask the user for input
                Console.WriteLine("");
                Console.WriteLine("[{0}] Awaiting user input", DateTime.Now.ToString());

                Console.Write("The user input is: ");
                input = Console.ReadKey();
                Console.WriteLine(" at {0}", DateTime.Now.ToString());

                //state transition action if there is a state transition
                Task2FST.GetAction(currentState, input.KeyChar.ToString())();

                //update the current state of FSM
                currentState = Task2FST.GetNextState(currentState, input.KeyChar.ToString());

                if (input.KeyChar == 'q') 
                {
                    Console.WriteLine("[{0}] Finite State Machine has quit", DateTime.Now.ToString());
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("{1} The Current State is {0}", currentState, DateTime.Now.ToString());
                }
                

            } while (input.KeyChar != 'q'); //q is reserve to quit the program

        }

        //create the finite state table for Task 2 of the assignment, and return the instance of the FST
        static FiniteStateTable CreatFST_Task2()
        {
            Console.WriteLine("[{0}] Creating FST for Task2", DateTime.Now.ToString());

            //states and event of the FST
            string[] Inputs = { "a", "b", "c", "q" };
            string[] States = { "S0", "S1", "S2" };

            FiniteStateTable obj = new FiniteStateTable(Inputs, States, "S0");
            TransitionAction transition = new TransitionAction();

            //creat each cell of a FST
            obj.SetNextState("S0", "a", "S1");
            Action S0_a = (Action)transition.Action_X + (Action)transition.Action_Y;
            obj.SetAction("S0", "a", S0_a);

            obj.SetNextState("S1", "a", "S0");
            Action S1_a = transition.Action_W;
            obj.SetAction("S1", "a", S1_a);

            obj.SetNextState("S2", "a", "S0");
            Action S2_a = transition.Action_W;
            obj.SetAction("S2", "a", S2_a);

            obj.SetNextState("S1", "b", "S2");
            Action S1_b = (Action)transition.Action_X + (Action)transition.Action_Z;
            obj.SetAction("S1", "b", S1_b);

            obj.SetNextState("S2", "c", "S1");
            Action S2_c = (Action)transition.Action_X + (Action)transition.Action_Y;
            obj.SetAction("S2", "c", S2_c);

            return obj;
        }

        //Write a array of string to a text file, will ask the user for the directory and the name of the file
        //arugument:
        //consoleLog: an array of string
        static void WriteToFile(string[] consoleLog)
        {
            string currentDir = Directory.GetCurrentDirectory();

            string fileName, dirName = "";

            //Ask the user to enter a valid directory name
            bool isDirNameValid = false;
            while (!isDirNameValid)
            {
                try
                {
                    //ask if the user want to use default path or a input a path
                    Console.WriteLine("Please enter a valid directory path to store the log file, Or enter NILL to use the default path");
                    dirName = Console.ReadLine();

                    //using the default directory which is where the program is running from
                    if (dirName == "NILL")
                    {
                        dirName = currentDir;
                    }

                    //check if the directory exist
                    if (Directory.Exists(dirName))
                    {
                        isDirNameValid = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception Handler: Please enter a valid directory path, or enter NILL for default path");
                }

            }

            Console.WriteLine("");

            //Ask the user to enter a valid file name
            bool isFileNameValid = false;
            while (!isFileNameValid)
            {
                try
                {
                    Console.WriteLine("please enter a valid file name");
                    fileName = Console.ReadLine();

                    //create the path for the test file
                    string filePath = dirName + @"\" + fileName + ".txt";

                    File.WriteAllLines(filePath, consoleLog);
                    Console.WriteLine("File Write successful");
                    Console.WriteLine(filePath);

                    //show the user where the file is stored

                    isFileNameValid = true;
                }
                catch (ArgumentException e)
                {
                    Console.Write($"Argument Exception Handle: Please enter a valid file, and avoid using the following charaters ");
                    Console.WriteLine(@"<>:""?|\/*");
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("some exception");
                }
                catch (Exception e)
                {
                    Console.Write($"general errror: Please enter a valid file, and avoid using the following charaters ");
                    Console.WriteLine(@"<>:""?|\/*");
                }

            }
        }
    }

    //structure contain neesccery state transition Actions for Task 2
    struct TransitionAction
    {
        //Actions delegates to be perform during state transition
        public void Action_L()
        {
            Console.WriteLine("[{0}] Action L Complete", DateTime.Now.ToString());
        }
        public void Action_J()
        {
            Console.WriteLine("[{0}] Action J Complete", DateTime.Now.ToString());
        }
        public void Action_K()
        {
            Console.WriteLine("[{0}] Action K Complete", DateTime.Now.ToString());
        }
        public void Action_X()
        {
            Console.WriteLine("[{0}] Action X Complete", DateTime.Now.ToString());
        }
        public void Action_Y()
        {
            Console.WriteLine("[{0}] Action Y Complete", DateTime.Now.ToString());
        }
        public void Action_Z()
        {
            Console.WriteLine("[{0}] Action Z Complete", DateTime.Now.ToString());
        }
        public void Action_W()
        {
            Console.WriteLine("[{0}] Action W Complete", DateTime.Now.ToString());
        }
    }

    //finite state table class, used to represent a finite state machine
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
        //return cell_FST of the Finiste state table using string inpuit
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
            int stateTransactionIndex = GetStateTransitionIndex(state);

            //account for when state transition can occur regardless of the input (eg. state based transition)
            if (GetCell(stateIndex, inputIndex).isEmpty_nextState)
            {
                if (GetCell(stateIndex, stateTransactionIndex).isTransitionCondition_state)
                {
                    return GetNextState(stateIndex, stateTransactionIndex);
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
            int stateTransactionIndex = GetStateTransitionIndex(state);

            //account for when state transition can occur regardless of the input (eg. state based transition)
            if (GetCell(stateIndex, inputIndex).isEmpty_action)
            {
                if (GetCell(stateIndex, stateTransactionIndex).isTransitionCondition_state)
                {
                    return GetAction(stateIndex, stateTransactionIndex);
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

        //Null action, default for transition that doesn't have an event doesnt associate with a transition
        private void Action_NULL()
        {
        }
    }

    //Custom TextWriter, over write the current Console.Write Function and enable logging
    //activity of the console
    class ConsoleLogger : System.IO.TextWriter
    {
        private System.IO.TextWriter oldOut;
        private StringBuilder line = new StringBuilder();
        private object locker = new object();
        private int newline;
        private List<string> Log;

        //Constructor
        //argument
        //_infoExchange: a string List to store each line of the console panel as a seperate item
        public ConsoleLogger(List<string> _Log)
        {

            oldOut = Console.Out;
            Console.SetOut(this);
            this.Log = _Log;

        }

        public override Encoding Encoding
        {
            get { return oldOut.Encoding; }
        }

        //Logging Function
        public override void Write(char value)
        {
            oldOut.Write(value);

            lock (locker)
            {
                //seperate each line on the Console by "\r\n"
                if (value == '\r') newline++;
                else if (value == '\n')
                {
                    Log.Add(line.ToString()); //store the new line
                    line.Length = newline = 0;
                }
                else
                {
                    for (; newline > 0; newline--) line.Append('\r');
                    line.Append(value); //contruct the line
                }
            }
        }
    }
}

