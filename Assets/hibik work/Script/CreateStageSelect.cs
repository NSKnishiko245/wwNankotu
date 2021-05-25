using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateStageSelect : MonoBehaviour
{
    static private int stageMax = 36;      // ステージの最大数
    static private int bookStageMax = 6;   // １冊あたりのステージの数


    //==============================================================
    // ステージセレクト作成
    //==============================================================
    public void Create()
    {
        // 本棚の本を削除
        GameObject book = GameObject.Find("BookModel" + (BookSelect.bookNum + 1));
        book.SetActive(false);

        // 本のマテリアルを設定
        GameObject bookL = GameObject.Find("book_L2");
        GameObject bookR = GameObject.Find("book_R2");
        GameObject bookBack = GameObject.Find("book_back2");

        Material material = Resources.Load<Material>("Material/Book/Book0" + (BookSelect.bookNum + 1));
        bookL.GetComponent<Renderer>().material = material;
        bookR.GetComponent<Renderer>().material = material;
        bookBack.GetComponent<Renderer>().material = material;

        // テキストファイルからタイトル文を取得
        TextAsset textAsset = Resources.Load("Text/TitleText", typeof(TextAsset)) as TextAsset;
        //一行づつ代入
        string[] titleString = textAsset.text.Split('_');

        // テキストファイルからストーリー文を取得
        textAsset = Resources.Load("Text/StoryText", typeof(TextAsset)) as TextAsset;
        //一行づつ代入
        string[] storyString = textAsset.text.Split('_');


        // 各ページの設定
        for (int i = 0; i < bookStageMax; i++)
        {
            // 親ページを設定
            GameObject bookStageL = GameObject.Find("BookStageL (" + (i + 1) + ")");
            GameObject bookStageR = GameObject.Find("BookStageR (" + (i + 1) + ")");

            // ステージ番号
            int stageNum = BookSelect.bookNum * bookStageMax + i + 1;
            Debug.Log(stageNum);

            //-----------------------------------
            // タイトルテキストの設定
            //-----------------------------------
            Text titleText = bookStageL.transform.Find("TitleText").GetComponent<Text>();
            titleText.text = titleString[stageNum - 1];

            //-----------------------------------
            // ストーリーテキストの設定
            //-----------------------------------
            Text storyText = bookStageL.transform.Find("StoryText").GetComponent<Text>();
            storyText.text = storyString[stageNum - 1];

            //-----------------------------------
            // メダルのUIの設定
            //-----------------------------------
            GameObject goldMedal = bookStageL.transform.Find("Medal/GoldImage").gameObject;
            GameObject silverMedal = bookStageL.transform.Find("Medal/SilverImage").gameObject;
            GameObject copperMedal = bookStageL.transform.Find("Medal/CopperImage").gameObject;

            // メダルを取得していたら色が付く
            if (StageSelectManager.score[stageNum].isGold)
            {
                goldMedal.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else goldMedal.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

            if (StageSelectManager.score[stageNum].isSilver)
            {
                silverMedal.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else silverMedal.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

            if (StageSelectManager.score[stageNum].isCopper)
            {
                copperMedal.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else copperMedal.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.2f);

            //-----------------------------------
            // ステージ画像の設定
            //-----------------------------------
            Image stageImage = bookStageR.transform.Find("StageImage").GetComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Sprite/Stage/st" + stageNum);
            stageImage.sprite = sprite;

            //-----------------------------------
            // ステージ番号のUIの設定
            //-----------------------------------
            Image tensPlaceImage = bookStageR.transform.Find("StageNum/TensPlaceImage").GetComponent<Image>();
            sprite = Resources.Load<Sprite>("Sprite/Number/" + (stageNum / 10));
            tensPlaceImage.sprite = sprite;

            Image onesPlaceImage = bookStageR.transform.Find("StageNum/OnesPlaceImage").GetComponent<Image>();
            sprite = Resources.Load<Sprite>("Sprite/Number/" + (stageNum % 10));
            onesPlaceImage.sprite = sprite;
        }
    }
}