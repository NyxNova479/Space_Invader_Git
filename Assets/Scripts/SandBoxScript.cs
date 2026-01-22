using UnityEngine;

public class SandBoxScript : MonoBehaviour
{

    private int _masterJahmi = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _masterJahmi = 14;
        Debug.Log($"masterJahmi valeur à l'init = {_masterJahmi}");
        Test(out _masterJahmi);
        Debug.Log($"masterJahmi valeur à la fin = {_masterJahmi}");
    }

    private int Test(out int glouglou)
    {
        glouglou = 2 + 2;
        Debug.Log($"valeur de glouglou {glouglou}");
        return glouglou;
    }
}
