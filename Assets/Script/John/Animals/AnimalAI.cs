using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum AIState
{
    None,
    Idle, //recover stanima
    Wandering, // walk around
    Searching, //find resource
    Eating, // eating
    Drinking, // drinking
    Mating, // reproduce
    Flee, // run from predator
    Chase, // chase target
    Thinking, // wait for next action
    Waiting // stop for reproduce
};

[System.Serializable]
public enum SearchMode
{
    None,
    Food,
    Water,
    Mate
};


public class AnimalAI : MonoBehaviour
{
    public float baseSpeed = 8.0f;

    public AIState state { get; private set; } = AIState.Idle;
    public SearchMode searchMode { get; private set; } = SearchMode.None;
    public BaseResource targetResource;

    public BaseAnimal animal;
    private AnimalUI animalUI;
    private NavMeshAgent navAgent;
    private Animator animator;
    private Coroutine stateMachineCoroutine;

    private LayerMask resourceLayerMask, animalLayerMask, waterLayerMask, targetLayerMask;
    private float lastGatherTime = 0f;

    private void Awake()
    {
        animal = GetComponent<BaseAnimal>();
        animalUI = GetComponent<AnimalUI>();
        navAgent = GetComponent<NavMeshAgent>();

    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        navAgent.speed = animal.currentStatus.movingSpeed;
        resourceLayerMask = LayerMask.GetMask("Resource");
        animalLayerMask = LayerMask.GetMask("Animal");
        waterLayerMask = LayerMask.GetMask("Water");
        stateMachineCoroutine = StartCoroutine(StateMachine());
    }

    private void OnDisable()
    {
        if (stateMachineCoroutine != null)
        {
            StopCoroutine(stateMachineCoroutine);
        }
    }

    /// <summary>
    /// Deicide what is need to search base on current condition
    /// </summary>
    private SearchMode GetSearchMode()
    {
        float hunger = animal.currentStatus.food; //higher mean less hunger
        float thirst = animal.currentStatus.water; //higher mean less thirst
        float reproductiveUrge = animal.currentStatus.reproductiveUrge; // higher mean less horny

        if (hunger < thirst && hunger < reproductiveUrge && CanRegatherResources())
        {
            if (animal.Type == AnimalType.Deer)
            {
                targetLayerMask = resourceLayerMask;
            }
            else
            {
                targetLayerMask = animalLayerMask;
            }

            return SearchMode.Food;
        }
        else if (thirst < hunger && hunger < reproductiveUrge)
        {
            targetLayerMask = waterLayerMask;
            return SearchMode.Water;
        }
        else if (reproductiveUrge < hunger && reproductiveUrge < thirst && EcosystemManager.instance.totalPopulation < EcosystemManager.instance.maxPopulation)
        {
            if (!animal.currentStatus.isBaby)
            {
                targetLayerMask = animalLayerMask;
                return SearchMode.Mate;
            }
            return SearchMode.None;
        }
        else
        {
            return SearchMode.None;
        }
    }

    public void MoveTo(Vector3 position)
    {
        navAgent.SetDestination(position);
    }

    /// <summary>
    /// Decide What next action to do, base on current state, call when current action is done
    /// </summary>
    /// <returns></returns>
    private AIState GetNextState()
    {
        if (state == AIState.Idle) //after think go to wander and search
        {
            return AIState.Wandering;
        }
        if (state == AIState.Wandering) // walk around until find
        {
            return AIState.Wandering;
        }
        if (state == AIState.Waiting) //WAIT
        {
            return AIState.Waiting;
        }
        if (state == AIState.Searching) //move to resource
        {
            if (targetResource != null)
            {
                switch (searchMode)
                {
                    case SearchMode.Food:
                        return AIState.Eating;
                    case SearchMode.Water:
                        return AIState.Drinking;
                    case SearchMode.Mate:
                        return AIState.Mating;
                    default:
                        return AIState.Idle;
                }
            }
        }
        if (state == AIState.Eating || state == AIState.Drinking || state == AIState.Mating || state == AIState.Thinking)
        { //after current action done, back to idle for loop
            return AIState.Idle;
        }


        return AIState.None;
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (state)
            {
                case AIState.Idle: // While idle thinking what resourse should found
                    Debug.Log("Idle");
                    Idle();

                    if (targetResource != null)
                    {
                        targetResource.Reset(); //End action release the resource for other
                    }

                    if (searchMode == SearchMode.None)
                    {
                        searchMode = GetSearchMode();
                    }

                    yield return new WaitForSeconds(2f); // Idle for 2 seconds
                    state = GetNextState();
                    break;

                case AIState.Wandering: //search and walk around
                    Debug.Log("Wandering For" + searchMode.ToString());
                    Collider[] colliders = Physics.OverlapSphere(transform.position, animal.currentStatus.fov, targetLayerMask);
                    // Only male will call this, to stop a female animal

                    foreach (Collider collider in colliders)
                    {
                        if (searchMode == SearchMode.Mate)
                        {
                            if (animal.currentStatus.isMale)
                            {
                                //Debug.Log("Collider");                          
                                targetResource = collider.GetComponent<BaseResource>();
                                if (targetResource.TryMate(animal)) //call this and stop the female
                                {
                                    yield return new WaitForSeconds(0.1f);
                                    SetNextState(AIState.Searching);
                                    break;
                                }
                            }
                        }
                        else if (searchMode == SearchMode.Food || searchMode == SearchMode.Water)
                        {
                            targetResource = collider.GetComponent<BaseResource>();
                            if (animal.Type == AnimalType.Deer)
                            {
                                if (targetResource.TryGather(animal))
                                {
                                    yield return new WaitForSeconds(0.1f);
                                    SetNextState(AIState.Searching);
                                    break;
                                }
                            }
                            if (animal.Type == AnimalType.Wolf)
                            {
                                if (targetResource.TryEatAnimal(animal))
                                {
                                    yield return new WaitForSeconds(0.1f);
                                    SetNextState(AIState.Searching);
                                    break;
                                }
                            }

                        }
                    }

                    // Implement wandering behavior
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance) //done with path
                    {
                        if (searchMode == SearchMode.None)
                        {
                            yield return new WaitForSeconds(0.5f);
                            SetNextState(AIState.Idle);
                            break;
                        }

                        Vector3 point;
                        if (RandomPoint(transform.position, animal.currentStatus.fov, out point)) //pass in our centre point and radius of area
                        {

                            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                            navAgent.SetDestination(point);
                        }
                    }


                    Walking();
                    yield return new WaitForSeconds(0.1f);

                    break;

                case AIState.Searching: // move to the target resource and performe action
                    Debug.Log("Searching");
                    Walking();
                    // Implement searching behavior
                    MoveTo(targetResource.transform.position);
                    if (navAgent.remainingDistance <= navAgent.stoppingDistance) //done with path
                    {
                        yield return null;
                        state = GetNextState();
                    }
                    yield return null;

                    break;

                case AIState.Eating:
                    Debug.Log("Eating");
                    Idle();
                    if (animal.Type == AnimalType.Deer)
                    {
                        targetResource.Eat(animal);
                    }
                    else if (animal.Type == AnimalType.Wolf)
                    {
                        targetResource.EatAnimal(animal);
                    }

                    searchMode = SearchMode.None;

                    yield return new WaitForSeconds(4f); // Eat for 4 seconds
                    lastGatherTime = Time.time;
                    state = GetNextState();
                    break;

                case AIState.Drinking:
                    Debug.Log("Drinking");
                    Idle();

                    targetResource.Drink(animal);
                    searchMode = SearchMode.None;

                    yield return new WaitForSeconds(4f); // Drink for 4 seconds

                    state = GetNextState();
                    break;

                case AIState.Mating:
                    Debug.Log("Mating");
                    Idle();
                    // Implement mating behavior
                    if (animal.currentStatus.isMale)
                    {
                        targetResource.Mate(animal);
                    }
                    else
                    {
                        BaseAnimal father = GetComponent<BaseResource>().currentAnimal;
                        animal.GiveBirth(father);
                    }
                    searchMode = SearchMode.None;

                    yield return new WaitForSeconds(6f);
                    state = GetNextState();
                    break;

                case AIState.Waiting:
                    Debug.Log("Waiting");
                    Idle();
                    // Implement mating behavior
                    yield return null;
                    break;
                default:
                    break;
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void SetNextState(AIState nextState)
    {
        state = nextState;
    }

    private void Walking()
    {
        animator.SetBool("isWalking", true);
    }
    private void Idle()
    {
        animator.SetBool("isWalking", false);
    }

    public bool CanRegatherResources()
    {
        // Calculate the time elapsed since the last gather
        float elapsedTime = Time.time - lastGatherTime;

        // Check if enough time has passed for regathering
        return elapsedTime >= EcosystemManager.instance.reGatherTime;
    }
}