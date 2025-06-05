using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1, // Peon
    Rook = 2, // Torre
    Knight = 3, // Caballo
    Bishop = 4, // Alfil
    Queen = 5, // Reina
    King = 6 // Rey
}

public class ChessPiece : MonoBehaviour
{
    public int team; // 0 white | 1 black
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private int speed = 10;
    private Vector3 desiredPosition;
    private Vector3 desiredRotation;
    private Vector3 desiredScale = Vector3.one;

    private float tileLimits = 1; // Los limites donde la pieza permanece dentro

    private bool isKinematic = false; // Si la pieza es Kinematic

    private bool isFree = true; // Si la pieza es libre del update. Para dejar de actualizar su transform

    private void Update()
    {
        if (!isFree)
        {
            if ((transform.position.x < desiredPosition.x - tileLimits || transform.position.x > desiredPosition.x + tileLimits) ||
                (transform.position.z < desiredPosition.z - tileLimits || transform.position.z > desiredPosition.z + tileLimits))
            {
                if (!isKinematic)
                {
                    GetComponent<Rigidbody>().isKinematic = true; // Set Kinematic true mientras se mueve la pieza
                    isKinematic = true;
                }

                // Vector3.Lerp(Vector3 a, Vector3 b, float t) sirve para mover un objeto gradualmente entre dos puntos. Lo mismo para la rotacion
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);
                transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * speed);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(desiredRotation), Time.deltaTime * speed);
            }
            else
            {
                if (isKinematic)
                {
                    GetComponent<Rigidbody>().isKinematic = false; // Set Kinematic false cuando no se mueva la pieza
                    isKinematic = false;
                    isFree = true;
                }
            }
        }
    }

    public virtual void SetPositionWithLimits(Vector3 position, Vector3 rotation, float limits, bool force = false)
    {
        desiredPosition = position;
        desiredRotation = (team == 1) ? rotation : rotation + new Vector3(0, 180, 0);
        if (force)
        {
            transform.position = desiredPosition;
            transform.rotation = Quaternion.Euler(desiredRotation);
        }

        tileLimits = limits;
        isFree = false;
    }
    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }

    public List<Vector2Int> GetAllEnemyAvailableMoves(ref ChessPiece[,] board)
    {
        List<Vector2Int> allEnemyAvailableMoves = new List<Vector2Int>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null && board[x, y].team != team) // Por cada pieza enemiga en el tablero hacemos lo siguiente
                {
                    List<Vector2Int> enemyAvailableMoves = board[x, y].GetAvailableMoves(ref board, 8, 8); // Conseguimos sus movimientos posibles
                    allEnemyAvailableMoves = allEnemyAvailableMoves.Union<Vector2Int>(enemyAvailableMoves).ToList<Vector2Int>(); // Y los guardamos evitando duplicados
                }
            }
        }

        return allEnemyAvailableMoves;
    }

    // "virtual" functions can be overriden by any subclass. In the subclass, "override" functions are overriding "virtual" functions.
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3, 3));
        r.Add(new Vector2Int(3, 4));
        r.Add(new Vector2Int(4, 3));
        r.Add(new Vector2Int(4, 4));

        return r;
    }
    // "ref" reference allows the method to modify the original array. Without "ref" it would modify a copy of that array leaving the original unchanged.
    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves,
    bool castlingWhiteKingS, bool castlingWhiteQueenS, bool castlingBlackKingS, bool castlingBlackQueenS, string possibleEnPassantT)
    {
        return SpecialMove.None;
    }

}
