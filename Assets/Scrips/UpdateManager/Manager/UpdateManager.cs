// UpdateManager.cs
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static UpdateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    // -------------------------

    // Danh sách cho từng vòng lặp
    private List<IUpdateListener> _updateListeners = new List<IUpdateListener>();
    private List<IFixedUpdateListener> _fixedUpdateListeners = new List<IFixedUpdateListener>();
    private List<ILateUpdateListener> _lateUpdateListeners = new List<ILateUpdateListener>();

    // Vòng lặp Update
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        for (int i = _updateListeners.Count - 1; i >= 0; i--)
        {
            _updateListeners[i]?.OnUpdate(deltaTime);
        }
    }

    // --- MỚI: Vòng lặp FixedUpdate ---
    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        for (int i = _fixedUpdateListeners.Count - 1; i >= 0; i--)
        {
            _fixedUpdateListeners[i]?.OnFixedUpdate(fixedDeltaTime);
        }
    }

    // --- MỚI: Vòng lặp LateUpdate ---
    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        for (int i = _lateUpdateListeners.Count - 1; i >= 0; i--)
        {
            _lateUpdateListeners[i]?.OnLateUpdate(deltaTime);
        }
    }

    #region Registration Methods
    // --- Update Listeners ---
    public void RegisterUpdateListener(IUpdateListener listener)
    {
        if (!_updateListeners.Contains(listener)) _updateListeners.Add(listener);
    }
    public void UnregisterUpdateListener(IUpdateListener listener)
    {
        if (_updateListeners.Contains(listener)) _updateListeners.Remove(listener);
    }

    // --- MỚI: FixedUpdate Listeners ---
    public void RegisterFixedUpdateListener(IFixedUpdateListener listener)
    {
        if (!_fixedUpdateListeners.Contains(listener)) _fixedUpdateListeners.Add(listener);
    }
    public void UnregisterFixedUpdateListener(IFixedUpdateListener listener)
    {
        if (_fixedUpdateListeners.Contains(listener)) _fixedUpdateListeners.Remove(listener);
    }

    // --- MỚI: LateUpdate Listeners ---
    public void RegisterLateUpdateListener(ILateUpdateListener listener)
    {
        if (!_lateUpdateListeners.Contains(listener)) _lateUpdateListeners.Add(listener);
    }
    public void UnregisterLateUpdateListener(ILateUpdateListener listener)
    {
        if (_lateUpdateListeners.Contains(listener)) _lateUpdateListeners.Remove(listener);
    }
    #endregion
}