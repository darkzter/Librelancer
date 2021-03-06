﻿using System;
using System.Collections.Generic;
using LibreLancer.Ini;
using LibreLancer.Dll;
namespace LibreLancer.Compatibility.GameData
{
	public class FreelancerIni : IniFile
	{
		public List<DllFile> Resources { get; private set; }
		public List<string> StartupMovies { get; private set; }

		public string DataPath { get; private set; }
		public string SolarPath { get; private set; }
		public string UniversePath { get; private set; }
		public string HudPath { get; private set; }
		public List<string> EquipmentPaths { get; private set; }
		public List<string> LoadoutPaths { get; private set; }
		public List<string> ShiparchPaths { get; private set; }
		public List<string> SoundPaths { get; private set; }
		public List<string> GraphPaths { get; private set; }
		public List<string> EffectPaths { get; private set; }
		public List<string> AsteroidPaths { get; private set; }
		public List<string> RichFontPaths { get; private set; }
		public List<string> PetalDbPaths { get; private set; }
		public string StarsPath { get; private set; }
		public string BodypartsPath { get; private set; }
		public string CostumesPath { get; private set; }
		public string EffectShapesPath { get; private set; }
		public Tuple<string, string> JsonResources { get; private set; }
		public FreelancerIni ()
		{
			EquipmentPaths = new List<string> ();
			LoadoutPaths = new List<string> ();
			ShiparchPaths = new List<string> ();
			SoundPaths = new List<string>();
			GraphPaths = new List<string>();
			EffectPaths = new List<string>();
			AsteroidPaths = new List<string> ();
			RichFontPaths = new List<string>();
			PetalDbPaths = new List<string>();
			StartupMovies = new List<string>();

			foreach (Section s in ParseFile("EXE\\freelancer.ini")) {
				switch (s.Name.ToLowerInvariant ()) {
				case "freelancer":
					foreach (Entry e in s) {
						if (e.Name.ToLowerInvariant () == "data path") {
							if (e.Count != 1)
								throw new Exception ("Invalid number of values in " + s.Name + " Entry " + e.Name + ": " + e.Count);
							if (DataPath != null)
								throw new Exception ("Duplicate " + e.Name + " Entry in " + s.Name);
							DataPath = "EXE\\" + e [0].ToString () + "\\";
						}
					}
					break;
				case "jsonresources":
					JsonResources = new Tuple<string, string>(s[0][0].ToString(), s[0][1].ToString());
					break;
				case "resources":
					Resources = new List<DllFile> ();
					//NOTE: Freelancer hardcodes resources.dll
					Resources.Add (new DllFile ("EXE\\resources.dll"));
					foreach (Entry e in s)
					{
						if (e.Name.ToLowerInvariant () != "dll")
							continue;
						Resources.Add (new DllFile ("EXE\\" + e [0]));
					}
					break;
				case "startup":
					foreach (Entry e in s) {
						if (e.Name.ToLowerInvariant () != "movie_file")
							continue;
						StartupMovies.Add (e [0].ToString());
					}
					break;
				case "data":
					foreach (Entry e in s) {
						switch (e.Name.ToLowerInvariant ()) {
						case "solar":
							SolarPath = DataPath + e [0].ToString ();
							break;
						case "universe":
							UniversePath = DataPath + e [0].ToString ();
							break;
						case "equipment":
							EquipmentPaths.Add(DataPath + e [0].ToString ());
							break;
						case "loadouts":
							LoadoutPaths.Add(DataPath + e [0].ToString ());
							break;
						case "stars":
							StarsPath = DataPath + e [0].ToString ();
							break;
						case "bodyparts":
							BodypartsPath = DataPath + e [0].ToString ();
							break;
						case "costumes":
							CostumesPath = DataPath + e [0];
							break;
						case "sounds":
							SoundPaths.Add(DataPath + e[0]);
							break;
						case "ships":
							ShiparchPaths.Add (DataPath + e [0].ToString ());
							break;
						case "rich_fonts":
							RichFontPaths.Add(DataPath + e[0].ToString());
							break;
						case "igraph":
							GraphPaths.Add(DataPath + e[0].ToString());
							break;
						case "effect_shapes":
							EffectShapesPath = DataPath + e[0].ToString();
							break;
						case "effects":
							EffectPaths.Add(DataPath + e[0].ToString());
							break;
						case "asteroids":
							AsteroidPaths.Add (DataPath + e [0].ToString ());
							break;
						case "petaldb":
							PetalDbPaths.Add(DataPath + e[0].ToString());
							break;
						case "hud":
							HudPath = DataPath + e[0].ToString();
							break;
						}
					}
					break;
				}
			}
		}
	}
}

