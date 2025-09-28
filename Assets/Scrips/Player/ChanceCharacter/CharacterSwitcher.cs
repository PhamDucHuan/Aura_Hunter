using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Available Characters")]
    [SerializeField] private List<CharacterStats> availableCharacters;

    [Header("Player Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private Animator playerAnimator;
    // Thêm các component khác nếu cần

    private int _currentCharacterIndex = 0;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Player.SwitchCharacter.performed += OnSwitchCharacter; // Giả sử bạn có action tên là "SwitchCharacter"
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.SwitchCharacter.performed -= OnSwitchCharacter;
        _playerInput.Player.Disable();
    }

    void Start()
    {
        if (availableCharacters.Count == 0)
        {
            Debug.LogError("No characters available to switch!");
            return;
        }
        // Bắt đầu với nhân vật đầu tiên trong danh sách
        SwitchToCharacter(_currentCharacterIndex);
    }

    private void OnSwitchCharacter(InputAction.CallbackContext context)
    {
        // Chuyển sang nhân vật tiếp theo, quay vòng lại nếu hết danh sách
        _currentCharacterIndex = (_currentCharacterIndex + 1) % availableCharacters.Count;
        SwitchToCharacter(_currentCharacterIndex);
    }

    public void SwitchToCharacter(int index)
    {
        if (index < 0 || index >= availableCharacters.Count) return;

        _currentCharacterIndex = index;
        CharacterStats newStats = availableCharacters[_currentCharacterIndex];

        Debug.Log("Switching to: " + newStats.characterName);

        // 1. Cập nhật Animator
        playerAnimator.runtimeAnimatorController = newStats.animatorController;

        // 2. Cập nhật các script với dữ liệu mới
        characterManager.Initialize(newStats);
        playerMovement.UpdateStats(newStats);
        playerAttack.UpdateStats(newStats);

        // (Tùy chọn) Có thể reset lại vị trí hoặc các trạng thái khác ở đây
    }
}
