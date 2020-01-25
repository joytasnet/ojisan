using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Networking;



public class CanvasController : MonoBehaviour
{
    private const int COUNT = 100;//ベスト100の取得
    private const float CELLHEIGHT = 65.0f;//セル一つ分の高さ
    public Text name;//ユーザーが入力した名前が入るTextオブジェクト
    public Text score;//ユーザーが入力した点数が入るTextオブジェクト
    public ToggleGroup tg;
    public GameObject startButton;
    public GameObject formPanel;
    public GameObject rankingPanel;
    public GameObject cells;
    public GameObject prefab;
    public Sprite[] sprites;

    float scaleFactor = 0.5f;

    //Sendボタンが押された時の処理
    public void OnSendClick()
    {
        StartCoroutine(PostConnect());
    }
    //コルーチンでWebに接続する。
    IEnumerator PostConnect()
    {
        //選択されているラジオボタンを取得(usingにSysatem.Linqが必須)
        Toggle tgl = tg.ActiveToggles().FirstOrDefault();
        //パラメーターを詰める
        WWWForm form = new WWWForm();
        form.AddField("name", name.text);
        form.AddField("score", score.text);
        form.AddField("sex", tgl.name == "Man" ? 0 : 1);
        form.AddField("count", COUNT);
        string url = "https://joytas.net/php/scoreapi/GetRanking.php";
        //Postで通信
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            //Unity標準のJSONパーサーをつかう。JSON文字列を元にクラスを作成する。
            RankingData ranking = JsonUtility.FromJson<RankingData>(uwr.downloadHandler.text);
            //ランキングい入った時の処理
            if (ranking.isRankingIn)
            {
                //ランキングの数だけ回す
                for (int i = 0; i < ranking.list.Count; i++)
                {
                    //cellをInstantiate
                    GameObject cell = Instantiate(prefab);
                    //自分のデータだったら背景を青にする
                    if (ranking.lastId == ranking.list[i].id)
                    {
                        Image panel = cell.GetComponent<Image>();
                        panel.color = Color.cyan;
                    }
                    //cellをCellsの子要素にする。
                    cell.transform.SetParent(cells.transform, false);

                    //cellの子要素Rankを取得
                    Text rank = cell.transform.Find("Rank").GetComponent<Text>();
                    //順位を表示
                    rank.text = (i + 1).ToString();
                    //性別に応じてアイコンを表示
                    Image image = cell.transform.Find("Thumb").GetComponent<Image>();
                    image.sprite = ranking.list[i].sex == 0 ? sprites[0] : sprites[1];
                    //名前を表示
                    Text name = cell.transform.Find("Name").GetComponent<Text>();
                    name.text = ranking.list[i].name;
                    //点数を表示
                    Text scoreText = cell.transform.Find("Score").GetComponent<Text>();
                    scoreText.text = ranking.list[i].score.ToString();

                }

                rankingPanel.SetActive(true);
                //表示位置にスクロールさせる

                if (ranking.rank > Screen.height/scaleFactor / CELLHEIGHT)
                {
                    Vector3 vec = cells.transform.localPosition;

                    float distination =
                        ranking.rank * CELLHEIGHT - Mathf.Max((Screen.height / scaleFactor) / 2, (Screen.height/scaleFactor) - (ranking.list.Count - ranking.rank) * CELLHEIGHT);
                    float diff = distination - vec.y;
                    while (diff > 0.1f)
                    {
                        vec = cells.transform.localPosition;
                        vec.y = Mathf.Lerp(vec.y, distination, 0.1f);
                        cells.transform.localPosition = vec;
                        diff = distination - vec.y;
                        yield return new WaitForSeconds(0.01f);

                    }

                }

            }
            formPanel.SetActive(false);
            startButton.SetActive(true);

        }

    }
}

//Jsonパースの為にクラスを作成する
[Serializable]
class RankingData
{
    public int lastId;
    public int rank;
    public List<Score> list;
    public bool isRankingIn;

}
[Serializable]
class Score
{
    public int id;
    public string name;
    public int score;
    public int sex;
}


