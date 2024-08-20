using UnityEngine;

public enum SoundName
{
    [InspectorName("UI/Button Click")] ButtonClick,
    [InspectorName("Board/Powerup/TNT")] TNT,
    [InspectorName("Board/Powerup/Disco Ball")] DiscoBall,
    [InspectorName("Board/Powerup/Disco Ball Combine")] DiscoBallCombine,
    [InspectorName("Board/Powerup/Rocket")] Rocket,
    [InspectorName("Board/Element/Element Pop")] ElementPop,
    [InspectorName("Board/Element/Powerup Combine")] PowerupCombine,
    [InspectorName("Board/Obstacle/Gift Box")] GiftBox,
    [InspectorName("Board/Obstacle/Wood Box")] WoodBox,
    [InspectorName("End Game/Win")] Win,
    [InspectorName("End Game/Lose")] Lose,
}

/* 
-> UI/
-> Board/Powerup/
-> Board/Element/
->Board/Obstacle/
-> End Game/
*/
