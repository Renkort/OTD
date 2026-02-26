using System.Collections.Generic;
using UnityEngine;


namespace Akkerman.InteractionSystem
{
    public class LightIndicator : MonoBehaviour
    {
        [SerializeField] private MeshRenderer indicatorMeshRenderer;
        [SerializeField] private Light indicatorLight;
        [SerializeField] private List<IndicatorState> indicatorStates;
        [SerializeField] private IndicatorState currentState;

        void Start()
        {
            SetState(indicatorStates[0].name);
        }


        public void SetState(string stateName)
        {
            if (stateName == currentState.name)
                return;

            int index = GetStateIndexByName(stateName);
            if (index == -1)
            {
                Debug.LogError("ERROR: CAN'T FIND INDICATOR STATE");
                return;
            }
            
            currentState = indicatorStates[index];
            indicatorMeshRenderer.material = currentState.material;
            indicatorLight.color = currentState.color;

        }

        private int GetStateIndexByName(string name)
        {
            for (int i = 0; i < indicatorStates.Count; i++)
            {
                if (indicatorStates[i].name == name)
                    return i;
            }
            return -1;
        }
    }
    
    [System.Serializable]
    public struct IndicatorState
    {
        public string name;
        public Color color;
        public Material material;
    }
}
