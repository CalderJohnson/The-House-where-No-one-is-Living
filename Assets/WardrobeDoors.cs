using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeDoors : MonoBehaviour
{
    
    public Animator leftDoorAnimator; 
    public Animator rightDoorAnimator; 
    
    
    
    

    public void OpenWardrobe()
    {  
        
        leftDoorAnimator.Play("DoorOpen_Left");
        rightDoorAnimator.Play("DoorOpen_Right");
    }
}
