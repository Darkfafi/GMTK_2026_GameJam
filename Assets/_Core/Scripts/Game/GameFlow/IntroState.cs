using System.Linq;
using System.Text;

namespace GMTK_2026
{
	public class IntroState : GameSceneStateBase
	{
		protected override void OnInit()
		{
			var root = new RaFolder("Home",
				new RaFolder("Documents",
					new RaFile("landing_protocols.md", LandingProtocols()),
					new RaFile("species_registry.md", SpeciesRegistry()),
					new RaFile("celestial_bodies.md", CelestialBodies()),
					new RaFile("occupation_permits.md", OccupationPermits()),
					new RaFile("recent_incidents.md", RecentIncidents())
				),
				new RaFolder("Photos",
					new RaFile("mars_surface.jpg", "[IMAGE: Mars surface panorama — rust-red terrain, distant Olympus Mons silhouette, thin atmospheric haze]"),
					new RaFile("station_alpha.jpg", "[IMAGE: Orbital Station Alpha exterior — modular ring structure, 12 docking bays, Earth in background]"),
					new RaFile("crew_2187.jpg", "[IMAGE: Station crew portrait — 12 members in uniform, Commander Reyes center, dated 2187.01.15]")
				),
				new RaFile("readme.md", Readme())
			);

			Dependency.FileSystemController.SetData(root);
		}

		protected override void OnDeinit()
		{

		}

		protected override void OnEnter()
		{
			Dependency.LogController.Log("Terminal initialized. Station Alpha File System loaded.", LogLevel.Info);
			Dependency.LogController.Log("Incoming requests queued. Awaiting operator input.", LogLevel.Info);
			FSM_GoToNextState();
		}

		protected override void OnExit(bool isSwitch)
		{

		}

		private static string Tags(System.Collections.Generic.IEnumerable<TagBase> tags)
			=> string.Join(", ", tags.Select(t => t.Name));

		private static string LandingProtocols()
		{
			return
@"# Landing Protocols

Every landing request must be verified against station records before responding.

## Verification Procedure
- Identify the pilot's species in the species registry
- Identify the planet's body type in the celestial bodies index
- Compare: every REQUIRED condition of the species must be present on the planet
- Any FATAL condition present on the planet means landing is DENIED
- Check occupation permits — certified equipment can negate specific hazards

## Rulings
- All requirements met, no fatal exposure: landing PERMITTED
- Missing requirement or fatal exposure: landing DENIED

When in doubt, check the files.";
		}

		private string SpeciesRegistry()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Species Registry");
			sb.AppendLine();

			foreach (SpeciesAspect species in GameCatalog.Species)
			{
				sb.AppendLine($"## {species.Name}");
				sb.AppendLine(species.Description);
				if (species.Requires.Count > 0)
				{
					sb.AppendLine($"- REQUIRED environment: {Tags(species.Requires)}");
				}
				if (species.Intolerances.Count > 0)
				{
					sb.AppendLine($"- FATAL exposure: {Tags(species.Intolerances)}");
				}
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private string CelestialBodies()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Celestial Bodies Index");
			sb.AppendLine();

			foreach (CelestialBodyAspect body in GameCatalog.CelestialBodies)
			{
				sb.AppendLine($"## {body.Name}");
				sb.AppendLine(body.Description);
				if (body.Provides.Count > 0)
				{
					sb.AppendLine($"- Surface conditions: {Tags(body.Provides)}");
				}
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private string OccupationPermits()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("# Occupation Permits");
			sb.AppendLine();
			sb.AppendLine("Certified equipment negates specific environmental hazards for the holder.");
			sb.AppendLine();

			foreach (OccupationAspect occupation in GameCatalog.Occupations)
			{
				sb.AppendLine($"## {occupation.Name}");
				sb.AppendLine(occupation.Description);
				sb.AppendLine(occupation.Removes.Count > 0
					? $"- Negates hazard: {Tags(occupation.Removes)}"
					: "- No environmental certification");
				sb.AppendLine();
			}

			return sb.ToString().TrimEnd();
		}

		private static string RecentIncidents()
		{
			return
@"# Recent Incidents

## 2187-03-15
Volcan pilot approved for an Ocean World landing.
Vessel lost. Operator reprimanded — Water exposure is FATAL to Volcans.

## 2187-03-12
Aquatoid landing on a Star correctly DENIED by Operator Tanaka.
No water present; extreme heat exposure.

## 2187-03-08
Vacuum Worker cleared for Ice Dwarf landing despite vacuum conditions.
Certified equipment negated the hazard. Correct ruling.

## 2187-03-05
Silathi pilot denied Oxygen-rich landing.
Correct ruling — Oxygen exposure is FATAL to Silathi.";
		}

		private static string Readme()
		{
			return
@"# Station Alpha — File System

Welcome, Operator.

This terminal contains all operational data needed to process incoming landing requests.

## Directory Structure
- Documents/ — Protocols, registries, and indexes
- Photos/ — Reference imagery (not operationally relevant)

## Your Responsibility
You are the final authority on landing approvals.
Check the rules carefully before deciding.

When in doubt, check the files.";
		}
	}
}
