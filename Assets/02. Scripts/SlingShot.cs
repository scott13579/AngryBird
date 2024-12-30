using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlingShot : MonoBehaviour
{
    public int birdCount;
    public TextMeshProUGUI birdCountText;
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    
    public ParticleSystem destroyParticle;

    public Transform center;
    public Transform idlePosition;
    
    public Vector3 currentPosition;
    
    public float maxLength;
    public float bottomBoundary;
        
    bool isMouseDown;
    
    public GameObject birdPrefab;
    public float birdPositionOffset;
    
    Rigidbody2D bird;
    Collider2D birdCollider;

    public float force;
    
    public GameObject loseImageUI;
    
    public GameObject RetryBtn;
    public GameObject HomeBtn;
    
    public AudioSource shootSound;
    
    public LineRenderer trajectoryLine; // 궤적을 그릴 LineRenderer
    public int trajectoryResolution = 30; // 궤적 계산 포인트 수
    
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0,stripPositions[0].position);
        lineRenderers[1].SetPosition(0,stripPositions[1].position);
        
        birdCountText.text = "X " + birdCount.ToString();
        
        trajectoryLine.positionCount = trajectoryResolution;
        trajectoryLine.enabled = false; // 초기에는 비활성화
        
        CreateBird();
    }
    
    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition 
                                                                       - center.position, maxLength);
            
            currentPosition = ClampBoundary(currentPosition);
            SetStrips(currentPosition);
            
            trajectoryLine.enabled = true;
            DrawTrajectory(currentPosition);
        }
        else
        {
            ResetStrips();
            
            // 궤적 LineRenderer 비활성화
            trajectoryLine.enabled = false;
            Debug.Log("궤적 비활성화");
        }
        
        CheckForLastShot();
    }

    void DrawTrajectory(Vector3 launchPosition)
    {
        // 최대 길이로 클램핑
        Vector3 clampedPosition = Vector3.ClampMagnitude(launchPosition - center.position, maxLength) + center.position;
    
        // 궤적 초기 속도 계산
        Vector3 initialVelocity = (clampedPosition - center.position) * (force * -1);

        // 궤적 포인트 계산
        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i / (float)trajectoryResolution; // 현재 시간 비율
            Vector3 trajectoryPoint = CalculateTrajectoryPoint(bird.transform.position, initialVelocity, t);
            trajectoryLine.SetPosition(i, trajectoryPoint);
        }
    }


    Vector3 CalculateTrajectoryPoint(Vector3 startPosition, Vector3 initialVelocity, float time)
    {
        // 포물선 운동 공식: P(t) = P0 + V0 * t + 0.5 * g * t^2
        Vector3 gravityEffect = Physics2D.gravity * (time * time * 0.5f);
        Vector3 displacement = initialVelocity * time;
        return startPosition + displacement + gravityEffect;
    }

    private void OnMouseDown()
    {
        if (birdCount > 0 && bird != null)
        {
            isMouseDown = true;
        }
    }
    
    private void OnMouseUp()
    {
        if (isMouseDown)
        {
            isMouseDown = false;
            Shoot();
        }
    }
    
    void CreateBird()
    {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;

        ResetStrips();
    }

    IEnumerator DestroyBirdAfterDelay(GameObject birdObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (birdObject != null)
        {
            Destroy(birdObject);
            GameObject.Instantiate(destroyParticle, birdObject.transform.position, Quaternion.identity);
            destroyParticle.Play();
            if (bird != null && bird.gameObject == birdObject)
            {
                bird = null;
                birdCollider = null;
            }
        }
    }
    
    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1,position);
        lineRenderers[1].SetPosition(1,position);

        if (bird)
        {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.right = -dir.normalized;
        }
    }

    void Shoot()
    {
        if (birdCount <= 0 || bird == null)
            return;

        bird.isKinematic = false;

        Vector3 birdForce = (currentPosition - center.position) * force * -1;
        bird.velocity = birdForce;

        if (birdCollider != null)
        {
            birdCollider.enabled = true;
        }

        StartCoroutine(DestroyBirdAfterDelay(bird.gameObject, 10.0f));

        bird = null;
        birdCollider = null;

        shootSound.Play();
        birdCountDown();
    }

    void birdCountDown()
    {
        birdCount--;
        birdCountText.text = "X " + birdCount.ToString();

        if (birdCount > 0)
        {
            CreateBird(); // birdCount가 0보다 클 때만 새 생성
        }
        else
        {
            
        }
    }

    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }

    public void CheckForLastShot()
    {
        if (birdCount <= 0)
        {
            StartCoroutine(WaitLoseTime());
        }
    }

    private IEnumerator WaitLoseTime()
    {
        yield return new WaitForSeconds(10f);
        RetryBtn.SetActive(false);
        HomeBtn.SetActive(false);
        loseImageUI.SetActive(true);
    }
}
