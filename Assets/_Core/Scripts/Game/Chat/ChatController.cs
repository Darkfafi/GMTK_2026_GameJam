using System;
using System.Collections;
using UnityEngine;

namespace GMTK_2026
{
	public class ChatController : MonoBehaviour
	{
		private const string BriefingText =
			"Pilots will hail you requesting landing clearance. <b>They will not tell you everything.</b>\n\n" +
			"<b>Interrogate them.</b> Ask what species they are, where they are from, what gear " +
			"they carry, what they are flying and where they are going. Not every pilot knows " +
			"their own details — some can only describe things.\n\n" +
			"<b>Verify in the file system.</b> The terminal on the left is yours to explore right " +
			"now. Start with <color=#00ff88>readme.md</color>, then the landing protocols. " +
			"A landing needs four conditions met — <color=#00ff88>pressure</color>, " +
			"<color=#00ff88>gravity</color>, <color=#00ff88>temperature</color> and " +
			"<color=#00ff88>composition</color> — plus a hull rated for the descent.\n\n" +
			"<b>Make the call.</b> Access or Decline  " +
			"<color=#ff4757>before the transmission times out.</color>\n\n" +
			"Take your time reading. And start the shift when you are ready";

		public event Action<PlayerChoice> DecisionMadeEvent;
		public event Action ShiftStartRequestedEvent;

		[SerializeField]
		private ChatUIWindow _uiWindow = null;

		private PilotPersona _persona;
		private PilotBrain _brain;
		private PilotRequestBase _request;
		private int _requestCounter;
		private float _unpromptedIn;
		private Coroutine _replyRoutine;

		public bool HasConversation => _request != null;

		private void OnEnable()
		{
			_uiWindow.MessageSubmittedEvent += OnMessageSubmitted;
			_uiWindow.AccessClickedEvent += OnAccessClicked;
			_uiWindow.DeclineClickedEvent += OnDeclineClicked;
			_uiWindow.StartShiftClickedEvent += OnStartShiftClicked;
		}

		private void OnDisable()
		{
			_uiWindow.MessageSubmittedEvent -= OnMessageSubmitted;
			_uiWindow.AccessClickedEvent -= OnAccessClicked;
			_uiWindow.DeclineClickedEvent -= OnDeclineClicked;
			_uiWindow.StartShiftClickedEvent -= OnStartShiftClicked;
		}

		// Overlay
		public void ShowBriefing()
		{
			_uiWindow.SetInteractable(false);
			_uiWindow.SetStartShiftButtonText("START SHIFT");
			_uiWindow.ShowBriefing(true, BriefingText);
		}

		public void ShowBriefing(string customText)
		{
			_uiWindow.SetInteractable(false);
			_uiWindow.ShowBriefing(true, customText);
		}

		public void SetStartShiftButtonText(string text)
		{
			_uiWindow.SetStartShiftButtonText(text);
		}

		public void HideBriefing()
		{
			_uiWindow.ShowBriefing(false);
		}

		private void OnStartShiftClicked() => ShiftStartRequestedEvent?.Invoke();

		public void StartConversation(PilotPersona pilotPersona)
		{
			StopAllCoroutines();
			_request = pilotPersona.Request;
			_persona = pilotPersona;
			_brain = new PilotBrain(_persona);
			_requestCounter++;

			_uiWindow.ClearMessages();
			_uiWindow.SetRequestId($"REQ-{_requestCounter:0000}");
			_uiWindow.SetTimer(_request.TimeRemaining);
			_uiWindow.SetInteractable(true);
			_uiWindow.AddMessage(ChatSpeaker.System, "— Incoming transmission —");

			ScheduleUnprompted();
			StartCoroutine(SayAfter(UnityEngine.Random.Range(0.6f, 1.2f), _brain.GetIntroLine()));
			_uiWindow.FocusInput();
		}

		public void EndConversation()
		{
			StopAllCoroutines();
			_replyRoutine = null;
			_uiWindow.ShowTyping(false);
			_uiWindow.SetInteractable(false);
			_request = null;
		}

		public void EndConversationInputOnly()
		{
			_uiWindow.SetInteractable(false);
			_request = null;
		}

		public void ShowSystemMessage(string message)
		{
			_uiWindow.AddMessage(ChatSpeaker.System, message);
		}

		public void ShowDecisionReaction(PlayerChoice choice)
		{
			StartCoroutine(SayAfter(0.6f, _brain.GetDecisionLine(choice)));
		}

		private void Update()
		{
			if (_request == null)
			{
				return;
			}

			_uiWindow.SetTimer(_request.TimeRemaining);

			// Idle chatter when the operator goes quiet.
			_unpromptedIn -= Time.deltaTime;
			if (_unpromptedIn <= 0f && _replyRoutine == null)
			{
				ScheduleUnprompted();
				string line = _brain.GetUnpromptedLine();
				_replyRoutine = StartCoroutine(ReplyRoutine(line, UnityEngine.Random.Range(0.8f, 1.5f)));
			}
		}

		private void OnMessageSubmitted(string text)
		{
			if (_request == null)
			{
				return;
			}

			_uiWindow.AddMessage(ChatSpeaker.Operator, EscapeRichText(text));
			ScheduleUnprompted(); // Prevent Idle Chatting when spoken to

			BrainResult result = _brain.Interpret(text);
			string reply = ComposeReply(result);
			float delay = ComputeDelay(result);

			if (_replyRoutine != null)
			{
				StopCoroutine(_replyRoutine);
			}
			_replyRoutine = StartCoroutine(ReplyRoutine(reply, delay));
		}

		private void OnAccessClicked() => DecisionMadeEvent?.Invoke(PlayerChoice.Approved);
		private void OnDeclineClicked() => DecisionMadeEvent?.Invoke(PlayerChoice.Denied);
		private string ComposeReply(BrainResult result)
		{
			switch (result.Kind)
			{
				case BrainResult.ResultKind.Clarify:
					return _brain.GetClarifyLine();

				case BrainResult.ResultKind.Answer:
					string combined = string.Empty;
					for (int i = 0; i < result.Topics.Count; i++)
					{
						string part = _brain.GetAnswerLine(result.Topics[i]);
						if (!string.IsNullOrEmpty(part) && part != "...")
						{
							combined = combined.Length > 0 ? $"{combined} {part}" : part;
						}
					}
					return combined.Length > 0 ? combined : _brain.GetFallbackLine();

				default:
					return _brain.GetFallbackLine();
			}
		}

		private float ComputeDelay(BrainResult result)
		{
			switch (result.Kind)
			{
				case BrainResult.ResultKind.Clarify:
					return UnityEngine.Random.Range(0.9f, 1.8f);
				case BrainResult.ResultKind.Answer when result.AsksAboutUnknown:
					return UnityEngine.Random.Range(2.5f, 4.5f);
				case BrainResult.ResultKind.Answer:
					return UnityEngine.Random.Range(0.8f, 1.8f) + _persona.Nervousness * 0.8f;
				default:
					return UnityEngine.Random.Range(1.5f, 3f);
			}
		}

		private IEnumerator ReplyRoutine(string line, float delay)
		{
			_uiWindow.ShowTyping(true);
			yield return new WaitForSeconds(delay);
			_uiWindow.ShowTyping(false);
			_uiWindow.AddMessage(ChatSpeaker.Pilot, AddStatic(line));
			_replyRoutine = null;
		}

		private IEnumerator SayAfter(float delay, string line)
		{
			yield return new WaitForSeconds(delay);
			_uiWindow.AddMessage(ChatSpeaker.Pilot, AddStatic(line));
		}

		private void ScheduleUnprompted()
		{
			_unpromptedIn = UnityEngine.Random.Range(12f, 34f);
		}

		private static string AddStatic(string text)
		{
			if (UnityEngine.Random.value <= 0.88f)
			{
				return text;
			}

			string[] words = text.Split(' ');
			int position = UnityEngine.Random.Range(0, words.Length);
			words[position] = "<i><color=#5a6f8a>[static]</color></i> " + words[position];
			return string.Join(" ", words);
		}

		private static string EscapeRichText(string text)
			=> text.Replace("<", "<noparse><</noparse>");
	}
}
