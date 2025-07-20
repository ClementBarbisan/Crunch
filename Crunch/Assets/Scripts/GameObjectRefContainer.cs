using System;
using UnityEngine;

public class GameObjectRefContainer : MonoBehaviour
{
    [SerializeField] private GameObjectRef _refObject;
    [SerializeField] private bool _activeAtStart = true;

    public void SetRef(GameObjectRef obj)
    {
        _refObject = obj;
        _refObject.RefObject = gameObject;
    }

    private void OnEnable()
    {
        if (_refObject == null)
            return;
        _refObject.RefObject = gameObject;
        gameObject.SetActive(_activeAtStart);
    }
}
