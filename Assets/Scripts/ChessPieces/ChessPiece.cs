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

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == 1) ? Vector3.zero : new Vector3(0, 180, 0)); // Se rotan 180 grados las piezas negras.
    }

    private void Update()
    {
        // Vector3.Lerp(Vector3 a, Vector3 b, float t) sirve para mover un objeto gradualmente entre dos puntos.
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
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
    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        return SpecialMove.None;
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
            transform.position = desiredPosition;
    }
    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }
}
