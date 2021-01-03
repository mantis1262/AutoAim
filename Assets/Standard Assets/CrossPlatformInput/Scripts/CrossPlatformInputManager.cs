using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

namespace UnityStandardAssets.CrossPlatformInput
{
	public static class CrossPlatformInputManager
	{
		public enum ActiveInputMethod
		{
			Hardware,
			Touch
		}


		private static VirtualInput activeInput;
		private static VirtualInput s_HardwareInput;


		static CrossPlatformInputManager()
		{
			s_HardwareInput = new StandaloneInput();
			activeInput = s_HardwareInput;
		}

		public static bool AxisExists(string name)
		{
			return activeInput.AxisExists(name);
		}

		public static void RegisterVirtualAxis(VirtualAxis axis)
		{
			activeInput.RegisterVirtualAxis(axis);
		}

		// returns a reference to a named virtual axis if it exists otherwise null
		public static VirtualAxis VirtualAxisReference(string name)
		{
			return activeInput.VirtualAxisReference(name);
		}


		// returns the platform appropriate axis for the given name
		public static float GetAxis(string name)
		{
			return GetAxis(name, false);
		}


		public static float GetAxisRaw(string name)
		{
			return GetAxis(name, true);
		}


		// private function handles both types of axis (raw and not raw)
		private static float GetAxis(string name, bool raw)
		{
			return activeInput.GetAxis(name, raw);
		}


		// -- Button handling --
		public static bool GetButton(string name)
		{
			return activeInput.GetButton(name);
		}


		public static bool GetButtonDown(string name)
		{
			return activeInput.GetButtonDown(name);
		}


		public static bool GetButtonUp(string name)
		{
			return activeInput.GetButtonUp(name);
		}


		public static void SetButtonDown(string name)
		{
			activeInput.SetButtonDown(name);
		}


		public static void SetButtonUp(string name)
		{
			activeInput.SetButtonUp(name);
		}


		public static void SetAxisPositive(string name)
		{
			activeInput.SetAxisPositive(name);
		}


		public static void SetAxisNegative(string name)
		{
			activeInput.SetAxisNegative(name);
		}


		public static void SetAxisZero(string name)
		{
			activeInput.SetAxisZero(name);
		}


		public static void SetAxis(string name, float value)
		{
			activeInput.SetAxis(name, value);
		}


		public static Vector3 mousePosition
		{
			get { return activeInput.MousePosition(); }
		}


		public static void SetVirtualMousePositionX(float f)
		{
			activeInput.SetVirtualMousePositionX(f);
		}


		public static void SetVirtualMousePositionY(float f)
		{
			activeInput.SetVirtualMousePositionY(f);
		}


		public static void SetVirtualMousePositionZ(float f)
		{
			activeInput.SetVirtualMousePositionZ(f);
		}


		// virtual axis and button classes - applies to mobile input
		// Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
		// Could also be implemented by other input devices - kinect, electronic sensors, etc
		public class VirtualAxis
		{
			public string name { get; private set; }
			private float m_Value;
			public bool matchWithInputManager { get; private set; }


			public VirtualAxis(string name)
				: this(name, true)
			{
			}


			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}



			// a controller gameobject (eg. a virtual thumbstick) should update this class
			public void Update(float value)
			{
				m_Value = value;
			}


			public float GetValue
			{
				get { return m_Value; }
			}


			public float GetValueRaw
			{
				get { return m_Value; }
			}
		}


	}
}
