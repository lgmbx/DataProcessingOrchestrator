using Microsoft.VisualBasic;
using System;

namespace DataProcessingOrchestrator
{
    public class ProcessingData
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int OrderNumber { get; set; }

        public decimal TotalPaid { get; set; }

        public bool IsApproved { get; set; } = false;

        public bool IsOrderProcessed { get; set; } = false;

        public bool IsOrderSent { get; set; } = false;

        public DateTime LastUpdate { get; set; } = DateTime.Now;

        public ProcessingStepEnum Step { get; set; }

        public ProcessingData()
        {
                
        }

        public ProcessingData(InputOrderModel inputOrder)
        {
            ProductName = inputOrder.ProductName;
            Quantity = inputOrder.Quantity;
            UnitPrice = inputOrder.UnitPrice;
        }

        public override string ToString()
        {
            return $"Order {OrderNumber} has finished processing with the following details: " +
                $"ProductName: {ProductName}, " +
                $"Quantity: {Quantity}, " +
                $"TotalPaid: {TotalPaid}, " +
                $"IsApproved: {IsApproved}, " +
                $"IsOrderProcessed: {IsOrderProcessed}, " +
                $"IsOrderSent: {IsOrderSent}, " +
                $"Step: {Step}";
        }
    }


}
