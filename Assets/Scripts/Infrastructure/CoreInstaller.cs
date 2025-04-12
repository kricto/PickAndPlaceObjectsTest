using Reflex.Core;
using Unity.Cinemachine;
using UnityEngine;

public class CoreInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private PlayerInputSystem _playerInputSystem;
    
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(_camera);
        containerBuilder.AddSingleton(_playerInputSystem);
    }
}
