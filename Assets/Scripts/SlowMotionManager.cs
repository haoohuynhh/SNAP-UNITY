using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionManager : MonoBehaviour
{
    public static SlowMotionManager Instance { get; private set; }

    [Header("Slow Motion Settings")]
    [SerializeField] private float slowdownFactor = 0.2f;       // Tốc độ của slow motion (0.2 = 20% tốc độ bình thường)
    [SerializeField] private float slowdownLength = 2f;         // Thời gian duy trì slow motion (giây)
    [SerializeField] private float transitionTime = 0.2f;       // Thời gian chuyển đổi (giây)
    [SerializeField] private bool activateOnStart = true;       // Kích hoạt khi game bắt đầu
    [SerializeField] private float startDelay = 0.5f;           // Độ trễ trước khi kích hoạt (giây)

    [Header("Effects")]
    [SerializeField] private AudioSource slowMotionSound;       // Âm thanh khi vào chế độ slow motion
    [SerializeField] private AudioSource normalSpeedSound;      // Âm thanh khi trở về tốc độ bình thường
    [SerializeField] private GameObject slowMotionVFX;          // Hiệu ứng hình ảnh
    
    private float originalFixedDeltaTime;
    private bool isInSlowMotion = false;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Start()
    {
        // Tự động kích hoạt slow motion khi bắt đầu game nếu activateOnStart = true
        if (activateOnStart)
        {
            StartCoroutine(ActivateWithDelay());
        }
    }
    
    private IEnumerator ActivateWithDelay()
    {
        // Đợi một khoảng thời gian trước khi kích hoạt slow motion
        yield return new WaitForSeconds(startDelay);
        
        // Kích hoạt slow motion
        DoSlowMotion();
    }
    
    /// <summary>
    /// Kích hoạt hiệu ứng slow motion với thông số mặc định
    /// </summary>
    public void DoSlowMotion()
    {
        if (!isInSlowMotion)
        {
            StartCoroutine(SlowMotionEffect());
        }
    }
    
    /// <summary>
    /// Kích hoạt hiệu ứng slow motion với các thông số tuỳ chỉnh
    /// </summary>
    public void DoSlowMotion(float slowFactor, float duration)
    {
        if (!isInSlowMotion)
        {
            StartCoroutine(SlowMotionEffect(slowFactor, duration));
        }
    }
    
    /// <summary>
    /// Hủy hiệu ứng slow motion và trở về tốc độ bình thường ngay lập tức
    /// </summary>
    public void CancelSlowMotion()
    {
        if (isInSlowMotion)
        {
            StopAllCoroutines();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = originalFixedDeltaTime;
            isInSlowMotion = false;
            
            if (normalSpeedSound != null)
            {
                normalSpeedSound.Play();
            }
        }
    }
    
    /// <summary>
    /// Coroutine xử lý hiệu ứng slow motion
    /// </summary>
    private IEnumerator SlowMotionEffect(float slowFactor = -1f, float duration = -1f)
    {
        // Sử dụng giá trị mặc định nếu không được chỉ định
        float factor = slowFactor > 0 ? slowFactor : slowdownFactor;
        float length = duration > 0 ? duration : slowdownLength;
        
        isInSlowMotion = true;
        
        // Hiệu ứng âm thanh khi bắt đầu slow motion
        if (slowMotionSound != null)        
        {
            slowMotionSound.pitch = 1f; // Giữ pitch không đổi
            slowMotionSound.Play();
        }
        
        // Hiệu ứng hình ảnh nếu có
        GameObject vfxInstance = null;
        if (slowMotionVFX != null)
        {
            vfxInstance = Instantiate(slowMotionVFX);
        }
        
        // Transition to slow motion
        float startTime = Time.unscaledTime;
        float t = 0;
        
        while (t < 1)
        {
            t = (Time.unscaledTime - startTime) / transitionTime;
            Time.timeScale = Mathf.Lerp(1f, factor, t);
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
            yield return null;
        }
        
        // Duy trì slow motion
        Time.timeScale = factor;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        
        yield return new WaitForSecondsRealtime(length);
        
        // Transition back to normal
        startTime = Time.unscaledTime;
        t = 0;
        
        while (t < 1)
        {
            t = (Time.unscaledTime - startTime) / transitionTime;
            Time.timeScale = Mathf.Lerp(factor, 1f, t);
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
            yield return null;
        }
        
        // Đảm bảo trở lại bình thường
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        
        // Phát âm thanh khi kết thúc slow motion
        if (normalSpeedSound != null)
        {
            normalSpeedSound.Play();
        }
        
        // Xóa hiệu ứng VFX nếu vẫn còn
        if (vfxInstance != null)
        {
            Destroy(vfxInstance);
        }
        
        isInSlowMotion = false;
    }
}