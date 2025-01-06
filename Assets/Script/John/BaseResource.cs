using UnityEngine;

public class BaseResource : MonoBehaviour
{
    public ResourceSetting Setting;
    public float value { get; private set; }
    public bool IsDeplted => value <= 0;

    public BaseAnimal currentAnimal; //what animal is looking for this resource

    private void Awake()
    {
        value = Random.Range(Setting.startValue.min, Setting.startValue.max);
    }

    public void Eat(BaseAnimal animal) {
        if (animal != currentAnimal) {
            Debug.Log("Cannot Eat");
            return;
        }

        animal.currentStatus.food += EcosystemManager.instance.foodGatherRate;
        if (animal.currentStatus.food > animal.maxFood) {
            animal.currentStatus.food = animal.maxFood;
        }
    }

    public bool TryEatAnimal(BaseAnimal predator)
    {
        if (currentAnimal != null)
        {
            return false;
        }
        AnimalAI preyAI = GetComponent<AnimalAI>();
        if (preyAI == null)
        {
            Debug.Log("Cannot mate with a non-animal object!");
            return false;
        }
        BaseAnimal preyAnimal = GetComponent<BaseAnimal>();
        if (preyAnimal == null)
        {
            Debug.Log("Cannot mate with a non-animal object!");
            return false;
        }
        //can't eat same  type animal
        if (preyAnimal.Type == predator.Type)
        {
            return false;
        }

        //change state to chase and flee
        //femaleAI.SetNextState(AIState.Waiting);
        //femaleAI.MoveTo(femaleAI.transform.position);

        currentAnimal = predator;

        return true;
    }

    public bool EatAnimal(BaseAnimal predator)
    {
        if (predator != currentAnimal)
        {
            return false;
        }

        BaseAnimal preyAnimal = GetComponent<BaseAnimal>();
        if (preyAnimal == null)
        {
            return false;
        }

        preyAnimal.Die(DieReason.EatByOther);

        predator.currentStatus.food += EcosystemManager.instance.foodGatherRate;
        if (predator.currentStatus.food > predator.maxFood)
        {
            predator.currentStatus.food = predator.maxFood;
        }

        return true;
    }

    public void Drink(BaseAnimal animal)
    {
        if (animal != currentAnimal)
        {
            Debug.Log("Cannot Drink");
            return;
        }

        animal.currentStatus.water += EcosystemManager.instance.waterGatherRate;

        if (animal.currentStatus.water > animal.maxWater)
        {
            animal.currentStatus.water = animal.maxWater;
        }
    }

    public void Reset()
    {
        currentAnimal = null;
    }

    public bool TryGather(BaseAnimal otherAnimal)
    {
        if (currentAnimal != null || IsDeplted)
            return false;
        currentAnimal = otherAnimal;

        return true;
    }

    //call ny male animal
    public bool TryMate(BaseAnimal maleAnimal) {
        if (currentAnimal != null) {
            return false;
        }
        AnimalAI femaleAI = GetComponent<AnimalAI>();
        if (femaleAI == null) {
            Debug.Log("Cannot mate with a non-animal object!");
            return false;
        }
        BaseAnimal femaleAnimal = GetComponent<BaseAnimal>();
        if (femaleAnimal == null)
        {
            Debug.Log("Cannot mate with a non-animal object!");
            return false;
        }
        //can't reproduce with different type animal
        if (femaleAnimal.Type != maleAnimal.Type) {
            return false;
        }

        //can't reproduce with same gender
        if (femaleAnimal.currentStatus.isMale && maleAnimal.currentStatus.isMale) {
            return false;
        }

        //not looking for a mate
        if (femaleAI.searchMode != SearchMode.Mate) {
            return false;
        }

        //stop this animal from moving
        femaleAI.SetNextState(AIState.Waiting);
        femaleAI.MoveTo(femaleAI.transform.position);

        currentAnimal = maleAnimal;

        return true;
    }

    public bool Mate(BaseAnimal maleAnimal) {
        if (maleAnimal != currentAnimal) {
            return false;
        }

        BaseAnimal femaleAnimal = GetComponent<BaseAnimal>();
        if (femaleAnimal == null) {
            return false;
        }

        AnimalAI femaleAI = GetComponent<AnimalAI>();
        if (femaleAI == null)
        {
            return false;
        }

        femaleAI.SetNextState(AIState.Mating);
        femaleAI.MoveTo(femaleAI.transform.position);

        femaleAnimal.currentStatus.reproductiveUrge = femaleAnimal.animalSetting.reproductiveUrge;
        maleAnimal.currentStatus.reproductiveUrge = maleAnimal.animalSetting.reproductiveUrge;

        return true;
    }
}
