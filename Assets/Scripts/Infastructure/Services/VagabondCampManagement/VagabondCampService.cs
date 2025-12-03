using System;
using System.Collections.Generic;
using System.Linq;
using Units.Vagabond;
using UnityEngine;

namespace Infastructure.Services.VagabondCampManagement
{
    public class VagabondCampService : IVagabondCampService
    {
        private readonly List<VagabondCamp> _rightSideVagabondCamp = new List<VagabondCamp>();
        private readonly List<VagabondCamp> _leftSideVagabondCamp = new List<VagabondCamp>();

        public void AddCamp(VagabondCamp vagabondCamp)
        {
            if (vagabondCamp == null)
                return;

            if (vagabondCamp.transform.position.x > 0)
                _rightSideVagabondCamp.Add(vagabondCamp);
            else
                _leftSideVagabondCamp.Add(vagabondCamp);
        }

        public void RemoveCamp(VagabondCamp vagabondCamp)
        {
            if (_rightSideVagabondCamp.Contains(vagabondCamp))
                _rightSideVagabondCamp.Remove(vagabondCamp);

            if (_leftSideVagabondCamp.Contains(vagabondCamp))
                _leftSideVagabondCamp.Remove(vagabondCamp);

            vagabondCamp.DestroyCamp();
        }

        public void CleanUp()
        {
            _rightSideVagabondCamp.Clear();
            _leftSideVagabondCamp.Clear();
        }


        public Vector3 GetClosestVagabondCampPosition(bool isRight, float positionX)
        {
            return isRight
                ? GetClosestCamp(_rightSideVagabondCamp, positionX).transform.position
                : GetClosestCamp(_leftSideVagabondCamp, positionX).transform.position;
        }

        public VagabondCamp GetClosestVagabondCamp(bool isRight, float positionX)
        {
            return isRight
                ? GetClosestCamp(_rightSideVagabondCamp, positionX)
                : GetClosestCamp(_leftSideVagabondCamp, positionX);
        }

        private VagabondCamp GetClosestCamp(List<VagabondCamp> currentVagabondCamps, float positionX)
        {
            if (currentVagabondCamps == null || currentVagabondCamps.Count == 0)
                return null;

            return currentVagabondCamps
                .OrderBy(camp => Mathf.Abs(camp.transform.position.x - positionX))
                .FirstOrDefault();
        }
    }
}