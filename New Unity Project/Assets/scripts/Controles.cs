// GENERATED AUTOMATICALLY FROM 'Assets/scripts/Controles.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controles : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controles()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controles"",
    ""maps"": [
        {
            ""name"": ""Coche"",
            ""id"": ""e7df32cf-c769-471d-8d6b-ba781366567d"",
            ""actions"": [
                {
                    ""name"": ""accelerar/desacelerar"",
                    ""type"": ""Value"",
                    ""id"": ""07652eaa-4884-4923-9886-524b63468ad0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""teclado"",
                    ""id"": ""03e58a82-0017-4016-9dc0-addc5a2d1fc5"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5cd357e6-3a55-478c-a26a-bcfbbb64a1d5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d9c3a14a-d2d0-49c9-8f13-bccd2d9bf4ba"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""030fffde-d430-4031-9fad-a5acaae21f25"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c3e1f112-ebc5-4b8e-bd03-35ac34452acf"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""keyboard"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""gamepad"",
                    ""id"": ""885ee591-8631-4735-af0f-595e640c114b"",
                    ""path"": ""2DVector(mode=2)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b062f567-b8a0-4678-ae50-2e252fcaf0aa"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""control"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2291c3fb-da53-4e47-96b6-c06bfa2e5e79"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""control"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2ee3fb44-7736-4e8a-a6d9-1c23277665b6"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""control"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f1b7379b-18eb-4265-95d7-09e79db3e25a"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""control"",
                    ""action"": ""accelerar/desacelerar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""keyboard"",
            ""bindingGroup"": ""keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""control"",
            ""bindingGroup"": ""control"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Coche
        m_Coche = asset.FindActionMap("Coche", throwIfNotFound: true);
        m_Coche_accelerardesacelerar = m_Coche.FindAction("accelerar/desacelerar", throwIfNotFound: true);
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

    // Coche
    private readonly InputActionMap m_Coche;
    private ICocheActions m_CocheActionsCallbackInterface;
    private readonly InputAction m_Coche_accelerardesacelerar;
    public struct CocheActions
    {
        private @Controles m_Wrapper;
        public CocheActions(@Controles wrapper) { m_Wrapper = wrapper; }
        public InputAction @accelerardesacelerar => m_Wrapper.m_Coche_accelerardesacelerar;
        public InputActionMap Get() { return m_Wrapper.m_Coche; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CocheActions set) { return set.Get(); }
        public void SetCallbacks(ICocheActions instance)
        {
            if (m_Wrapper.m_CocheActionsCallbackInterface != null)
            {
                @accelerardesacelerar.started -= m_Wrapper.m_CocheActionsCallbackInterface.OnAccelerardesacelerar;
                @accelerardesacelerar.performed -= m_Wrapper.m_CocheActionsCallbackInterface.OnAccelerardesacelerar;
                @accelerardesacelerar.canceled -= m_Wrapper.m_CocheActionsCallbackInterface.OnAccelerardesacelerar;
            }
            m_Wrapper.m_CocheActionsCallbackInterface = instance;
            if (instance != null)
            {
                @accelerardesacelerar.started += instance.OnAccelerardesacelerar;
                @accelerardesacelerar.performed += instance.OnAccelerardesacelerar;
                @accelerardesacelerar.canceled += instance.OnAccelerardesacelerar;
            }
        }
    }
    public CocheActions @Coche => new CocheActions(this);
    private int m_keyboardSchemeIndex = -1;
    public InputControlScheme keyboardScheme
    {
        get
        {
            if (m_keyboardSchemeIndex == -1) m_keyboardSchemeIndex = asset.FindControlSchemeIndex("keyboard");
            return asset.controlSchemes[m_keyboardSchemeIndex];
        }
    }
    private int m_controlSchemeIndex = -1;
    public InputControlScheme controlScheme
    {
        get
        {
            if (m_controlSchemeIndex == -1) m_controlSchemeIndex = asset.FindControlSchemeIndex("control");
            return asset.controlSchemes[m_controlSchemeIndex];
        }
    }
    public interface ICocheActions
    {
        void OnAccelerardesacelerar(InputAction.CallbackContext context);
    }
}
