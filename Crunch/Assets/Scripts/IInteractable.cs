public interface IInteractable
{
    bool Heavy { get; }
    void Interact();
    void OnScream();
    void OnThrow();
}
