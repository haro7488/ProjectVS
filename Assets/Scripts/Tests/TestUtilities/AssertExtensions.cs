using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Vs.Tests.Utilities
{
    /// <summary>
    /// 커스텀 Assert 확장 메서드
    /// </summary>
    public static class AssertExtensions
    {
        #region Numeric Comparisons

        /// <summary>
        /// 두 float 값이 tolerance 이내로 같은지 확인
        /// </summary>
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance = 0.0001f, string message = null)
        {
            if (Mathf.Abs(expected - actual) > tolerance)
            {
                Assert.Fail(message ?? $"Expected {expected} but was {actual} (tolerance: {tolerance})");
            }
        }

        /// <summary>
        /// 두 Vector3 값이 tolerance 이내로 같은지 확인
        /// </summary>
        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual, float tolerance = 0.0001f, string message = null)
        {
            if (Vector3.Distance(expected, actual) > tolerance)
            {
                Assert.Fail(message ?? $"Expected {expected} but was {actual} (tolerance: {tolerance})");
            }
        }

        /// <summary>
        /// 값이 범위 내에 있는지 확인
        /// </summary>
        public static void IsInRange(float value, float min, float max, string message = null)
        {
            if (value < min || value > max)
            {
                Assert.Fail(message ?? $"Expected value {value} to be in range [{min}, {max}]");
            }
        }

        /// <summary>
        /// 값이 0보다 큰지 확인
        /// </summary>
        public static void IsPositive(float value, string message = null)
        {
            if (value <= 0)
            {
                Assert.Fail(message ?? $"Expected positive value but was {value}");
            }
        }

        /// <summary>
        /// 값이 0 이상인지 확인
        /// </summary>
        public static void IsNonNegative(float value, string message = null)
        {
            if (value < 0)
            {
                Assert.Fail(message ?? $"Expected non-negative value but was {value}");
            }
        }

        #endregion

        #region Collection Assertions

        /// <summary>
        /// 컬렉션에 특정 타입의 요소가 있는지 확인
        /// </summary>
        public static void ContainsType<T>(IEnumerable<object> collection, string message = null)
        {
            if (!collection.Any(item => item is T))
            {
                Assert.Fail(message ?? $"Collection does not contain any element of type {typeof(T).Name}");
            }
        }

        /// <summary>
        /// 컬렉션에 특정 타입의 요소가 없는지 확인
        /// </summary>
        public static void DoesNotContainType<T>(IEnumerable<object> collection, string message = null)
        {
            if (collection.Any(item => item is T))
            {
                Assert.Fail(message ?? $"Collection unexpectedly contains element of type {typeof(T).Name}");
            }
        }

        /// <summary>
        /// 컬렉션의 모든 요소가 특정 조건을 만족하는지 확인
        /// </summary>
        public static void AllSatisfy<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            foreach (var item in collection)
            {
                if (!predicate(item))
                {
                    Assert.Fail(message ?? $"Element {item} does not satisfy the predicate");
                }
            }
        }

        /// <summary>
        /// 컬렉션의 적어도 하나의 요소가 특정 조건을 만족하는지 확인
        /// </summary>
        public static void AnySatisfy<T>(IEnumerable<T> collection, Func<T, bool> predicate, string message = null)
        {
            if (!collection.Any(predicate))
            {
                Assert.Fail(message ?? "No element satisfies the predicate");
            }
        }

        /// <summary>
        /// 컬렉션에 중복 요소가 없는지 확인
        /// </summary>
        public static void HasNoDuplicates<T>(IEnumerable<T> collection, string message = null)
        {
            var list = collection.ToList();
            var distinctCount = list.Distinct().Count();

            if (list.Count != distinctCount)
            {
                Assert.Fail(message ?? "Collection contains duplicate elements");
            }
        }

        /// <summary>
        /// 컬렉션의 크기가 정확히 expected인지 확인
        /// </summary>
        public static void HasCount<T>(IEnumerable<T> collection, int expected, string message = null)
        {
            var count = collection.Count();
            if (count != expected)
            {
                Assert.Fail(message ?? $"Expected collection count to be {expected} but was {count}");
            }
        }

        #endregion

        #region Event Assertions

        private static readonly Dictionary<string, bool> EventFiredFlags = new();

        /// <summary>
        /// 이벤트 핸들러 등록 (이벤트 발생 여부 추적용)
        /// </summary>
        public static Action CreateEventTracker(string eventName)
        {
            EventFiredFlags[eventName] = false;
            return () => EventFiredFlags[eventName] = true;
        }

        /// <summary>
        /// 이벤트 핸들러 등록 (제네릭)
        /// </summary>
        public static Action<T> CreateEventTracker<T>(string eventName)
        {
            EventFiredFlags[eventName] = false;
            return (_) => EventFiredFlags[eventName] = true;
        }

        /// <summary>
        /// 이벤트가 발생했는지 확인
        /// </summary>
        public static void EventWasFired(string eventName, string message = null)
        {
            if (!EventFiredFlags.TryGetValue(eventName, out var fired) || !fired)
            {
                Assert.Fail(message ?? $"Event '{eventName}' was not fired");
            }
        }

        /// <summary>
        /// 이벤트가 발생하지 않았는지 확인
        /// </summary>
        public static void EventWasNotFired(string eventName, string message = null)
        {
            if (EventFiredFlags.TryGetValue(eventName, out var fired) && fired)
            {
                Assert.Fail(message ?? $"Event '{eventName}' was unexpectedly fired");
            }
        }

        /// <summary>
        /// 이벤트 추적 상태 초기화
        /// </summary>
        public static void ClearEventTrackers()
        {
            EventFiredFlags.Clear();
        }

        #endregion

        #region Unity Specific

        /// <summary>
        /// GameObject가 활성화되어 있는지 확인
        /// </summary>
        public static void IsActive(GameObject gameObject, string message = null)
        {
            if (!gameObject.activeInHierarchy)
            {
                Assert.Fail(message ?? $"GameObject '{gameObject.name}' is not active");
            }
        }

        /// <summary>
        /// GameObject가 비활성화되어 있는지 확인
        /// </summary>
        public static void IsNotActive(GameObject gameObject, string message = null)
        {
            if (gameObject.activeInHierarchy)
            {
                Assert.Fail(message ?? $"GameObject '{gameObject.name}' is unexpectedly active");
            }
        }

        /// <summary>
        /// 컴포넌트가 존재하는지 확인
        /// </summary>
        public static void HasComponent<T>(GameObject gameObject, string message = null) where T : Component
        {
            if (gameObject.GetComponent<T>() == null)
            {
                Assert.Fail(message ?? $"GameObject '{gameObject.name}' does not have component {typeof(T).Name}");
            }
        }

        /// <summary>
        /// 컴포넌트가 존재하지 않는지 확인
        /// </summary>
        public static void DoesNotHaveComponent<T>(GameObject gameObject, string message = null) where T : Component
        {
            if (gameObject.GetComponent<T>() != null)
            {
                Assert.Fail(message ?? $"GameObject '{gameObject.name}' unexpectedly has component {typeof(T).Name}");
            }
        }

        /// <summary>
        /// Transform 위치가 예상값과 같은지 확인
        /// </summary>
        public static void IsAtPosition(Transform transform, Vector3 expected, float tolerance = 0.01f, string message = null)
        {
            AreApproximatelyEqual(expected, transform.position, tolerance,
                message ?? $"Expected position {expected} but was {transform.position}");
        }

        #endregion
    }
}
