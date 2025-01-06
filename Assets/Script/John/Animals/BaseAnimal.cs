using UnityEngine;

public enum AnimalType
{
    Deer,
    Wolf
};

public enum DieReason
{
    OldAge,
    Hunger,
    Thirst,
    EatByOther
};



public class BaseAnimal : MonoBehaviour
{
    private AnimalType type;

    public AnimalType Type {
        get { return type; }
        set { type = value; }
    }

    public AnimalSetting animalSetting;
    public Genes currentStatus;

    //public UIManager animalUI;
    [HideInInspector] public float timeToDie;
    [HideInInspector] public float maxFood;
    [HideInInspector] public float maxWater;

    private void Awake()
    {        
        //animalUI = GetComponentInChildren<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //animalUI.UpdataUI();
        UpdateState();  
    }

    public void Spawn(bool fromStart, BaseAnimal father =null, BaseAnimal mother=null)
    {   

        if (fromStart)
        {
            animalSetting.genes = AnimalSetting.RandomGenes(animalSetting);
        }
        else {            
            animalSetting.genes = AnimalSetting.InheritedGenes(mother, father);
        }      
        CopyStateFromSetting();

        // Get the SkinnedMeshRenderer component of the specific object
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        // Create a new material instance based on the shared material
        Material[] originalMaterials = skinnedMeshRenderer.sharedMaterials;
        Material[] newMaterials = new Material[originalMaterials.Length];
        int indexTOChange = 0;
        if (Type == AnimalType.Deer)
        {
            indexTOChange = 1;
        }

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            // Create a new material instance for each material in the array
            newMaterials[i] = new Material(originalMaterials[i]);

            // Modify the new material instance as needed (e.g., change its color)
            if (i == indexTOChange)
            {
                newMaterials[i].color = currentStatus.mainMaterialColor;
            }
        }
        // Assign the new material instances to the renderer of the specific object
        skinnedMeshRenderer.materials = newMaterials;


        timeToDie = currentStatus.age;
        maxFood = currentStatus.food;
        maxWater = currentStatus.water;
        currentStatus.age = 0f;

    }

    public void GiveBirth(BaseAnimal father) {
        BaseAnimal baby = EcosystemManager.instance.SpawnAnimal(Type, transform.position,false, father, this);
    }

    public void Die(DieReason reason) {
        EcosystemManager.instance.KillAnimal(this);
        Debug.Log($"{gameObject.name} died of {reason}");
    }

    private void UpdateState() {
       
        //Furture Plan: Modify size, hp base on food and water   
        if (currentStatus.isBaby) {
            float scaleSize = Mathf.Lerp(0.2f, currentStatus.size, currentStatus.age / 18f);

            transform.localScale = Vector3.one * scaleSize;
        }

        //Increase Age
        currentStatus.age += Time.deltaTime * EcosystemManager.instance.ageSpeed;
        if (currentStatus.age >= timeToDie) {
            Die(DieReason.OldAge);
            return;
        }

        //TODO: scale size base on age

        //Increase Hunger       
        currentStatus.food -= Time.deltaTime * EcosystemManager.instance.hungerSpeed;

        //Increae Thirst
        currentStatus.water -= Time.deltaTime * EcosystemManager.instance.thirstSpeed;

        if (currentStatus.isBaby)
        {
            currentStatus.reproductiveUrge = animalSetting.reproductiveUrge;
        }
        else {
            //Increase reproductiveUrge
            currentStatus.reproductiveUrge -= Time.deltaTime * EcosystemManager.instance.reproductiveUrgeSpeed;
        }

        if (currentStatus.food <= 0) {
            Die(DieReason.Hunger);
        }
        if (currentStatus.water <= 0) {
            Die(DieReason.Thirst);
        }
    }

    private void CopyStateFromSetting() {
        currentStatus = new Genes
        {
            isMale = animalSetting.genes.isMale,
            isBaby = animalSetting.genes.isBaby,
            age = animalSetting.genes.age,
            staminas = animalSetting.genes.staminas,
            movingSpeed = animalSetting.genes.movingSpeed,
            runningSpeed = animalSetting.genes.runningSpeed,
            food = animalSetting.genes.food,
            water = animalSetting.genes.water,
            fov = animalSetting.genes.fov,
            reproductiveUrge = animalSetting.genes.reproductiveUrge,
            size = animalSetting.genes.size,
            mainMaterialColor = animalSetting.genes.mainMaterialColor
        };
    }

}
