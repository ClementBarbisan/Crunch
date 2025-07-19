using System;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{

    [Header("Navigation")]
    public Station currentStation;
    public NavMeshAgent agent;

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


    public AStateNPC CurrentState { get; private set; }
    public float WorkStress { get; private set; }
    private bool _isHeldByPlayer;


    #region Unity Events

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        InitNpc();
    }

    void Update()
    {
        if (_isHeldByPlayer)
        {
            //TODO: held by player logic here

            return;
        }
        if (CurrentState.ShouldLeaveState(this)) // Changing state
        {
            CurrentState.OnLeaveState(this);


            if (CurrentState.StateCategory == EStateCategory.Working)
            {
                if(WorkStress >= OverworkedMax)
                {
                    CurrentState = _overworkedState;
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
            CurrentState.OnUpdateState(this);
            WorkStress = Mathf.Clamp01(WorkStress - _stressDecrementSpeed*Time.deltaTime);
        }

        //Debug.Log("stress "+WorkStress);
    }

    #endregion

    #region Private Methods
    private void InitNpc()
    {
        //could be random later
        WorkStress = WorkStressAtStart;
        CurrentState = _workingState;

        CurrentState.OnEnterState(this);

    }
    #endregion

    #region Public Methods

    public void DEBUG_ChangeColor(Color color) // To see states changes visually for now
    {
        GetComponent<Renderer>().material.color = color;
    }
    #endregion

    #region Events Callbacks

    public void GetScreamedAt()
    {
        WorkStress = Mathf.Clamp01(WorkStress + _screamStressBoost * Time.deltaTime);
    }

    #endregion


}
