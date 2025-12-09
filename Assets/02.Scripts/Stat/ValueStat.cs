using System;
using UnityEngine;

// 프로그래머

// 하드스킬 (기술 중심)
// 프로그래밍 언어, 엔진에 대한 이해, 최적화, 툴
// 특정 도메인 지식
// 취업을 가능케 함

// 소프트스킬 (
// 커뮤니케이션 (말 알아듣기, 잘 설득하기, QA)
// 문제를 정의하거나 보고 능력
// 책임감, 협업태도, 멘탈/시간관리

[Serializable]
public class ValueStat
{
    // 소모되지 않는 스탯
    [SerializeField] private float _value;
    public float Value => _value;

    public void Increase(float amount)
    {
        _value += amount;
    }

    public void Decrease(float amount)
    {
        _value -= amount;
    }

    public void SetValue(float value)
    {
        _value = value;
    }
}
