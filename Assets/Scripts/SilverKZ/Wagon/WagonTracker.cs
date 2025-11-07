using System;
using System.Collections.Generic;
using UnityEngine;

public class WagonTracker : MonoBehaviour
{
    [SerializeField] private List<Transform> _waypoints;

    private int _currentIndex = 0;

    public static Action<int> onUpdateProgress;

    private void Start()
    {
        onUpdateProgress?.Invoke(0);
    }

    private void Update()
    {
        if (_currentIndex < _waypoints.Count - 1)
        {
            float distToNext = Vector3.Distance(transform.position, _waypoints[_currentIndex + 1].position);

            if (distToNext < 2f) 
            {
                _currentIndex++;
                int progress = (int)(Mathf.Clamp01((float)_currentIndex / ((float)_waypoints.Count - 1)) * 100);
                onUpdateProgress?.Invoke(progress);
            }
        }
    }
}
