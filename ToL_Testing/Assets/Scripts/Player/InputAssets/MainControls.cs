// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player/InputAssets/MainControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace BaD.Modules.Input
{
    public class MainControls : IInputActionCollection, IDisposable
    {
        private InputActionAsset asset;
        public MainControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""0582a91b-a94a-4cc9-9dba-244ae161997b"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""d3fcbae5-7c8f-48df-9076-4b8e0bbbb072"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""d22ea5a6-17c2-4dfe-8472-96faaa883379"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""97ede167-a2c7-478a-9646-36c3a252a5d2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""6add4c80-9d43-4022-b8cb-0bcd45a51785"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""6fdf1e5d-d677-4b03-8d61-40f098778d05"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraHorizontal"",
                    ""type"": ""Value"",
                    ""id"": ""439fe60b-c900-49ff-bd6b-a7ccbdec096d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraVertical"",
                    ""type"": ""Value"",
                    ""id"": ""45353313-98ed-4554-8958-dab3538a1193"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""5192ca47-a7eb-48e4-b8a6-4c1dae237c36"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a5a66709-5b20-4488-9886-ac67b88d1793"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b2d2bbdd-a664-4c52-a831-b0adcd2010f3"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c9d4e677-0142-431e-b386-e7d71cfedb48"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f47d6e18-b2cb-445b-a41b-a5489e80cc26"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""999a53e8-25cb-42eb-8abd-958715d35eaa"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6d169ea-dba1-4697-8925-259ff85d4a7b"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""339d4ff9-b482-4ebf-975d-af7fb1c2c111"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f53d1864-0f4d-44a6-ac70-0793c7675352"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40818ec4-5d27-4b82-8bea-ff94be5cf930"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""576962b0-5535-4b11-84e2-2fd703e95624"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""161d5062-2a9a-414f-94f8-cf1b7c5f454b"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae55d445-84f3-45e5-825a-97b8bcbe9af4"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97f6fcc2-bbbe-421f-946b-6928276d55c5"",
                    ""path"": ""<Gamepad>/dpad/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7805205-b89b-48c2-bccf-72c66b617590"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": ""Invert,Scale(factor=0)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CameraVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""77a83f66-e80b-4ca0-89b4-0468ac440c35"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Right Stick/Y"",
                    ""id"": ""fc569934-4c8e-4a57-8167-ef3fd7c712c9"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraVertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""button"",
                    ""id"": ""451bfefb-84a5-4c04-9a5a-d21e5db815a6"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""5b1c9bc2-ef58-4a68-b0dd-8d3cd1fd669d"",
                    ""path"": ""<Gamepad>/rightStick/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=30)"",
                    ""groups"": """",
                    ""action"": ""CameraVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a6125b51-41c7-48ab-b174-3bc456f532ab"",
                    ""path"": ""<Gamepad>/rightStick/x"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CameraHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3667d4d-c718-4899-8dc9-911d9169d2d4"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0)"",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""DragRotate"",
                    ""id"": ""897d99a8-661e-4d36-adcd-30020ff4f655"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraHorizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""button"",
                    ""id"": ""553a6a66-047c-4910-9284-c105a0e672e3"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""79eb6489-b82d-4f9a-921d-95ab171d818e"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
            m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
            m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
            m_Player_Pause = m_Player.FindAction("Pause", throwIfNotFound: true);
            m_Player_CameraZoom = m_Player.FindAction("CameraZoom", throwIfNotFound: true);
            m_Player_CameraHorizontal = m_Player.FindAction("CameraHorizontal", throwIfNotFound: true);
            m_Player_CameraVertical = m_Player.FindAction("CameraVertical", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private readonly InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private readonly InputAction m_Player_Movement;
        private readonly InputAction m_Player_Attack;
        private readonly InputAction m_Player_Interact;
        private readonly InputAction m_Player_Pause;
        private readonly InputAction m_Player_CameraZoom;
        private readonly InputAction m_Player_CameraHorizontal;
        private readonly InputAction m_Player_CameraVertical;
        public struct PlayerActions
        {
            private MainControls m_Wrapper;
            public PlayerActions(MainControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Movement => m_Wrapper.m_Player_Movement;
            public InputAction @Attack => m_Wrapper.m_Player_Attack;
            public InputAction @Interact => m_Wrapper.m_Player_Interact;
            public InputAction @Pause => m_Wrapper.m_Player_Pause;
            public InputAction @CameraZoom => m_Wrapper.m_Player_CameraZoom;
            public InputAction @CameraHorizontal => m_Wrapper.m_Player_CameraHorizontal;
            public InputAction @CameraVertical => m_Wrapper.m_Player_CameraVertical;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                    Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    Interact.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    Interact.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    Interact.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteract;
                    Pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    Pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    Pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    CameraZoom.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                    CameraZoom.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                    CameraZoom.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraZoom;
                    CameraHorizontal.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraHorizontal;
                    CameraHorizontal.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraHorizontal;
                    CameraHorizontal.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraHorizontal;
                    CameraVertical.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraVertical;
                    CameraVertical.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraVertical;
                    CameraVertical.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCameraVertical;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    Movement.started += instance.OnMovement;
                    Movement.performed += instance.OnMovement;
                    Movement.canceled += instance.OnMovement;
                    Attack.started += instance.OnAttack;
                    Attack.performed += instance.OnAttack;
                    Attack.canceled += instance.OnAttack;
                    Interact.started += instance.OnInteract;
                    Interact.performed += instance.OnInteract;
                    Interact.canceled += instance.OnInteract;
                    Pause.started += instance.OnPause;
                    Pause.performed += instance.OnPause;
                    Pause.canceled += instance.OnPause;
                    CameraZoom.started += instance.OnCameraZoom;
                    CameraZoom.performed += instance.OnCameraZoom;
                    CameraZoom.canceled += instance.OnCameraZoom;
                    CameraHorizontal.started += instance.OnCameraHorizontal;
                    CameraHorizontal.performed += instance.OnCameraHorizontal;
                    CameraHorizontal.canceled += instance.OnCameraHorizontal;
                    CameraVertical.started += instance.OnCameraVertical;
                    CameraVertical.performed += instance.OnCameraVertical;
                    CameraVertical.canceled += instance.OnCameraVertical;
                }
            }
        }
        public PlayerActions @Player => new PlayerActions(this);
        private int m_KeyboardandMouseSchemeIndex = -1;
        public InputControlScheme KeyboardandMouseScheme
        {
            get
            {
                if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
                return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMovement(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnPause(InputAction.CallbackContext context);
            void OnCameraZoom(InputAction.CallbackContext context);
            void OnCameraHorizontal(InputAction.CallbackContext context);
            void OnCameraVertical(InputAction.CallbackContext context);
        }
    }
}
