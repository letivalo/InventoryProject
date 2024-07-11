using InventoryProject.BuisnessLogic;
using InventoryProject.IOLayer;
public interface IInventoryManager
{
    event Action<OperationStateIs.TellUser>? OperationStep;
    bool? SuccessFlag { get; }
    void FetchInventory();
    void QueryCreateItem();
    void QueryLocateItem();
    void QueryDeleteItem();
    void QueryReplaceItem();
    void AssignResponse(string response);
    Dictionary<string, InventoryItem> Inventory { get; }
}

