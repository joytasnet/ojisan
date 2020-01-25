using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeController : MonoBehaviour
{
    public Text score;
    public GameObject startButton;
    Coroutine coroutineId;
    bool isPlay;
    public GameObject formPanel;
    public GameObject RankingPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlay && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit, 100.0f))
            {
                if (hit.collider.name == "Cube")
                {
                    StopCoroutine(coroutineId);
                    isPlay = false;
                    formPanel.SetActive(true);
                }
            }
        }

    }
    IEnumerator RandomNum()
    {
        while (true)
        {
            transform.Rotate(0, 10.0f, 0);
            yield return new WaitForSeconds(0.3f);
            score.text = Random.Range(0, 100000).ToString();
        }

   
    }
    public void StartClick()
    {
        coroutineId = StartCoroutine(RandomNum());
        startButton.SetActive(false);
        isPlay = true;
        Transform cells = RankingPanel.transform.Find("Cells");
        for(int i = 0; i < cells.childCount; i++)
        {
            GameObject.Destroy(cells.GetChild(i).gameObject);
        }
        Vector3 vec = cells.localPosition;
        vec.y = 0f;
        cells.localPosition = vec;
        RankingPanel.SetActive(false);
    }
}
