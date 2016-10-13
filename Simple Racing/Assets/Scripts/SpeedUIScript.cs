using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpeedUIScript : MonoBehaviour
{
    public string playerTag;

    private Text UIText;

    CarController Car;

    void Awake()
    {
        Car = GameObject.FindGameObjectWithTag(playerTag).GetComponent<CarController>();
        UIText = GetComponent<Text>();
        StartCoroutine("UpdateText");
    }

    private IEnumerator UpdateText()
    {
        while (true)
        {
            UIText.text = ((int)Car.Speed).ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

}
