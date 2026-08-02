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

		private PilotPersona _persona;

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

			_persona = CreatePilot();

			Dependency.RequestsController.SetData(new RequestsController.CoreData()
			{
				Request = _persona.Request
			});

			Dependency.ChatController.StartConversation(_persona);
		}

		protected override void OnExit(bool isSwitch)
		{
			StopAllCoroutines();
			Dependency.ChatController.DecisionMadeEvent -= OnDecisionMade;
			Dependency.RequestsController.RequestExpiredEvent -= OnRequestExpired;
			Dependency.ChatController.EndConversation();
			Dependency.RequestsController.ClearData();
			_persona = null;
		}

		private PilotPersona CreatePilot()
		{
			int processed = Dependency.StatsController.RequestsProcessed;

			// -- Pilot --
			// What species is the Pilot
			SpeciesAspect species = Pick(GameCatalog.Species);

			// Create the Pilot Creature
			CreatureEntity pilot = new CreatureEntity(Pick(PilotNames))
				.Apply(species)
				.Apply(Pick(GameCatalog.Occupations));


			// -- Target --
			// First Request & 25% returning to their home planet. (No Gear Needed)
			// Rest require investigation by the user 
			CelestialBodyAspect body = processed == 0 || Random.value < 0.25f
				? (GameCatalog.FindBody(species.Origin) ?? Pick(GameCatalog.CelestialBodies))
				: Pick(GameCatalog.CelestialBodies);

			PlanetEntity planet = new PlanetEntity(body.Name).Apply(body);

			// -- Gear --
			foreach (EquipmentAspect equipment in RollEquipment(species))
			{
				pilot.Apply(equipment);
			}

			ShipEntity ship = new ShipEntity(Pick(ShipNames)).Apply(RollShipClass(body));

			// -- Brain & Intention --
			LandingPilotRequest req = new LandingPilotRequest(pilot, planet, ship, _requestTimeLimit);
			float? clarity;
			float? cooperation;
			float? nervousness;
			Dictionary<ChatTopic, float> customKnowledge = new Dictionary<ChatTopic, float>();

			// 1st, 2nd, 3rd pilots
			if (processed < 3)
			{
				clarity = Random.Range(0.85f, 1.0f);
				cooperation = Random.Range(0.85f, 1.0f);
				nervousness = Random.Range(0.0f, 0.2f);

				// They know everything perfectly
				customKnowledge[ChatTopic.Species] = 1f;
				customKnowledge[ChatTopic.ShipClass] = 1f;
				customKnowledge[ChatTopic.Equipment] = 1f;
				customKnowledge[ChatTopic.Needs] = 1f;
				customKnowledge[ChatTopic.Body] = 1f;
				customKnowledge[ChatTopic.Occupation] = 1f;
			}
			// 4th, 5th, 6th pilots
			else if (processed >= 3 && processed < 6)
			{
				clarity = Random.Range(0.35f, 0.6f);
				cooperation = Random.Range(0.35f, 0.6f);
				nervousness = Random.Range(0.3f, 0.7f);

				// Amnesia/vague: roll random fields to be vague (0.5f) or unknown (0f)
				customKnowledge[ChatTopic.Species] = Random.value < 0.5f ? 0.5f : 0f;
				customKnowledge[ChatTopic.ShipClass] = Random.value < 0.5f ? 0.5f : 0f;
				customKnowledge[ChatTopic.Equipment] = Random.value < 0.5f ? 0.5f : 0f;
				customKnowledge[ChatTopic.Needs] = Random.value < 0.5f ? 0.5f : 0f;
				customKnowledge[ChatTopic.Body] = Random.value < 0.5f ? 0.5f : 0f;
			}
			else // isHard
			{
				clarity = Random.Range(0.2f, 0.5f);
				cooperation = Random.Range(0.2f, 0.5f);
				nervousness = Random.Range(0.6f, 0.95f);

				// Normal hard pilot (very amnesiac and uncooperative)
				customKnowledge[ChatTopic.Species] = 0f;
				customKnowledge[ChatTopic.ShipClass] = 0f;
				customKnowledge[ChatTopic.Equipment] = 0f;
				customKnowledge[ChatTopic.Needs] = 0f;
				customKnowledge[ChatTopic.Body] = 0f;
				customKnowledge[ChatTopic.Occupation] = 0f;
			}

			return new PilotPersona(req, clarity, cooperation, nervousness, customKnowledge);
		}

		private static ShipAspect RollShipClass(CelestialBodyAspect body)
		{
			// 35% of the Pilots just grab a ship at random
			if (Random.value < 0.35f)
			{
				return Pick(GameCatalog.ShipClasses);
			}

			// 75% of the Pilots actually choose a Ship safe to land on target planet
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

			// 35% of the pilots bring no gear
			if (Random.value < 0.35f)
			{
				return carried;
			}

			List<EquipmentAspect> certified = GameCatalog.Equipment
				.Where(e => e.CanBeEquippedBy(species)).ToList();

			int pieces = Random.value < 0.25f ? 2 : 1;
			for (int i = 0; i < pieces; i++)
			{
				// 75% use equipment they are allowed to use
				// 25% are using uncertified equipment
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
			if (_persona == null || _persona.Request is not PilotRequestBase request || request.IsResolved)
			{
				return;
			}
			request.Resolve();

			RequestVerdict verdict = request.Evaluate();
			bool playerApproved = choice == PlayerChoice.Approved;
			bool correct = playerApproved == verdict.IsApproved;

			string pilot = request.Pilot?.Name;
			string decision = playerApproved ? "ACCESS" : "DECLINE";
			string why = verdict.IsApproved
				? "All survival checks passed."
				: string.Join(" ", verdict.Reasons);

			Dependency.ChatController.ShowSystemMessage(
				$"— {decision} — Clearance {(playerApproved ? "GRANTED" : "DENIED")} —");

			if (correct)
			{
				float elapsedSeconds = request.TimeLimit - request.TimeRemaining;
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

			FinishRequest(choice);
		}

		private void OnRequestExpired(PilotRequestBase request)
		{
			if (request != _persona.Request || request.IsResolved)
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

			FinishRequest(PlayerChoice.None);
		}

		private void FinishRequest(PlayerChoice choice)
		{
			// Freeze input, let the pilot get their parting line in, then move on.
			Dependency.ChatController.ShowDecisionReaction(choice);
			Dependency.ChatController.EndConversationInputOnly();
			StartCoroutine(NextRequestAfterDelay());
		}

		private IEnumerator NextRequestAfterDelay()
		{
			yield return new WaitForSeconds(_resultDisplaySeconds);

			Dependency.ChatController.EndConversation();
			Dependency.RequestsController.RemoveRequest(_persona.Request);

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