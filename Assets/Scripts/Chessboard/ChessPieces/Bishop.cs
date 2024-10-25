using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece // Inherit-Heredar from ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Top Right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if (board[x, y] == null) // There is nothing
                r.Add(new Vector2Int(x, y));

            if (board[x, y] != null) // There is something
            {
                if (board[x, y].team != team) // It is the enemy
                    r.Add(new Vector2Int(x, y));

                break; // We can not pass through the enemy or ally
            }
        }

        // Top Left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if (board[x, y] == null) // There is nothing
                r.Add(new Vector2Int(x, y));

            if (board[x, y] != null) // There is something
            {
                if (board[x, y].team != team) // It is the enemy
                    r.Add(new Vector2Int(x, y));

                break; // We can not pass through the enemy or ally
            }
        }

        // Bottom Right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if (board[x, y] == null) // There is nothing
                r.Add(new Vector2Int(x, y));

            if (board[x, y] != null) // There is something
            {
                if (board[x, y].team != team) // It is the enemy
                    r.Add(new Vector2Int(x, y));

                break; // We can not pass through the enemy or ally
            }
        }

        // Bottom Left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (board[x, y] == null) // There is nothing
                r.Add(new Vector2Int(x, y));

            if (board[x, y] != null) // There is something
            {
                if (board[x, y].team != team) // It is the enemy
                    r.Add(new Vector2Int(x, y));

                break; // We can not pass through the enemy or ally
            }
        }

        return r;
    }
}
