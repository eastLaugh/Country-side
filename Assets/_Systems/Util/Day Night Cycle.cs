using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float dayDuration = 120.0f; // 一天的持续时间（以秒为单位）

    private Transform sunTransform;
    private float timeOfDay = 0.0f;
    private float rotationSpeed;

    private void Start()
    {
        sunTransform = transform;
        rotationSpeed = 360.0f / dayDuration;
    }

    private void Update()
    {
        // 更新时间
        timeOfDay += Time.deltaTime;
        timeOfDay %= dayDuration;

        // 根据时间设置光照方向
        UpdateSunPosition();
    }

    private void UpdateSunPosition()
    {
        // 计算当前时间的旋转角度，模拟太阳的轨迹
        float angle = (timeOfDay / dayDuration) * 360.0f;

        // 更新自身的旋转来改变光照方向
        sunTransform.rotation = Quaternion.Euler(angle, 0.0f, 0.0f);
    }
}
