using UnityEngine;
using TMPro;  

public class BrazaletetUI : MonoBehaviour
{
    public TextMeshProUGUI braceletCountText;

    private void Update()
    {
        braceletCountText.text = Bracelet.braceletsCollected.ToString();
    }
}