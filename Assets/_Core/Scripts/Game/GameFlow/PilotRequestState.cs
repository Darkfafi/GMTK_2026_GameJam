using UnityEngine;

namespace GMTK_2026
{
	public class PilotRequestState : GameSceneStateBase
	{
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

			var request = new LandingPilotRequest(
				new CreatureEntity("Bob").Apply(SpeciesAspect.Human),
				new PlanetEntity("Sol").Apply(CelestialBodyAspect.Star),
				new ShipEntity("Shuttle"));

			Dependency.RequestsController.SetData(new RequestsController.CoreData()
			{
				Request = request
			});
		}

		protected override void OnExit(bool isSwitch)
		{
			Dependency.RequestsController.RequestSubmittedEvent -= OnRequestSubmitted;
			Dependency.RequestsController.RequestExpiredEvent -= OnRequestExpired;
		}

		private void OnRequestSubmitted(PilotRequestBase request, PlayerChoice choice)
		{
			RequestVerdict verdict = request.Evaluate();
			bool playerApproved = choice == PlayerChoice.Approved;
			bool correct = playerApproved == verdict.IsApproved;

			Debug.Log($"{request.Pilot?.Name}: player {choice}, truth {(verdict.IsApproved ? "Approve" : "Deny")} -> {(correct ? "CORRECT" : "WRONG")}");

			Dependency.RequestsController.RemoveRequest(request);
		}

		private void OnRequestExpired(PilotRequestBase request)
		{
			Debug.Log($"{request.Pilot?.Name}: request expired (missed).");
			Dependency.RequestsController.RemoveRequest(request);
		}
	}
}