using System.Collections.Generic;

namespace DataProcessingOrchestrator
{
    public static class ProcessingSteps
    {
        public static List<ProcessingStepEnum> Steps => new()
        {
            ProcessingStepEnum.OrderRequest,
            ProcessingStepEnum.Payment,
            ProcessingStepEnum.Approval,
            ProcessingStepEnum.ProcessOrder,
            ProcessingStepEnum.SendOrder,
        };
    }

    public enum ProcessingStepEnum
    {
       OrderRequest,
       Payment,
       Approval,
       ProcessOrder,
       SendOrder,
    }
}
