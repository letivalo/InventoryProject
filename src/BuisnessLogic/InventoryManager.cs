using InventoryProject.IOLayer;
namespace InventoryProject.BuisnessLogic
{
    public class InventoryManager : IInventoryManager
    {
        public event Action<OperationStateIs.TellUser>? OperationStep;
        private readonly Dictionary<string, InventoryItem> inventory = new();
        private string? pendingSku;
        private string? pendingName;
        private int pendingQty;
        private bool? successFlag = null;

        public List<OperationStateIs.TellUser> Questions { get; private set; } = new();
        private int currentQuestionIndex = 0;

        public bool? SuccessFlag => successFlag;

        public void FetchInventory()
        {
            // Console operations shouldn't be here, this is a band-aid solution because I'm too tired to figure out the invokation rn
            Console.Clear();
            if (inventory.Count == 0)
            {
                Console.WriteLine(OperationStateIs.responses[OperationStateIs.TellUser.inventoryEmpty]);
            }
            else
            {
                InventoryDisplay.DisplayInventory(inventory);
                OperationStep?.Invoke(OperationStateIs.TellUser.Default);
            }
        }

        public void QueryCreateItem()
        {
            Questions = new List<OperationStateIs.TellUser> { OperationStateIs.TellUser.askSku, OperationStateIs.TellUser.askName, OperationStateIs.TellUser.askQty };
            currentQuestionIndex = 0;
            OperationStep?.Invoke(Questions[currentQuestionIndex]);
        }

        public void QueryLocateItem()
        {
            Questions = new List<OperationStateIs.TellUser> { OperationStateIs.TellUser.askSkuLocate };
            currentQuestionIndex = 0;
            OperationStep?.Invoke(Questions[currentQuestionIndex]);
        }

        public void QueryDeleteItem()
        {
            Questions = new List<OperationStateIs.TellUser> { OperationStateIs.TellUser.askSkuDelete };
            currentQuestionIndex = 0;
            OperationStep?.Invoke(Questions[currentQuestionIndex]);
        }

        public void QueryReplaceItem()
        {
            Questions = new List<OperationStateIs.TellUser> { OperationStateIs.TellUser.askNewSku, OperationStateIs.TellUser.askNewName, OperationStateIs.TellUser.askNewQty };
            currentQuestionIndex = 0;
            OperationStep?.Invoke(Questions[currentQuestionIndex]);
        }

        public void AssignResponse(string? response)
        {
            if (currentQuestionIndex >= Questions.Count)
            {
                OperationStep?.Invoke(OperationStateIs.TellUser.Default);
                return;
            }

            if (string.IsNullOrEmpty(response))
            {
                OperationStep?.Invoke(OperationStateIs.TellUser.Default);
                return;
            }

            switch (Questions[currentQuestionIndex])
            {
                case OperationStateIs.TellUser.askSku:
                case OperationStateIs.TellUser.askSkuLocate:
                case OperationStateIs.TellUser.askSkuDelete:
                case OperationStateIs.TellUser.askNewSku:
                    pendingSku = response;
                    if (string.IsNullOrEmpty(pendingSku) || (Questions[currentQuestionIndex] != OperationStateIs.TellUser.askSkuLocate && Questions[currentQuestionIndex] != OperationStateIs.TellUser.askSkuDelete && inventory.ContainsKey(pendingSku)))
                    {
                        OperationStep?.Invoke(OperationStateIs.TellUser.invalidSku);
                        return;
                    }
                    break;

                case OperationStateIs.TellUser.askName:
                case OperationStateIs.TellUser.askNewName:
                    pendingName = response;
                    break;

                case OperationStateIs.TellUser.askQty:
                case OperationStateIs.TellUser.askNewQty:
                    if (!int.TryParse(response, out pendingQty))
                    {
                        OperationStep?.Invoke(OperationStateIs.TellUser.invalidQty);
                        return;
                    }
                    break;
            }

            currentQuestionIndex++;
            if (currentQuestionIndex < Questions.Count)
            {
                OperationStep?.Invoke(Questions[currentQuestionIndex]);
            }
            else
            {
                CompleteOperation();
            }
        }

        private void CompleteOperation()
        {
            bool success = false;
            if (!string.IsNullOrEmpty(pendingSku))
            {
                switch (Questions[0])
                {
                    case OperationStateIs.TellUser.askSku:
                        if (!string.IsNullOrEmpty(pendingName))
                        {
                            success = CreateItem(pendingSku, pendingName, pendingQty);
                        }
                        break;

                    case OperationStateIs.TellUser.askSkuLocate:
                        success = LocateItem(pendingSku);
                        break;

                    case OperationStateIs.TellUser.askSkuDelete:
                        success = DeleteItem(pendingSku);
                        break;

                    case OperationStateIs.TellUser.askNewSku:
                        if (!string.IsNullOrEmpty(pendingName))
                        {
                            success = ReplaceItem(pendingSku, pendingName, pendingQty);
                        }
                        break;
                    case OperationStateIs.TellUser.inventoryEmpty:
                        success = VoidOperation();
                        break;
                }
            }

            successFlag = success;
            OperationStep?.Invoke(OperationStateIs.TellUser.Clear);

            if (success)
            {
                OperationStep?.Invoke(OperationStateIs.TellUser.opSuccess);
            }
            else
            {
                OperationStep?.Invoke(OperationStateIs.TellUser.opError);
            }

            OperationStep?.Invoke(OperationStateIs.TellUser.Default);
            ResetState();
        }

        private bool CreateItem(string sku, string name, int quantity)
        {
            if (!string.IsNullOrEmpty(sku) && !inventory.ContainsKey(sku))
            {
                InventoryItem newItem = new() { SKU = sku, Name = name, Quantity = quantity };
                inventory.Add(sku, newItem);
                return true;
            }
            return false;
        }

        private bool LocateItem(string sku)
        {
            if (!string.IsNullOrEmpty(sku) && inventory.TryGetValue(sku, out InventoryItem? item))
            {
                return true;
            }
            return false;
        }

        private bool DeleteItem(string sku)
        {
            if (!string.IsNullOrEmpty(sku) && inventory.ContainsKey(sku))
            {
                inventory.Remove(sku);
                return true;
            }
            return false;
        }

        private bool ReplaceItem(string sku, string newName, int newQuantity)
        {
            if (!string.IsNullOrEmpty(sku) && inventory.TryGetValue(sku, out InventoryItem? item))
            {
                item.Name = newName;
                item.Quantity = newQuantity;
                return true;
            }
            return false;
        }

        private bool VoidOperation()
        {
            return true;
        }

        private void ResetState()
        {
            pendingSku = null;
            pendingName = null;
            successFlag = null;
            pendingQty = 0;
            Questions.Clear();
            currentQuestionIndex = 0;
        }

        public Dictionary<string, InventoryItem> Inventory => inventory;
    }

    public class InventoryItem
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
