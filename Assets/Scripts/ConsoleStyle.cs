using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConsoleGUI
{
	public class ConsoleStyle
	{
		private int _childControlHeight = 50;
		private int _childControlWidth = 50;

		private bool _isSetConsoleSize = false;


		// 操作窗口矩形

		private Rect _optTopCenterWindowRect;

		private GUIStyle _btnToggleOffStyle = null;



		public int childWidth => _childControlWidth;
		public int childHeight => _childControlHeight;


		public GUIStyle bntToggleOffStyle => _btnToggleOffStyle;


		public Rect optTopCenterWindowRect => _optTopCenterWindowRect;

		private int _lastScreenWidth = 0;
		private int _lastScreenHeight = 0;
		public void Update()
		{
			if (!_isSetConsoleSize || (_lastScreenWidth != Screen.width) || (_lastScreenHeight != Screen.height))
				RefreshSize();
		}

		private void RefreshSize()
		{
			_isSetConsoleSize = true;
			_lastScreenWidth = Screen.width;
			_lastScreenHeight = Screen.height;

			var screenWidth = Screen.width;
			var screenHeight = Screen.height;

			_optTopCenterWindowRect = new Rect(Vector2.zero, new Vector2(screenWidth / 8, screenHeight / 10));
			_optTopCenterWindowRect.center = new Vector2(screenWidth / 3, screenHeight / 20);


			float heightScale = screenHeight * 1.0f / 720;
			_childControlHeight = Mathf.FloorToInt(_childControlHeight * heightScale);
			_childControlWidth = screenWidth / 6;

			GUI.skin.button.fontSize = (int)(20 * heightScale);
			GUI.skin.textField.fontSize = (int)(20 * heightScale);
			GUI.skin.label.fontSize = (int)(20 * heightScale);
			GUI.skin.textField.alignment = TextAnchor.MiddleCenter;


			GUI.skin.verticalScrollbarThumb.fixedWidth = 30;
			GUI.skin.verticalScrollbar.fixedWidth = GUI.skin.verticalScrollbarThumb.fixedWidth + 2;
			GUI.skin.horizontalScrollbar.fixedHeight = GUI.skin.horizontalScrollbarThumb.fixedHeight + 2;

			_btnToggleOffStyle = new GUIStyle(GUI.skin.button);
			_btnToggleOffStyle.normal.background = null;
			_btnToggleOffStyle.onNormal.background = null;
			_btnToggleOffStyle.onActive.background = null;
			_btnToggleOffStyle.onHover.background = null;
			_btnToggleOffStyle.onFocused.background = null;
		}

        public bool DrawToggle(bool toggle, string text)
        {
            if (GUILayout.Button(text, toggle ? GUI.skin.button : bntToggleOffStyle,
            GUILayout.MaxWidth(childWidth), GUILayout.MaxHeight(childHeight)))
            {
                toggle = !toggle;
            }
            return toggle;
        }
    }

}
