using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece // Inherit-Heredar from ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Right
        if (currentX + 1 < tileCountX)
        {
            // Right
            if (board[currentX + 1, currentY] == null)
                r.Add(new Vector2Int(currentX + 1, currentY));
            else if (board[currentX + 1, currentY].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY));

            // Right Top
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX + 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
                else if (board[currentX + 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY + 1));
            }

            // Right Bottom
            if (currentY - 1 >= 0)
            {
                if (board[currentX + 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
                else if (board[currentX + 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX + 1, currentY - 1));
            }
        }

        // Left
        if (currentX - 1 >= 0)
        {
            // Left
            if (board[currentX - 1, currentY] == null)
                r.Add(new Vector2Int(currentX - 1, currentY));
            else if (board[currentX - 1, currentY].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY));

            // Left Top
            if (currentY + 1 < tileCountY)
            {
                if (board[currentX - 1, currentY + 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
                else if (board[currentX - 1, currentY + 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY + 1));
            }

            // Left Bottom
            if (currentY - 1 >= 0)
            {
                if (board[currentX - 1, currentY - 1] == null)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
                else if (board[currentX - 1, currentY - 1].team != team)
                    r.Add(new Vector2Int(currentX - 1, currentY - 1));
            }
        }

        // Up
        if (currentY + 1 < tileCountY)
            if (board[currentX, currentY + 1] == null || board[currentX, currentY + 1].team != team)
                r.Add(new Vector2Int(currentX, currentY + 1));

        // Down
        if (currentY - 1 >= 0)
            if (board[currentX, currentY - 1] == null || board[currentX, currentY - 1].team != team)
                r.Add(new Vector2Int(currentX, currentY - 1));

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        List<Vector2Int> allEnemyAvailableMoves = new List<Vector2Int>();
        Vector2Int castlingPath_2, castlingPath_3, castlingPath_4, castlingPath_5, castlingPath_6; // [0, 0]
        var noCheck = new Vector2Int(0, 0);

        int ourY = (team == 0) ? 0 : 7; // Check if we are white team or black team and save Y position
        SpecialMove r = SpecialMove.None;

        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ourY); // We find a move where our king has been moved; in 4,0 (white) or 4,7 (black) position
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ourY); // We find a move where our leftRook has been moved; in 0,0 (white) or 0,7 (black) position
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ourY); // We find a move where our rightRook has been moved; in 7,0 (white) or 7,7 (black) position

        if (kingMove == null && currentX == 4) // If our king has never been moved
        {
            // Left Rook
            if (leftRook == null) // If our leftRook has never been moved
                if (board[0, ourY].type == ChessPieceType.Rook && board[0, ourY].team == team) // Check again, if our leftRook is where it should be
                    if (board[3, ourY] == null && board[2, ourY] == null && board[1, ourY] == null) // Check if there is no piece between our king and our leftRook
                    {
                        allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque
                        /*foreach (var item in allEnemyAvailableMoves)
                        {
                            Debug.Log(item.ToString());
                        }*/
                        //castlingPath_0 = allEnemyAvailableMoves.Find(m => m.x == 0 && m.y == ourY); // Donde esta la torre izquierda
                        //castlingPath_1 = allEnemyAvailableMoves.Find(m => m.x == 1 && m.y == ourY);
                        castlingPath_2 = allEnemyAvailableMoves.Find(m => m.x == 2 && m.y == ourY); // Donde acabaria el rey
                        castlingPath_3 = allEnemyAvailableMoves.Find(m => m.x == 3 && m.y == ourY); // Donde acabaria la torre izquierda
                        castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey
                        //Debug.Log(castlingPath_0);
                        //Debug.Log(castlingPath_1);
                        //Debug.Log(castlingPath_2);
                        //Debug.Log(castlingPath_3);
                        //Debug.Log(castlingPath_4);
                        if (castlingPath_2 == noCheck && castlingPath_3 == noCheck && castlingPath_4 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                        {
                            availableMoves.Add(new Vector2Int(2, ourY));
                            r = SpecialMove.Castling;
                        }
                    }
            // Right Rook
            if (rightRook == null) // If our rightRook has never been moved
                if (board[7, ourY].type == ChessPieceType.Rook && board[7, ourY].team == team) // Check again, if our rightRook is where it should be
                    if (board[5, ourY] == null && board[6, ourY] == null) // Check if there is no piece between our king and our rightRook
                    {
                        allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque
                        /*foreach (var item in allEnemyAvailableMoves)
                        {
                            Debug.Log(item.ToString());
                        }*/
                        castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey
                        castlingPath_5 = allEnemyAvailableMoves.Find(m => m.x == 5 && m.y == ourY); // Donde acabaria la torre derecha
                        castlingPath_6 = allEnemyAvailableMoves.Find(m => m.x == 6 && m.y == ourY); // Donde acabaria el rey
                        //castlingPath_7 = allEnemyAvailableMoves.Find(m => m.x == 7 && m.y == ourY); // Donde esta la torre derecha

                        if (castlingPath_4 == noCheck && castlingPath_5 == noCheck && castlingPath_6 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                        {
                            availableMoves.Add(new Vector2Int(6, ourY));
                            r = SpecialMove.Castling;
                        }
                    }
        }
        return r;
    }
}
