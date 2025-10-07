using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public static class HelperFunctions
{
    public static bool AnyKeyPressed()
    {
        if (Keyboard.current.anyKey.isPressed || Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
            return true;
        return false;
    }
    public static Vector2 MousePosition()
    {
        return Mouse.current.position.ReadValue();
    }
    public static bool RightMousePressed()
    {
        if (Mouse.current.rightButton.isPressed)
            return true;
        return false;
    }
    public static bool OverrideKeyPressed()
    {
        if (Keyboard.current.shiftKey.isPressed)
            return true;
        return false;
    }
    public static bool SecondOverrideKeyPressed()
    {
        if (Keyboard.current.spaceKey.isPressed)
            return true;
        return false;
    }
    public static bool SecondOverrideKeyDown()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;
        return false;
    }
    public static bool SecondOverrideKeyUp()
    {
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            return true;
        return false;
    }
    public static bool DeathKeyPressed()
    {
        if (Keyboard.current.dKey.wasPressedThisFrame)
            return true;
        return false;
    }
    public static bool MuteKeyDown()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
            return true;
        return false;
    }
    public static void RemoveOption(TMP_Dropdown dropdown, string option)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == option)
            {
                dropdown.options.RemoveAt(i);
                dropdown.RefreshShownValue();
                break;
            }
        }
    }
    public static bool CheckInScene(string sceneName)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(sceneName))
            return true;
        return false;
    }
    public static bool ValidateIntInput(TMP_InputField inputField, out int outputInt)
    {
        Color normalTextColour = new(0.196f, 0.196f, 0.196f);
        outputInt = 0;
        if (inputField.textComponent.color == normalTextColour)
        {
            if (int.TryParse(inputField.text, out int innerOutputInt))
            {
                outputInt = innerOutputInt;
                return true;
            }
        }
        return false;
    }
    public static int DiceRoll()
    {
        return RandomNumber(1, 6);
    }
    public static bool CoinFlip()
    {
        if (RandomNumber(0, 1) == 1)
            return true;

        return false;
    }
    public static Vector3 ConvertPhysicalPosToMathPos(Vector3 physicalPos)
    {
        return new(Mathf.CeilToInt(physicalPos.x + 0.5f), Mathf.CeilToInt(physicalPos.z), Mathf.CeilToInt(physicalPos.y + 0.5f));
    }
    public static Vector3 ConvertMathPosToPhysicalPos(Vector3 mathPos)
    {
        return new(mathPos.x - 0.5f, mathPos.z, mathPos.y - 0.5f);
    }
    public static int RandomNumber(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }
    public static int RandomShotNumber()
    {
        if (RightMousePressed())
            return 1;
        else
            return RandomNumber(1, 100);
    }
    public static int RandomCritNumber()
    {
        if (RightMousePressed())
            return 1;
        else
            return RandomNumber(1, 100);
    }
    public static bool RandomDipelecCoinFlip()
    {
        if (RightMousePressed())
            return true;
        else
        {
            if (RandomNumber(0, 1) == 1)
                return true;
        }
        return false;
    }
    public static bool IsWithinAngle(Vector3 pointA, Vector3 pointB, Vector3 centralPoint, float angleThreshold)
    {
        Debug.Log($"Checking is within angle: {angleThreshold}");
        return CalculateAngle180(pointA, pointB, centralPoint) <= angleThreshold;
    }
    public static float CalculateAngle180(Vector3 pointA, Vector3 pointB, Vector3 centralPoint)
    {
        Vector2 directionA = new Vector2(pointA.x - centralPoint.x, pointA.y - centralPoint.y).normalized;
        Vector2 directionB = new Vector2(pointB.x - centralPoint.x, pointB.y - centralPoint.y).normalized;
        Debug.Log($"angle is {Vector2.Angle(directionA, directionB)}");
        return Vector2.Angle(directionA, directionB);
    }
    public static float CalculateAngle360(Vector3 pointA, Vector3 pointB, Vector3 centralPoint)
    {
        Vector2 directionA = new Vector2(pointA.x - centralPoint.x, pointA.y - centralPoint.y).normalized;
        Vector2 directionB = new Vector2(pointB.x - centralPoint.x, pointB.y - centralPoint.y).normalized;

        float angle = Vector2.SignedAngle(directionA, directionB);

        if (angle < 0)
            angle += 360;

        return angle;
    }
    public static int CalculateSuppression(int existingSup, int addingSup)
    {
        float fExistingSup = existingSup / 100f, fAddingSup = addingSup / 100f;
        Debug.Log($"{fExistingSup}|{fAddingSup}");

        return Mathf.RoundToInt(100 * (1 - (1 - fExistingSup) * (1 - fAddingSup)));
    }
    public static Tuple<int, int> CalculateScatteredCoordinates(int targetX, int targetY, float scatterDegree, float scatterDistance)
    {
        // Convert degree to radians
        double radians = Math.PI * scatterDegree / 180.0;

        // Calculate new coordinates
        int newX = Mathf.RoundToInt((float)(targetX + scatterDistance * Math.Cos(radians)));
        int newY = Mathf.RoundToInt((float)(targetY + scatterDistance * Math.Sin(radians)));

        return Tuple.Create(newX, newY);
    }
    public static string SoldierBinocedString(Soldier soldierBinoced)
    {
        return $"{soldierBinoced.Id} ({soldierBinoced.X}, {soldierBinoced.Y}, {soldierBinoced.Z})";
    }
    public static string FindStringInColXReturnStringInColYInMatrix(string[,] matrix, string searchString, int x, int y)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            if (matrix[i, x] == searchString)
                return matrix[i, y];
        return null;
    }
    public static string PrintList<T>(List<T> list)
    {
        string str = "";
        //str += "[";
        foreach (object obj in list)
        {
            str += obj.ToString();
            if (list.Count > 1 && obj != (object)list.Last())
                str += ", ";
        }
        //str += "]";
        return str;
    }
    public static int Factorial(int number)
    {
        if (number == 0)
            return 1;
        else
        {
            int result = 1;

            while (number != 1)
            {
                result *= number;
                number--;
            }
            return result;
        }
    }
    public static float BinomialProbability(int n, int x, float p, float q)
    {
        if (n >= x)
            return Factorial(n) / (Factorial(n - x) * Factorial(x)) * Mathf.Pow(p, x) * Mathf.Pow(q, n - x);
        else
            return 0;
    }
    public static float CumulativeBinomialProbability(int n, int x, float p, float q)
    {
        if (n >= x)
        {
            float result = 0;

            while (x <= n)
            {
                result += BinomialProbability(n, x, p, q);
                x++;
            }
            return result;
        }
        else
            return 0;
    }
}
