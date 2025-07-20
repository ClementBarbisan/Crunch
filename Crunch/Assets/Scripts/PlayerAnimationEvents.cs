using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeReference] private PlayerInteractor interactor;

    public void ThrowCallback()
    {
        Debug.Log("ThrowCallback called");
        interactor.Throw();
    }
}
