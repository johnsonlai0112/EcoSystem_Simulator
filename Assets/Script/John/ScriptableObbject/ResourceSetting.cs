using UnityEngine;

public enum ResourceType
{
    Food,
    Water,
    Mate
};

public enum FoodType
{
    None,
    Plant,
    Animal
};

[CreateAssetMenu(fileName = "New Resource", menuName = "Ecosystem/Enviroment Resource")]
public class ResourceSetting : ScriptableObject
{
    public ResourceType type;
    public FoodType foodType;
    public AttributeRange startValue = new AttributeRange { min = 100f, max = 500f };
}
