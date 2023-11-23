using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Collections;
using UnityEngine.Rendering;

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
	public GameObject createCompletedUI, randomAlertUI;
	private readonly List<string> allSoldierIds = new();
	public int currentTeam = 1, maxSoldiersPerTeam, primaries, weapons, supports, soldierIndex = 1;
	public readonly List<string> bannedNames = new()
	{
		"aeolus","ahole","anal","analannie","analprobe","analsex","anilingus","anus","apeshit","areola","areole","arian","arrse","arse",
		"arsehole","aryan","ass","assfuck","asshole","assbag","assbagger","assbandit","assbang","assbanged","assbanger","assbangs","assbite","assblaster","assclown",
		"asscock","asscracker","asses","assface","assfaces","assfuck","assfucker","ass-fucker","assfukka","assgoblin","asshat","ass-hat","asshead","assho1e","asshole",
		"assholes","asshopper","asshore","ass-jabber","assjacker","assjockey","asskiss","asskisser","assklown","asslick","asslicker","asslover","assman","assmaster",
		"assmonkey","assmucus","assmunch","assmuncher","assnigger","asspacker","asspirate","asspirate","asspuppies","assranger","assshit","assshole","asssucker","asswad",
		"asswhole","asswhore","asswipe","asswipes","axe wound","axewound","axwound","babyjuice","badfuck","ballgag","ball gag","ballgravy","ballkick","ball sack","ballbag",
		"balls","ballsack","bangbros","banging","bareback","barenaked","barf","barface","barfface","bastard","bastardo","bastards","bastinado","bazongas","bazooms","bbw","bdsm",
		"beaner","beaners","beastial","beastiality","beatch","beeyotch","bellend","beotch","bestial","bestiality","biatch","bigblack","big breasts","bigtits","bigtit","big tits",
		"bigbastard","bigbutt","bimbo","bimbos","bisexual","bi-sexual","bitch","bitch tit","bitchass","bitched","bitcher","bitchers","bitches","bitchez","bitchin","bitching",
		"bitchtits","bitchy","blackcock","black cock","blow job","blowme","blowmi","blowjob","bollick","bollock","bollocks","bollok","bollox","bondage","boned","boner","boners",
		"bong","boob","boobies","boobs","booby","booger","bookie","boong","boonga","booobs","boooobs","booooobs","bootee","bootie","booty","booty call","booze","boozer","boozy",
		"bosom","bosomy","bowel","bowels","bra","breast","breastjob","breastlova","breastman","breasts","breeder","buceta","bugger","buggered","buggery","bukkake","bull shit",
		"bullcrap","bulldike","bulldyke","bullet vibe","bullshit","bullshits","bullturds","bum","bum boy","bumblefuck","bumclat","bumfuck","bummer","bung","bung hole","bunga",
		"bunghole","busty","butchdike","butchdyke","butt","butt fuck","butt plug","buttbang","butt-bang","buttcheeks","buttface","buttfuck","butt-fuck","buttfucka","buttfucker",
		"butt-fucker","butthead","butthole","buttman","buttmuch","buttmunch","buttmuncher","butt-pirate","buttplug","caca","camel toe","cameltoe","camgirl","camslut","camwhore",
		"cervix","chesticle","childfucka","chinc","chincs","chink","chinky","choad","choade","chode","chodes","chotabags","circlejerk","climax","clit","clitlicker","clitlicka",
		"clitface","clitfuck","clitoris","clitorus","clits","clitty","clogwog","clunge","clustafuck","cocain","cocaine","cock","c-o-c-k","cockpocket","cock pocket","cocksnot",
		"cock snot","cock sucker","cockass","cockbite","cockblock","cockburger","cockeye","cockface","cockfucker","cockhead","cockjockey","cockknoker","cocklicker","cocklover",
		"cocklump","cockmaster","cockmongler","cockmonkey","cockmunch","cockmuncher","cocknose","cocknugget","cocks","cockshit","cocksmith","cocksmoke","cocksmoker","cocksniffa",
		"cocksucer","cocksuck","cocksuck","cocksucked","cocksucker","cock-sucker","cocksuckas","cocksucks","cocksuka","cocksukka","cockwaffle","coital","cok","cokmuncher",
		"coksucka","commie","condom","coochie","coochy","coon","coonnass","coons","cooter","corksucker","cornhole","corpwhore","crabs","cracker","crackwhore","crap","crappy",
		"creampie","cretin","cum","cumchugger","cum freak","cum guzzler","cumbubble","cumdump","cumdumpster","cumguzzler","cumjockey","cummer","cummin","cumming","cums","cumshot",
		"cumshots","cumslut","cumstain","cumtart","cunilingus","cunn","cunnie","cunntt","cunny","cunt","c-u-n-t","cunt hair","cunthair","cuntass","cuntbag","cuntface","cuntfuck",
		"cuntfucker","cunthole","cunthunter","cuntlick","cuntlick","cuntlicker","cuntlicker","cuntlicking","cuntrag","cunts","cuntsicle","cuntslut","cuntstruck","cuntsucker",
		"cyberfuc","cyberfuck","cyberfucked","cyberfucker","cyberfuckers","cyberfucking","cybersex","dago","dagos","dammit","damn","damned","damnit","darkie","darn","date rape",
		"daterape","dawgistyle","dipthroat","deggo","dickhed","dickhead","dick head","dick hole","dickbag","dickbeater","dickbrain","dickdipper","dickface","dickfuck","dickhead",
		"dickheads","dickhole","dickish","dickjuice","dickmilk","dickmonger","dickripper","dicks","dicksipper","dickslap","dicksucker","diksucking","diktickler","dickwad",
		"dickweasel","dickweed","dikwhipper","dickwod","dickzipper","diddle","dike","dildo","dildos","diligaf","dillweed","dimwit","dingle","dinglebery","dink","dinks","dipship",
		"dipshit","dirsa","dirty","dog style","dog-fucker","doggistyle","doggin","dogging","doggy style","doggystyle","doggy-style","dolcett","dominatrix","dommes","dong",
		"donkipunch","donkiribba","doochbag","doofus","doosh","dopey","double dong","douche","douchebag","douchebags","douche-fag","douchey","dp action","dryhump","dry hump",
		"duche","dumass","dumb ass","dumbass","dumbasses","dumbcunt","dumbfuck","dumbshit","dummy","dumshit","dyke","dykes","eatadick","eat a dick","eatadik","eatpussy","ecchi",
		"ejaculate","ejaculated","ejakulate","enlargement","erect","erection","erotic","erotism","escort","essohbee","eunuch","extacy","extasy","f u c k","f u c k a","facefucker",
		"facefuckua","facefuker","facefuka","facial","fack","fag","fagbag","fagfucker","fagg","fagged","fagging","faggit","faggitt","faggot","faggotcock","faggots","faggs","fagot",
		"fagots","fags","fagtard","faig","faigt","fanny","fannyflap","fannyflaps","fannyfucka","fanyy","fart","fartknocka","fastfuck","fat","fatass","fatfuck","fatfucker","fcuk",
		"fcuker","fcuking","fecal","fellate","fellatio","femdom","fingerbang","fingerfuck","fingering","fingerer","fingeringa","fisted","fistfuck","fistfucked","fistfucker",
		"fistfucker","fistfuckers","fistfucking","fistfucka","fistfucks","fisting","fleshflute","flogthelog","floozy","fondle","footfetish","footfuck","footfucker","footjob",
		"footlicka","footlicker","foreskin","freakfuck","freakyfuck","freefuck","fuc","fuck","f-u-c-k","fuckbutton","fuckhole","fuck hole","fuckoff","fuck off","fuckpuppet",
		"fucktrophy","fuckyomama","fuck you","fuckyou","fucka","fuckass","fuck-ass","fuckbag","fuck-bitch","fuckbitch","fuckboy","fuckbrain","fuckbutt","fuckbutter","fucked",
		"fuckedup","fucker","fuckers","fuckersucker","fuckface","fuckfreak","fuckhead","fuckheads","fuckher","fuckhole","fuckin","fucking","fuckings","fukmi","fuckme","fuckmeat",
		"fuckmehard","fuckmonkey","fucknugget","fucknut","fucknutt","fuckoff","fucks","fuckstick","fucktard","fuck-tard","fucktards","fucktart","fucktoy","fucktwat","fuckup",
		"fuckwad","fuckwhit","fuckwhore","fuckwit","fuckwitt","fuckyou","fudgepacka","fuk","fuker","fukker","fukkers","fukkin","fuks","fukwhit","fukwit","fuq","futanari","fux",
		"gae","gai","gang bang","gangbang","gang-bang","gangbanged","gangbangs","gay sex","gayass","gaybob","gaydo","gayfuck","gayfuckist","gaylord","gays","gaysex","gaytard",
		"gaywad","jendabenda","genitals","gey","ghay","ghey","gigolo","girlontop","glans","gokkun","golliwog","gonad","gonads","gonorrehea","gooch","gook","gooks","goregasm",
		"grope","groupsex","groopsex","group sex","gspot","g-spot","guido","guro","hamflap","ham flap","hand job","handjob","hard on","hardon","headfuck","hentai","heroin",
		"herpes","heshe","he-she","he she","hitler","ho","hoar","hoare","hobag","hoe","hoer","homo","homoerotic","homoey","hooker","hoor","hooter","hooters","hore","horniest",
		"horny","hotchick","hot chick","hotpussy","hot pussy","hot sex","hotsex","hump","humped","humper","humpa","humping","hussy","hymen","inbred","incest","injun","intercours",
		"intacourse","intercourz","intercours","jack off","jackass","jackhole","jackoff","jack-off","jagof","jagov","jagoff","jail bait","jailbait","jerk","jerk off","jerkass",
		"jerked","jerkoff","jerk-off","jigaboo","jiggaboo","jiggerboo","jiz","jizz","jizzed","kawk","kike","kinbaku","kinkster","kinky","kkk","klan","knob","knobbing","knobead",
		"knobed","knobend","knobhead","knobjocky","knobjockey","knobjokey","kock","kondum","kondums","kooch","kooches","kootch","kum","kummer","kumming","kums","kunilingus","kunt",
		"kwif","kyke","labia","lameass","lardass","lesbian","lesbians","lesbo","lesbos","lezbian","lezbians","lezbo","lezbos","leza","lezie","lezzie","lezzies","lezzy","lmao",
		"lmfao","looney","lube","lust","lusting","lusty","makemecum","makemicum","masterbate","masturbate","mcfagget","menstruate","midget","milf","minge","minger","molest","mothafuck",
		"mothafucka","mohafucas","mothafucaz","mothafuks","mothafuckn","mothafucks","mtherfucker","mthrfucker","muff","muffdiver","muffpuff","muff diver","muffdive","munta","munter",
		"mudafecker","mudafuckaz","mudafucker","nad","nads","naked","nappy","nasi","nasism","nazi","nazism","needick","needdick","negro","neonazi","nigaboo","nigga","niggah","niggas",
		"niggaz","nigger","niggers","niggle","niglet","nimfo","nymfo","nimpho","nipple","nipples","nob","nobjokey","nobjockey","nobhead","nobjocky","nobjokey","nsfw","nude","nudity",
		"numbnuts","nutbutter","nutsack","nutter","nympho","octopussy","octopussi","octopusy","octopusi","omorashi","omorash","oral","orally","orgasim","orgasims","orgasm","orgasmic",
		"orgasms","orgies","orgy","ovary","ovum","ovums","ova","paedo","paedophile","paedofile","pansy","pantie","panties","pecker","peckerhead","peckerwood","pedo","pedobear",
		"pedophile","pedofilea","pedofiliac","pee","peepee","pegger","penetrator","penial","penile","penis","phalli","phallic","phonesex","phuck","phuk","phuks","phuq","pigfucker",
		"pikey","pimp","piss","pissoff","pisspig","pissed","pissedoff","pisser","pissers","pisses","pissflaps","pissin","pissing","piss-off","playboy","pms","polesmoker","poof","poon",
		"poonani","poonany","poontang","poop","poopchute","poopuncher","porn","porno","pornograph","pornos","prostitute","pube","pubes","pubic","pubis","punani","punanny","punany",
		"punkass","pusse","pussi","pussies","pussy","pussyfart","pussylick","pussypound","queaf","queef","queer","queerbait","queerhole","raghead","rape","raped","raper","rapey",
		"raping","rapist","rectal","rectum","rectus","reetard","retard","retarded","rimjaw","rimjob","rimming","ritard","rosy palm","rtard","r-tard","rumprammer","sandnigger","scat",
		"schlong","scroat","scrote","scrotum","scum","seaman","seamen","semen","sex","sexo","sexual","sexy","shemale","shit","shitass","shitbag","shitbagger","shitblimp","shitbrains",
		"shitbreath","shitcan","shitcunt","shitdick","shite","shiteater","shited","shitey","shitface","shitfaced","shitfuck","shitfull","shithead","shitheads","shithole","shithouse",
		"shiting","shitings","shits","shitstain","shitt","shitted","shitter","shitters","shittier","shittiest","shitting","shittings","shitty","shota","sissy","skank","skulfuck",
		"skullfuck","slag","slanteye","slave","slut","slutbag","smeg","smegma","smut","smutty","snuff","sodom","sodomize","sodomise","sodomy","spastic","spaztic","spaz","sperm",
		"splooge","strapon","stroke","stupid","suckass","swastika","swastiker","tampon","tard","tart","teabag","tea bag","teat","teats","teets","teet","teste","testee","testes","testical",
		"testicle","testis","threesome","tit","titi","tities","tits","titt","titties","titty","towelhead","towelhed","tranny","transexual","twat","twathead","twatlips","twats","twatty",
		"twatwaffle","twunt","twunter","undies","undress","upskirt","urethra","urinal","urine","vag","vagina","vajayjay","viagra","vibrator","vibrater","virgin","vjayjay","vomit",
		"vom","vulva","wank","wanka","wanker","wankjob","whoar","whore","whorebag","whored","whoreface","whorehouse","whores","whorz","whoring","wigger","wog","wop","wap","yobo","yobbo",
		"zoofile","zoophile","Alphanite","Gamoid","Thetin","Zetae","Omeg","Ajax","Castor","Odysseus","Hector","Achileas",
		"Frollo","Elton","Chicago","Omega","Lucky","Simon","Sunshine","Brick","Spectre","Shriek","Rook","Wheels","Cellophane","Cyclops","Ratcliffe","Expendable",
		"Rico","Storm","Victor","Willow","Horus","Ra","Sekmet","Streaker","Rasputin","Seraph","Mercury","Oryx","McClane","Rambo","Miyagi","Grief","Roach","Nikolai","Woods","Zeus","Hermes",
		"Hades","Theia","Ares","Achilles","Bricke","Geist","Cyrus","Stavanger","Zaiel","Krypton","Lechwe","Leshy","Ceramics","Bundaberg","Gaston","Chaplin","Mars","Khorne","Dwayne","Victoire",
		"Churchill","Quill","Stark","Ghost","Discofinger","Saladin","Manhattan","Anubis","Mewham","Conan","Lillehammer","Atlas","Bighammer","Wraith","Jazzhands","Ivan","Kamrov","Price",
		"Amun","Wick","Smith","Heyzeus","Pascal","Hathor","Osiris","Neo","Pete","Shard","Armour","Body","Exo","Ghillie","Food Pack","Drink","Food","Medikit","Water","Canteen","Claymore",
		"Smoke","Grenade","Frag","Tabun","Flashbang","Knife","Riot Shield","Binoculars","Deployment","Beacon","Etool","E tool","E-tool","Suppressor","Thermal","Camera","ULF","UHF","Radio",
		"Backpack","Bag","Brace","Belt","Logistics","Poison","Satchel","Syringe","Garand","Carbine","Arisaka","ACOG-FAL","ACOGFAL","ACOGFAL","AK","LSW","SAW","Intervention","Barrett",
		"Dragunov","Thompson","UMP","SPAS","Ithaca","Olympus","Glock","Sidearm","Magnum","Rifle","MG","LMG","Sniper","Shotgun","SMG","Recruit","Private","Lieutenant","Sergeant","Corporal",
		"Captain","Major","Colonel","Brigadier","General","Commander","Spartan","Survivor","Runner","Evader","Assassin","Seeker","Chameleon","Scout","Leadership","Health","Resilience",
		"Speed","Evasion","Stealth","Perception","Camouflage","Siteradius","Meleed","Melee","Infantryman","Operator","Earthquake","Hunter","Cyclone","Hammer","Wolf","Strength","Diplomacy",
		"Electronics","Healing","Hercules","Diplomat","Technician","Medic","Jesus","Heyzeus","Allah","Alah","Alla","Ala","Asspipe","Niga","Nigga","Adept","Aficionado","Avenger","Exactor",
		"Masochist","Bull","Colossus","Calculator","Daredevil","Spider","Dissuader","Chemist","Fighter","Pugilist","Guardsman","Sentinel","Gunner","Cannoneer","Ghost","Informer","Double Agent",
		"Inspirer","Galvaniser","Insulator","Absorber","Jammer","Corrupter","Learner","Mastermind","Locater","Patriot","Zealot","Planner","Prophet","Politician","Revoker","Pacifier","Shadow",
		"Deadeye","Spotter","Tracker","Sprinter","Olympian","Tactician","Creator","Vaulter","Acrobat","Witness","Hypnotist"
    };
	public List<string> selectedPortraits;
	public List<string> selectedCommanderTerrains;
	public List<string> selectedSkills;
	public List<string> selectedAbilities;
	private readonly string[] randomNames = 
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
	private readonly int[,] teamBreakdown =
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

	public void OpenRandomAlertUI()
	{
		randomAlertUI.SetActive(true);
	}
    public void CloseRandomAlertUI()
    {
        randomAlertUI.SetActive(false);
    }
	public void RandomTeamNonCoroutine()
	{
		StartCoroutine(RandomTeam());
	}
    public IEnumerator RandomTeam()
    {
		for (int i = soldierIndex; i <= maxSoldiersPerTeam; i++)
        {
			RandomSoldier();
			yield return new WaitForEndOfFrame();
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
		string name = "Anubis";
		while (bannedNames.Contains(name))
			name = randomNames[UnityEngine.Random.Range(0, randomNames.Length)];

        soldierName.text = name;
    }
	public int[] CreateRandomIntArray(int length)
	{
		int[] arr = new int[length];
		for (int i = 0; i < arr.Length; i++)
			arr[i] = i;

		arr = arr.OrderBy(x => UnityEngine.Random.Range(0, activePortraitDropdown.options.Count)).ToArray();

		return arr;
	}
	public void RandomTerrain()
    {
		if (soldierIdentifier.text.Contains("Commander"))
		{
			bool checkingValid = true;

			while (checkingValid)
			{
				terrainDropdown.value = UnityEngine.Random.Range(1, terrainDropdown.options.Count);
				if (!selectedCommanderTerrains.Contains(terrainDropdown.options[terrainDropdown.value].text))
					checkingValid = false;
			}
		}
		else
			terrainDropdown.value = UnityEngine.Random.Range(1, terrainDropdown.options.Count);
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
			activeSpecialityDropdown.value = UnityEngine.Random.Range(1, activeSpecialityDropdown.options.Count);
			if (!selectedSkills.Contains(activeSpecialityDropdown.options[activeSpecialityDropdown.value].text))
				checkingValid = false;
		}
	}
	public void RandomAbility()
	{
		var checkingValid = true;
		while (checkingValid)
        {
			abilityDropdown.value = UnityEngine.Random.Range(1, abilityDropdown.options.Count);
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
			print("Details invalid.");
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
		if (soldierName.text.Length > 0 && soldierName.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color == new Color(0.196f, 0.196f, 0.196f) && terrainDropdown.value != 0 && activeSpecialityDropdown.value != 0 && abilityDropdown.value != 0)
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
		if (!bannedNames.Contains(soldierName.text, StringComparer.OrdinalIgnoreCase) && Regex.Match(soldierName.text, @"^[a-zA-Z'-]+$", RegexOptions.IgnoreCase).Success)
            soldierName.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = new Color(0.196f, 0.196f, 0.196f);
        else
            soldierName.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.red;
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
