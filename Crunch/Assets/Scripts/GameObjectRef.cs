using UnityEngine;

[CreateAssetMenu(fileName = "RefObject", menuName = "ScriptableObjects/GameObjectRef")]
public class GameObjectRef : ScriptableObject
{
    private GameObject _refObject;
    
    public GameObject RefObject
    {
        get => _refObject;
        set => _refObject = value;
    }

    public bool Exists {
        get
        {
            if (_refObject)
                return (true);
            return (false);
        }
    }
}
