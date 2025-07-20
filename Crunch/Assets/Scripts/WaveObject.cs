using UnityEngine;

[CreateAssetMenu(fileName = "WAveObject", menuName = "ScriptableObjects/WaveObject")]
public class WaveObject : ScriptableObject
{
    [SerializeField] private GameObjectRef _posCoffeMachine;
    [SerializeField] private GameObjectRef[] startPos;
    [SerializeField] private InteractableCoffeeMachine _coffeeMachine;
    [SerializeField] private NPC _hyperactifNPC;
    [SerializeField] private NPC _toumouNPC;
    [SerializeField] private int nbToumou;
    [SerializeField] private int nbHyperactif;
    [SerializeField] private float WaveDuration;
    [SerializeField] private float WaveScore;

    public void CleanSceneAndSpawnNewStuff()
    {
        InteractableCoffeeMachine coffeeMachine = FindFirstObjectByType<InteractableCoffeeMachine>();
        if (coffeeMachine)
        {
            coffeeMachine.transform.position = _posCoffeMachine.RefObject.transform.position;
            coffeeMachine.transform.forward = _posCoffeMachine.RefObject.transform.forward;
        }
        else
        {
            Instantiate(_coffeeMachine.gameObject, _posCoffeMachine.RefObject.transform.position,
                _posCoffeMachine.RefObject.transform.rotation); 
        }
        NPC[] allNPCS = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        for (int i = 0; i < allNPCS.Length; i++)
        {
            Destroy(allNPCS[i].gameObject);
        }
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player.transform.position = Vector3.zero;
        GameObjectRef[] starts = startPos;
        for (int i = 0; i < nbToumou; i++)
        {
            Instantiate(_toumouNPC, starts[i % starts.Length].RefObject.transform.position + new Vector3(Random.Range(-1, 1), 0,
                Random.Range(-1, 1)), Quaternion.identity);
        }
        for (int i = 0; i < nbHyperactif; i++)
        {
            Instantiate(_hyperactifNPC, starts[i % starts.Length].RefObject.transform.position + new Vector3(Random.Range(-1, 1),
                0,
                Random.Range(-1, 1)), Quaternion.identity);
        }

        GameManager.Instance.waveDuration = WaveDuration;
        GameManager.Instance.waveGoalScore = WaveScore;
    }
}
