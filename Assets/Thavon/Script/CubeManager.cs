using UnityEngine;

public class CubeManager : MonoBehaviour
{
    private CubeResetPos[] cubes;

    void Start()
    {
        cubes = FindObjectsOfType<CubeResetPos>();
    }

    public void ResetAllCubes()
    {
        foreach (CubeResetPos cube in cubes)
        {
            cube.ResetCube();
        }
    }
}
