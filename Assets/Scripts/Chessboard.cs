using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class Chessboard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial; // El material de las baldosas invisibles.
    [SerializeField] private float tileSize = 1.0f; // El tamaño de cada baldosa invisible.
    [SerializeField] private float yOffset = 0.2f; // La distancia entre el tablero y las baldosas invisibles.
    [SerializeField] private Vector3 boardCenter = Vector3.zero; // El centro del tablero.
    [SerializeField] private float deathSize = 0.5f; // El tamaño de las piezas derrotadas.
    //[SerializeField] private float deathSpacing = 0.3f; // El espacio entre cada pieza derrotada.
    //[SerializeField] private float dragOffset = 1.0f; // La distancia para elevar las piezas a la hora de moverlas.
    [SerializeField] private GameObject victoryScreen;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs; // Los prefabs de las piezas de ajedrez.
    [SerializeField] private Material[] teamMaterials; // Los materiales de los equipos del ajedrez, blanco y negro.

    // LOGIC
    private ChessPiece[,] chessPieces; // La logica del tablero, contiene las piezas del ajedrez. 32 piezas en un tablero de 8x8, si esta vacio no hay pieza en esa baldosa.
    private ChessPiece currentlyDragging; // La ficha que se esta arrastrando.
    private List<Vector2Int> availableMoves = new List<Vector2Int>(); // Lista de movimientos posibles.
    private List<ChessPiece> deadWhites = new List<ChessPiece>(); // Lista de las piezas blancas derrotadas.
    private List<ChessPiece> deadBlacks = new List<ChessPiece>(); // Lista de las piezas negras derrotadas.
    private const int TILE_COUNT_X = 8; // Cantidad de baldosas en eje X.
    private const int TILE_COUNT_Y = 8; // Cantidad de baldosas en eje Y.
    private GameObject[,] tiles; // Las baldosas invisibles 8x8.
    private Camera currentCamera; // La camara actual.
    private Vector2Int currentHover; // Cursor actual.
    private Vector3 bounds; // Los limites del tablero. En este caso se utiliza para saber el punto inicial desde donde generar las baldosas invisibles.
    private bool isWhiteTurn;

    private SpecialMove specialMove; // Puede ser 0, EnPassant, Castling o Promotion.
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>(); // La lista que contiene todos los movimientos del tablero. Lista[antiguaPosicion(x, y), nuevaPosicion(x, y)]

    private void Awake()
    {
        isWhiteTurn = true;

        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllpieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tile i have hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we are hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                // Sentence if true or false. Sentence ? true : false --> "Ternary operator"
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0)) // Si presionamos el click izquierdo del raton
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn? If is white turn and you are trying to grab a white piece or if is black turn and you are trying to grab a black piece, then continue.
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn))
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        // Get a list of available moves of the piece that is currently dragging.
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // Get what special move is possible and add the special move to available moves.
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves); // References reduce memory use

                        // Check if our king will be in danger if we move our piece.
                        // Crearemos una simulación para cada movimiento posible de la pieza y comprobaremos si el rey termina siendo amenazado al realizar los movimientos.
                        PreventCheck();

                        // Highlight tiles using availableMoves
                        HighlightTiles();
                    }
                }
            }

            // If we are releasing the mouse buttom
            if (currentlyDragging != null && Input.GetMouseButtonUp(0)) // Si soltamos el click izquierdo del raton. Up = Soltar, 0 = LeftButton
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                currentlyDragging = null;
                RemoveHighlightTiles(); // Every time we stop dragging a piece we remove the HighlightTiles
            }

        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                // Sentence if true or false. Sentence ? true : false --> "Ternary operator"
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles(); // Every time we stop dragging a piece we remove the HighlightTiles
            }
        }

        // If we're dragging a piece
        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset); // invisible Plane for Raycast
            float distance = 0.0f;
            // if ray in camera view hits horizontalPlane (should happen always). distance will be the distance between horizontalPlane and raycast.
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + boardCenter + (Vector3.up * yOffset * 25)); // distance is used to move smoothly the piece that is currentlyDragging.
        }
    }

    // Generate the board. Se generan las baldosas.
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3(-(tileCountX / 2) * tileSize, 0, -(tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) + bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) + bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) + bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) + bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Spawning of the pieces. Se crean las piezas de ajedrez.
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        // White team
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

        // Black team
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>(); // we do -1 because ChessPieceType has None 0

        cp.type = type; // type: 0-6
        cp.team = team; // team: 0 white | 1 black
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        //cp.GetComponent<MeshRenderer>().material = teamMaterials[((team == 0) ? 0 : 6) + ((int)type - 1)]; // if there is more than 2 materials

        return cp;
    }

    // Positioning. Se posicionan todas las piezas de ajedrez al estado inicial.
    private void PositionAllpieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false) // la variable "force" determina si quieres mover la ficha de forma rapida (teletransporte) o lenta
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) + bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Highlight Tiles. Se resaltan las baldosas con los movimientos posibles.
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        availableMoves.Clear();
    }

    // Checkmate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton()
    {
        // Hide UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(2).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        // Fields reset
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();

        // Clean up. Destroying gameObjects
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null)
                    Destroy(chessPieces[x, y].gameObject);

                chessPieces[x, y] = null;
            }
        }

        for (int i = 0; i < deadWhites.Count; i++)
            Destroy(deadWhites[i].gameObject);
        for (int i = 0; i < deadBlacks.Count; i++)
            Destroy(deadBlacks[i].gameObject);

        deadWhites.Clear();
        deadBlacks.Clear();

        // ReStart the game
        SpawnAllPieces();
        PositionAllpieces();
        isWhiteTurn = true;
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    // Special Moves
    private void ProcessSpecialMove()
    {
        // En Passant - Captura al paso
        if (specialMove == SpecialMove.EnPassant)
        {
            var newMove = moveList[moveList.Count - 1];
            ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            if (myPawn.currentX == enemyPawn.currentX)
            {
                if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
                {
                    if (enemyPawn.team == 0) // White Pawn
                    {
                        deadWhites.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * deathSize);
                        enemyPawn.SetPosition(
                            new Vector3(8 * tileSize, 2 * yOffset, 0) // Fuera de la tabla 0-7 --> -1 y 8. x2 yOffset por la geometria de la tabla.
                            + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                            + new Vector3((tileSize), 0, 0) // Distancia extra para posicionar segun la geometria de la tabla.
                            + (Vector3.forward * (tileSize / 2)) * (deadWhites.Count - 1)); // Distancia entre las piezas derrotadas.
                    }
                    else // enemyPawn.team == 1. Black Pawn
                    {
                        deadBlacks.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * deathSize);
                        enemyPawn.SetPosition(
                            new Vector3(0, 2 * yOffset, 8 * tileSize) // Fuera de la tabla 0-7 --> -1 y 8. x2 yOffset por la geometria de la tabla.
                            + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                            + new Vector3(-(tileSize), 0, 0) // Distancia extra para posicionar segun la geometria de la tabla.
                            + (Vector3.back * (tileSize / 2)) * (deadBlacks.Count - 1)); // Distancia entre las piezas derrotadas.
                    }
                    chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }
        }

        // Castling - Enroque
        if (specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            int ourY = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 0 : 7; // Check if we are white team or black team and save Y position

            // Left Rook
            if (lastMove[1].x == 2 && lastMove[1].y == ourY)
            {
                ChessPiece rook = chessPieces[0, ourY];
                chessPieces[3, ourY] = rook;
                PositionSinglePiece(3, ourY);
                chessPieces[0, ourY] = null;
            }
            // Right Rook
            else if (lastMove[1].x == 6 && lastMove[1].y == ourY)
            {
                ChessPiece rook = chessPieces[7, ourY];
                chessPieces[5, ourY] = rook;
                PositionSinglePiece(5, ourY);
                chessPieces[7, ourY] = null;
            }
        }

        // Promotion - Promoción
        if (specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if (targetPawn.type == ChessPieceType.Pawn) // It's a Pawn
            {
                if (targetPawn.team == 0 && lastMove[1].y == 7) // White team and last position 7
                {
                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0); // Create white newQueen
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Set position smoother
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject); // Destroy Pawn
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen; // Set newQueen
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y); // Update position
                }
                if (targetPawn.team == 1 && lastMove[1].y == 0) // Black team and last  position 0
                {
                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 1); // Create black newQueen
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Set position smoother
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject); // Destroy Pawn
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen; // Set newQueen
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y); // Update position
                }
            }
        }
    }
    private void PreventCheck() // Before we move (while we are dragging a piece, in Update() function), check with a simulation if the king gonna be in danger, if so, remove that move as an possible option.
    {
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].type == ChessPieceType.King)
                        if (chessPieces[x, y].team == currentlyDragging.team)
                            targetKing = chessPieces[x, y];

        // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
        SimulateMoveForSinglePiece(currentlyDragging, ref availableMoves, targetKing);
    }
    private void SimulateMoveForSinglePiece(ChessPiece cPiece, ref List<Vector2Int> pMoves, ChessPiece targetKing)
    {
        // Save the current values, to reset after the function call
        int actualX = cPiece.currentX;
        int actualY = cPiece.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going through all the moves, simulate them and check if we're in check
        for (int i = 0; i < pMoves.Count; i++)
        {
            // Simulation move position x and y
            int simX = pMoves[i].x;
            int simY = pMoves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
            // Did we simulate the king's move. Si simulamos los movimientos del rey, la posición del rey cambiara en esta simulación.
            if (cPiece.type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);

            // Copy the board [,] and not a reference and store attacking pieces
            ChessPiece[,] simulationBoard = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>(); // Enemy pieces
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulationBoard[x, y] = chessPieces[x, y];
                        if (simulationBoard[x, y].team != cPiece.team)
                            simAttackingPieces.Add(simulationBoard[x, y]);
                    }
                }
            }

            // Simulate that move
            simulationBoard[actualX, actualY] = null;
            cPiece.currentX = simX;
            cPiece.currentY = simY;
            simulationBoard[simX, simY] = cPiece;

            // Did one of the piece got taken down during our simulation, then delete it.
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);

            // Check if we did an EnPassant, if so, remove the pawn below or above
            if (specialMove == SpecialMove.EnPassant)
            {
                if (simY > 4) // SimY = 5 // It was a white piece move, so we remove the black piece below it.
                {
                    deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY - 1); // simY - 1 = 4
                    if (deadPiece != null)
                    {
                        simAttackingPieces.Remove(deadPiece);
                        simulationBoard[deadPiece.currentX, deadPiece.currentY] = null;
                    }
                }
                else // if (simY < 3) // simY = 2 // It was a black piece move, so we remove the white piece above it.
                {
                    deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY + 1); // simY + 1 = 3
                    if (deadPiece != null)
                    {
                        simAttackingPieces.Remove(deadPiece);
                        simulationBoard[deadPiece.currentX, deadPiece.currentY] = null;
                    }
                }
            }

            // Get all the simulated attacking pieces moves
            List<Vector2Int> simAttackingMoves = GetAllAvailableMoves(ref simulationBoard, simAttackingPieces);

            // Is the king in trouble? if so, remove the move.
            // Comprobamos si el rey esta siendo amenazado despues de hacer el movimiento en la simulación, si lo esta, eliminaremos el movimiento
            if (ContainsValidMove(ref simAttackingMoves, kingPositionThisSim))
            {
                movesToRemove.Add(pMoves[i]);
            }

            // Restore the actual ChessPiece data
            cPiece.currentX = actualX;
            cPiece.currentY = actualY;
        }

        // Remove from the current available move list
        for (int i = 0; i < movesToRemove.Count; i++)
            pMoves.Remove(movesToRemove[i]);
    }
    private List<Vector2Int> GetAllAvailableMoves(ref ChessPiece[,] board, List<ChessPiece> pieces)
    {
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int a = 0; a < pieces.Count; a++)
        {
            var pieceMoves = pieces[a].GetAvailableMoves(ref board, TILE_COUNT_X, TILE_COUNT_Y);
            for (int b = 0; b < pieceMoves.Count; b++)
                currentAvailableMoves.Add(pieceMoves[b]);
        }
        return currentAvailableMoves;
    }
    private int CheckForCheckmateAndStalemate() // After the enemy moves (in MoveTo() function), check if our king is in danger or not, and check throught a simulation if we have any possible moves to do.
    {
        // Which team's turn is it?
        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;

        // Get references
        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].team == targetTeam)
                    {
                        defendingPieces.Add(chessPieces[x, y]);
                        if (chessPieces[x, y].type == ChessPieceType.King)
                            targetKing = chessPieces[x, y];
                    }
                    else
                    {
                        attackingPieces.Add(chessPieces[x, y]);
                    }
                }

        // Get a list of all available enemy's moves
        List<Vector2Int> currentAvailableMoves = GetAllAvailableMoves(ref chessPieces, attackingPieces);

        // CHECK. Are we in check right now? Comprobar si estamos en JAQUE (el rey siendo amenazado por alguna pieza enemiga)
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            // King is under attack, can we move something to help him?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) // There is a piece that can protect our king or we can move our king
                    return 0;
            }

            return 1; // CHECKMATE. There is no piece that can protect our king and we cant move it. Checkmate exit. JAQUEMATE.
        }
        // NO estamos en JAQUE
        else
        {
            // King is not under attack, can we move something?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) // There is a piece that can be moved or we can move our king
                    return 0;
            }

            return 2; // STALEMATE. There is no piece that can be moved. Stalemate exit. TABLAS.
        }

        //return 0;
    }

    // Operations
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    private bool MoveTo(ChessPiece chessPiece, int x, int y)
    {
        if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            return false;

        Vector2Int previousPosition = new Vector2Int(chessPiece.currentX, chessPiece.currentY);

        // Check if there is another piece on the target position
        if (chessPieces[x, y] != null)
        {
            ChessPiece otherChessPiece = chessPieces[x, y];

            // If its our team. 0 == 0 or 1 == 1. Dont Move.
            if (chessPiece.team == otherChessPiece.team)
                return false;

            // If its the enemy team and we are black
            if (otherChessPiece.team == 0)
            {
                // If the enemy piece is the king. BLACK WINS. No debe ocurrir porque nunca se llega a comer un rey. Antes ocurre el JaqueMate. Más abajo se procesa.
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(1);

                // Else, move dead Piece
                deadWhites.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize);
                otherChessPiece.SetPosition(
                    new Vector3(8 * tileSize, 2 * yOffset, 0) // Fuera de la tabla 0-7 --> -1 y 8. x2 yOffset por la geometria de la tabla.
                    + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                    + new Vector3((tileSize), 0, 0) // Distancia extra para posicionar segun la geometria de la tabla.
                    + (Vector3.forward * (tileSize / 2)) * (deadWhites.Count - 1)); // Distancia entre las piezas derrotadas.
            }
            // If its the enemy team and we are white
            else
            {
                // If the enemy piece is the king. WHITE WINS. No debe ocurrir porque nunca se llega a comer un rey. Antes ocurre el JaqueMate. Más abajo se procesa.
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(0);

                // Else, move dead Piece
                deadBlacks.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize);
                otherChessPiece.SetPosition(
                    new Vector3(0, 2 * yOffset, 8 * tileSize) // Fuera de la tabla 0-7 --> -1 y 8. x2 yOffset por la geometria de la tabla.
                    + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                    + new Vector3(-(tileSize), 0, 0) // Distancia extra para posicionar segun la geometria de la tabla.
                    + (Vector3.back * (tileSize / 2)) * (deadBlacks.Count - 1)); // Distancia entre las piezas derrotadas.
            }
        }

        chessPieces[x, y] = chessPiece;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn; // Cambiamos el turno.

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) }); // We add the move done in a list (previous move and new move).

        ProcessSpecialMove(); // If there is a special move and we have to process an additional kill or something.

        // GAMEOVER. Comprobar si este movimiento es un JAQUEMATE o TABLAS. Si es asi, termina el juego.
        switch (CheckForCheckmateAndStalemate())
        {
            default: // Case 0. NO ocurre JAQUEMATE ni TABLAS
                break;
            case 1: // JAQUEMATE
                CheckMate(chessPiece.team);
                break;
            case 2: // TABLAS
                CheckMate(2);
                break;
        }

        return true;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // (-1, -1) Invalid --> Crash
    }
}
