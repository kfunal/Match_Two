using UnityEngine;

[System.Serializable]
public enum ElementType
{
    [InspectorName("Match Elements/Blue Element")] BlueElement,
    [InspectorName("Match Elements/Green Element")] GreenElement,
    [InspectorName("Match Elements/Red Element")] RedElement,
    [InspectorName("Match Elements/Yellow Element")] YellowElement,

    [InspectorName("Powerups/Disco Ball")] DiscoBall,
    [InspectorName("Powerups/TNT")] TNT,
    [InspectorName("Powerups/Horizontal Rocket")] HorizontalRocket,
    [InspectorName("Powerups/Vertical Rocket")] VerticalRocket,

    [InspectorName("Obstacles/WoodBox")] WoodBox,
    [InspectorName("Obstacles/GiftBox")] GiftBox,

}