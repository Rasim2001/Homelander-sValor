using System;

namespace Infastructure.Services.PurchaseDelay
{
    public interface IPurchaseDelayService
    {
        Action<string> OnDelayStarted { get; set; }
        Action<string> OnDelayExited { get; set; }
        void AddDelay(string uniqueId);
        bool DelayIsActive(string uniqueId);
    }
}