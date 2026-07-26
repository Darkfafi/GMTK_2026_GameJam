using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMTK_2026
{
	public enum ChatSpeaker
	{
		Pilot,
		Operator,
		System,
	}

	public class ChatUIWindow : MonoBehaviour
	{
		public event Action<string> MessageSubmittedEvent;
		public event Action AccessClickedEvent;
		public event Action DeclineClickedEvent;
		public event Action StartShiftClickedEvent;

		[SerializeField]
		private UIDocument _document = null;

		private VisualElement _signal;
		private Label _requestId;
		private Label _timer;
		private ScrollView _messages;
		private VisualElement _typingRow;
		private TextField _input;
		private Button _send;
		private Button _access;
		private Button _decline;
		private VisualElement _briefing;
		private Label _briefingBody;
		private Button _startShift;

		private readonly VisualElement[] _signalBars = new VisualElement[5];

		private void OnEnable()
		{
			VisualElement root = _document.rootVisualElement;

			_signal = root.Q<VisualElement>("chat-signal");
			_requestId = root.Q<Label>("chat-id");
			_timer = root.Q<Label>("chat-timer");
			_messages = root.Q<ScrollView>("chat-messages");
			_typingRow = root.Q<VisualElement>("chat-typing");
			_input = root.Q<TextField>("chat-input");
			_send = root.Q<Button>("chat-send");
			_access = root.Q<Button>("chat-access");
			_decline = root.Q<Button>("chat-decline");
			_briefing = root.Q<VisualElement>("chat-briefing");
			_briefingBody = root.Q<Label>("briefing-body");
			_startShift = root.Q<Button>("chat-start-shift");

			if (_startShift != null)
			{
				_startShift.clicked += OnStartShiftClicked;
			}

			BuildSignalBars();
			BuildTypingBubble();

			if (_send != null)
			{
				_send.clicked += SubmitInput;
			}
			if (_access != null)
			{
				_access.clicked += OnAccessClicked;
			}
			if (_decline != null)
			{
				_decline.clicked += OnDeclineClicked;
			}
			if (_input != null)
			{
				_input.RegisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
				root.RegisterCallback<KeyDownEvent>(OnGlobalKeyDown, TrickleDown.TrickleDown);
			}

			ShowTyping(false);
			SetInteractable(false);
		}

		private void OnDisable()
		{
			if (_send != null)
			{
				_send.clicked -= SubmitInput;
			}
			if (_access != null)
			{
				_access.clicked -= OnAccessClicked;
			}
			if (_decline != null)
			{
				_decline.clicked -= OnDeclineClicked;
			}
			if (_startShift != null)
			{
				_startShift.clicked -= OnStartShiftClicked;
			}
			if (_input != null)
			{
				_input.UnregisterCallback<KeyDownEvent>(OnInputKeyDown, TrickleDown.TrickleDown);
				_document.rootVisualElement?.UnregisterCallback<KeyDownEvent>(OnGlobalKeyDown, TrickleDown.TrickleDown);
			}
		}

		public void AddMessage(ChatSpeaker speaker, string richText)
		{
			if (_messages == null)
			{
				return;
			}

			VisualElement row = new VisualElement { pickingMode = PickingMode.Ignore };
			row.AddToClassList("msg");

			if (speaker == ChatSpeaker.System)
			{
				row.AddToClassList("msg--sys");
			}
			else
			{
				bool isPilot = speaker == ChatSpeaker.Pilot;
				row.AddToClassList(isPilot ? "msg--pilot" : "msg--op");

				Label avatar = new Label(isPilot ? "📡" : "🎧") { pickingMode = PickingMode.Ignore };
				avatar.AddToClassList("msg-av");
				row.Add(avatar);
			}

			Label bubble = new Label(richText) { pickingMode = PickingMode.Ignore, enableRichText = true };
			bubble.AddToClassList("msg-b");
			row.Add(bubble);

			_messages.Add(row);

			// slide/fade in, then keep the newest message in view
			row.schedule.Execute(() => row.AddToClassList("msg--in")).ExecuteLater(16);
			ScrollToBottom();
		}

		public void ClearMessages()
		{
			_messages?.Clear();
			ShowTyping(false);
		}

		public void ShowTyping(bool show)
		{
			if (_typingRow != null)
			{
				_typingRow.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
			}
		}

		private void ScrollToBottom()
		{
			_messages.schedule.Execute(() =>
			{
				if (_messages.verticalScroller != null)
				{
					_messages.verticalScroller.value = _messages.verticalScroller.highValue;
				}
			}).ExecuteLater(32);
		}

		public void SetRequestId(string id)
		{
			if (_requestId != null)
			{
				_requestId.text = id;
			}
		}

		public void SetTimer(float secondsRemaining)
		{
			if (_timer == null)
			{
				return;
			}

			int total = Mathf.Max(0, Mathf.CeilToInt(secondsRemaining));
			_timer.text = $"{total / 60}:{total % 60:00}";

			_timer.RemoveFromClassList("chat-timer--warn");
			_timer.RemoveFromClassList("chat-timer--crit");
			if (total <= 10)
			{
				_timer.AddToClassList("chat-timer--crit");
			}
			else if (total <= 30)
			{
				_timer.AddToClassList("chat-timer--warn");
			}
		}

		public void SetInteractable(bool interactable)
		{
			_input?.SetEnabled(interactable);
			_send?.SetEnabled(interactable);
			_access?.SetEnabled(interactable);
			_decline?.SetEnabled(interactable);
		}

		public void FocusInput()
		{
			if (_input == null)
			{
				return;
			}

			_input.schedule.Execute(() =>
			{
				_input.Blur();
				_input.Focus();
				int end = _input.value?.Length ?? 0;
				_input.SelectRange(end, end);
			}).ExecuteLater(16);
		}

		private void OnInputKeyDown(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
			{
				evt.StopPropagation();
				SubmitInput();
			}
		}

		private void OnGlobalKeyDown(KeyDownEvent evt)
		{
			if (_input == null || !_input.enabledInHierarchy || IsInputFocused())
			{
				return;
			}

			if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
			{
				evt.StopPropagation();
				SubmitInput();
				return;
			}

			if (evt.character == '\0' || char.IsControl(evt.character) || evt.ctrlKey || evt.altKey || evt.commandKey)
			{
				return;
			}

			evt.StopPropagation();
			_input.value += evt.character;
			FocusInput();
		}

		private bool IsInputFocused()
		{
			Focusable focused = _input.panel?.focusController?.focusedElement;
			return focused == _input || (focused is VisualElement element && _input.Contains(element));
		}

		private void SubmitInput()
		{
			if (_input == null)
			{
				return;
			}

			string text = _input.value?.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			_input.value = string.Empty;
			MessageSubmittedEvent?.Invoke(text);
			FocusInput();
		}

		private void OnAccessClicked() => AccessClickedEvent?.Invoke();
		private void OnDeclineClicked() => DeclineClickedEvent?.Invoke();
		private void OnStartShiftClicked() => StartShiftClickedEvent?.Invoke();

		public void ShowBriefing(bool show, string body = null)
		{
			if (_briefing != null)
			{
				_briefing.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
			}
			if (show && body != null && _briefingBody != null)
			{
				_briefingBody.enableRichText = true;
				_briefingBody.text = TerminalMarkdown.ToRichText(body);
			}
		}

		public void SetStartShiftButtonText(string text)
		{
			if (_startShift != null)
			{
				_startShift.text = text;
			}
		}

		private void BuildSignalBars()
		{
			if (_signal == null)
			{
				return;
			}

			_signal.Clear();
			for (int i = 0; i < _signalBars.Length; i++)
			{
				VisualElement bar = new VisualElement { pickingMode = PickingMode.Ignore };
				bar.AddToClassList("sig-bar");
				bar.style.height = 4 + i * 3;
				_signalBars[i] = bar;
				_signal.Add(bar);
			}

			_signal.schedule.Execute(RandomizeSignal).Every(3000);
			RandomizeSignal();
		}

		private void RandomizeSignal()
		{
			int strength = 3 + UnityEngine.Random.Range(0, 3);
			for (int i = 0; i < _signalBars.Length; i++)
			{
				_signalBars[i].EnableInClassList("sig-bar--on", i < strength);
			}
		}

		private void BuildTypingBubble()
		{
			if (_typingRow == null)
			{
				return;
			}

			_typingRow.Clear();

			Label avatar = new Label("📡") { pickingMode = PickingMode.Ignore };
			avatar.AddToClassList("msg-av");
			_typingRow.Add(avatar);
			_typingRow.Add(new TypingIndicatorElement());
		}
	}
}
