using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece // Inherit-Heredar from ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (board[currentX, i] == null) // There is nothing
                r.Add(new Vector2Int(currentX, i));

            if (board[currentX, i] != null) // There is something
            {
                if (board[currentX, i].team != team) // It is the enemy
                    r.Add(new Vector2Int(currentX, i));

                break; // We can not pass through the enemy or ally
            }
        }

        // Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if (board[currentX, i] == null) // There is nothing
                r.Add(new Vector2Int(currentX, i));

            if (board[currentX, i] != null) // There is something
            {
                if (board[currentX, i].team != team) // It is the enemy
                    r.Add(new Vector2Int(currentX, i));

                break; // We can not pass through the enemy or ally
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null) // There is nothing
                r.Add(new Vector2Int(i, currentY));

            if (board[i, currentY] != null) // There is something
            {
                if (board[i, currentY].team != team) // It is the enemy
                    r.Add(new Vector2Int(i, currentY));

                break; // We can not pass through the enemy or ally
            }
        }

        // Right
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            if (board[i, currentY] == null) // There is nothing
                r.Add(new Vector2Int(i, currentY));

            if (board[i, currentY] != null) // There is something
            {
                if (board[i, currentY].team != team) // It is the enemy
                    r.Add(new Vector2Int(i, currentY));

                break; // We can not pass through the enemy or ally
            }
        }

        return r;
    }
}
