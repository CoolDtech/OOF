using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItem : PassiveItem
{   

    protected override void ApplyModifier()
    {   
        
        player.CurrentMight += passiveItemData.Multipler /100f;
    }
}
