using InventoryProject.BuisnessLogic;

namespace InventoryProject.IOLayer
{
    public static class InventoryDisplay
    {
        public static void DisplayInventory(Dictionary<string, InventoryItem> inventory)
        {
            // Initial column width setup
            int maxSkuLength = 9; // Based on SKU format
            int quantityHeaderLength = "Quantity".Length;
            int maxQuantityLength = inventory.Values.Max(item => item.Quantity.ToString().Length);
            maxQuantityLength = Math.Max(maxQuantityLength, quantityHeaderLength);

            // Find the maximum length of item names and calculate initial widths
            int maxNameLength = inventory.Values.Max(item => item.Name?.Length ?? 0);
            int skuColumnWidth = maxSkuLength;
            int nameColumnWidth = maxNameLength;
            int quantityColumnWidth = maxQuantityLength;

            // Calculate the total width for the table
            int visualWidthBuffer = 4; //An additional buffer referenced by all proceeding width calculations
            int totalTableWidth = skuColumnWidth + nameColumnWidth + quantityColumnWidth + visualWidthBuffer;

            string inventoryCountLine = $"{inventory.Count} Items Currently in Inventory:";
            int minimumSeparatorLength = inventoryCountLine.Length + visualWidthBuffer;

            // Adjust table width based on the longest line
            totalTableWidth = Math.Max(totalTableWidth, minimumSeparatorLength);

            // Adjust name column width for centering if names are short
            if (totalTableWidth > (skuColumnWidth + maxNameLength + quantityColumnWidth + visualWidthBuffer))
            {
                nameColumnWidth = totalTableWidth - skuColumnWidth - quantityColumnWidth - visualWidthBuffer;
            }

            string tableSeparator = new string('-', totalTableWidth);

            Console.Clear();
            Console.WriteLine(inventoryCountLine);
            Console.WriteLine(tableSeparator);

            // Print headers
            string headerFormat = "{0,-" + skuColumnWidth + "} {1,-" + nameColumnWidth + "} {2,-" + quantityColumnWidth + "}";
            Console.WriteLine(headerFormat, "SKU", "Name", "Quantity");
            Console.WriteLine(tableSeparator);

            foreach (var item in inventory)
            {
                // Print each item row
                Console.WriteLine(headerFormat, item.Key, item.Value.Name, item.Value.Quantity);
            }
            Console.WriteLine(tableSeparator);
        }
    }
}

