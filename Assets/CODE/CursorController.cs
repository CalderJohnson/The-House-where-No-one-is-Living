using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClicked;
    // Start is called before the first frame update
    void Start()
    {
        if(Input.GetMouseButtonDown(0)){
            ChangeCursor(cursorClicked);
        }
    }

    private void Awake(){
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void ChangeCursor(Texture2D cursorType){
        Vector2 hotspot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, hotspot,CursorMode.Auto);
    }
}
