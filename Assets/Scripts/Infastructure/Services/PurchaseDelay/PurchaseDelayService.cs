using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infastructure.Services.PurchaseDelay
{
    public class PurchaseDelayService : IPurchaseDelayService
    {
        private readonly List<string> _delayList = new List<string>();

        private readonly ICoroutineRunner _coroutineRunner;

        public Action<string> OnDelayStarted { get; set; }
        public Action<string> OnDelayExited { get; set; }

        public PurchaseDelayService(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void AddDelay(string uniqueId)
        {
            _delayList.Add(uniqueId);

            OnDelayStarted?.Invoke(uniqueId);

            _coroutineRunner.StartCoroutine(StartPurchaseDelayCoroutine(uniqueId));
        }

        public bool DelayIsActive(string uniqueId) =>
            _delayList.Contains(uniqueId);

        private IEnumerator StartPurchaseDelayCoroutine(string uniqueId)
        {
            yield return new WaitForSeconds(2);

            if (_delayList.Contains(uniqueId))
                _delayList.Remove(uniqueId);

            OnDelayExited?.Invoke(uniqueId);
        }
    }
}