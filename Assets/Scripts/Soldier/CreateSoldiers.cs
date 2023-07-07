using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CreateSoldiers : MonoBehaviour, IDataPersistence
{
	public Soldier baseSoldier;
	public SetTeamParameters setTeamParameters;
	public TMP_Dropdown terrainDropdown;
	public GameObject alpineDropdownObj, desertDropdownObj, jungleDropdownObj, urbanDropdownObj, commanderAlpineDropdownObj, commanderDesertDropdownObj, commanderJungleDropdownObj, commanderUrbanDropdownObj;
	public TMP_Dropdown alpineDropdown, desertDropdown, jungleDropdown, urbanDropdown, commanderAlpineDropdown, commanderDesertDropdown, commanderJungleDropdown, commanderUrbanDropdown;
	public TMP_Dropdown activePortraitDropdown;
	public GameObject commanderSpecialityDropdownObj, primarySpecialityDropdownObj, weaponSpecialityDropdownObj, supportSpecialityDropdownObj;
	public TMP_Dropdown commanderSpecialityDropdown, primarySpecialityDropdown, weaponSpecialityDropdown, supportSpecialityDropdown;
	public TMP_Dropdown activeSpecialityDropdown;
	public TMP_Dropdown abilityDropdown;
	public List<Soldier> soldiers = new();
	public TextMeshProUGUI soldierIdentifier, playerIdentifier;
	public TMP_InputField soldierName;
	public GameObject createCompletedUI;
	private List<string> allSoldierIds = new();
	public int currentTeam = 1, maxSoldiersPerTeam, primaries, weapons, supports, soldierIndex = 1;
	public List<string> bannedNames = new()
	{
		"Anubis", "Frollo", "Elton", "Chicago", "Omega", "Lucky", "Simon", "Sunshine", "Brick", "Spectre", "Shriek", "Rook", "Wheels", "Heyzeus", "Mewham", "Mars", "Khorne",
		"Smith", "Dwayne", "Victor", "Churchill", "Bighammer", "Quill", "Stark", "Cellophane", "Cyclops", "Ratcliffe", "Expendable", "Rico", "Storm", "Victoire", "Willow", 
		"Horus", "Ra", "Sekmet", "Streaker", "Rasputin", "Seraph", "Mercury", "Oryx", "McClane", "Rambo", "Miyagi", "Grief", "Roach", "Nikolai", "Woods", "Zeus", "Hermes", 
		"Hades", "Theia", "Ares", "Achilles", "Bricke", "Geist", "Cyrus", "Stavanger", "Zaiel", "Krypton", "Lechwe", "Leshy", "Ceramics", "Bundaberg", "Gaston", "Chaplin", 
		"Conan", "Lillehammer", "Atlas", "Wraith", "Jazzhands", "Ivan", "Kamrov", "Price", "Amun", "Wick", "Pascal", "Hathor", "Osiris", "Neo", "Pete", "Ghost", "Discofinger", 
		"Saladin", "Manhattan"
	};
	public List<string> selectedPortraits;
	public List<string> selectedCommanderTerrains;
	public List<string> selectedSkills;
	public List<string> selectedAbilities;
	private string[] randomNames = 
	{
		"Cory","Pharrell","Bender","Asim","Marc","Sean","Kurt","Edison","Caolan","Shaurya","Jamie-Lee","Rhodri","Enrique","Joel","Reginald","Shaquille","Cruz",
		"Rudy","Kelly","Tyrique","Willie","Imaan","Jaden","Emilio","Trevor","Yanis","Connor","Sammy","Quinn","Rudi","Ashlea","Lewie","Shawn","Rayan","Maheen","Bhavik",
		"Eduard","Callam","Eryk","Shakeel","Aaron","Malcolm","Eugene","Lloyd","Kayan","Beau","Woody","Sanjay","Marni","Sulayman","Kiaan","Asif","Kenan","Yusuf",
		"Dexter","Reece","Hakim","Cassian","Diesel","Olly","Antoine","Meredith","Lynn","Maxime","Dev","Ayub","Jonah","Rudra","Lachlan","Randy","Donnie","Jordan","Milan",
		"Rickie","Lukas","Sonya","Dylan","Hugh","Ariel","Cleveland","Brandan","Borys","Sunil","Leo","Devon","Max","Lewis","Niam","Zack","Aydin","Ashlee",
		"Archer","Sidney","Kobe","Faraz","Sahib","Douglas","Ariyan","Webster","Bailey","Moss","Rutledge","Xiong","Payne","Bannister","Mullins","Baxter","Wilcox","Barker",
		"Herman","Dickson","Bernard","Wilder","Pitt","Vo","Hayward","Stanley","Gilmore","Ador","Gale","Goff","Mathews","Connor","Poole","Alexi","North","French","Montgomery",
		"Chaney","Martins","Villa","Mills","Stephens","Houston","Phoenix","Buxton","Downes","Vu","Burks","Lim","West","Jensen","Hartley","Naylor","Wang","Coles","Peterson",
		"Shepherd","Marsh","Bane","Ritter","Gallagher","Mcfarland","Justice","Oconnor","Fletcher","Hahn","Caleb","Robles","Griffiths","Sears","Espinosa","Edwards","Bird",
		"Driscoll","Bell","Davies","Mcneill","Franks","Millington","Mclean","Moon","Frank","Martinez","Shannon","Irwin","Neale","Mellor","Armitage","Finch","Small",
		"Hubert","Freeman","Baker","Carrillo","Timms","Thomas","Watkins","Stafford","Mckenzie","Goddard","Maddox","Simpson","Wooten","Mathis","Stamp", "Banubis"
	};
	private int[,] teamBreakdown =
	{ 
		{ 1, 1, 1 },
		{ 2, 1, 1 },
		{ 2, 2, 1 },
		{ 3, 2, 1 },
		{ 3, 2, 2 },
		{ 3, 3, 2 },
		{ 4, 3, 2 },
		{ 4, 4, 2 },
		{ 4, 4, 3 },
		{ 5, 4, 3 },
		{ 5, 5, 3 },
		{ 6, 5, 3 },
		{ 6, 6, 3 }
	};
	private string[,] specialities =
	{
		{ "Commander (L)", "Leadership" },
		{ "Spartan (H)", "Health" },
		{ "Survivor (R)", "Resilience" },
		{ "Runner (S)", "Speed" },
		{ "Evader (E)", "Evasion" },
		{ "Assassin (F)", "Stealth" },
		{ "Seeker (P)", "Perceptiveness" },
		{ "Chameleon (C)", "Camouflage" },
		{ "Scout (SR)", "Sight Radius" },
		{ "Infantryman (Ri)", "Rifle" },
		{ "Operator (AR)", "Assault Rifle" },
		{ "Earthquake (LMG)", "Light Machine Gun" },
		{ "Hunter (Sn)", "Sniper Rifle" },
		{ "Cyclone (SMG)", "Sub-Machine Gun" },
		{ "Hammer (Sh)", "Shotgun" },
		{ "Wolf (M)", "Melee" },
		{ "Hercules (Str)", "Strength" },
		{ "Diplomat (Dip)", "Diplomacy" },
		{ "Technician (Elec)", "Electronics" },
		{ "Medic (Heal)", "Healing" }
	};
	private string[][] abilities =
	{
		new string[] { "Adept", "Aficionado" },
		new string[] { "Avenger", "Exactor" },
		new string[] { "Bloodletter", "Masochist" },
		new string[] { "Bull", "Colossus" },
		new string[] { "Calculator", "Supercomputer" },
		new string[] { "Daredevil", "Spider" },
		new string[] { "Dissuader", "Omen of Death" },
		new string[] { "Experimentalist", "Chemist" },
		new string[] { "Fighter", "Pugilist" },
		new string[] { "Guardsman", "Sentinel" },
		new string[] { "Gunner", "Cannoneer" },
		new string[] { "Illusionist", "Ghost" },
		new string[] { "Informer", "Double Agent" },
		new string[] { "Inspirer", "Galvaniser" },
		new string[] { "Insulator", "Absorber" },
		new string[] { "Jammer", "Corrupter" },
		new string[] { "Learner", "Mastermind" },
		new string[] { "Locater", "Logistician" },
		new string[] { "Patriot", "Zealot" },
		new string[] { "Planner", "Prophet" },
		new string[] { "Politician", "Master's Ally" },
		new string[] { "Revoker", "Pacifier" },
		new string[] { "Shadow", "Shapeshifter" },
		new string[] { "Sharpshooter", "Deadeye" },
		new string[] { "Spotter", "Tracker" },
		new string[] { "Sprinter", "Olympian" },
		new string[] { "Tactician", "Creator" },
		new string[] { "Tranquiliser", "Anaesthetist" },
		new string[] { "Vaulter", "Acrobat" },
		new string[] { "Witness", "Hypnotist" },
	};

	public void LoadData(GameData data)
	{

	}

	public void SaveData(ref GameData data)
	{
		IEnumerable<Soldier> allSoldiers = FindObjectsOfType<Soldier>();
		foreach (Soldier soldier in allSoldiers)
			allSoldierIds.Add(soldier.id);

		data.allSoldiersIds = allSoldierIds;
		data.maxTeams = setTeamParameters.maxTeams;
	}

	public void RandomTeam()
    {
		for (int i = soldierIndex; i <= maxSoldiersPerTeam; i++)
        {
			RandomSoldier();
			ConfirmButtonPressed();
			CheckSoldierIndex();
			CheckPlayerIdentifier();
			CheckSoldierIdentifier();
			CheckPortraitDropdown();
		}
    }
	public void RandomSoldier()
    {
		RandomName();
		RandomTerrain();
		CheckPortraitDropdown();
		RandomPortrait();
		RandomSpeciality();
		RandomAbility();
	}
	public void RandomName()
    {
		soldierName.text = randomNames[Random.Range(0, randomNames.Length)];
    }
	public int[] CreateRandomIntArray(int length)
	{
		int[] arr = new int[length];
		for (int i = 0; i < arr.Length; i++)
			arr[i] = i;

		arr = arr.OrderBy(x => Random.Range(0, activePortraitDropdown.options.Count)).ToArray();

		return arr;
	}
	public void RandomTerrain()
    {
		Debug.Log("random terrain");
		if (soldierIdentifier.text.Contains("Commander"))
		{
			bool checkingValid = true;

			while (checkingValid)
			{
				terrainDropdown.value = Random.Range(1, terrainDropdown.options.Count);
				if (!selectedCommanderTerrains.Contains(terrainDropdown.options[terrainDropdown.value].text))
					checkingValid = false;
			}
		}
		else
			terrainDropdown.value = Random.Range(1, terrainDropdown.options.Count);
    }
	public void RandomPortrait()
	{
		bool valid = false;
		int[] range = CreateRandomIntArray(activePortraitDropdown.options.Count);

		for (int i = 0; i < range.Length; i++)
        {
			activePortraitDropdown.value = range[i];
			if (!selectedPortraits.Contains(activePortraitDropdown.options[activePortraitDropdown.value].text))
            {
				valid = true;
				break;
			}
		}

		if (!valid)
        {
			if (terrainDropdown.value == 4)
				terrainDropdown.value = 1;
			else
				terrainDropdown.value++;

			CheckPortraitDropdown();
			RandomPortrait();
        }
	}
	public void RandomSpeciality()
	{
		bool checkingValid = true;

		while (checkingValid)
        {
			activeSpecialityDropdown.value = Random.Range(1, activeSpecialityDropdown.options.Count);
			if (!selectedSkills.Contains(activeSpecialityDropdown.options[activeSpecialityDropdown.value].text))
				checkingValid = false;
		}
	}
	public void RandomAbility()
	{
		var checkingValid = true;
		while (checkingValid)
        {
			abilityDropdown.value = Random.Range(1, abilityDropdown.options.Count);
			if (!selectedAbilities.Contains(abilityDropdown.options[abilityDropdown.value].text))
				checkingValid = false;
		}
	}

	public void SetupConfirmed()
    {
		maxSoldiersPerTeam = setTeamParameters.maxSoldiers;
		primaries = teamBreakdown[maxSoldiersPerTeam - 3, 0];
		weapons = teamBreakdown[maxSoldiersPerTeam - 3, 1];
		supports = teamBreakdown[maxSoldiersPerTeam - 3, 2];
		soldierIndex = 1;
	}

	public void ConfirmButtonPressed()
	{
		if (CheckValidDetails())
		{
			Debug.Log(soldierName.text + ": " + terrainDropdown.options[terrainDropdown.value].text + " " + GetSpeciality() + " " + abilityDropdown.options[abilityDropdown.value].text);
			Instantiate(baseSoldier).Init(soldierName.text, currentTeam, terrainDropdown.options[terrainDropdown.value].text, activePortraitDropdown.options[activePortraitDropdown.value].image, activePortraitDropdown.options[activePortraitDropdown.value].text, GetSpeciality(), abilityDropdown.options[abilityDropdown.value].text);

			//refresh input fields and exclude previously chosen options
			bannedNames.Add(soldierName.text); 
			soldierName.text = "";
            //exclude commander terrains that have been selected
            if (GetSpeciality().Equals("Leadership"))
				selectedCommanderTerrains.Add(terrainDropdown.options[terrainDropdown.value].text);

            terrainDropdown.value = 0;
			selectedSkills.Add(activeSpecialityDropdown.options[activeSpecialityDropdown.value].text);
			activeSpecialityDropdown.value = 0;
			selectedAbilities.Add(abilityDropdown.options[abilityDropdown.value].text);
			abilityDropdown.value = 0;
			selectedPortraits.Add(activePortraitDropdown.options[activePortraitDropdown.value].text);
            

            //increment soldier and player as necessary
            if (soldierIndex < maxSoldiersPerTeam)
			{
				soldierIndex++;
			}
			else
			{
				if (currentTeam < setTeamParameters.maxTeams)
				{
                    currentTeam++;
					soldierIndex = 1;
					selectedSkills.Clear();	
					//selectedAbilities.Clear();
				}
				else
					createCompletedUI.SetActive(true);
			}

			//push greyed out options to dropdowns
			commanderSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			commanderSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedSkills);
			primarySpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			primarySpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedSkills);
			weaponSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			weaponSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedSkills);
			supportSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			supportSpecialityDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedSkills);
			abilityDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			abilityDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedAbilities);
			alpineDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			alpineDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedPortraits);
			desertDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			desertDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedPortraits);
			jungleDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			jungleDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedPortraits);
			urbanDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			urbanDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedPortraits);
			terrainDropdown.GetComponent<DropdownController>().optionsToGrey.Clear();
			//if commander grey out commander chosen terrains
			if (soldierIndex == 1)
				terrainDropdown.GetComponent<DropdownController>().optionsToGrey.AddRange(selectedCommanderTerrains);
        }
		else
			Debug.Log("Details invalid.");
	}

	public string GetSpeciality()
    {
        return activeSpecialityDropdown.options[activeSpecialityDropdown.value].text switch
        {
            "Commander (L)" => "Leadership",
            "Spartan (H)" => "Health",
            "Survivor (R)" => "Resilience",
            "Runner (S)" => "Speed",
            "Evader (E)" => "Evasion",
            "Assassin (F)" => "Stealth",
            "Seeker (P)" => "Perceptiveness",
            "Chameleon (C)" => "Camouflage",
            "Scout (SR)" => "Sight Radius",
            "Infantryman (Ri)" => "Rifle",
            "Operator (AR)" => "Assault Rifle",
            "Earthquake (LMG)" => "Light Machine Gun",
            "Hunter (Sn)" => "Sniper Rifle",
            "Cyclone (SMG)" => "Sub-Machine Gun",
            "Hammer (Sh)" => "Shotgun",
            "Wolf (M)" => "Melee",
            "Hercules (Str)" => "Strength",
            "Diplomat (Dip)" => "Diplomacy",
            "Technician (Elec)" => "Electronics",
            "Medic (Heal)" => "Healing",
            _ => "",
        };
    }

	public bool CheckValidDetails()
    {
		if (soldierName.text.Length > 0 && !bannedNames.Contains(soldierName.text) && terrainDropdown.value != 0 && activeSpecialityDropdown.value != 0 && abilityDropdown.value != 0)
			return true;
		else
            return false;
    }

    public void Update()
	{
		CheckSoldierIndex();
        CheckSoldierName();
        CheckPlayerIdentifier();
		CheckSoldierIdentifier();
		CheckPortraitDropdown();
	}
	
	public void CheckSoldierIndex()
	{
		if (soldierIndex == 1)
            soldierIdentifier.text = "Commander";
		else if (soldierIndex <= primaries)
            soldierIdentifier.text = "Primary Speciality " + soldierIndex;
        else if (soldierIndex <= primaries + weapons)
            soldierIdentifier.text = "Weapon Speciality " + (soldierIndex - primaries);
        else if (soldierIndex <= primaries + weapons + supports)
            soldierIdentifier.text = "Support Speciality " + (soldierIndex - primaries - weapons);
	}
	public void CheckSoldierName()
	{
		if (bannedNames.Contains(soldierName.text))
			soldierName.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
		else
			soldierName.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0.196f, 0.196f, 0.196f);
	}
	public void CheckPlayerIdentifier()
	{
		playerIdentifier.text = "Creating Soldiers for Team " + currentTeam;
	}
	public void CheckSoldierIdentifier()
    {
		if (soldierIdentifier.text.Contains("Commander"))
        {
			commanderSpecialityDropdownObj.SetActive(true);
			activeSpecialityDropdown = commanderSpecialityDropdown;
		}
		else
			commanderSpecialityDropdownObj.SetActive(false);

		if (soldierIdentifier.text.Contains("Primary"))
		{
			primarySpecialityDropdownObj.SetActive(true);
			activeSpecialityDropdown = primarySpecialityDropdown;
		}
		else
			primarySpecialityDropdownObj.SetActive(false);

		if (soldierIdentifier.text.Contains("Weapon"))
		{
			weaponSpecialityDropdownObj.SetActive(true);
			activeSpecialityDropdown = weaponSpecialityDropdown;
		}
		else
			weaponSpecialityDropdownObj.SetActive(false);

		if (soldierIdentifier.text.Contains("Support"))
		{
			supportSpecialityDropdownObj.SetActive(true);
			activeSpecialityDropdown = supportSpecialityDropdown;
		}
		else
			supportSpecialityDropdownObj.SetActive(false);
	}
	public void CheckPortraitDropdown()
	{
		if (terrainDropdown.value > 0)
        {
			if (soldierIdentifier.text.Contains("Commander"))
			{
				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Alpine"))
				{
					commanderAlpineDropdownObj.SetActive(true);
					activePortraitDropdown = commanderAlpineDropdown;
				}
				else
					commanderAlpineDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Desert"))
				{
					commanderDesertDropdownObj.SetActive(true);
					activePortraitDropdown = commanderDesertDropdown;
				}
				else
					commanderDesertDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Jungle"))
				{
					commanderJungleDropdownObj.SetActive(true);
					activePortraitDropdown = commanderJungleDropdown;
				}
				else
					commanderJungleDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Urban"))
				{
					commanderUrbanDropdownObj.SetActive(true);
					activePortraitDropdown = commanderUrbanDropdown;
				}
				else
					commanderUrbanDropdownObj.SetActive(false);
			}
			else
			{
				commanderAlpineDropdownObj.SetActive(false);
				commanderDesertDropdownObj.SetActive(false);
				commanderJungleDropdownObj.SetActive(false);
				commanderUrbanDropdownObj.SetActive(false);
				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Alpine"))
				{
					alpineDropdownObj.SetActive(true);
					activePortraitDropdown = alpineDropdown;
				}
				else
					alpineDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Desert"))
				{
					desertDropdownObj.SetActive(true);
					activePortraitDropdown = desertDropdown;
				}
				else
					desertDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Jungle"))
				{
					jungleDropdownObj.SetActive(true);
					activePortraitDropdown = jungleDropdown;
				}
				else
					jungleDropdownObj.SetActive(false);

				if (terrainDropdown.options[terrainDropdown.value].text.Contains("Urban"))
				{
					urbanDropdownObj.SetActive(true);
					activePortraitDropdown = urbanDropdown;
				}
				else
					urbanDropdownObj.SetActive(false);
			}
		}
		else
        {
			commanderAlpineDropdownObj.SetActive(false);
			commanderDesertDropdownObj.SetActive(false);
			commanderJungleDropdownObj.SetActive(false);
			commanderUrbanDropdownObj.SetActive(false);
			alpineDropdownObj.SetActive(false);
			desertDropdownObj.SetActive(false);
			jungleDropdownObj.SetActive(false);
			urbanDropdownObj.SetActive(false);
			activePortraitDropdown = null;
        }
	}

}
