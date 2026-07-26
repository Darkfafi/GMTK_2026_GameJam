using System.Collections;
using UnityEngine;

namespace GMTK_2026
{
	public class PilotRequestState : GameSceneStateBase
	{
		[SerializeField]
		private float _resultDisplaySeconds = 3f;

		[SerializeField]
		private float _requestTimeLimit = 120f;

		private static readonly string[] PilotNames = { "Bob", "Zara", "Krix", "Reyes", "Nexus-7", "Vex", "Grok", "Sarah", "Unit-12", "Zyx" };
		private static readonly string[] PlanetNames = { "Sol", "Europa", "Titan", "Kepler-442b", "Vesta Prime", "Nyx-3" };
		private static readonly string[] ShipNames = { "Star Hopper", "Ice Breaker", "Dark Freighter", "ISV Vanguard", "Haul Master", "Deep Probe", "Pathfinder II" };

		private LandingPilotRequest _request;

		protected override void OnInit()
		{

		}

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.ChatController.DecisionMadeEvent += OnDecisionMade;
			Dependency.RequestsController.RequestExpiredEvent += OnRequestExpired;

			_request = CreateRandomRequest();

			Dependency.RequestsController.SetData(new RequestsController.CoreData()
			{
				Request = _request
			});
			Dependency.ChatController.StartConversation(_request);
		}

		protected override void OnExit(bool isSwitch)
		{
			StopAllCoroutines();
			Dependency.ChatController.DecisionMadeEvent -= OnDecisionMade;
			Dependency.RequestsController.RequestExpiredEvent -= OnRequestExpired;
			Dependency.ChatController.EndConversation();
			Dependency.RequestsController.ClearData();
			_request = null;
		}

		private LandingPilotRequest CreateRandomRequest()
		{
			CreatureEntity pilot = new CreatureEntity(Pick(PilotNames))
				.Apply(Pick(GameCatalog.Species))
				.Apply(Pick(GameCatalog.Occupations));

			PlanetEntity planet = new PlanetEntity(Pick(PlanetNames))
				.Apply(Pick(GameCatalog.CelestialBodies));

			ShipEntity ship = new ShipEntity(Pick(ShipNames));

			return new LandingPilotRequest(pilot, planet, ship, _requestTimeLimit);
		}

		private static T Pick<T>(T[] options) => options[Random.Range(0, options.Length)];

		private void OnDecisionMade(PlayerChoice choice)
		{
			if (_request == null || _request.IsResolved)
			{
				return;
			}
			_request.Resolve();

			RequestVerdict verdict = _request.Evaluate();
			bool playerApproved = choice == PlayerChoice.Approved;
			bool correct = playerApproved == verdict.IsApproved;

			string pilot = _request.Pilot?.Name;
			string decision = playerApproved ? "ACCESS" : "DECLINE";
			string why = verdict.IsApproved
				? "All survival checks passed."
				: string.Join(" ", verdict.Reasons);

			Dependency.ChatController.ShowSystemMessage(
				$"— {decision} — Clearance {(playerApproved ? "GRANTED" : "DENIED")} —");

			if (correct)
			{
				Dependency.StatsController.RegisterCorrect();
				Dependency.LogController.Log($"{pilot} — {decision} — Correct", LogLevel.Accent);
				Dependency.ToastWindow.Show("Correct decision", ToastType.Ok);
			}
			else
			{
				Dependency.StatsController.RegisterIncorrect();
				Dependency.LogController.Log($"{pilot} — {decision} — WRONG", LogLevel.Danger);
				Dependency.LogController.Log($"Reason: {why}", LogLevel.Warning);
				Dependency.ToastWindow.Show("Wrong decision — see log", ToastType.Error);
			}

			FinishRequest(choice, timedOut: false);
		}

		private void OnRequestExpired(PilotRequestBase request)
		{
			if (request != _request || request.IsResolved)
			{
				return;
			}
			request.Resolve();

			RequestVerdict verdict = request.Evaluate();
			string correctAction = verdict.IsApproved ? "ACCESS" : "DECLINE";

			Dependency.ChatController.ShowSystemMessage("— Transmission timed out — auto-declined —");
			Dependency.StatsController.RegisterIncorrect();
			Dependency.LogController.Log($"{request.Pilot?.Name} — request timed out", LogLevel.Warning);
			Dependency.LogController.Log($"Correct action was {correctAction}", LogLevel.Info);
			Dependency.ToastWindow.Show("Request timed out", ToastType.Error);

			FinishRequest(PlayerChoice.Denied, timedOut: true);
		}

		private void FinishRequest(PlayerChoice choice, bool timedOut)
		{
			// Freeze input, let the pilot get their parting line in, then move on.
			Dependency.ChatController.ShowDecisionReaction(choice, timedOut);
			Dependency.ChatController.EndConversationInputOnly();
			StartCoroutine(NextRequestAfterDelay());
		}

		private IEnumerator NextRequestAfterDelay()
		{
			yield return new WaitForSeconds(_resultDisplaySeconds);

			Dependency.ChatController.EndConversation();
			Dependency.RequestsController.RemoveRequest(_request);
			FSM_GoToNextState();
		}
	}
}
