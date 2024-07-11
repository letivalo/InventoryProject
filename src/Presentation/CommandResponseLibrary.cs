namespace InventoryProject.IOLayer
{
    public class CommandValue
    {
        public Action<IInventoryManager> CommandMethod { get; }
        public string Description { get; }

        public CommandValue(Action<IInventoryManager> commandMethod, string description)
        {
            CommandMethod = commandMethod;
            Description = description;
        }

        public static readonly Dictionary<string, CommandValue> Commands = new()
        {
            { "help", new CommandValue(CommandHandler.PrintCommandList, "Displays the list of available commands.") },
            { "refreshInventory", new CommandValue(im => im.FetchInventory(), "Displays the current inventory.") },
            { "item-create", new CommandValue(im => im.QueryCreateItem(), "Creates a new item in the inventory.") },
            { "item-locate", new CommandValue(im => im.QueryLocateItem(), "Locates an item in the inventory by SKU.") },
            { "item-delete", new CommandValue(im => im.QueryDeleteItem(), "Deletes an item from the inventory by SKU.") },
            { "item-replace", new CommandValue(im => im.QueryReplaceItem(), "Replaces the details of an item in the inventory.") },
            { "clear", new CommandValue(_ => Console.Clear(), "Clears the console screen.") },
            { "exit", new CommandValue(_ => Program.Exit(), "Exits the program.") }
        };
    }

    public class OperationStateIs
    {
        public enum TellUser
        {
            Clear,
            Default,
            opSuccess,
            opError,
            // Query Responses
            askSku,
            askName,
            askQty,
            askSkuLocate,
            // Replacement Query Responses
            askNewSku,
            askNewName,
            askNewQty,
            askSkuDelete,
            // Invalid Statement Responses
            invalidSku,
            invalidQty,
            InvalidName,
            // Data Persistence Responses
            inventoryEmpty,
            invalidInput
        }
        public static readonly Dictionary<TellUser, string> responses = new Dictionary<TellUser, string>
    {
        {TellUser.Clear,"Console Cleared Successully!"},
        {TellUser.Default, "Enter a command:"},
        {TellUser.opSuccess, "Operation Completed Successfully."},
        {TellUser.opError, "Invalid Operation"},
        // Query Responses
        {TellUser.askSku, "Enter SKU:"},
        {TellUser.askName, "Enter Name:"},
        {TellUser.askQty, "Enter Quantity:"},
        {TellUser.askSkuLocate, ""},
        // Replacement Query Responses
        {TellUser.askNewSku, "Enter SKU to replace:"},
        {TellUser.askNewName, "Enter New Name for Item"},
        {TellUser.askNewQty, "Enter New Qty of Item"},
        {TellUser.askSkuDelete, "Enter SKU to delete:"},
        // Invalid Statement Responses
        {TellUser.invalidInput, "Invalid Input."},
        {TellUser.invalidSku, "Invalid SKU or item already exists. Please try again:"},
        {TellUser.invalidQty, "Invalid input. Please enter a numeric quantity:"},
        {TellUser.inventoryEmpty, "The inventory is currently empty, please load an existing inventory file or create a new one."}
    };
    }
}