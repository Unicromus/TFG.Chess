using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece // Inherit-Heredar from ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        if (currentY == tileCountY - 1 || currentY == 0) // Limite del tablero
            return r;

        // One in front
        if (board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        // Two in front. Only at start; row 1 and row 6.
        if (board[currentX, currentY + direction] == null)
        {
            // White team
            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            // Black team
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
        }

        // Kill move RIGHT
        if (currentX != tileCountX - 1) // tileCountX = 8
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        // Kill move LEFT
        if (currentX != 0)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves,
    bool castlingWhiteKingS, bool castlingWhiteQueenS, bool castlingBlackKingS, bool castlingBlackQueenS, string possibleEnPassantT)
    {
        int direction = (team == 0) ? 1 : -1;

        // Promotion
        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1))
            return SpecialMove.Promotion;

        // En Passant
        if (possibleEnPassantT != "-")
            if (moveList.Count > 0)
            {
                Vector2Int[] lastMove = moveList[moveList.Count - 1];

                if (board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn) // If the last piece moved was a pawn
                    if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) // If the last move was a +2 in either direction
                        if (board[lastMove[1].x, lastMove[1].y].team != team) // If the move was from the other team. Always have to be.
                            if (lastMove[1].y == currentY) // If both pawns are on the same Y
                            {
                                if (lastMove[1].x == currentX - 1) // Landed Left
                                {
                                    availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                    return SpecialMove.EnPassant;
                                }
                                if (lastMove[1].x == currentX + 1) // Landed Right
                                {
                                    availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                    return SpecialMove.EnPassant;
                                }
                            }
            }

        return SpecialMove.None;
    }
}
