namespace InventoryProject.IOLayer
{
    public class CommandHandler
    {
        private readonly IInventoryManager _inventoryManager;
        private static OperationStateIs.TellUser currentOperation = OperationStateIs.TellUser.Default;

        public CommandHandler(IInventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public bool ProcessCommand()
        {
            // Subscribe to the OperationStep event
            _inventoryManager.OperationStep += OnNextOperationStep;

            if (currentOperation == OperationStateIs.TellUser.Default)
            {
                // If the currentOperation is at the default state, prompt user for a starting command, assign user input to userInput string
                Console.WriteLine(OperationStateIs.responses[OperationStateIs.TellUser.Default]);
                string? userInput = Console.ReadLine();
                // If the userInput string is not empty and matches a defined command, invoke CommandMethod from Inventory Manager
                if (!string.IsNullOrEmpty(userInput) && CommandValue.Commands.TryGetValue(userInput, out CommandValue? commandValue))
                {
                    commandValue.CommandMethod.Invoke(_inventoryManager);
                }
                // Otherwise, clear terminal, tell user that input is invalid, request fresh starting command
                else
                {
                    {
                        Console.Clear();
                        if (OperationStateIs.responses.TryGetValue(OperationStateIs.TellUser.invalidInput, out string? invalidInputMessage))
                        {
                            Console.WriteLine(invalidInputMessage);
                        }
                        currentOperation = OperationStateIs.TellUser.Default;
                    }
                }
            }
            else
            {
                // If operation state *is not* set to default:
                if (!OperationStateIs.responses.TryGetValue(currentOperation, out string? response))
                {
                    response = OperationStateIs.responses[OperationStateIs.TellUser.Default];
                }
                // Write the response selected by the Inventory Manager to the Console
                Console.WriteLine(response);
                // If user input is not null or empty, assign it to userInput, forward userInput to Inventory Manager via AssignResponse
                string? userInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(userInput))
                {
                    _inventoryManager.AssignResponse(userInput);
                }
                else
                {
                    if (OperationStateIs.responses.TryGetValue(OperationStateIs.TellUser.invalidInput, out string? invalidInputMessage))
                    {
                        Console.WriteLine(invalidInputMessage);
                    }
                }
            }
            // Unsubscribe from OperationStep. Return ProcessCommand true otherwise program will end. Starts event loop over again.
            _inventoryManager.OperationStep -= OnNextOperationStep;
            return true;
        }

        // Event handler to proceed to the next operation step
        private void OnNextOperationStep(OperationStateIs.TellUser operation)
        {
            bool? successFlag = _inventoryManager.SuccessFlag;

            if (operation == OperationStateIs.TellUser.Clear && successFlag == null)
            {
                Console.Clear();
                currentOperation = OperationStateIs.TellUser.Default;
            }
            else if (operation == OperationStateIs.TellUser.Clear && successFlag == true)
            {
                Console.Clear();
                if (OperationStateIs.responses.TryGetValue(OperationStateIs.TellUser.opSuccess, out string? successMessage))
                {
                    Console.WriteLine(successMessage);
                }
                currentOperation = OperationStateIs.TellUser.Default;
            }
            else if (operation == OperationStateIs.TellUser.Clear && successFlag == false)
            {
                Console.Clear();
                if (OperationStateIs.responses.TryGetValue(OperationStateIs.TellUser.opError, out string? errorMessage))
                {
                    Console.WriteLine(errorMessage);
                }
                currentOperation = OperationStateIs.TellUser.Default;
            }
            else
            {
                currentOperation = operation;
            }
        }


        public static void PrintCommandList(IInventoryManager _)
        {
            Console.Clear();
            foreach (var command in CommandValue.Commands)
            {
                Console.WriteLine($"{command.Key}: {command.Value.Description}");
            }
        }
    }
}