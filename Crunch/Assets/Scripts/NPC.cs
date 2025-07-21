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
    [field: SerializeField] public ParticleSystem StunVFX { get; set; }


    [Header("Animations")]
    [field: SerializeField] public Animator animator { get; set; }
    [field: SerializeField] private float isWalkingAnimThreshold = 0.1f;
    public string _isWalkingParamName = "isWalking";
    public string _isWorkingParamName = "isWorking";
    [SerializeField] private Renderer _faceRenderer;
    
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
    [SerializeField] private float _workStressValueOnThrow = 0.5f;
    [SerializeField] private AudioSource _audioSourceGrab;
    [SerializeField] private AudioSource _audioSourceThrown;
    public AudioSource _audioSourceSleeping;
    [SerializeField] private AudioSource _audioSourceWork;
    [SerializeField] private AudioSource _audioSourceFrenzy;
    #region Unity Events

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        
    }

    void Start()
    {
        InitNpc();
    }

    public void SwitchFaceRenderer(float face)
    {
        _faceRenderer.material.SetFloat("_faceNumber", face);
    }
    
    /*void SwitchWorkingVFX()
    {
        if (IsWorking)
        {
            _audioSourceWork.Play();
        }
        else
        {

            _audioSourceWork.Stop();
        }
    }*/
    
    void Update()
    {
        //SwitchWorkingVFX();
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
                StunVFX.gameObject.SetActive(false);
                //can get up and be active again
                timerGettingUp = -1;
            }
        }
        if (isHeldByPlayer)
        {
            //TODO: held by player logic here, change animation, 
            _audioSourceGrab.Play();
            if (CurrentStation)
            {
                CurrentStation.freeStation = true;
                CurrentStation.currentNPC = null;
                CurrentStation = null;
            }
            IsWorking = false;
            for (int i = 0; i < UnderworkedVFXs.Length; i++)
            {
                UnderworkedVFXs[i].Stop();
            }
            if (animator != null)
            {
                animator.SetBool(_isWorkingParamName, false);
            }
            return;
        }


        if (isThrown)
        {
            _audioSourceGrab.Stop();
            _audioSourceThrown.Play();
            // Player have throw NPC, he's flying waiting to collide with something
            return;
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
                GameManager.Instance.StatsTrauma();
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
            _audioSourceThrown.Stop();
            isThrown = false;
            Agent.enabled = false;
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            if (collision.collider.TryGetComponent(out Station station))
            {
                if (!station.freeStation)
                {
                    station.currentNPC.transform.position = transform.position;
                    station.currentNPC.IsWorking = false;

                    if (station.currentNPC.animator != null)
                        station.currentNPC.animator.SetBool(station.currentNPC._isWorkingParamName, false);
                }
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = station.transform.position + station.transform.forward*.5f;
                transform.forward = -station.transform.forward;
                station.freeStation = false;
                station.currentNPC = this;
                CurrentStation = station;
                IsWorking = true;
                WorkStress = _workStressValueOnThrow;
            }
            else
            {
                Debug.Log("Hit not station");
                StunVFX.gameObject.SetActive(true);
                timerGettingUp = TimeBeforeGettingUp;
            }
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
        GameManager.Instance.StatsScreams();
    }

    public void OnThrow()
    {
        isHeldByPlayer = false;
        isThrown = true;

        timerGettingUp = -1;
    }

    #endregion


}
