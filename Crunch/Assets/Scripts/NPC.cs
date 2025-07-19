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
    [Range(0, 1), SerializeField] private float _workStressAtStart = 0.5f;
    [Range(0, 1), SerializeField] private float _screamStressBoost = 0.125f;
    [SerializeField] private float _stressDecrementSpeed;
    [field: SerializeField] public float OverworkedMin { get; set; } = 0.85f;
    [field: SerializeField] public float OverworkedMax { get; set; } = 0.75f;
    [field: SerializeField] public float UnderworkedMin { get; set; } = 0.05f;
    [field: SerializeField] public float UnderworkdMax { get; set; } = 0.4f;



    public AStateNPC CurrentState { get; private set; }
    private float _workStress;
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
        if (Input.GetKeyUp(KeyCode.Escape)) //// ONLY FOR DEBUG PURPOSES
        {
            GetScreamedAt();
        }


        if (_isHeldByPlayer)
        {
            //TODO: held by player logic here

            return;
        }
        if (CurrentState.ShouldLeaveState(this)) // Changing state
        {
            if(CurrentState.StateCategory == EStateCategory.Working)
            {
                if(_workStress >= OverworkedMin)
                {
                    CurrentState = _overworkedState;
                }
                else if(_workStress <= UnderworkedMin)
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
        }
        else // Calling state update
        {
            CurrentState.OnUpdateState(this);
            _workStress = Mathf.Clamp01(_workStress - _stressDecrementSpeed*Time.deltaTime);
        }
    }

    #endregion

    #region Private Methods
    private void InitNpc()
    {
        //could be random later
        CurrentState = _workingState;

        _workStress = _workStressAtStart;
    }
    #endregion

    #region Public Methods

    #endregion

    #region Events Callbacks

    public void GetScreamedAt()
    {
        _workStress = Mathf.Clamp01(_workStress + _screamStressBoost * Time.deltaTime);
    }

    #endregion


}
