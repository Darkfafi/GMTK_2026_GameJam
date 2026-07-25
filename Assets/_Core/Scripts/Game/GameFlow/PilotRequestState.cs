using System.Collections;
using UnityEngine;

namespace GMTK_2026
{
	public class PilotRequestState : GameSceneStateBase
	{
		[SerializeField]
		private float _resultDisplaySeconds = 2.2f;

		[SerializeField]
		private float _requestTimeLimit = 30f;

		private static readonly string[] PilotNames = { "Bob", "Zara", "Krix", "Reyes", "Nexus-7", "Vex", "Grok", "Sarah", "Unit-12", "Zyx" };
		private static readonly string[] PlanetNames = { "Sol", "Europa", "Titan", "Kepler-442b", "Vesta Prime", "Nyx-3" };
		private static readonly string[] ShipNames = { "Star Hopper", "Ice Breaker", "Dark Freighter", "ISV Vanguard", "Haul Master", "Deep Probe", "Pathfinder II" };

		protected override void OnInit()
		{

		}

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.RequestsController.RequestSubmittedEvent += OnRequestSubmitted;
			Dependency.RequestsController.RequestExpiredEvent += OnRequestExpired;

			Dependency.RequestsController.SetData(new RequestsController.CoreData()
			{
				Request = CreateRandomRequest()
			});
		}

		protected override void OnExit(bool isSwitch)
		{
			StopAllCoroutines();
			Dependency.RequestsController.RequestSubmittedEvent -= OnRequestSubmitted;
			Dependency.RequestsController.RequestExpiredEvent -= OnRequestExpired;
			Dependency.RequestsController.ClearData();
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

		private void OnRequestSubmitted(PilotRequestBase request, PlayerChoice choice)
		{
			if (request.IsResolved)
			{
				return;
			}
			request.Resolve();

			RequestVerdict verdict = request.Evaluate();
			bool playerApproved = choice == PlayerChoice.Approved;
			bool correct = playerApproved == verdict.IsApproved;

			string pilot = request.Pilot?.Name;
			string decision = choice == PlayerChoice.Approved ? "ACCESS" : "DECLINE";
			string why = verdict.IsApproved
				? "All checks passed."
				: string.Join(" ", verdict.Reasons);

			if (correct)
			{
				Dependency.StatsController.RegisterCorrect();
				Dependency.LogController.Log($"{pilot} — {decision} — Correct", LogLevel.Accent);
				Dependency.RequestsController.ShowResult(request, true, $"✓ CORRECT — {why}");
				Dependency.ToastWindow.Show("Correct decision", ToastType.Ok);
			}
			else
			{
				Dependency.StatsController.RegisterIncorrect();
				Dependency.LogController.Log($"{pilot} — {decision} — WRONG", LogLevel.Danger);
				Dependency.LogController.Log($"Reason: {why}", LogLevel.Warning);
				Dependency.RequestsController.ShowResult(request, false, $"✗ WRONG — {why}");
				Dependency.ToastWindow.Show("Wrong decision — see log", ToastType.Error);
			}

			StartCoroutine(FinishAfterDelay(request));
		}

		private void OnRequestExpired(PilotRequestBase request)
		{
			if (request.IsResolved)
			{
				return;
			}
			request.Resolve();

			RequestVerdict verdict = request.Evaluate();
			string correctAction = verdict.IsApproved ? "ACCESS" : "DECLINE";

			Dependency.StatsController.RegisterIncorrect();
			Dependency.LogController.Log($"{request.Pilot?.Name} — request timed out", LogLevel.Warning);
			Dependency.RequestsController.ShowResult(request, false, $"⏱ TIMED OUT — correct action was {correctAction}");
			Dependency.ToastWindow.Show("Request timed out", ToastType.Error);

			StartCoroutine(FinishAfterDelay(request));
		}

		private IEnumerator FinishAfterDelay(PilotRequestBase request)
		{
			yield return new WaitForSeconds(_resultDisplaySeconds);

			Dependency.RequestsController.RemoveRequest(request);
			FSM_GoToNextState();
		}
	}
}
