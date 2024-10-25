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

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves,
    bool castlingWhiteKingS, bool castlingWhiteQueenS, bool castlingBlackKingS, bool castlingBlackQueenS, string possibleEnPassantT)
    {
        List<Vector2Int> allEnemyAvailableMoves = new List<Vector2Int>();
        Vector2Int castlingPath_2, castlingPath_3, castlingPath_4, castlingPath_5, castlingPath_6; // [0, 0]
        var noCheck = new Vector2Int(0, 0);

        int ourY = (team == 0) ? 0 : 7; // Check if we are white team or black team and save Y position
        SpecialMove specialM = SpecialMove.None;

        if (team == 0) // White team
        {
            if (castlingWhiteKingS || castlingWhiteQueenS && currentX == 4) // One or both castling are possible and our king is in the right position
            {
                if (castlingWhiteKingS && board[7, ourY] != null) // Right Rook - King Side
                    if (board[7, ourY].type == ChessPieceType.Rook && board[7, ourY].team == team) // Check if our Right Rook is where it should be
                        if (board[5, ourY] == null && board[6, ourY] == null) // Check if there is no piece between our king and our rightRook
                        {
                            allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque

                            castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey
                            castlingPath_5 = allEnemyAvailableMoves.Find(m => m.x == 5 && m.y == ourY); // Donde acabaria la torre derecha
                            castlingPath_6 = allEnemyAvailableMoves.Find(m => m.x == 6 && m.y == ourY); // Donde acabaria el rey
                            //castlingPath_7 = allEnemyAvailableMoves.Find(m => m.x == 7 && m.y == ourY); // Donde esta la torre derecha

                            if (castlingPath_4 == noCheck && castlingPath_5 == noCheck && castlingPath_6 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                            {
                                availableMoves.Add(new Vector2Int(6, ourY));
                                specialM = SpecialMove.Castling;
                            }
                        }
                if (castlingWhiteQueenS && board[0, ourY] != null) // Left Rook - Queen Side
                    if (board[0, ourY].type == ChessPieceType.Rook && board[0, ourY].team == team) // Check if our Left Rook is where it should be
                        if (board[3, ourY] == null && board[2, ourY] == null && board[1, ourY] == null) // Check if there is no piece between our king and our leftRook
                        {
                            allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque

                            //castlingPath_0 = allEnemyAvailableMoves.Find(m => m.x == 0 && m.y == ourY); // Donde esta la torre izquierda
                            //castlingPath_1 = allEnemyAvailableMoves.Find(m => m.x == 1 && m.y == ourY);
                            castlingPath_2 = allEnemyAvailableMoves.Find(m => m.x == 2 && m.y == ourY); // Donde acabaria el rey
                            castlingPath_3 = allEnemyAvailableMoves.Find(m => m.x == 3 && m.y == ourY); // Donde acabaria la torre izquierda
                            castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey

                            if (castlingPath_2 == noCheck && castlingPath_3 == noCheck && castlingPath_4 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                            {
                                availableMoves.Add(new Vector2Int(2, ourY));
                                specialM = SpecialMove.Castling;
                            }
                        }
            }
        }
        else if (team == 1) // Black team
        {
            if (castlingBlackKingS || castlingBlackQueenS && currentX == 4) // One or both castling are possible and our king is in the right position
            {
                if (castlingBlackKingS && board[7, ourY] != null) // Right Rook - King Side
                    if (board[7, ourY].type == ChessPieceType.Rook && board[7, ourY].team == team) // Check if our Right Rook is where it should be
                        if (board[5, ourY] == null && board[6, ourY] == null) // Check if there is no piece between our king and our rightRook
                        {
                            allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque

                            castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey
                            castlingPath_5 = allEnemyAvailableMoves.Find(m => m.x == 5 && m.y == ourY); // Donde acabaria la torre derecha
                            castlingPath_6 = allEnemyAvailableMoves.Find(m => m.x == 6 && m.y == ourY); // Donde acabaria el rey
                                                                                                        //castlingPath_7 = allEnemyAvailableMoves.Find(m => m.x == 7 && m.y == ourY); // Donde esta la torre derecha

                            if (castlingPath_4 == noCheck && castlingPath_5 == noCheck && castlingPath_6 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                            {
                                availableMoves.Add(new Vector2Int(6, ourY));
                                specialM = SpecialMove.Castling;
                            }
                        }
                if (castlingBlackQueenS && board[0, ourY] != null) // Left Rook - Queen Side
                    if (board[0, ourY].type == ChessPieceType.Rook && board[0, ourY].team == team) // Check if our Left Rook is where it should be
                        if (board[3, ourY] == null && board[2, ourY] == null && board[1, ourY] == null) // Check if there is no piece between our king and our leftRook
                        {
                            allEnemyAvailableMoves = GetAllEnemyAvailableMoves(ref board); // Queremos saber si el enemigo esta amenazando las ubicaciones del enroque

                            //castlingPath_0 = allEnemyAvailableMoves.Find(m => m.x == 0 && m.y == ourY); // Donde esta la torre izquierda
                            //castlingPath_1 = allEnemyAvailableMoves.Find(m => m.x == 1 && m.y == ourY);
                            castlingPath_2 = allEnemyAvailableMoves.Find(m => m.x == 2 && m.y == ourY); // Donde acabaria el rey
                            castlingPath_3 = allEnemyAvailableMoves.Find(m => m.x == 3 && m.y == ourY); // Donde acabaria la torre izquierda
                            castlingPath_4 = allEnemyAvailableMoves.Find(m => m.x == 4 && m.y == ourY); // Donde esta el rey

                            if (castlingPath_2 == noCheck && castlingPath_3 == noCheck && castlingPath_4 == noCheck) // The King can not be in check, pass or end the move on a threatened square on the board
                            {
                                availableMoves.Add(new Vector2Int(2, ourY));
                                specialM = SpecialMove.Castling;
                            }
                        }
            }
        }
        return specialM;
    }
}
