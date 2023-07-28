using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.WindowsRuntime;

public class WeatherGen : MonoBehaviour, IDataPersistence
{
    public MainGame game;

    Dictionary<int, string> visibility = new()
    {
        {0, "Zero"},
        {1, "Poor"},
        {2, "Moderate"},
        {3, "Good"},
        {4, "Full"}
    };
    Dictionary<int, string> windSpeed = new()
    {
        {0, "Zero"},
        {1, "Light"},
        {2, "Moderate"},
        {3, "Strong"}
    };
    Dictionary<int, string> windDirection = new()
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
    Dictionary<int, string> rain = new()
    {
        {0, "Zero"},
        {1, "Light"},
        {2, "Moderate"},
        {3, "Heavy"},
        {4, "Torrential"}
    };

    private List<List<int>> weather = new();
    public List<string> savedWeather = new();
    private int vis, sp, dir, rn;
    public void LoadData(GameData data)
    {
        savedWeather = data.savedWeather;
    }

    public void SaveData(ref GameData data)
    {
        data.savedWeather = savedWeather;
    }

    public void GenerateWeather()
    {
        vis = game.RandomNumber(0, 4);
        sp = game.RandomNumber(0, 3);
        dir = game.RandomNumber(0, 7);
        rn = game.RandomNumber(0, 4);

        for (int i = 0; i < game.maxRounds; i++)
        {
            List<int> turnWeather = new();
            turnWeather.Add(vis);
            turnWeather.Add(sp);
            turnWeather.Add(dir);
            turnWeather.Add(rn);
            weather.Add(turnWeather);

            vis = NextVis(vis);
            sp = NextWindSpeed(sp);
            dir = NextWindDirection(dir);
            rn = NextRain(rn);
        }

        foreach (List<int> w in weather)
        {
            string weatherString = "";
            weatherString += visibility[w[0]] + " visibility, ";
            if (w[1] == 0)
            {
                weatherString += "no wind, ";
            }
            else
            {
                weatherString += windSpeed[w[1]] + " ";
                weatherString += windDirection[w[2]] + " wind, ";
            }
            weatherString += rain[w[3]] + " rain.";
            savedWeather.Add(weatherString);
        }
    }

    private int NextVis(int vis)
    {
        if (vis == 0)
        {
            if (game.CoinFlip())
                vis++;
        }
        else if (vis == 4)
        {
            if (game.CoinFlip())
                vis--;
        }
        else
        {
            if (game.RandomNumber(1, 5) >= 4)
            {
                if (game.CoinFlip())
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
            if (game.CoinFlip())
                sp++;
        }
        else if (sp == 3)
        {
            if (game.CoinFlip())
                sp--;
        }
        else
        {
            if (game.RandomNumber(1, 5) >= 4)
            {
                if (game.CoinFlip())
                    sp--;
                else
                    sp++;
            }
        }

        return sp;
    }
    private int NextWindDirection(int dir)
    {
        if (game.RandomNumber(1, 3) == 3)
        {
            if (game.CoinFlip())
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
            if (game.CoinFlip())
                rn++;
        }
        else if (rn == 4)
        {
            if (game.CoinFlip())
                rn--;
        }
        else
        {
            if (game.RandomNumber(1, 5) >= 4)
            {
                if (game.CoinFlip())
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
    public string CurrentWeather
    {
        get { return savedWeather[game.currentRound - 1]; }
        set { savedWeather[game.currentRound - 1] = value; }
    }
    public string LastTurnWeather
    {
        get
        {
            return savedWeather[game.currentRound - 2];
        }
    }
    public string NextTurnWeather
    {
        get
        {
            return savedWeather[game.currentRound];
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
            CurrentWeather = CurrentWeather.Replace(CurrentVis + " visibility", value);
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
    }
    public Vector2 CurrentWindDirection
    {
        get
        {
            if (CurrentWeather.Contains("Eastern"))
            {
                if (CurrentWeather.Contains("North"))
                    return new(1, 1);
                else if (CurrentWeather.Contains("South"))
                    return new(1, -1);
                else
                    return new(1, 0);
            }
            else if (CurrentWeather.Contains("Western"))
            {
                if (CurrentWeather.Contains("North"))
                    return new(-1, 1);
                else if (CurrentWeather.Contains("South"))
                    return new(-1, -1);
                else
                    return new(-1, 0);
            }
            else if (CurrentWeather.Contains("Northern"))
                return new(0, 1);
            else if (CurrentWeather.Contains("Southern"))
                return new(0, -1);
            else
                return new(0, 0);
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
    }
}
