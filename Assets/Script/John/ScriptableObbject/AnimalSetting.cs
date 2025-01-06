using static System.Math;
using UnityEngine;

public class Genes
{
    public bool isMale;
    public bool isBaby;
    public float age;
    public float staminas;
    public float movingSpeed;
    public float runningSpeed;
    public float food;
    public float water;
    public float fov;
    public float reproductiveUrge;
    public float size;
    public Color mainMaterialColor;
}

//For the range of random value
[System.Serializable]
public class AttributeRange
{
    [Tooltip("Minimum value for this attribute.")]
    public float min;
    [Tooltip("Maximum value for this attribute.")]
    public float max;

}

[CreateAssetMenu(fileName = "New Genes Setting", menuName = "Ecosystem/Genes Setting")]
public class AnimalSetting : ScriptableObject
{
    static readonly System.Random prng = new System.Random();

    [Header("Start Attribute Ranges")]
    [Tooltip("How long the animal can live")]
    public AttributeRange ageRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The staminas of animal for running")]
    public AttributeRange staminasRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The base moving speed of the animal")]
    public AttributeRange movingSpeedRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The running speed of animal (It should higher than the movingSpeedRange max)")]
    public AttributeRange runningSpeedRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The hunger of the animal, will decrease slowly and recoverd when eating")]
    public AttributeRange foodRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The thirst of the animal, will decrease slowly and recoverd when drinking")]
    public AttributeRange waterRange = new AttributeRange { min = 0f, max = 100f };
    [Tooltip("The radius of view(FOV) use to searching resource ")]
    public AttributeRange viewRadius = new AttributeRange { min = 1f, max = 10f };

    [Header("Mutation Setting")]
    public float mutationChance = 0.2f;
    public float maxMutationAmount = 0.3f;

    [Header("Size Setting")]
    [Range(1.0f, 2.0f)]
    public float size = 1.0f;

    [Header("Material Setting")]
    public Color mainMaterialColor;

    [Header("Reproductive Urge (Fixed)")]
    public float reproductiveUrge = 100f;

    [HideInInspector] public Genes genes;
   
    //For change in UI should be change in scriptable object
    private void Awake()
    {
        if (EcosystemManager.instance != null) {
            mutationChance = EcosystemManager.instance.mutationChance;
        }
    }

    public static Genes RandomGenes(AnimalSetting setting)
    {
        Genes gene = new Genes();
        gene.isMale = RandomValue() < 0.5f;
        gene.isBaby = false;
        gene.age = Random.Range(setting.ageRange.min, setting.ageRange.max);
        gene.staminas = Random.Range(setting.staminasRange.min, setting.staminasRange.max);
        gene.movingSpeed = Random.Range(setting.movingSpeedRange.min, setting.movingSpeedRange.max);
        gene.runningSpeed = Random.Range(setting.runningSpeedRange.min, setting.runningSpeedRange.max);
        gene.food = Random.Range(setting.foodRange.min, setting.foodRange.max);
        gene.water = Random.Range(setting.waterRange.min, setting.waterRange.max);
        gene.fov = Random.Range(setting.viewRadius.min, setting.viewRadius.max);
        gene.size = setting.size;
        gene.mainMaterialColor = setting.mainMaterialColor;
        gene.reproductiveUrge = setting.reproductiveUrge;
        return gene;
    }

    public static Genes InheritedGenes(BaseAnimal mother, BaseAnimal father)
    {
        Genes genes = new Genes();

        //Create a new gene (Crossover)
        genes.isMale = RandomValue() < 0.5f;
        genes.isBaby = true;

        genes.age = (RandomValue() < 0.5) ? mother.timeToDie : father.timeToDie;
        genes.staminas = (RandomValue() < 0.5) ? mother.currentStatus.staminas : father.currentStatus.staminas;
        genes.movingSpeed = (RandomValue() < 0.5) ? mother.currentStatus.movingSpeed : father.currentStatus.movingSpeed;
        genes.runningSpeed = (RandomValue() < 0.5) ? mother.currentStatus.runningSpeed : father.currentStatus.runningSpeed;
        genes.food = (RandomValue() < 0.5) ? mother.maxFood : father.maxFood;
        genes.water = (RandomValue() < 0.5) ? mother.maxWater : father.maxWater;
        genes.fov = (RandomValue() < 0.5) ? mother.currentStatus.fov : father.currentStatus.fov;
        genes.size = (RandomValue() < 0.5) ? mother.currentStatus.size : father.currentStatus.size;
        genes.mainMaterialColor = (RandomValue() < 0.5) ? mother.currentStatus.mainMaterialColor : father.currentStatus.mainMaterialColor;
        genes.reproductiveUrge = mother.animalSetting.reproductiveUrge;
        // Mutation
        if (RandomValue() < mother.animalSetting.mutationChance) {
            float mutationAmount = RandomPosNegValue() * mother.animalSetting.maxMutationAmount;
            genes.age += mutationAmount;
            genes.staminas += mutationAmount;
            genes.movingSpeed += mutationAmount;
            genes.runningSpeed += mutationAmount;
            genes.food += mutationAmount;
            genes.water += mutationAmount;
            genes.size += mutationAmount;
            genes.mainMaterialColor = Random.ColorHSV(); ;
            Debug.Log("Mutated");
        }
        //Debug.Log("Child age" + genes.age.ToString() + " Mom age" + mother.timeToDie.ToString() + " Dad age" + father.timeToDie.ToString() + "Child fov" + genes.fov.ToString() + " Mom age" + mother.currentStatus.fov.ToString() + " Dad age" + father.currentStatus.fov.ToString());
        return genes;
    }

    static float RandomValue()
    {
        return (float)prng.NextDouble();
    }

    public static float RandomValue(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
    public static float RandomPosNegValue()
    {
        return Random.Range(-1, 1);
    }
}
