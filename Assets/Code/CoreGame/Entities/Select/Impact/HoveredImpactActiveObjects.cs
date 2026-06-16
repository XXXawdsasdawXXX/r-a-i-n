using System;
using UnityEngine;

namespace CoreGame.Entities.Select
{
    [Serializable]
    public class HoveredImpactActiveObjects : SelectableImpact
    {
        [SerializeField] private GameObject[] _objects;
        
        
        public override void Hovered(bool isHover)
        {
            foreach (GameObject gameObject in _objects)
            {
                gameObject.SetActive(isHover);
            }
        }

        public override  void Pressed()
        {
         
        }
    }
}