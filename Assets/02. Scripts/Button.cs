using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        // 현재 씬 이름 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 스테이지 이름에서 번호 추출
        string stagePrefix = "Stage";
        if (currentSceneName.StartsWith(stagePrefix))
        {
            // 번호 부분만 가져오기
            string stageNumberString = currentSceneName.Substring(stagePrefix.Length);
            
            // 문자열을 숫자로 변환
            if (int.TryParse(stageNumberString, out int stageNumber))
            {
                // 다음 스테이지 번호 계산
                int nextStageNumber = stageNumber + 1;

                // 다음 스테이지 이름 생성
                nextScene = $"{stagePrefix}{nextStageNumber}";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickRestartButton()
    {
        Debug.Log("OnClickRestart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickHomeButton()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnClickStartButton()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    public void OnClickStage1Button()
    {
        SceneManager.LoadScene("Stage1");
    }

    public void OnClickStage2Button()
    {
        SceneManager.LoadScene("Stage2");
    }

    public void OnClickStage3Button()
    {
        SceneManager.LoadScene("Stage3");
    }

    public void OnClickStageSelectButton()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    public void OnClickNextStageButton()
    {
        SceneManager.LoadScene($"{nextScene}");
    }
}
