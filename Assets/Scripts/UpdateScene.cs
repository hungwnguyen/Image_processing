using UnityEngine;

public class UpdateScene : MonoBehaviour
{
    [SerializeField] private GameObject portrait;
    [SerializeField] private GameObject landscape;

    private void Awake()
    {
        if (Screen.width > Screen.height)
        {
            landscape.SetActive(true);
            portrait.SetActive(false);
        }
        else
        {
            landscape.SetActive(false);
            portrait.SetActive(true);
        }
    }
}
