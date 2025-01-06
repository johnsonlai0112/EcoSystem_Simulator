using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GenerateSetting
{
    [Tooltip("Maximum value for this attribute.")]
    public AnimalType animalType;
    [Tooltip("Minimum value for this attribute.")]
    public float numberToGenerate;
}

[System.Serializable]
public class ResourceConfig
{
    [Tooltip("Maximum value for this attribute.")]
    public ResourceType resourceType;
    [Tooltip("Maximum value for this attribute.")]
    public FoodType foodType;
    [Tooltip("Minimum value for this attribute.")]
    public GameObject resourceObject;
}

public class EcosystemManager : MonoBehaviour
{
    public static EcosystemManager instance { get; private set; }

    public GameObject BaseAnimalPrefab;
    [HideInInspector] public GameObject animalParent;

    private Dictionary<AnimalType, int> animalCounts = new Dictionary<AnimalType, int>();
    private Dictionary<AnimalType, int> reproducedCounts = new Dictionary<AnimalType, int>();
    // Calculate total population count
    public int highestPopulation = 0;
    public int totalPopulation = 0;
    public int totalReproducedCount = 0;
    public List<GenerateSetting> generateSetting = new List<GenerateSetting>();

    public bool showGUI = true;

    [Header("Population Setting")]
    public float maxPopulation = 100f;

    [Header("Attribute Controller")]
    public float ageSpeed = 0.125f;
    public float staminaRLossRate = 1.0f;
    public float hungerSpeed = 0.15f;
    public float thirstSpeed = 0.125f;
    public float reproductiveUrgeSpeed = 0.25f;
    [HideInInspector] public float mutationChance = 0.2f;

    [Header("Resource Setting")]
    public float foodGatherRate = 10f;
    public float waterGatherRate = 10f;
    public float animalGatherRate = 30f;
    public float reGatherTime = 10f;

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (SliderController.instance != null) {
            CopySetting();
        }
        SpawnAnimals();
    }

    public BaseAnimal SpawnAnimal(AnimalType typeToSpawn, Vector3 spawnPosition, bool fromStart = false, BaseAnimal father = null, BaseAnimal mother = null) {
        // create base animal
        GameObject animalGameObject = Instantiate(BaseAnimalPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), animalParent.transform);

        //setup animal type and scriptable object
        BaseAnimal animal = animalGameObject.GetComponent<BaseAnimal>();
        animal.animalSetting = Resources.Load<AnimalSetting>("Genes/" + typeToSpawn.ToString());
        animal.Type = typeToSpawn;

        //setup model and animator
        GameObject animalPrefab = Resources.Load<GameObject>("Prefabs/" + typeToSpawn.ToString());
        GameObject animalObject = Instantiate(animalPrefab, animalGameObject.transform);
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("Animator/" + typeToSpawn.ToString());
        if (controller != null) {
            animalObject.GetComponent<Animator>().runtimeAnimatorController = controller;
        }
        animalObject.name = "Model";
        //keep count for naming convertion
        animalCounts[typeToSpawn] = animalCounts.ContainsKey(typeToSpawn) ? animalCounts[typeToSpawn] + 1 : 1;
        animalGameObject.name = typeToSpawn.ToString() + "_" + animalCounts[typeToSpawn].ToString();

        //start spawn function
        if (fromStart)
        {
            animal.Spawn(fromStart);
        }
        else {
            animal.Spawn(fromStart, father, mother);
            reproducedCounts[typeToSpawn] = reproducedCounts.ContainsKey(typeToSpawn) ? reproducedCounts[typeToSpawn] + 1 : 1;
            totalReproducedCount += 1;
        }

        totalPopulation += 1;

        return animal;
    }

    public void KillAnimal(BaseAnimal animal) {
        Destroy(animal.gameObject);
        totalPopulation -= 1;
    }

    private AnimalSpawnZone GetSpawnZone(AnimalType type) {
        var allZones = Object.FindObjectsByType(typeof(AnimalSpawnZone), FindObjectsSortMode.None);
        foreach (AnimalSpawnZone zone in allZones) {
            if (zone.animalType == type)
            {
                return zone;
            }
        }

        return null;
    }

    private void SpawnAnimals() {
        animalParent = new GameObject("Animals");


        for (int i = 0; i < generateSetting.Count; i++) {
            if (generateSetting[i].numberToGenerate != 0)
            {
                AnimalSpawnZone zone = GetSpawnZone(generateSetting[i].animalType);
                Vector3 spawnPosition = new Vector3();
                for (int j = 0; j < generateSetting[i].numberToGenerate; j++)
                {
                    spawnPosition = zone.GetRandomPosition();
                    SpawnAnimal(generateSetting[i].animalType, spawnPosition, true);
                }
            }        
        }
    }

    // Define the minimum and maximum time scales
    public float minTimeScale = 0.1f;
    public float maxTimeScale = 10f;

    // Define the current time scale
    public float currentTimeScale = 1f;

    private void OnGUI()
    {
        if (showGUI) {
            // Define GUI style for the labels
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;

            if (totalPopulation > highestPopulation)
            {
                highestPopulation = totalPopulation;
            }

            GUI.backgroundColor = new Color(0, 0, 0, 0.5f);
            GUI.Box(new Rect(0, 0, 410, 150), GUIContent.none);
            // Display population count and reproduced count
            GUI.Label(new Rect(10, 10, 400, 30), "Highest Population In History: " + highestPopulation.ToString(), style);
            GUI.Label(new Rect(10, 40, 400, 30), "Current Population: " + totalPopulation.ToString(), style);
            GUI.Label(new Rect(10, 70, 400, 30), "Reproduced Count: " + totalReproducedCount.ToString(), style);
            // Create a horizontal slider for adjusting the time scale
            currentTimeScale = GUI.HorizontalSlider(new Rect(10, 100, 300, 20), currentTimeScale, minTimeScale, maxTimeScale);
            // Apply the new time scale
            Time.timeScale = currentTimeScale;
            GUI.Label(new Rect(10, 130, 400, 30), "Time Scale: " + currentTimeScale.ToString("0.00"));
        }
        
    }

    private void CopySetting() {
        maxPopulation = SliderController.instance.sliders[0].value;
        ageSpeed = SliderController.instance.sliders[1].value;
        hungerSpeed = SliderController.instance.sliders[2].value;
        thirstSpeed = SliderController.instance.sliders[3].value;
        reproductiveUrgeSpeed = SliderController.instance.sliders[4].value;
        foodGatherRate = SliderController.instance.sliders[5].value;
        waterGatherRate = SliderController.instance.sliders[6].value;
        animalGatherRate = SliderController.instance.sliders[7].value;
        reGatherTime = SliderController.instance.sliders[8].value;
        mutationChance = SliderController.instance.sliders[9].value;

        for (int i = 0; i < generateSetting.Count; i++) {
            if (generateSetting[i].animalType == AnimalType.Deer)
            {
                generateSetting[i].numberToGenerate = SliderController.instance.sliders[10].value;
            }
            else if(generateSetting[i].animalType == AnimalType.Wolf)
            {
                generateSetting[i].numberToGenerate = SliderController.instance.sliders[11].value;
            }
        }
    }

}
