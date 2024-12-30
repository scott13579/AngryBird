using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject WinUI;
    public GameObject RetryBtn;
    public GameObject HomeBtn;
    public GameObject nextStageBtn;

    private int totalMonsters;
    private int deadMonsters;

    private void Start()
    {
        totalMonsters = FindObjectsOfType<Monster>().Length; // 초기 몬스터 개수 가져오기
        Debug.Log(totalMonsters);
        deadMonsters = 0;

        Monster.OnMonsterDied += HandleMonsterDeath; // 몬스터 죽음 이벤트 구독
    }

    private void OnDestroy()
    {
        Monster.OnMonsterDied -= HandleMonsterDeath; // 구독 해제
    }

    private void HandleMonsterDeath()
    {
        deadMonsters++;
        Debug.Log($"deadMonsters : {deadMonsters}");
        if (deadMonsters >= totalMonsters)
        {
            RetryBtn.SetActive(false);
            HomeBtn.SetActive(false);
            WinUI.SetActive(true); // 모든 몬스터가 죽었을 때 Win UI 활성화
            
                    
            // 현재 씬 이름 체크
            string currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log(currentSceneName);
            if (currentSceneName == "Stage3")
            {
                nextStageBtn.SetActive(false);
            }
        }
    }
}
