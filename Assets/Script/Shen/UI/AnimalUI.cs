using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalUI : MonoBehaviour
{
    public Transform cam;
    public BaseAnimal animal;
    public AnimalAI animalAI;
    public TMP_Text[] texts = new TMP_Text[6];
    public Image genderImage;
    public Sprite maleSprite;
    public Sprite femaleSprite;
    public Slider[] sliders = new Slider[3];

    // Start is called before the first frame update
    void Start()
    {
        animal = gameObject.GetComponentInParent<BaseAnimal>();
        animalAI = gameObject.GetComponentInParent<AnimalAI>();
        //0 = status, 1 = age, 2 = running, 3 = hunger, 4 = thirst, 5 = horniness
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (animal.currentStatus.isMale)
        {
            genderImage.sprite = maleSprite;
        }
        else
        {
            genderImage.sprite = femaleSprite;
        }

        texts[2].SetText(animal.currentStatus.runningSpeed.ToString());

        sliders[0].maxValue = animal.maxFood;
        sliders[1].maxValue = animal.maxWater;
        sliders[2].maxValue = animal.animalSetting.reproductiveUrge;
    }

    
    private void Update()
    {
        //status
        texts[0].SetText(animalAI.state.ToString() + "  " + animalAI.searchMode.ToString());
        //age
        texts[1].SetText(animal.currentStatus.age.ToString());
        
        ////hunger
        //texts[3].SetText(animal.currentStatus.food.ToString());
        sliders[0].value = animal.currentStatus.food;
        ////thirst
        //texts[4].SetText(animal.currentStatus.water.ToString());
        sliders[1].value = animal.currentStatus.water;
        ////horniness
        //texts[5].SetText(animal.currentStatus.reproductiveUrge.ToString());
        sliders[2].value = animal.currentStatus.reproductiveUrge;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
