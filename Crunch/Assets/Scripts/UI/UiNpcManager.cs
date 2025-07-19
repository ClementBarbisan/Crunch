using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiNpcManager : MonoBehaviour
{
    [SerializeField] private StressProgressBar prefabStressProgressBar;
    private Dictionary<int,StressProgressBar> stressBarDict;

    public static UiNpcManager Instance;
    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("singleton UiNpcManager is already instantiated");
            Destroy(gameObject);
        }
    }
    public StressProgressBar RegisterNewNpc(NPC newNpc)
    {
        StressProgressBar newStressBar = Instantiate(prefabStressProgressBar, this.transform);
        return newStressBar;
    }

    private void OnGUI()
    {
        
    }
}
