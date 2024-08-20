using UnityEngine;

public enum PoolType
{
    [InspectorName("Board/Match Elements/Red Element")] RedElement,
    [InspectorName("Board/Match Elements/Blue Element")] BlueElement,
    [InspectorName("Board/Match Elements/Yellow Element")] YellowElement,
    [InspectorName("Board/Match Elements/Green Element")] GreenElement,
    None,

    [InspectorName("Board/Powerups/Horizontal Rocket")] HorizontalRocket,
    [InspectorName("Board/Powerups/Vertical Rocket")] VerticalRocket,
    [InspectorName("Board/Powerups/TNT")] TNT,
    [InspectorName("Board/Powerups/DiscoBall")] DiscoBall,

    [InspectorName("Board/Effects/Disco Ball Line")] DiscoBallLine,
    [InspectorName("Board/Obstacles/WoodBox")] WoodBox,
    [InspectorName("Board/Obstacles/GiftBox")] GiftBox,

}

/* 
 -> Board/Match Elements
 -> Board/Powerups
 -> Board/Effects
 -> Board/Obstacles
*/
