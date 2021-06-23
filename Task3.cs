using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace MECHENG313_Assignment_2_Task_3
{
    class Program
    {
        //program entry point
        static void Main(string[] args)
        {

            List<string> LoggerString = new List<string>();

            var Logger = new HelperClassNMethods.ConsoleLogger(LoggerString);

            Task3();

            HelperClassNMethods.HelperFunctions.WriteToFile(LoggerString.ToArray());

        }

        //Runs the logic for the finite state machine describe in Task 3 of the assignment
        static void Task3()
        {
            //initiate variables
            HelperClassNMethods.FiniteStateTable Task3FST = HelperClassNMethods.HelperFunctions.CreateTask3FST(); //create the FST for Task 2
            ConsoleKeyInfo userInput; //store the last user input

            string currentState = Task3FST.GetInitalState(); //the current state is the inital state of the FST

            Console.WriteLine("[{1}] The Inital State is {0}", HelperClassNMethods.HelperFunctions.StateSplitter(currentState), DateTime.Now.ToString());

            do
            {
                //ask the user for input
                Console.WriteLine("");
                Console.WriteLine("[{0}] Awaiting user input", DateTime.Now.ToString());
                Console.Write("The User Input is: ");
                userInput = Console.ReadKey();
                Console.WriteLine(" at [{0}]", DateTime.Now.ToString());

                //state transition action if there is a state transition
                Task3FST.GetAction(currentState, userInput.KeyChar.ToString())();

                //update the current state of FSM 
                currentState = Task3FST.GetNextState(currentState, userInput.KeyChar.ToString());

                //prompt to quit the program
                if (userInput.KeyChar == 'q')
                {
                    Console.WriteLine("[{0}] Finite State Machine has quit", DateTime.Now.ToString());
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine("[{1}] The current state is:{0} ", HelperClassNMethods.HelperFunctions.StateSplitter(currentState), DateTime.Now.ToString());
                }
                

            } while (userInput.KeyChar != 'q'); //q will exit the loop and go into save

        }        
    }
}

//namespace to store various helper Classes and Methods
namespace HelperClassNMethods
{
    //Action delegate
    //this delegate is used to store state transition actions
    public delegate void Action();

    //structure contain necessery state transition Actions for Task 3
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

        //this method will run Action J,K,L on different threads when it's called
        //could be modified to run other Actions as well
        public void MultiThreadAction_JKL()
        {
            Action transationActions = (Action)Action_J + (Action)Action_K + (Action)Action_L;
            var ListOfToBeCompletedActions = transationActions.GetInvocationList();
            
            //multi threading 
            Thread[] multiActionThread = new Thread[ListOfToBeCompletedActions.Length];
            bool isActionsCompleted = false;

            for (int i = 0; i < ListOfToBeCompletedActions.Length; i++)
            {
                multiActionThread[i] = new Thread(actionWrapper);
                multiActionThread[i].Start(ListOfToBeCompletedActions[i]);
            }

            //ensure state transition is on hold until all actions are completed
            while (!isActionsCompleted)
            {
                isActionsCompleted = true;
                foreach (Thread action in multiActionThread)
                {
                    if (action.IsAlive)
                    {
                        isActionsCompleted = false;
                    }
                }
            }
        }

        //function wrapper to allow the delegate function to be run on different threads
        public static void actionWrapper(object _action)
        {
            Action action = (Action)_action;
            action.DynamicInvoke();
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
            public string nextState;  //next state on the FST
            public Action action;  //Actions associated with each input
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
                }
            }
            empty_FST[0, 0].isEmpty_action = true;
            empty_FST[0, 0].isEmpty_nextState = true;
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
                    return this.empty_FST[0, 0];
                }
                else
                {
                    return this.FST[inputIndex, stateIndex];
                }
            }
        }



        //set the nextState variable in cell_FST of the Finite state table using index of FST 2D array
        //arguments:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Input array
        //nextStateIndex: index of the next state in the States array
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
            }
        }
        //return the next state as a string of a cell in the FST using index of FST 2D array
        //arguments:
        //stateIndex: index of the state in States array
        //inputIndex: index of the event in Inputs array
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

            //account for when state transation can occur regardless of the input (eg. state based transation)
            if (GetCell(stateIndex, inputIndex).isEmpty_nextState)
            {
                if (!(GetCell(stateIndex, stateTransitionIndex).isEmpty_nextState))
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

        //set the action associate with a transation of a cell in FST using index of FST 2D array
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
                
            }
        }
        //return the Action delegate associated with a state transition of a cell in FST using index of FST 2D array
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
                return Action_NULL;
            }
            else
            {
                return GetCell(stateIndex, inputIndex).action;
            }
        }
        //return the Action delegate associated with a state transition of a cell in FST using string
        //argument:
        //state: state in States array, string
        //input: event in Inputs array, string
        public Action GetAction(string state, string input)
        {
            int stateIndex = GetStateIndex(state);
            int inputIndex = GetInputIndex(input);
            int stateTransitionIndex = GetStateTransitionIndex(state);

            //account for when state transation can occur regardless of the input (eg. state based transation)
            if (GetCell(stateIndex, inputIndex).isEmpty_action)
            {
                if (!(GetCell(stateIndex, stateTransitionIndex).isEmpty_action))
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
                //Console.WriteLine("setting InitalState failed, default to first element in the States array");
                this.initialState = States[0];
            }

        }
        //return the initial state of the FST
        public string GetInitalState()
        {
            return this.initialState;
        }


        //return the index of the event in the Inputs array from a event string
        //arguments:
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

            //account for substring (eg only account for two FSM, could implment more but will complicate the code)
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
                    currentState = state.Substring(firstSInd, stateLength);
                }

                //check if the state is one of the "event" that will result in a state transation
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
                //Console.WriteLine("IsValidCell method error, no such cell");
                return false;
            }

        }

        //Null action, default for transition that doesn't have an event associated with a transition
        private void Action_NULL()
        {
        }
    }

    //create finiteState Table
    static class HelperFunctions
    {
        //create a large FST for TASK3
        public static FiniteStateTable CreateTask3FST()
        {
            Console.WriteLine("[{0}] Creating FST for Task3", DateTime.Now.ToString());

            string[] Inputs = { "a", "b", "c", "S1", "q" };
            string[] States = { "S0SA", "S0SB", "S1SA", "S1SB", "S2SA", "S2SB" };

            FiniteStateTable obj = new FiniteStateTable(Inputs, States, "S0SB");
            TransitionAction transation = new TransitionAction();


            Action XY = (Action)transation.Action_X + (Action)transation.Action_Y;
            Action W = (Action)transation.Action_W;
            Action XZ = (Action)transation.Action_X + (Action)transation.Action_Z;
            Action WnJKL = (Action)W + (Action)transation.MultiThreadAction_JKL;
            Action XZnJKL = (Action)XZ + (Action)transation.MultiThreadAction_JKL;
            Action JKL = (Action)transation.MultiThreadAction_JKL;

            obj.SetNextState("S0SA", "a", "S1SB");
            obj.SetAction("S0SA", "a", XY);

            obj.SetNextState("S0SB", "a", "S1SB");
            obj.SetAction("S0SB", "a", XY);

            obj.SetNextState("S1SA", "a", "S0SB");
            obj.SetAction("S1SA", "a", W);

            obj.SetNextState("S1SA", "b", "S2SA");
            obj.SetAction("S1SA", "b", XZ);

            obj.SetNextState("S1SB", "a", "S0SA");
            obj.SetAction("S1SB", "a", WnJKL);

            obj.SetNextState("S1SB", "b", "S2SA");
            obj.SetAction("S1SB", "b", XZnJKL);

            obj.SetNextState("S1SB", "S1", "S1SA");
            obj.SetAction("S1SB", "S1", JKL);

            obj.SetNextState("S2SA", "a", "S0SB");
            obj.SetAction("S2SA", "a", W);

            obj.SetNextState("S2SA", "c", "S1SA");
            obj.SetAction("S2SA", "c", XY);

            obj.SetNextState("S2SB", "a", "S0SB");
            obj.SetAction("S2SB", "a", W);

            obj.SetNextState("S2SB", "c", "S1SB");
            obj.SetAction("S2SB", "c", XY);

            return obj;
        }

        //Write a array of string to a text file, will ask the user for the directory and the name of the file
        //arugument:
        //consoleLog: an array of string
        public static void WriteToFile(string[] consoleLog)
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
                    Console.WriteLine("Please enter a valid directory path to store the log file, Or enter NULL to use the default path");
                    dirName = Console.ReadLine();

                    //using the default directory which is where the program is running from
                    if (dirName == "NULL")
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
                    Console.WriteLine("Exception Handler: Please enter a valid directory path, or enter NULL for default path(Current Folder)");
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

                    //show the user where the file is stored
                    Console.WriteLine("File Write successful");
                    Console.WriteLine(filePath);
                    isFileNameValid = true;
                }
                catch (ArgumentException e)
                {
                    Console.Write($"Argument Exception Handle: Please enter a valid filename, and avoid using the following characters ");
                    Console.WriteLine(@"<>:""?|\/*");
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("some exception");
                }
                catch (Exception e)
                {
                    Console.Write($"general error: Please enter a valid filename, and avoid using the following characters ");
                    Console.WriteLine(@"<>:""?|\/*");
                }

            }
        }

        //return a string decribe the current state of the con-current FSMs
        //arugument:
        //currentState: a string describe the current state of the FSMs
        //              each state should start with a "S" with no space between the states
        public static string StateSplitter(string currentState)
        {
            int e = 0;
            int firstSInd, nextSInd;
            string combineStates = "";
            firstSInd = currentState.IndexOf('S');

            do
            {

                //find all the S in current State

                nextSInd = currentState.IndexOf('S', (firstSInd + 1));

                //while there are more "S" in the current state

                int stateLength = nextSInd - firstSInd;
                if (stateLength <= 0)
                {
                    combineStates += $" Machine {e} State: ";
                    combineStates += currentState.Substring(firstSInd);
                    combineStates += ",";
                    //Console.WriteLine(currentState.Substring(firstSInd));
                }
                else
                {
                    combineStates += $" Machine {e} State: ";
                    combineStates += currentState.Substring(firstSInd, stateLength);
                    combineStates += ",";

                    //Console.WriteLine(currentState.Substring(firstSInd, stateLength));
                }


                e++;
                firstSInd = nextSInd;

            } while (nextSInd != -1);

            return combineStates;

        }
    }

    //Custom TextWriter, overwrite the current Console.Write Function and enable logging
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