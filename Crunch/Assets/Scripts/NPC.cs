using System;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour, IInteractable
{
    [Header("States")] 
    [SerializeField] bool isHeavy;
    [SerializeField] private bool _isLazy;
    
    [Header("Navigation")]
    public Station CurrentStation;
    public NavMeshAgent Agent;

    [Header("States")]
    [SerializeField] private AStateNPC _overworkedState;
    [SerializeField] private AStateNPC _workingState;
    [SerializeField] private AStateNPC _underworkedState;


    [Header("Working Stress Values")]
    [Range(0, 1), SerializeField] private float WorkStressAtStart = 0.5f;
    [Range(0, 1), SerializeField] private float _screamStressBoostLazy = 0.125f;
    [Range(0, 1), SerializeField] private float _screamStressBoostHyperactif = 0.125f;
    [SerializeField] private float _stressDecrementSpeed;
    [field: SerializeField] public float OverworkedMin { get; set; } = 0.75f;
    [field: SerializeField] public float OverworkedMax { get; set; } = 0.85f;
    [field: SerializeField] public float UnderworkedMin { get; set; } = 0.05f;
    [field: SerializeField] public float UnderworkedMax { get; set; } = 0.4f;

    [Header("Efficiency")]

    [field: SerializeField] public float WorkEfficiencyRate { get; set; } = 0.4f;
    [field: SerializeField] public float FrenzyWorkEfficiencyRate { get; set; } = 0.8f;
    [field: SerializeField] public float DistanceToDestination { get; set; } = 1.5f;

    [Header("State VFXs")]
    [field: SerializeField] public ParticleSystem[] UnderworkedVFXs { get; set; }
    [field: SerializeField] public ParticleSystem[] OverworkedVFXs { get; set; }
    [field: SerializeField] public ParticleSystem[] WorkingVFXs { get; set; }


    [Header("Animations")]
    [field: SerializeField] public Animator animator { get; set; }
    [field: SerializeField] private float isWalkingAnimThreshold = 0.1f;
    public string _isWalkingParamName = "isWalking";
    public string _isWorkingParamName = "isWorking";

    [Header("UI")]
    public StressProgressBar stressProgressBar;
    public Transform TransformReferenceUI;


    [Header("Others")]
    public Renderer DEBUG_Renderer;

    public AStateNPC CurrentState { get; private set; }
    public float WorkStress { get; set; }

    public bool Heavy => isHeavy;

    public bool isHeldByPlayer, isThrown;
    public bool IsWorking { get; set; }
    public float FrenzyTime = 10;
    public float TimeCounter, OldTimer = 10;
    public bool finishFrenzy = false;

    public float TimeBeforeGettingUp = 2;
    private float timerGettingUp = -1;
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
            
            if (CurrentStation)
            {
                CurrentStation.freeStation = true;
                CurrentStation.currentNPC = null;
                CurrentStation = null;
            }
            IsWorking = false;

            if (animator != null)
            {
                animator.SetBool(_isWorkingParamName, false);
            }
            return;
        }


        if (isThrown)
        {
            // Player have throw NPC, he's flying waiting to collide with something
            return;
        }
        if (timerGettingUp != -1)
        {
            timerGettingUp -= Time.deltaTime;
            if(timerGettingUp>0f)
            {
                //NPC is on the floor waiting before getting up
                return;
            }
            else
            {
                //can get up and be active again
                timerGettingUp = -1;
            }
        }


        if (animator != null)
        {
            animator.SetBool(_isWalkingParamName, Agent.velocity.magnitude >= isWalkingAnimThreshold);
        }

        // npc is active ( not thrown, not on the ground, not held by player)
        if (CurrentState.ShouldLeaveState(this) && (CurrentState.StateCategory != EStateCategory.Overworked || TimeCounter < 0)) //Changing state
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
        Debug.Log(collision.transform.name);
        if (collision.transform.CompareTag("Player"))
            return;
        
        if (isThrown)
        {
            isThrown = false;
            Agent.enabled = true;
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            GetComponent<Rigidbody>().isKinematic = true;

            timerGettingUp = TimeBeforeGettingUp;
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
        if (_isLazy)
        {
            WorkStress = Mathf.Clamp01(WorkStress + _screamStressBoostLazy);
        }
        else
        {
            WorkStress = Mathf.Clamp01(WorkStress + _screamStressBoostHyperactif);
        }
    }

    #endregion

    #region Public Methods

    public void DEBUG_ChangeColor(Color color) // To see states changes visually for now
    {
        DEBUG_Renderer.material.color = color;
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

    public void OnThrow()
    {
        isHeldByPlayer = false;
        isThrown = true;

        timerGettingUp = -1;
    }

    #endregion


}
