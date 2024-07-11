using InventoryProject.BuisnessLogic;
using InventoryProject.IOLayer;


namespace InventoryProject
{
    public class Program
    {
        private static bool ContinueRunning = true;
        public static void Main()
        {
            IInventoryManager inventoryManager = new InventoryManager();
            CommandHandler commandHandler = new CommandHandler(inventoryManager);

            while (ContinueRunning == true)
            {
                commandHandler.ProcessCommand();
            }
            if (ContinueRunning == false)
            {
                Environment.Exit(0);
                // Impliment code to exit code with value greater than 0 if exception is caught
            }
        }
        public static void Exit()
        {
            ContinueRunning = false;
        }
    }
}
