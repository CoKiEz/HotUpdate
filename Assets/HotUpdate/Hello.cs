using UnityEngine;

public class Hello
{
    public static void Run()
    {
        GameObject.Find("SquareLeft").transform.Translate(-0.1f,0,0f);
        GameObject.Find("SquareRight").transform.Translate(0.1f,0,0f);
    }
    
}
