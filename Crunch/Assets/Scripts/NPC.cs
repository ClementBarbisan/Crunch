using System;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("States")] 
    [SerializeField] bool isHeavy;
    
    [Header("Navigation")]
    public Station CurrentStation;
    public NavMeshAgent Agent;

    [Header("States")]
    [SerializeField] private AStateNPC _overworkedState;
    [SerializeField] private AStateNPC _workingState;
    [SerializeField] private AStateNPC _underworkedState;


    [Header("Working Stress Values")]
    [Range(0, 1), SerializeField] private float WorkStressAtStart = 0.5f;
    [Range(0, 1), SerializeField] private float _screamStressBoost = 0.125f;
    [SerializeField] private float _stressDecrementSpeed;
    [field: SerializeField] public float OverworkedMin { get; set; } = 0.75f;
    [field: SerializeField] public float OverworkedMax { get; set; } = 0.85f;
    [field: SerializeField] public float UnderworkedMin { get; set; } = 0.05f;
    [field: SerializeField] public float UnderworkedMax { get; set; } = 0.4f;

    [Header("Efficiency")]

    [field: SerializeField] public float WorkEfficiencyRate { get; set; } = 0.4f;
    [field: SerializeField] public float DistanceToDestination { get; set; } = 1f;


    [Header("UI")]
    public StressProgressBar stressProgressBar;
    public Transform TransformReferenceUI;

    [Header("Others")]

    public AStateNPC CurrentState { get; private set; }
    public float WorkStress { get; set; }

    public bool Heavy => isHeavy;

    public bool isHeldByPlayer, isThrown;
    public bool IsWorking;
    public float FrenzyTime = 10;
    public float TimeCounter, OldTimer = 10;
    public bool finishFrenzy = false;
    #region Unity Events

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        InitNpc();
    }

    void Update()
    {
        if (isHeldByPlayer)
        {
            //TODO: held by player logic here, change animation, 
            return;
        }

        if (isThrown)
        {
            // Player have throw NPC, he's flying waiting to collide with something
            return;
        }
        if (CurrentState.ShouldLeaveState(this) || (CurrentState.StateCategory == EStateCategory.Overworked && TimeCounter > 0)) //Changing state
        {
            CurrentState.OnLeaveState(this);

            if (CurrentState.StateCategory == EStateCategory.Working)
            {
                if(WorkStress >= OverworkedMax)
                {
                    CurrentState = _overworkedState;
                    TimeCounter = FrenzyTime;
                    OldTimer = FrenzyTime;
                }
                else if(WorkStress <= UnderworkedMin)
                {
                    CurrentState = _underworkedState;
                }
            }
            else if(CurrentState.StateCategory == EStateCategory.Overworked)
            {
                CurrentState = _workingState;
            }
            else // leaving an underworked state
            {
                CurrentState = _workingState;
            }

            CurrentState.OnEnterState(this);
        }
        else // Calling state update
        {
            TimeCounter -= Time.deltaTime;
            CurrentState.OnUpdateState(this);
            WorkStress = Mathf.Clamp01(WorkStress - _stressDecrementSpeed * Time.deltaTime);
            OldTimer = TimeCounter;
        }

        //Debug.Log("stress "+WorkStress);
    }

    private void OnDestroy()
    {
        Destroy(stressProgressBar); // Remove this NPC UI before destroying itself
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
            return;
        
        if (isThrown)
        {
            isThrown = false;
            Agent.isStopped = false;
            GetComponent<Rigidbody>().isKinematic = true;
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }
    }

    #endregion

    #region Private Methods
    private void InitNpc()
    {
        //could be random later
        WorkStress = WorkStressAtStart;
        CurrentState = _workingState;

        CurrentState.OnEnterState(this);

        stressProgressBar =  UiNpcManager.Instance.RegisterNewNpc(this);
        stressProgressBar.Npc = this;
    }

    private void GetScreamedAt()
    {
        WorkStress = Mathf.Clamp01(WorkStress + _screamStressBoost);
    }

    #endregion

    #region Public Methods

    public void DEBUG_ChangeColor(Color color) // To see states changes visually for now
    {
        GetComponent<Renderer>().material.color = color;
    }
    #endregion

    #region Events Callbacks
    
    public void Interact()
    {
        
    }

    public void OnScream()
    {
        GetScreamedAt();
    }

    #endregion


}
