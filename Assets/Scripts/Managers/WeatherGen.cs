using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.ObjectModel;

public class WeatherGen : MonoBehaviour, IDataPersistence
{
    public MainGame game;

    readonly Dictionary<int, string> visibility = new()
    {
        {0, "Zero"},
        {1, "Poor"},
        {2, "Moderate"},
        {3, "Good"},
        {4, "Full"}
    };
    readonly Dictionary<int, string> windSpeed = new()
    {
        {0, "Zero"},
        {1, "Light"},
        {2, "Moderate"},
        {3, "Strong"}
    };
    readonly Dictionary<int, string> windDirection = new()
    {
        {0, "Northern"},
        {1, "North-Eastern"},
        {2, "Eastern"},
        {3, "South-Eastern"},
        {4, "Southern"},
        {5, "South-Western"},
        {6, "Western"},
        {7, "North-Western"}
    };
    readonly Dictionary<int, string> rain = new()
    {
        {0, "Zero"},
        {1, "Light"},
        {2, "Moderate"},
        {3, "Heavy"},
        {4, "Torrential"}
    };
    readonly Dictionary<string, Vector2> windDirectionVectors = new()
    {
        {"Northern", new(0, 1)},
        {"North-Eastern", new(1, 1)},
        {"Eastern", new(1, 0)},
        {"South-Eastern", new(1, -1)},
        {"Southern", new(0, -1)},
        {"South-Western", new(-1, -1)},
        {"Western", new(-1, 0)},
        {"North-Western", new(-1, 1)},
        {"Zero", new(0, 0)}
    };

    public List<string> savedWeather = new();
    private int vis, sp, dir, rn;
    public void LoadData(GameData data)
    {
        savedWeather = data.savedWeather;

        isDataLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        data.savedWeather = savedWeather;
    }

    public void GenerateWeather(int maxRounds)
    {
        int k = 0;
        vis = HelperFunctions.RandomNumber(0, 4);
        sp = HelperFunctions.RandomNumber(0, 3);
        dir = HelperFunctions.RandomNumber(0, 7);
        rn = HelperFunctions.RandomNumber(0, 4);

        for (int i = 1; i <= maxRounds; i++)
        {
            for (int j = 1; j <= 2; j++)
            {
                if (k % 3 == 0)
                {
                    vis = NextVis(vis);
                    sp = NextWindSpeed(sp);
                    dir = NextWindDirection(dir);
                    rn = NextRain(rn);
                }
                string weatherString = "";
                weatherString += visibility[vis] + " visibility, ";
                weatherString += windSpeed[sp] + " ";
                weatherString += windDirection[dir] + " wind, ";
                weatherString += rain[rn] + " rain.";
                savedWeather.Add(weatherString);
                k++;
            }
        }
    }

    private int NextVis(int vis)
    {
        if (vis == 0)
        {
            if (HelperFunctions.CoinFlip())
                vis++;
        }
        else if (vis == 4)
        {
            if (HelperFunctions.CoinFlip())
                vis--;
        }
        else
        {
            if (HelperFunctions.RandomNumber(1, 5) >= 4)
            {
                if (HelperFunctions.CoinFlip())
                    vis--;
                else
                    vis++;
            }
        }

        return vis;
    }
    private int NextWindSpeed(int sp)
    {
        if (sp == 0)
        {
            if (HelperFunctions.CoinFlip())
                sp++;
        }
        else if (sp == 3)
        {
            if (HelperFunctions.CoinFlip())
                sp--;
        }
        else
        {
            if (HelperFunctions.RandomNumber(1, 5) >= 4)
            {
                if (HelperFunctions.CoinFlip())
                    sp--;
                else
                    sp++;
            }
        }

        return sp;
    }
    private int NextWindDirection(int dir)
    {
        if (HelperFunctions.RandomNumber(1, 3) == 3)
        {
            if (HelperFunctions.CoinFlip())
            {
                dir--;
                if (dir < 0)
                    dir = 7;
            }
            else
            {
                dir++;
                if (dir > 7)
                    dir = 0;
            }
        }

        return dir;
    }
    private int NextRain(int rn)
    {
        if (rn == 0)
        {
            if (HelperFunctions.CoinFlip())
                rn++;
        }
        else if (rn == 4)
        {
            if (HelperFunctions.CoinFlip())
                rn--;
        }
        else
        {
            if (HelperFunctions.RandomNumber(1, 5) >= 4)
            {
                if (HelperFunctions.CoinFlip())
                    rn--;
                else
                    rn++;
            }
        }

        return rn;
    }
    public string DecreasedWindspeed(string currentWindspeed)
    {
        string decreasedWindspeed = currentWindspeed switch
        {
            "Strong" => "Moderate",
            "Moderate" => "Light",
            _ => "Zero",
        };

        return decreasedWindspeed;
    }
    public string IncreasedWindspeed(string currentWindspeed)
    {
        string increasedWindspeed = currentWindspeed switch
        {
            "Zero" => "Light",
            "Light" => "Moderate",
            _ => "Strong",
        };

        return increasedWindspeed;
    }
    public string DecreasedRain(string currentRain)
    {
        string decreasedRain = currentRain switch
        {
            "Torrential" => "Heavy",
            "Heavy" => "Moderate",
            "Moderate" => "Light",
            _ => "Zero",
        };

        return decreasedRain;
    }
    public string IncreasedRain(string currentRain)
    {
        string increasedRain = currentRain switch
        {
            "Zero" => "Light",
            "Light" => "Moderate",
            "Moderate" => "Heavy",
            _ => "Torrential",
        };

        return increasedRain;
    }
    public bool CheckVisChange(out string increaseOrDecrease)
    {
        increaseOrDecrease = string.Empty;
        int currentVisVal, nextTurnVisVal;

        currentVisVal = CurrentVis switch
        {
            "Full" => 0,
            "Good" => 1,
            "Moderate" => 2,
            "Poor" => 3,
            "Zero" => 4,
            _ => 0,
        };
        nextTurnVisVal = NextTurnVis switch
        {
            "Full" => 0,
            "Good" => 1,
            "Moderate" => 2,
            "Poor" => 3,
            "Zero" => 4,
            _ => 0,
        };

        if (currentVisVal < nextTurnVisVal)
        {
            increaseOrDecrease = "decrease";
            return true;
        }
        else if (currentVisVal > nextTurnVisVal)
        {
            increaseOrDecrease = "increase";
            return true;
        }
        
        return false;
    }
    public bool IsZeroVis()
    {
        return CurrentVis.Equals("Zero");
    }
    public bool IsPoorVis()
    {
        return CurrentVis.Equals("Poor");
    }
    public bool IsModerateVis()
    {
        return CurrentVis.Equals("Moderate");
    }
    public bool IsGoodVis()
    {
        return CurrentVis.Equals("Good");
    }
    public bool IsFullVis()
    {
        return CurrentVis.Equals("Full");
    }
    public string CurrentWeather
    {
        get { return savedWeather[game.currentTurn]; }
        set { savedWeather[game.currentTurn] = value; }
    }
    public string LastTurnWeather
    {
        get
        {
            return savedWeather[game.currentTurn - 1];
        }
    }
    public string NextTurnWeather
    {
        get
        {
            return savedWeather[game.currentTurn + 1];
        }
    }
    public string LastTurnVis
    {
        get
        {
            if (LastTurnWeather.Contains("Zero visibility"))
                return "Zero";
            else if (LastTurnWeather.Contains("Poor visibility"))
                return "Poor";
            else if (LastTurnWeather.Contains("Moderate visibility"))
                return "Moderate";
            else if (LastTurnWeather.Contains("Good visibility"))
                return "Good";
            else
                return "Full";
        }
    }
    public string CurrentVis
    {
        get
        {
            if (CurrentWeather.Contains("Zero visibility"))
                return "Zero";
            else if (CurrentWeather.Contains("Poor visibility"))
                return "Poor";
            else if (CurrentWeather.Contains("Moderate visibility"))
                return "Moderate";
            else if (CurrentWeather.Contains("Good visibility"))
                return "Good";
            else
                return "Full";
        }
        set 
        { 
            CurrentWeather = CurrentWeather.Replace($"{CurrentVis} visibility", $"{value} visibility");
        }
    }
    public string NextTurnVis
    {
        get
        {
            if (NextTurnWeather.Contains("Zero visibility"))
                return "Zero";
            else if (NextTurnWeather.Contains("Poor visibility"))
                return "Poor";
            else if (NextTurnWeather.Contains("Moderate visibility"))
                return "Moderate";
            else if (NextTurnWeather.Contains("Good visibility"))
                return "Good";
            else
                return "Full";
        }
    }
    public string LastTurnRain
    {
        get
        {
            if (LastTurnWeather.Contains("Torrential rain"))
                return "Torrential";
            else if (LastTurnWeather.Contains("Heavy rain"))
                return "Heavy";
            else if (LastTurnWeather.Contains("Moderate rain"))
                return "Moderate";
            else if (LastTurnWeather.Contains("Light rain"))
                return "Light";
            else
                return "Zero";
        }
    }
    public string CurrentRain
    {
        get
        {
            if (CurrentWeather.Contains("Torrential rain"))
                return "Torrential";
            else if (CurrentWeather.Contains("Heavy rain"))
                return "Heavy";
            else if (CurrentWeather.Contains("Moderate rain"))
                return "Moderate";
            else if (CurrentWeather.Contains("Light rain"))
                return "Light";
            else
                return "Zero";
        }
        set
        {
            CurrentWeather = CurrentWeather.Replace($"{CurrentRain} rain", $"{value} rain");
        }
    }
    public string NextTurnRain
    {
        get
        {
            if (NextTurnWeather.Contains("Torrential rain"))
                return "Torrential";
            else if (NextTurnWeather.Contains("Heavy rain"))
                return "Heavy";
            else if (NextTurnWeather.Contains("Moderate rain"))
                return "Moderate";
            else if (NextTurnWeather.Contains("Light rain"))
                return "Light";
            else
                return "Zero";
        }
    }
    public string LastTurnWindDirection
    {
        get
        {
            if (LastTurnWeather.Contains("Eastern"))
            {
                if (LastTurnWeather.Contains("North"))
                    return "North-Eastern";
                else if (LastTurnWeather.Contains("South"))
                    return "South-Eastern";
                else
                    return "Eastern";
            }
            else if (LastTurnWeather.Contains("Western"))
            {
                if (LastTurnWeather.Contains("North"))
                    return "North-Western";
                else if (LastTurnWeather.Contains("South"))
                    return "South-Western";
                else
                    return "Western";
            }
            else if (LastTurnWeather.Contains("Northern"))
                return "Northern";
            else if (LastTurnWeather.Contains("Southern"))
                return "Southern";
            else
                return "Zero";
        }
    }
    public string CurrentWindDirection
    {
        get
        {
            if (CurrentWeather.Contains("Eastern"))
            {
                if (CurrentWeather.Contains("North"))
                    return "North-Eastern";
                else if (CurrentWeather.Contains("South"))
                    return "South-Eastern";
                else
                    return "Eastern";
            }
            else if (CurrentWeather.Contains("Western"))
            {
                if (CurrentWeather.Contains("North"))
                    return "North-Western";
                else if (CurrentWeather.Contains("South"))
                    return "South-Western";
                else
                    return "Western";
            }
            else if (CurrentWeather.Contains("Northern"))
                return "Northern";
            else if (CurrentWeather.Contains("Southern"))
                return "Southern";
            else
                return "Zero";
        }
        set
        {
            CurrentWeather = CurrentWeather.Replace($"{CurrentWindSpeed} {CurrentWindDirection} wind", $"{CurrentWindSpeed} {value} wind");
        }
    }
    public Vector2 CurrentWindDirectionVector
    {
        get
        {
            return windDirectionVectors[CurrentWindDirection];
        }
    }
    public string NextTurnWindDirection
    {
        get
        {
            if (NextTurnWeather.Contains("Eastern"))
            {
                if (NextTurnWeather.Contains("North"))
                    return "North-Eastern";
                else if (NextTurnWeather.Contains("South"))
                    return "South-Eastern";
                else
                    return "Eastern";
            }
            else if (NextTurnWeather.Contains("Western"))
            {
                if (NextTurnWeather.Contains("North"))
                    return "North-Western";
                else if (NextTurnWeather.Contains("South"))
                    return "South-Western";
                else
                    return "Western";
            }
            else if (NextTurnWeather.Contains("Northern"))
                return "Northern";
            else if (NextTurnWeather.Contains("Southern"))
                return "Southern";
            else
                return "Zero";
        }
    }
    public string LastTurnWindSpeed
    {
        get
        {
            if (Regex.IsMatch(LastTurnWeather, @"(?<= Strong)(.*)(?= wind)"))
                return "Strong";
            else if (Regex.IsMatch(LastTurnWeather, @"(?<= Moderate)(.*)(?= wind)"))
                return "Moderate";
            else if (Regex.IsMatch(LastTurnWeather, @"(?<= Light)(.*)(?= wind)"))
                return "Light";
            else
                return "Zero";
        }
    }
    public string CurrentWindSpeed
    {
        get
        {
            if (Regex.IsMatch(CurrentWeather, @"(?<= Strong)(.*)(?= wind)"))
                return "Strong";
            else if (Regex.IsMatch(CurrentWeather, @"(?<= Moderate)(.*)(?= wind)"))
                return "Moderate";
            else if (Regex.IsMatch(CurrentWeather, @"(?<= Light)(.*)(?= wind)"))
                return "Light";
            else
                return "Zero";
        }
        set
        {
            CurrentWeather = CurrentWeather.Replace($"{CurrentWindSpeed} {CurrentWindDirection} wind", $"{value} {CurrentWindDirection} wind");
        }
    }
    public string NextTurnWindSpeed
    {
        get
        {
            if (Regex.IsMatch(NextTurnWeather, @"(?<= Strong)(.*)(?= wind)"))
                return "Strong";
            else if (Regex.IsMatch(NextTurnWeather, @"(?<= Moderate)(.*)(?= wind)"))
                return "Moderate";
            else if (Regex.IsMatch(NextTurnWeather, @"(?<= Light)(.*)(?= wind)"))
                return "Light";
            else
                return "Zero";
        }
    }

    [SerializeField]
    private bool isDataLoaded;
    public bool IsDataLoaded { get { return isDataLoaded; } }
}
