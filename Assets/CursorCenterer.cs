using UnityEngine;

public class CursorCenterer : MonoBehaviour
{
    public Texture2D cursorTex;
    
    private void Start()
    {
        Vector2 hotSpot = new Vector2(cursorTex.width / 2, cursorTex.height / 2);
        Cursor.SetCursor(cursorTex, hotSpot, CursorMode.Auto);
    }
}
