using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK_2026
{
	public class PilotRequestState : GameSceneStateBase
	{
		[SerializeField]
		private float _resultDisplaySeconds = 3f;

		[SerializeField]
		private float _requestTimeLimit = 180f;

		private static readonly string[] PilotNames = { "Bob", "Zara", "Krix", "Reyes", "Nexus-7", "Vex", "Grok", "Sarah", "Unit-12", "Zyx", "Marla", "Oto", "Ferren", "Sil" };
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
			int processed = Dependency.StatsController.RequestsProcessed; // 0 to 7
			bool isEasy = processed < 3;       // 1st, 2nd, 3rd pilots
			bool isMedium = processed >= 3 && processed < 6; // 4th, 5th, 6th pilots
			bool isHard = processed >= 6;      // 7th, 8th pilots (lies possible)

			SpeciesAspect species = Pick(GameCatalog.Species);

			CreatureEntity pilot = new CreatureEntity(Pick(PilotNames))
				.Apply(species)
				.Apply(Pick(GameCatalog.Occupations));

			// A quarter of runs are homecomings: trivially survivable, no gear needed.
			// The rest are real puzzles somewhere else in the system.
			CelestialBodyAspect body = Random.value < 0.25f
				? (GameCatalog.FindBody(species.Origin) ?? Pick(GameCatalog.CelestialBodies))
				: Pick(GameCatalog.CelestialBodies);

			PlanetEntity planet = new PlanetEntity(body.Name).Apply(body);

			foreach (EquipmentAspect equipment in RollEquipment(species))
			{
				pilot.Apply(equipment);
			}

			ShipEntity ship = new ShipEntity(Pick(ShipNames)).Apply(RollShipClass(body));

			LandingPilotRequest req = new LandingPilotRequest(pilot, planet, ship, _requestTimeLimit);

			// Scale difficulty
			if (isEasy)
			{
				req.Clarity = Random.Range(0.85f, 1.0f);
				req.Cooperation = Random.Range(0.85f, 1.0f);
				req.Nervousness = Random.Range(0.0f, 0.2f);

				// They know everything perfectly
				req.CustomKnowledge[ChatTopic.Species] = 1f;
				req.CustomKnowledge[ChatTopic.ShipClass] = 1f;
				req.CustomKnowledge[ChatTopic.Equipment] = 1f;
				req.CustomKnowledge[ChatTopic.Needs] = 1f;
				req.CustomKnowledge[ChatTopic.Body] = 1f;
				req.CustomKnowledge[ChatTopic.Occupation] = 1f;
			}
			else if (isMedium)
			{
				req.Clarity = Random.Range(0.35f, 0.6f);
				req.Cooperation = Random.Range(0.35f, 0.6f);
				req.Nervousness = Random.Range(0.3f, 0.7f);

				// Amnesia/vague: roll random fields to be vague (0.5f) or unknown (0f)
				req.CustomKnowledge[ChatTopic.Species] = Random.value < 0.5f ? 0.5f : 0f;
				req.CustomKnowledge[ChatTopic.ShipClass] = Random.value < 0.5f ? 0.5f : 0f;
				req.CustomKnowledge[ChatTopic.Equipment] = Random.value < 0.5f ? 0.5f : 0f;
				req.CustomKnowledge[ChatTopic.Needs] = Random.value < 0.5f ? 0.5f : 0f;
				req.CustomKnowledge[ChatTopic.Body] = Random.value < 0.5f ? 0.5f : 0f;
			}
			else // isHard
			{
				req.Clarity = Random.Range(0.2f, 0.5f);
				req.Cooperation = Random.Range(0.2f, 0.5f);
				req.Nervousness = Random.Range(0.6f, 0.95f);

				// Normal hard pilot (very amnesiac and uncooperative)
				req.CustomKnowledge[ChatTopic.Species] = 0f;
				req.CustomKnowledge[ChatTopic.ShipClass] = 0f;
				req.CustomKnowledge[ChatTopic.Equipment] = 0f;
				req.CustomKnowledge[ChatTopic.Needs] = 0f;
				req.CustomKnowledge[ChatTopic.Body] = 0f;
				req.CustomKnowledge[ChatTopic.Occupation] = 0f;
			}

			return req;
		}

		private static ShipAspect RollShipClass(CelestialBodyAspect body)
		{
			if (Random.value < 0.35f)
			{
				return Pick(GameCatalog.ShipClasses);
			}

			List<ShipAspect> rated = GameCatalog.ShipClasses.Where(shipClass =>
				Covers(shipClass.Hull.Pressure, body.Environment.Pressure) &&
				Covers(shipClass.Hull.Gravity, body.Environment.Gravity) &&
				Covers(shipClass.Hull.Temperature, body.Environment.AverageTemperature)).ToList();

			return rated.Count > 0 ? rated[Random.Range(0, rated.Count)] : Pick(GameCatalog.ShipClasses);
		}

		private static bool Covers(FloatRange? range, float value)
			=> range.HasValue && range.Value.Contains(value);

		private static List<EquipmentAspect> RollEquipment(SpeciesAspect species)
		{
			List<EquipmentAspect> carried = new List<EquipmentAspect>();
			if (Random.value < 0.35f)
			{
				return carried; // No equipment
			}

			List<EquipmentAspect> certified = GameCatalog.Equipment
				.Where(e => e.CanBeEquippedBy(species)).ToList();

			int pieces = Random.value < 0.25f ? 2 : 1;
			for (int i = 0; i < pieces; i++)
			{
				bool useCertified = certified.Count > 0 && Random.value < 0.75f;
				EquipmentAspect pick = useCertified
					? certified[Random.Range(0, certified.Count)]
					: Pick(GameCatalog.Equipment);

				if (!carried.Contains(pick))
				{
					carried.Add(pick);
				}
			}
			return carried;
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
				float elapsedSeconds = _request.TimeLimit - _request.TimeRemaining;
				Dependency.StatsController.RegisterCorrect(elapsedSeconds);
				Dependency.LogController.Log($"{pilot} — {decision} — Correct", LogLevel.Accent);
				Dependency.ToastWindow.Show("Correct decision", ToastType.Ok);
			}
			else
			{
				Dependency.StatsController.RegisterIncorrect(pilot, decision, why);
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

			Dependency.StatsController.RegisterTimeout(request.Pilot?.Name, correctAction);
			Dependency.ChatController.ShowSystemMessage("— Transmission timed out — auto-declined —");
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

			SessionStatsController stats = Dependency.StatsController;
			if (stats.Mistakes >= 3)
			{
				TriggerCatastrophicFailure();
			}
			else if (stats.RequestsProcessed >= 8)
			{
				TriggerShiftComplete();
			}
			else
			{
				FSM_GoToNextState();
			}
		}

		private void TriggerCatastrophicFailure()
		{
			Dependency.ChatController.ShowSystemMessage("[SYSTEM CRITICAL] EMERGENCY QUARANTINE INITIATED.");
			Dependency.LogController.Log("SYSTEM MALFUNCTION: CRITICAL CATASTROPHIC FAILURE.", LogLevel.Danger);
			Dependency.LogController.Log("Emergency quarantine active. Sector seal breached.", LogLevel.Danger);

			string summary = GetMistakesSummary();
			string ratingText =
@"<size=18><color=#ff4757><b>GAME OVER</b></color></size>

**CRITICAL FAILURE — SHIFT TERMINATED**

The station has suffered catastrophic failure after sustaining **3 severe procedural violations**. Your landing operator clearance has been revoked.

### [STATS] Shift Summary:
- **Cleared Pilots:** " + Dependency.StatsController.Correct + @"
- **Total Score:** " + Dependency.StatsController.Score + @" pts
- **Integrity Level:** 0%

### [INCIDENTS] Incident Report (What Went Wrong):
" + summary + @"

*Station Alpha has been locked down. Operator must re-certify.*";

			Dependency.ChatController.SetStartShiftButtonText("RETRY SHIFT");
			Dependency.ChatController.ShiftStartRequestedEvent += OnRestartShift;
			Dependency.ChatController.ShowBriefing(ratingText);
		}

		private void TriggerShiftComplete()
		{
			SessionStatsController stats = Dependency.StatsController;
			string rating = "B Rank";
			if (stats.Mistakes == 0) rating = "S Rank (PERFECT)";
			else if (stats.Mistakes == 1) rating = "A Rank (OUTSTANDING)";

			string summary = GetMistakesSummary();
			string ratingText =
@"# [SUCCESS] SHIFT COMPLETED

**RATING: " + rating + @"**

Congratulations, Operator. You have successfully completed your shift quota of **8 requests**.

### [STATS] Performance Report:
- **Rating:** " + rating + @"
- **Cleared Pilots:** " + stats.Correct + @"
- **Total Score:** " + stats.Score + @" pts
- **Station Integrity:** " + (100 - (stats.Mistakes * 33)) + @"%

### [INCIDENTS] Incident Log (What Went Wrong):
" + (stats.Mistakes == 0 ? "*Perfect run. No incidents recorded.*" : summary) + @"

*Your performance has been transmitted to Sector Command.*";

			Dependency.ChatController.SetStartShiftButtonText("START NEW SHIFT");
			Dependency.ChatController.ShiftStartRequestedEvent += OnRestartShift;
			Dependency.ChatController.ShowBriefing(ratingText);
		}

		private void OnRestartShift()
		{
			Dependency.ChatController.ShiftStartRequestedEvent -= OnRestartShift;
			Dependency.ChatController.HideBriefing();
			Dependency.StatsController.ResetStats();
			Dependency.LogController.Clear();
			FSM_GoToNextState();
		}

		private string GetMistakesSummary()
		{
			if (Dependency.StatsController.MistakeLogs.Count == 0)
			{
				return "*No mistakes recorded.*";
			}
			return string.Join("\n\n", Dependency.StatsController.MistakeLogs);
		}
	}
}