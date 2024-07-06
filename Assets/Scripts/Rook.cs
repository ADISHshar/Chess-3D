using System.Collections;
using UnityEngine;

public class Rook : Chessman
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];

        int i;

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (Move(i, CurrentY, ref r))
            {
                Debug.Log("Blocked at (" + i + ", " + CurrentY + ")");
                break;
            }
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (Move(i, CurrentY, ref r))
            {
                Debug.Log("Blocked at (" + i + ", " + CurrentY + ")");
                break;
            }
        }

        // Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (Move(CurrentX, i, ref r))
            {
                Debug.Log("Blocked at (" + CurrentX + ", " + i + ")");
                break;
            }
        }

        // Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (Move(CurrentX, i, ref r))
            {
                Debug.Log("Blocked at (" + CurrentX + ", " + i + ")");
                break;
            }
        }

        return r;

    }
}
