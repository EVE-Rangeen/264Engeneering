using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour
{
    [Header("经验类型")]
    [SerializeField] private ExpType _expType = ExpType.Normal;
    [SerializeField] private int _normalExpAmount = 10;
    [SerializeField] private int _highExpAmount = 20;
    [SerializeField] private int _veryHighExpAmount = 30;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰撞对象是玩家，则添加经验
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (_expType)
            {
                case ExpType.Normal:
                    ExperienceLevelController.instance.AddExperience(_normalExpAmount);
                    break;
                case ExpType.High:
                    ExperienceLevelController.instance.AddExperience(_highExpAmount);
                    break;
                case ExpType.VeryHigh:
                    ExperienceLevelController.instance.AddExperience(_veryHighExpAmount);
                    break;
            }
        }
    }
}
