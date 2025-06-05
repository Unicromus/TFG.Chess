using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public enum PromotionPieceType
{
    None = 0,
    Rook = 1, // Torre
    Knight = 2, // Caballo
    Bishop = 3, // Alfil
    Queen = 4, // Reina
}

public enum GameMode
{
    OnePlayer = 0,
    PlayerVSRandomBot,
    PlayerVSAggressiveRandomBot,
}

public enum Team
{
    White = 0,
    Black = 1,
}

public class Chessboard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial; // El material de las baldosas invisibles.
    [SerializeField] private float tileSize = 0.05f; // El tama�o de cada baldosa invisible.
    [SerializeField] private float yOffset = 0.0025f; // La distancia entre el centro del tablero (las baldosas visibles) y las baldosas invisibles.
    [SerializeField] private Vector3 boardCenterOffSet = new Vector3(0f, 0.0175f, 0f); // Representa la distancia entre el centro del tablero (transform) y las baldosas visibles.
                                                                                       // Esta variable se debe actualizar al cambiar de geometria del tablero.
    [SerializeField] private float deathSize = 0.5f; // El tama�o de las piezas derrotadas.
    [SerializeField] private float deathOffset = 0.005f; // La altura extra de la geometria del tablero para colocar las piezas derrotadas.
    //[SerializeField] private float deathSpacing = 0.3f; // El espacio entre cada pieza derrotada.
    [SerializeField] private float dragOffset = 0.1f; // La distancia para elevar las piezas a la hora de moverlas o hubicarlas.

    [Header("Victory screen")]
    [SerializeField] private GameObject victoryScreen; // Canvas Victory Panel.

    [Header("Promotion menu")]
    [SerializeField] private GameObject promotionMenu; // Canvas Promotion Menu.

    [Header("Chess clock")]
    [SerializeField] private ChessClock chessClock; // Reloj Digital.

    [Header("Prefabs & Materials")]
    [SerializeField] private Transform storePieces; // El padre donde guardar las piezas de ajedrez.
    [SerializeField] private GameObject[] piecePrefabs; // Los prefabs de las piezas de ajedrez.
    [SerializeField] private Material[] teamMaterials; // Los materiales de los equipos del ajedrez, blanco y negro.

    [Header("Sounds")]
    [SerializeField] private AudioClip shoutoutSoundClip;
    [SerializeField] private AudioClip resetSoundClip;
    [SerializeField] private AudioClip captureSoundClip;
    [SerializeField] private AudioClip castleSoundClip;
    [SerializeField] private AudioClip promoteSoundClip;
    [SerializeField] private AudioClip moveSelfSoundClip;
    [SerializeField] private AudioClip moveOpponentSoundClip;
    [SerializeField] private AudioClip moveCheckSoundClip;
    [SerializeField] private AudioClip gameDrawSoundClip;
    [SerializeField] private AudioClip gameEndSoundClip;
    [SerializeField] private AudioClip gameWinSoundClip;
    [SerializeField] private AudioClip gameLoseSoundClip;

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
    private Vector3 boardCenter; // El centro del tablero (las baldosas visibles). boardCenter = transform + boardCenterOffSet
    private Vector3 bounds; // Los limites del tablero. En este caso se utiliza para saber el punto inicial desde donde generar las baldosas invisibles.

    private SpecialMove specialMove; // Puede ser 0, EnPassant, Castling o Promotion.
    private bool isPromoting; // If any piece is being promoted.
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>(); // La lista que contiene todos los movimientos del tablero. Lista[antiguaPosicion(x, y), nuevaPosicion(x, y)]
    private ChessPiece lastPieceMoved;

    // Chess State LOGIC - Forsyth-Edwards Notation (FEN)
    private bool isWhiteTurn; // Turno de las piezas blancas?
    private bool castlingWhiteKingSide;
    private bool castlingWhiteQueenSide;
    private bool castlingBlackKingSide;
    private bool castlingBlackQueenSide;
    private string possibleEnPassantTarget; // the square behind the pawn (that is a possible En Passant target) in algebraic notation.
    private int halfmoveClock; // how many moves both players have made since the last pawn advance or piece capture.
    private int fullmoveNumber; // the number of completed turns in the game. This number is incremented by one every time Black moves.

    // AI LOGIC
    private GameMode gameMode;
    private Team playerTeam;

    private const int MIN_TIME_DELAY_MOVE = 2; // el tiempo M�NIMO que puede llegar a tardar la IA en hacer un movimiento.
    private const int MAX_TIME_DELAY_MOVE = 11; // el tiempo M�XIMO que puede llegar a tardar la IA en hacer un movimiento.
    private float timeDelayMove; // el tiempo que tarda la IA en hacer un movimiento.

    private const float TIME_DELAY_CLOCK = 1.0f;
    private float timeDelayClock; // el tiempo que tarda la IA en cambiar de turno con el reloj.
    private bool botNeedPressClock; // determina cuando empezar a contar los segundos para darle al cambio de turno con el reloj

    private void Awake()
    {
        isPromoting = false;

        isWhiteTurn = true;
        castlingWhiteKingSide = true;
        castlingWhiteQueenSide = true;
        castlingBlackKingSide = true;
        castlingBlackQueenSide = true;
        possibleEnPassantTarget = "-";
        halfmoveClock = 0;
        fullmoveNumber = 1;

        gameMode = GameMode.OnePlayer;
        playerTeam = Team.White;

        timeDelayMove = 2.0f;
        timeDelayClock = 2.0f;
        botNeedPressClock = false;

        boardCenter = boardCenterOffSet;
        bounds = new Vector3(-(TILE_COUNT_X / 2) * tileSize, 0, -(TILE_COUNT_Y / 2) * tileSize) + boardCenter;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllpieces();
    }

    private void Update()
    {
        /* Recordar aplicar transform.TransformPoint a las transformaciones locales para calcular su posici�n global.
         * De esta forma, se tendra en cuenta los cambios de las transformaci�nes del tablero, en caso de que se haya movido, rotado o escalado.
         * Se debe aplicar al final, despues de haber calculado todas las transformaciones locales.
         * Se utiliza sobre todo al llamar a la funci�n Piece.SetPositionWithLimits, ya que hay que pasarle la posici�n global.
         */
        boardCenter = boardCenterOffSet; 
        bounds = new Vector3(-(TILE_COUNT_X / 2) * tileSize, 0, -(TILE_COUNT_Y / 2) * tileSize) + boardCenter;

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit hit;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1.0f); // Dibujar el raycast para saber donde apunta.

        //RaycastHit[] hits = Physics.RaycastAll(ray, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")); // Obtener todos los objetos golpeados por el raycast
        //foreach (var hit in hits)
        //{

        //if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Tile") || 
        //    hit.transform.gameObject.layer == LayerMask.NameToLayer("Hover") || 
        //    hit.transform.gameObject.layer == LayerMask.NameToLayer("Highlight"))

        // El Raycast solo detectara los objetos con las mascaras definidas en GetMask
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tile i have hit
            //Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject); // CUIDADO: si el padre tiene rigidbody, info.transform.gameObject referencia al padre.
            Vector2Int hitPosition = LookupTileIndex(hit.collider.transform.gameObject); // CUIDADO: si el padre tiene rigidbody, info.collider.transform.gameObject referencia al collider (hijo en este caso).

            //Debug.Log("Hit: " + hit.collider.transform.gameObject);

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
                    // Is it our turn?
                    if ((gameMode == GameMode.OnePlayer &&
                        ((isWhiteTurn && chessPieces[hitPosition.x, hitPosition.y].team == 0 && chessClock.GetIsTimerWhite()) ||
                        (!isWhiteTurn && chessPieces[hitPosition.x, hitPosition.y].team == 1 && chessClock.GetIsTimerBlack())))
                        ||
                        ((gameMode == GameMode.PlayerVSRandomBot || gameMode == GameMode.PlayerVSAggressiveRandomBot) &&
                        ((isWhiteTurn && chessPieces[hitPosition.x, hitPosition.y].team == 0 && chessClock.GetIsTimerWhite() && playerTeam == Team.White) ||
                        (!isWhiteTurn && chessPieces[hitPosition.x, hitPosition.y].team == 1 && chessClock.GetIsTimerBlack() && playerTeam == Team.Black))))
                    {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        // Get a list of available moves of the piece that is currently dragging.
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // Get what special move is possible and add the special move to available moves.
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves,
                            castlingWhiteKingSide, castlingWhiteQueenSide, castlingBlackKingSide, castlingBlackQueenSide, possibleEnPassantTarget); // References reduce memory use
                                                                                                                                                    // Check if our king will be in danger if we move our piece, if so, remove that move as an possible option.
                                                                                                                                                    // Crearemos una simulaci�n para cada movimiento posible de la pieza y comprobaremos si el rey termina siendo amenazado al realizar los movimientos.
                        PreventCheck(ref chessPieces, currentlyDragging, ref availableMoves);

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
                {
                    // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                    // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                    currentlyDragging.SetPositionWithLimits(transform.TransformPoint(GetTileCenter(previousPosition.x, previousPosition.y)), transform.rotation.eulerAngles, tileSize / 4, false);
                }

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
            if (currentlyDragging && Input.GetMouseButtonUp(0)) // Si soltamos una pieza fuera de la tabla, restablecemos su posicion anterior
            {
                // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                currentlyDragging.SetPositionWithLimits(transform.TransformPoint(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY)), transform.rotation.eulerAngles, tileSize / 4, false);
                currentlyDragging = null;
                RemoveHighlightTiles(); // Every time we stop dragging a piece we remove the HighlightTiles
            }
        }


        // If we're dragging a piece
        if (currentlyDragging)
        {
            if (!currentlyDragging.GetComponent<Rigidbody>().isKinematic)
                currentlyDragging.GetComponent<Rigidbody>().isKinematic = true; // Set Kinematic true a la pieza mientras se agarra para que no le afecte las fisicas.

            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * (2*yOffset + transform.position.y + boardCenter.y)); // invisible Plane for Raycast
            float distance = 0.0f;
            // if ray in camera view hits horizontalPlane (should happen always). distance will be the distance between horizontalPlane and raycast.
            if (horizontalPlane.Raycast(ray, out distance))
            {
                // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                currentlyDragging.SetPositionWithLimits(ray.GetPoint(distance) + (Vector3.up * dragOffset), transform.rotation.eulerAngles, 0.0f); // distance is used to move smoothly the piece that is currentlyDragging.
            }
        }

        /* Random AI Bot Move */
        if ((gameMode == GameMode.PlayerVSRandomBot || gameMode == GameMode.PlayerVSAggressiveRandomBot) &&
            ((isWhiteTurn && playerTeam == Team.Black && chessClock.GetIsTimerWhite()) ||
            (!isWhiteTurn && playerTeam == Team.White && chessClock.GetIsTimerBlack())))
        {
            if (timeDelayMove > 0) // Tiempo de espera - CONTADOR
                timeDelayMove -= Time.deltaTime;

            if (gameMode == GameMode.PlayerVSRandomBot)
            {
                if (timeDelayMove <= 0) // Tiempo de espera - FINALIZADO
                {
                    timeDelayMove = UnityEngine.Random.Range(MIN_TIME_DELAY_MOVE, MAX_TIME_DELAY_MOVE); // Tiempo de espera - ACTUALIZADO

                    RandomMove(isWhiteTurn); // Movimiento de la IA

                    timeDelayClock = TIME_DELAY_CLOCK;
                    botNeedPressClock = true;
                }
            }
            else if (gameMode == GameMode.PlayerVSAggressiveRandomBot)
            {
                if (timeDelayMove <= 0) // Tiempo de espera
                {
                    timeDelayMove = UnityEngine.Random.Range(MIN_TIME_DELAY_MOVE, MAX_TIME_DELAY_MOVE); // Tiempo de espera - ACTUALIZADO

                    AggressiveRandomMove(isWhiteTurn); // Movimiento de la IA

                    timeDelayClock = TIME_DELAY_CLOCK;
                    botNeedPressClock = true;
                }
            }
        }

        if (botNeedPressClock)
        {
            if (timeDelayClock > 0) // Tiempo de espera - CONTADOR
                timeDelayClock -= Time.deltaTime;

            if (timeDelayClock <= 0)
            {
                // Press Clock
                if (playerTeam == Team.White)
                    chessClock.BlackSwap();
                if (playerTeam == Team.Black)
                    chessClock.WhiteSwap();

                botNeedPressClock = false;
            }

        }
    }

    // Generate the board. Se generan las baldosas.
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
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
        vertices[0] = transform.TransformPoint(new Vector3(x * tileSize, yOffset, y * tileSize) + bounds);
        vertices[1] = transform.TransformPoint(new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) + bounds);
        vertices[2] = transform.TransformPoint(new Vector3((x + 1) * tileSize, yOffset, y * tileSize) + bounds);
        vertices[3] = transform.TransformPoint(new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) + bounds);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        var collider = tileObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

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
        // Se instancia la pieza(prefab). 
        ChessPiece cp = Instantiate(piecePrefabs[(int)type - 1], // we do -1 because ChessPieceType has None 0
            transform.position + new Vector3(0f, dragOffset, 0f), // se instancia encima del centro del tablero
            transform.rotation, // Aqui se soluciono un problema cambiando Quaternion.Euler(new Vector3(0, 0, 0)) a utilizar la rotaci�n del tablero.
            storePieces).GetComponent<ChessPiece>(); // Puede ser teniendo como padre al mismo tablero (transform) o en un gameobject nuevo (storePieces).

        cp.type = type; // type: 0-6
        cp.team = team; // team: 0 white | 1 black
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        //cp.GetComponent<MeshRenderer>().material = teamMaterials[((team == 0) ? 0 : 6) + ((int)type - 1)]; // if there is more than 2 materials

        if (team == 0)
        {
            cp.gameObject.transform.rotation *= Quaternion.Euler(0, 180, 0); // Se rotan 180 grados las piezas blancas.
        }

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
        // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
        // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
        chessPieces[x, y].SetPositionWithLimits(transform.TransformPoint(GetTileCenter(x, y)), transform.rotation.eulerAngles, tileSize / 4, force);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        // numero y altura de baldosa invisible + inicio y altura de las baldosas invisibles + el centro de la valdosa invisible + altura segura para no chocar con otras piezas
        return new Vector3(x * tileSize, yOffset, y * tileSize) + bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + new Vector3(0, dragOffset, 0);
    }
    // Repositioning. Se vuelven a colocar las piezas en su lugar correspondiente, segun el estado en el que se encuentra la partida.
    private void RePositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(chessPieces[x, y].currentX, chessPieces[x, y].currentY, true);

        for (int i = 0; i < deadWhites.Count; i++)
        {
            // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
            // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
            deadWhites[i].SetPositionWithLimits(transform.TransformPoint(
                new Vector3(8 * tileSize, 0, 0) // Fuera de la tabla 0-7 --> -1 y 8.
                + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                + new Vector3((tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                + (Vector3.forward * (tileSize / 2)) * (i)), // Distancia entre las piezas derrotadas.
                transform.rotation.eulerAngles,
                tileSize / 16, true); // Los limites donde la pieza permanece dentro
        }
        for (int i = 0; i < deadBlacks.Count; i++)
        {
            // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
            // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
            deadBlacks[i].SetPositionWithLimits(transform.TransformPoint(
                new Vector3(0, 0, 8 * tileSize) // Fuera de la tabla 0-7 --> -1 y 8.
                + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                + new Vector3(-(tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                + (Vector3.back * (tileSize / 2)) * (i)), // Distancia entre las piezas derrotadas.
                transform.rotation.eulerAngles,
                tileSize / 16, true); // Los limites donde la pieza permanece dentro
        }

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(resetSoundClip, transform, 1f);
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
        // play sound FX
        if ((playerTeam == Team.White && team == 0) || (playerTeam == Team.Black && team == 1))
            SoundFXManager.Instance.PlaySoundFXClip(gameWinSoundClip, transform, 1f);
        else if ((playerTeam == Team.White && team == 1) || (playerTeam == Team.Black && team == 0))
            SoundFXManager.Instance.PlaySoundFXClip(gameLoseSoundClip, transform, 1f);

        DisplayVictory(team); // 0 or 1
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    private void DisplayPromotionMenu()
    {
        promotionMenu.SetActive(true);
    }
    private void HidePromotionMenu()
    {
        promotionMenu.SetActive(false);

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(promoteSoundClip, transform, 1f);
    }

    // Buttons
    public void OnStartButton()
    {
        // Destroy GameObjects and reset Fields
        CleanChess();

        // ReStart the game
        SpawnAllPieces();
        PositionAllpieces();
    }
    public void OnResetButton()
    {
        // Hide UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false); // White Victory
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false); // Black Victory
        victoryScreen.transform.GetChild(2).gameObject.SetActive(false); // StaleMate
        victoryScreen.transform.GetChild(3).gameObject.SetActive(false); // DeadPosition
        victoryScreen.transform.GetChild(4).gameObject.SetActive(false); // 75MoveRule
        victoryScreen.SetActive(false);

        // Destroy GameObjects and reset Fields
        CleanChess();

        // ReStart the game
        SpawnAllPieces();
        PositionAllpieces();

        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(resetSoundClip, transform, 1f);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
    public String OnGetForsythEdwardsNotation()
    {
        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(shoutoutSoundClip, transform, 1f);

        String chessState = GetChessState();
        Debug.Log(chessState);
        return chessState;
    }
    public void OnSetForsythEdwardsNotation(String chessS)
    {
        // play sound FX
        SoundFXManager.Instance.PlaySoundFXClip(resetSoundClip, transform, 1f);

        SetChessState(chessS);
        String chessState = GetChessState();
        Debug.Log(chessState);
    }
    public void OnSetPromotionPiece(PromotionPieceType pPieceType)
    {
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

        ChessPiece promotionPiece = null;

        switch (pPieceType)
        {
            default:
                break;
            case PromotionPieceType.Rook:
                promotionPiece = SpawnSinglePiece(ChessPieceType.Rook, targetPawn.team); // Create white|black Rook
                break;
            case PromotionPieceType.Knight:
                promotionPiece = SpawnSinglePiece(ChessPieceType.Knight, targetPawn.team); // Create white|black Knight
                break;
            case PromotionPieceType.Bishop:
                promotionPiece = SpawnSinglePiece(ChessPieceType.Bishop, targetPawn.team); // Create white|black Bishop
                break;
            case PromotionPieceType.Queen:
                promotionPiece = SpawnSinglePiece(ChessPieceType.Queen, targetPawn.team); // Create white|black Queen
                break;
        }
        if (promotionPiece != null)
        {
            promotionPiece.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Set position smoother
            Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject); // Destroy Pawn
            chessPieces[lastMove[1].x, lastMove[1].y] = promotionPiece; // Set promotionPiece
            PositionSinglePiece(lastMove[1].x, lastMove[1].y); // Update position

            isPromoting = false;
            HidePromotionMenu();
        }
    }
    public void OnRePositionAllPieces()
    {
        RePositionAllPieces();
    }

    // Getters & Setters
    public void SetGameMode(GameMode gMode)
    {
        gameMode = gMode;
    }
    public void SetPlayerTeam(Team t)
    {
        playerTeam = t;
    }

    public Team GetPlayerTeam()
    {
        return playerTeam;
    }
    public bool GetIsWhiteTurn()
    {
        return isWhiteTurn;
    }
    public bool GetIsPromoting()
    {
        return isPromoting;
    }
    public ChessPiece GetCurrentlyDragging()
    {
        return currentlyDragging;
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
                        // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                        // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                        enemyPawn.SetPositionWithLimits(transform.TransformPoint(
                            new Vector3(8 * tileSize, 0, 0) // Fuera de la tabla 0-7 --> -1 y 8.
                            + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                            + new Vector3((tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                            + (Vector3.forward * (tileSize / 2)) * (deadWhites.Count - 1)), // Distancia entre las piezas derrotadas.
                            transform.rotation.eulerAngles,
                            tileSize / 16); // Los limites donde la pieza permanece dentro
                    }
                    else // enemyPawn.team == 1. Black Pawn
                    {
                        deadBlacks.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * deathSize);
                        // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                        // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                        enemyPawn.SetPositionWithLimits(transform.TransformPoint(
                            new Vector3(0, 0, 8 * tileSize) // Fuera de la tabla 0-7 --> -1 y 8.
                            + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                            + new Vector3(-(tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                            + (Vector3.back * (tileSize / 2)) * (deadBlacks.Count - 1)), // Distancia entre las piezas derrotadas.
                            transform.rotation.eulerAngles,
                            tileSize / 16); // Los limites donde la pieza permanece dentro
                    }
                    chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;

                    // play sound FX
                    SoundFXManager.Instance.PlaySoundFXClip(captureSoundClip, transform, 1f);
                }
            }
            // play sound FX
            else
            {
                if (playerTeam == Team.White)
                    SoundFXManager.Instance.PlaySoundFXClip(moveSelfSoundClip, transform, 1f);
                else if (playerTeam == Team.Black)
                    SoundFXManager.Instance.PlaySoundFXClip(moveOpponentSoundClip, transform, 1f);
            }
        }

        // Castling - Enroque
        if (specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            int ourY = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 0 : 7; // Check if we are white team or black team and save Y position

            // Right Rook - King Side
            if (lastMove[1].x == 6 && lastMove[1].y == ourY)
            {
                ChessPiece rook = chessPieces[7, ourY];
                chessPieces[5, ourY] = rook;
                PositionSinglePiece(5, ourY);
                chessPieces[7, ourY] = null;
            }
            // Left Rook - Queen Side
            else if (lastMove[1].x == 2 && lastMove[1].y == ourY)
            {
                ChessPiece rook = chessPieces[0, ourY];
                chessPieces[3, ourY] = rook;
                PositionSinglePiece(3, ourY);
                chessPieces[0, ourY] = null;
            }

            // play sound FX
            if ((lastMove[1].x == 6 && lastMove[1].y == ourY) || (lastMove[1].x == 2 && lastMove[1].y == ourY))
                SoundFXManager.Instance.PlaySoundFXClip(castleSoundClip, transform, 1f);
            else
            {
                if (playerTeam == Team.White)
                    SoundFXManager.Instance.PlaySoundFXClip(moveSelfSoundClip, transform, 1f);
                else if (playerTeam == Team.Black)
                    SoundFXManager.Instance.PlaySoundFXClip(moveOpponentSoundClip, transform, 1f);
            }
        }

        // Promotion - Promoci�n
        if (specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if (targetPawn.type == ChessPieceType.Pawn) // It's a Pawn
            {
                if (targetPawn.team == 0 && lastMove[1].y == 7 || targetPawn.team == 1 && lastMove[1].y == 0)
                {
                    isPromoting = true;
                    // AI Random Promotion Choose
                    if ((gameMode == GameMode.PlayerVSRandomBot || gameMode == GameMode.PlayerVSAggressiveRandomBot) &&
                        ((isWhiteTurn && playerTeam == Team.Black && chessClock.GetIsTimerWhite()) ||
                        (!isWhiteTurn && playerTeam == Team.White && chessClock.GetIsTimerBlack())))
                    {
                        System.Random rnd = new System.Random(); // Random object
                        int promotionPieceProbability = rnd.Next(0, 100); // Get random index

                        if (promotionPieceProbability <= 94) // 0 .. 94 ~~ 94%
                            OnSetPromotionPiece(PromotionPieceType.Queen);
                        else if (promotionPieceProbability <= 94 + 3) // 95 .. 97 ~~ 3%
                            OnSetPromotionPiece(PromotionPieceType.Knight);
                        else if (promotionPieceProbability <= 94 + 3 + 2) // 98 .. 99 ~~ 2%
                            OnSetPromotionPiece(PromotionPieceType.Rook);
                        else if (promotionPieceProbability <= 94 + 3 + 2 + 1) // 99 .. 100 ~~ 1%
                            OnSetPromotionPiece(PromotionPieceType.Bishop);
                        else // In case there is an error
                            OnSetPromotionPiece(PromotionPieceType.Queen);
                    }
                    // Menu de eleccion
                    else
                    {
                        DisplayPromotionMenu();
                    }
                }
            }

            // play sound FX
            SoundFXManager.Instance.PlaySoundFXClip(promoteSoundClip, transform, 1f);
        }
    }
    private void PreventCheck(ref ChessPiece[,] board, ChessPiece cPiece, ref List<Vector2Int> pMoves) // Before we move (while we are dragging a piece, in Update() function), check with a simulation if the king gonna be in danger, if so, remove that move as an possible option.
    {
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (board[x, y] != null)
                    if (board[x, y].type == ChessPieceType.King)
                        if (board[x, y].team == cPiece.team)
                            targetKing = board[x, y];

        // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
        SimulateMoveForSinglePiece(ref board, cPiece, ref pMoves, targetKing);
    }
    private void SimulateMoveForSinglePiece(ref ChessPiece[,] board, ChessPiece cPiece, ref List<Vector2Int> pMoves, ChessPiece targetKing)
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
            // Did we simulate the king's move. Si simulamos los movimientos del rey, la posici�n del rey cambiara en esta simulaci�n.
            if (cPiece.type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);

            // Copy the board [,] and not a reference and store attacking pieces
            ChessPiece[,] simulationBoard = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>(); // Enemy pieces
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (board[x, y] != null)
                    {
                        simulationBoard[x, y] = board[x, y];
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
            // Comprobamos si el rey esta siendo amenazado despues de hacer el movimiento en la simulaci�n, si lo esta, eliminaremos el movimiento
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
    private int CheckForCheckmateAndDraws() // After the enemy moves (in MoveTo() function), check if our king is in danger or not (JAQUE), and check throught a simulation if we have any possible moves to do (TABLAS).
    {
        if (halfmoveClock > 150)
            return 5; // DRAW - 75 MOVE RULE. TABLAS - REGLA DE LOS 75 MOVIMIENTOS.

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
        List<Vector2Int> attackingMoves = GetAllAvailableMoves(ref chessPieces, attackingPieces);

        // CHECK. Are we in check right now? Comprobar si estamos en JAQUE (el rey siendo amenazado por alguna pieza enemiga)
        if (ContainsValidMove(ref attackingMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            // King is under attack, can we move something to help him?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
                SimulateMoveForSinglePiece(ref chessPieces, defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) // There is a piece that can protect our king or we can move our king
                    return 1; // CHECK. Game continue
            }

            return 2; // CHECKMATE. There is no piece that can protect our king and we cant move it. Checkmate exit. JAQUEMATE.
        }
        // NO estamos en JAQUE
        else
        {
            // King is not under attack, can we move something?
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                // SIMULATION. Since we're sending ref moves, we will be deleting moves that are putting us in check.
                SimulateMoveForSinglePiece(ref chessPieces, defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0) // There is a piece that can be moved or we can move our king
                {
                    if (CheckDeadPosition(defendingPieces, attackingPieces))
                        return 4; // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.

                    return 0; // REGULAR MOVE. Game continue
                }
            }

            return 3; // DRAW - STALEMATE. There is no piece that can be moved. TABLAS - AHOGADO.
        }

        //return 0;
    }
    private Boolean CheckDeadPosition(List<ChessPiece> defendingPieces, List<ChessPiece> attackingPieces)
    {
        int defendingPawn = defendingPieces.Count(p => p.type == ChessPieceType.Pawn);
        int defendingRook = defendingPieces.Count(p => p.type == ChessPieceType.Rook);
        int defendingKnight = defendingPieces.Count(p => p.type == ChessPieceType.Knight);
        int defendingBishop = defendingPieces.Count(p => p.type == ChessPieceType.Bishop);
        int defendingQueen = defendingPieces.Count(p => p.type == ChessPieceType.Queen);

        int attackingPawn = attackingPieces.Count(p => p.type == ChessPieceType.Pawn);
        int attackingRook = attackingPieces.Count(p => p.type == ChessPieceType.Rook);
        int attackingKnight = attackingPieces.Count(p => p.type == ChessPieceType.Knight);
        int attackingBishop = attackingPieces.Count(p => p.type == ChessPieceType.Bishop);
        int attackingQueen = attackingPieces.Count(p => p.type == ChessPieceType.Queen);

        if (defendingPawn == 0 && defendingRook == 0 && defendingKnight == 0 && defendingBishop == 0 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 0 && attackingBishop == 0 && attackingQueen == 0) // King vs King
            return true; // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.
        if (defendingPawn == 0 && defendingRook == 0 && defendingKnight == 1 && defendingBishop == 0 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 0 && attackingBishop == 0 && attackingQueen == 0 ||
            defendingPawn == 0 && defendingRook == 0 && defendingKnight == 0 && defendingBishop == 0 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 1 && attackingBishop == 0 && attackingQueen == 0) // King and Knight vs King
            return true; // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.
        if (defendingPawn == 0 && defendingRook == 0 && defendingKnight == 0 && defendingBishop == 1 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 0 && attackingBishop == 0 && attackingQueen == 0 ||
            defendingPawn == 0 && defendingRook == 0 && defendingKnight == 0 && defendingBishop == 0 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 0 && attackingBishop == 1 && attackingQueen == 0) // King and Bishop vs King
            return true; // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.
        if (defendingPawn == 0 && defendingRook == 0 && defendingKnight == 0 && defendingBishop == 1 && defendingQueen == 0 &&
            attackingPawn == 0 && attackingRook == 0 && attackingKnight == 0 && attackingBishop == 1 && attackingQueen == 0) // King and Bishop vs King and Bishop
            return true; // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.
        return false;
    }

    // Operations
    private void CleanChess()
    {
        // Fields reset
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();

        isPromoting = false;

        isWhiteTurn = true;
        castlingWhiteKingSide = true;
        castlingWhiteQueenSide = true;
        castlingBlackKingSide = true;
        castlingBlackQueenSide = true;
        possibleEnPassantTarget = "-";
        halfmoveClock = 0;
        fullmoveNumber = 1;

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
    }
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

        bool pieceCapture = false;

        // Check if there is another piece on the target position and move it to dead positions
        if (chessPieces[x, y] != null)
        {
            pieceCapture = true;

            ChessPiece otherChessPiece = chessPieces[x, y];

            // If its our team. 0 == 0 or 1 == 1. Dont Move.
            if (chessPiece.team == otherChessPiece.team)
                return false;

            // If its the enemy team and we are black
            if (otherChessPiece.team == 0)
            {
                // If the enemy piece is the king. BLACK WINS. No debe ocurrir porque nunca se llega a comer un rey. Antes ocurre el JaqueMate. M�s abajo se procesa.
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(1);

                // Else, move dead Piece
                deadWhites.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize);
                // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                otherChessPiece.SetPositionWithLimits(transform.TransformPoint(
                    new Vector3(8 * tileSize, 0, 0) // Fuera de la tabla 0-7 --> -1 y 8.
                    + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                    + new Vector3((tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                    + (Vector3.forward * (tileSize / 2)) * (deadWhites.Count - 1)), // Distancia entre las piezas derrotadas.
                    transform.rotation.eulerAngles,
                    tileSize / 16); // Los limites donde la pieza permanece dentro
            }
            // If its the enemy team and we are white
            else
            {
                // If the enemy piece is the king. WHITE WINS. No debe ocurrir porque nunca se llega a comer un rey. Antes ocurre el JaqueMate. M�s abajo se procesa.
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(0);

                // Else, move dead Piece
                deadBlacks.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize);
                // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
                // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
                otherChessPiece.SetPositionWithLimits(transform.TransformPoint(
                    new Vector3(0, 0, 8 * tileSize) // Fuera de la tabla 0-7 --> -1 y 8.
                    + bounds // Punto inicial desde donde se generan las baldosas. Contiene boardCenter.
                    + new Vector3(-(tileSize), deathOffset + dragOffset, 0) // Distancia y altura extra para posicionar segun la geometria de la tabla.
                    + (Vector3.back * (tileSize / 2)) * (deadBlacks.Count - 1)), // Distancia entre las piezas derrotadas.
                    transform.rotation.eulerAngles,
                    tileSize / 16); // Los limites donde la pieza permanece dentro
            }
        }

        /* Castling Rights */
        UpdateCastlingRights(chessPiece, x, y); // Before Logic Update

        /* Halfmove Clock */
        if (chessPieces[x, y] == null && specialMove != SpecialMove.EnPassant && chessPiece.type != ChessPieceType.Pawn) // No piece capture and no pawn piece. Before Logic Update
            halfmoveClock += 1;
        else
            halfmoveClock = 0;

        /* Fullmove Number */
        if (chessPiece.team == 1) // Black turn
            fullmoveNumber += 1;

        // Logic Update
        chessPieces[x, y] = chessPiece;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) }); // We add the move done in a list (previous move and new move).

        lastPieceMoved = chessPiece; // Guardamos la pieza como ultima en ser movida.

        /* Possible En Passant Target */
        possibleEnPassantTarget = SavePossibleEnPassantTarget(lastPieceMoved, moveList);

        ProcessSpecialMove(); // If there is a special move and we have to process an additional kill or something.

        // GAMEOVER. Comprobar si este movimiento es un JAQUEMATE o TABLAS. Si es asi, termina el juego.
        switch (CheckForCheckmateAndDraws())
        {
            default: // Case 0. REGULAR MOVE. Game continue
                // play sound FX
                if (specialMove == SpecialMove.None)
                {
                    if (pieceCapture)
                        SoundFXManager.Instance.PlaySoundFXClip(captureSoundClip, transform, 1f);
                    else
                    {
                        if (playerTeam == Team.White)
                            SoundFXManager.Instance.PlaySoundFXClip(moveSelfSoundClip, transform, 1f);
                        else if (playerTeam == Team.Black)
                            SoundFXManager.Instance.PlaySoundFXClip(moveOpponentSoundClip, transform, 1f);
                    }
                }
                break;
            case 1: // CHECK. JAQUE. Game continue
                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(moveCheckSoundClip, transform, 1f);
                break;
            case 2: // CHECKMATE. JAQUEMATE.
                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(gameEndSoundClip, transform, 1f);
                CheckMate(chessPiece.team);
                break;
            case 3: // DRAW - STALEMATE. TABLAS - AHOGADO.
                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(gameDrawSoundClip, transform, 1f);

                CheckMate(2);
                break;
            case 4: // DRAW - DEAD POSITION. TABLAS - POSICI�N MUERTA.
                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(gameDrawSoundClip, transform, 1f);

                CheckMate(3);
                break;
            case 5: // DRAW - 75 MOVE RULE. TABLAS - REGLA DE LOS 75 MOVIMIENTOS.
                // play sound FX
                SoundFXManager.Instance.PlaySoundFXClip(gameDrawSoundClip, transform, 1f);

                CheckMate(4);
                break;
        }

        // Cambiamos el turno
        isWhiteTurn = !isWhiteTurn;

        return true;
    }
    private void UpdateCastlingRights(ChessPiece lastPM, int x, int y)
    {
        if (lastPM.team == 0) // White team
        {
            if (castlingWhiteKingSide == true || castlingWhiteQueenSide == true) // One or both castling are possible
            {
                if (lastPM.type == ChessPieceType.King) // King moved?
                {
                    castlingWhiteKingSide = false;
                    castlingWhiteQueenSide = false;
                }
                else if (lastPM.type == ChessPieceType.Rook) // Rook moved?
                {
                    if (castlingWhiteKingSide == true) // Right Rook - King Side
                        if (lastPM.currentX == 7 && lastPM.currentY == 0) castlingWhiteKingSide = false;
                    if (castlingWhiteQueenSide == true) // Left Rook - Queen Side
                        if (lastPM.currentX == 0 && lastPM.currentY == 0) castlingWhiteQueenSide = false;
                }
                else // Other piece moved?
                {
                    if (chessPieces[x, y] != null) // It killed something?
                        if (chessPieces[x, y].type == ChessPieceType.Rook) // It was a Rook?
                        {
                            if (castlingBlackKingSide == true) // Right Rook - King Side
                                if (chessPieces[x, y].currentX == 7 && chessPieces[x, y].currentY == 7) castlingBlackKingSide = false;
                            if (castlingBlackQueenSide == true) // Left Rook - Queen Side
                                if (chessPieces[x, y].currentX == 0 && chessPieces[x, y].currentY == 7) castlingBlackQueenSide = false;
                        }
                }
            }
        }
        else if (lastPM.team == 1) // Black team
        {
            if (castlingBlackKingSide == true || castlingBlackQueenSide == true) // One or both castling are possible
            {
                if (lastPM.type == ChessPieceType.King) // King moved?
                {
                    castlingBlackKingSide = false;
                    castlingBlackQueenSide = false;
                }
                else if (lastPM.type == ChessPieceType.Rook) // Rook moved?
                {
                    if (castlingBlackKingSide == true) // Right Rook - King Side
                        if (lastPM.currentX == 7 && lastPM.currentY == 7) castlingBlackKingSide = false;
                    if (castlingBlackQueenSide == true) // Left Rook - Queen Side
                        if (lastPM.currentX == 0 && lastPM.currentY == 7) castlingBlackQueenSide = false;
                }
                else // Other piece moved?
                {
                    if (chessPieces[x, y] != null) // It killed something?
                        if (chessPieces[x, y].type == ChessPieceType.Rook) // It was a Rook?
                        {
                            if (castlingWhiteKingSide == true) // Right Rook - King Side
                                if (chessPieces[x, y].currentX == 7 && chessPieces[x, y].currentY == 0) castlingWhiteKingSide = false;
                            if (castlingWhiteQueenSide == true) // Left Rook - Queen Side
                                if (chessPieces[x, y].currentX == 0 && chessPieces[x, y].currentY == 0) castlingWhiteQueenSide = false;
                        }
                }
            }
        }
    }
    private string SavePossibleEnPassantTarget(ChessPiece lastPM, List<Vector2Int[]> moveL)
    {
        string possibleEnPassantTarget = "-";

        if (lastPM == null) return possibleEnPassantTarget;

        Vector2Int[] lastMoves = moveL.Last();
        var lastOldPosition = lastMoves[0];
        var lastNewPosition = lastMoves[1];

        if (lastPM.currentX != lastNewPosition.x || lastPM.currentY != lastNewPosition.y) return "Error"; // lastNewPosition should be lastPieceMoved position

        if (lastPM.type == ChessPieceType.Pawn) // pawn
        {
            if (lastPM.team == 0) // white
            {
                if (lastOldPosition.y == 1 && lastNewPosition.y == 3) // +2 Y move
                {
                    switch (lastNewPosition.x)
                    {
                        case 0:
                            possibleEnPassantTarget = "a";
                            break;
                        case 1:
                            possibleEnPassantTarget = "b";
                            break;
                        case 2:
                            possibleEnPassantTarget = "c";
                            break;
                        case 3:
                            possibleEnPassantTarget = "d";
                            break;
                        case 4:
                            possibleEnPassantTarget = "e";
                            break;
                        case 5:
                            possibleEnPassantTarget = "f";
                            break;
                        case 6:
                            possibleEnPassantTarget = "g";
                            break;
                        case 7:
                            possibleEnPassantTarget = "h";
                            break;
                        default:
                            return "Error";
                    }
                    return possibleEnPassantTarget += (lastNewPosition.y).ToString(); // No hacemos y - 1 porque el tablero empieza desde 1, no desde 0.
                }
            }
            if (lastPM.team == 1) // black
            {
                if (lastOldPosition.y == 6 && lastNewPosition.y == 4) // -2 Y move
                {
                    switch (lastNewPosition.x)
                    {
                        case 0:
                            possibleEnPassantTarget = "a";
                            break;
                        case 1:
                            possibleEnPassantTarget = "b";
                            break;
                        case 2:
                            possibleEnPassantTarget = "c";
                            break;
                        case 3:
                            possibleEnPassantTarget = "d";
                            break;
                        case 4:
                            possibleEnPassantTarget = "e";
                            break;
                        case 5:
                            possibleEnPassantTarget = "f";
                            break;
                        case 6:
                            possibleEnPassantTarget = "g";
                            break;
                        case 7:
                            possibleEnPassantTarget = "h";
                            break;
                        default:
                            return "Error";
                    }
                    return possibleEnPassantTarget += (lastNewPosition.y + 2).ToString(); // Hacemos y + 2 porque el tablero empieza desde 1, no desde 0.
                }
            }
        }

        return possibleEnPassantTarget;
    }
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // (-1, -1) Invalid --> Crash
    }


    // Chess State

    /* Forsyth-Edwards Notation (FEN) */
    /* Standard starting chess position --> FEN: "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" */
    /* Piece Placement: The first field "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR" represents the placement of pieces. */
    /* Active Color: The second field "w" indicates who moves next. */
    /* Castling Rights: The third field "KQkq" tells if the players can castle and to what side. */
    /* Possible En Passant Targets: The fourth field "-" adds the square behind the pawn (that is a possible En Passant target) in algebraic notation. */
    /* Halfmove Clock: The fifth field "0" informs how many moves both players have made since the last pawn advance or piece capture. */
    /* Fullmove Number: The sixth field "1" shows the number of completed turns in the game. This number is incremented by one every time Black moves. */
    private String GetChessState()
    {
        string piecePlacement = GetPiecePlacement(); // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR
        string activeColor = GetActiveColor(); // w or b
        string castlingRights = GetCastlingRights(); // KQ (white kingside and queenside) kq (black kingside and queenside)
        string possibleEnPassantTarget = GetPossibleEnPassantTarget(); // - or the square behind the pawn
        string halfmoveClock = GetHalfmoveClock(); // 0 or halfmoveClock
        string fullmoveNumber = GetFullmoveNumber(); // 1 or fullmoveNumber

        string chessState = piecePlacement + " " + activeColor + " " + castlingRights + " " + possibleEnPassantTarget + " " + halfmoveClock + " " + fullmoveNumber;

        return chessState;
    }
    private String GetPiecePlacement()
    {
        string piecePlacement = "";
        int nullPieces = 0;

        // Las coordenadas ser�n: 1.fila de [0, 7] a [7, 7] --> 2.fila de [0, 6] a [7, 6] --> ... --> 8.fila de [0, 0] a [7, 0]
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                if (chessPieces[x, y] == null)
                {
                    nullPieces++;
                }
                if (chessPieces[x, y] != null)
                {
                    if (nullPieces > 0)
                        piecePlacement += nullPieces.ToString();
                    nullPieces = 0;

                    if (chessPieces[x, y].type == ChessPieceType.Pawn)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "P";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "p";
                    }
                    if (chessPieces[x, y].type == ChessPieceType.Rook)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "R";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "r";
                    }
                    if (chessPieces[x, y].type == ChessPieceType.Knight)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "N";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "n";
                    }
                    if (chessPieces[x, y].type == ChessPieceType.Bishop)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "B";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "b";
                    }
                    if (chessPieces[x, y].type == ChessPieceType.Queen)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "Q";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "q";
                    }
                    if (chessPieces[x, y].type == ChessPieceType.King)
                    {
                        if (chessPieces[x, y].team == 0)
                            piecePlacement += "K";
                        if (chessPieces[x, y].team == 1)
                            piecePlacement += "k";
                    }
                }
            }
            if (nullPieces > 0)
                piecePlacement += nullPieces.ToString();
            nullPieces = 0;
            if (y > 0)
                piecePlacement += "/";
        }

        return piecePlacement;
    }
    private String GetActiveColor()
    {
        if (isWhiteTurn)
            return "w";
        if (!isWhiteTurn)
            return "b";

        return "Error";
    }
    private String GetCastlingRights()
    {
        string castlingRights = "";

        if ((castlingWhiteKingSide == false) && (castlingWhiteQueenSide == false) && (castlingBlackKingSide == false) && (castlingBlackQueenSide == false)) return "-";

        if (castlingWhiteKingSide == true) castlingRights += "K";
        if (castlingWhiteQueenSide == true) castlingRights += "Q";
        if (castlingBlackKingSide == true) castlingRights += "k";
        if (castlingBlackQueenSide == true) castlingRights += "q";

        return castlingRights;
    }
    private String GetPossibleEnPassantTarget()
    {
        return possibleEnPassantTarget;
    }
    private String GetHalfmoveClock()
    {
        return halfmoveClock.ToString();
    }
    private String GetFullmoveNumber()
    {
        return fullmoveNumber.ToString();
    }

    private void SetChessState(String chessState)
    {
        string[] csFields = chessState.Split(new char[] { ' ' });

        if (csFields.Count() != 6)
        {
            Debug.Log("Error: The notation is wrong. There is not 6 fields.");
            return;
        }

        SetPiecePlacement(csFields[0]);
        SetActiveColor(csFields[1]);
        SetCastlingRights(csFields[2]);
        SetPossibleEnPassantTarget(csFields[3]);
        SetHalfmoveClock(csFields[4]);
        SetFullmoveNumber(csFields[5]);
    }
    private void SetPiecePlacement(String piecePlacement)
    {
        string[] chessRowsPieces = piecePlacement.Split('/');
        int x = 0, y = 7;
        int whiteTeam = 0, blackTeam = 1;
        int number;

        CleanChess();

        foreach (string row in chessRowsPieces)
        {
            foreach (char piece in row)
            {
                if (x > 7)
                {
                    Debug.Log("Error: The notation is wrong. x > 7");
                    break;
                }
                if (y < 0)
                {
                    Debug.Log("Error: The notation is wrong. y < 0");
                    goto End;
                }

                if (piece == 'P')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'p')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'R')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'r')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'N')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'n')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'B')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'b')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'Q')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'q')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'K')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }
                if (piece == 'k')
                {
                    chessPieces[x, y] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
                    PositionSinglePiece(x, y, true);
                    x++; continue;
                }

                bool success = int.TryParse(piece.ToString(), out number); // int number = (int)(piece - '0');
                if (success)
                {
                    x += number;
                }
                else // the notation is wrong
                {
                    Debug.Log("Error: The notation is wrong. Incorrect character");
                    goto End;
                }
            }
            y--;
            x = 0;
        }
    End:
        Debug.Log("- Piece Placement Finished -");
    }
    private void SetActiveColor(String activeColor)
    {
        if (activeColor == "w")
            isWhiteTurn = true;
        if (activeColor == "b")
            isWhiteTurn = false;
    }
    private void SetCastlingRights(String castingRights)
    {
        if (castingRights.Contains("-"))
        {
            castlingWhiteKingSide = false;
            castlingWhiteQueenSide = false;
            castlingBlackKingSide = false;
            castlingBlackQueenSide = false;
        }

        if (castingRights.Contains("K")) castlingWhiteKingSide = true;
        else castlingWhiteKingSide = false;
        if (castingRights.Contains("Q")) castlingWhiteQueenSide = true;
        else castlingWhiteQueenSide = false;
        if (castingRights.Contains("k")) castlingBlackKingSide = true;
        else castlingBlackKingSide = false;
        if (castingRights.Contains("q")) castlingBlackQueenSide = true;
        else castlingBlackQueenSide = false;
    }
    private void SetPossibleEnPassantTarget(String possibleEnPassantT)
    {
        possibleEnPassantTarget = possibleEnPassantT;
    }
    private void SetHalfmoveClock(String halfmoveC)
    {
        int hmC = 0;
        int.TryParse(halfmoveC, out hmC);
        halfmoveClock = hmC;
    }
    private void SetFullmoveNumber(String fullmoveN)
    {
        int fmN = 0;
        int.TryParse(fullmoveN, out fmN);
        fullmoveNumber = fmN;
    }

    /* Random AI Bot */
    private void RandomMove(bool turn)
    {
        int targetTeam = turn ? 0 : 1;
        List<Vector2Int> pieceMoves = new List<Vector2Int>();
        List<ChessPiece> botPieces = new List<ChessPiece>();

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].team == targetTeam)
                    {
                        pieceMoves = chessPieces[x, y].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y); // Get a list of available moves of the piece that is currently dragging.

                        specialMove = chessPieces[x, y].GetSpecialMoves(ref chessPieces, ref moveList, ref pieceMoves,
                            castlingWhiteKingSide, castlingWhiteQueenSide, castlingBlackKingSide, castlingBlackQueenSide, possibleEnPassantTarget); // Get what special move is possible and add the special move to available moves.

                        PreventCheck(ref chessPieces, chessPieces[x, y], ref pieceMoves); // Check if our king will be in danger if we move our piece, if so, remove that move as an possible option.

                        if (pieceMoves.Any())
                            botPieces.Add(chessPieces[x, y]);
                    }

        /* Choose a random piece */
        System.Random rnd = new System.Random(); // Random object
        if (!botPieces.Any())
        {
            Debug.Log("RandomBot: No pieces with available moves.");
            return;
        }
        int index = rnd.Next(botPieces.Count); // Get random index
        ChessPiece randomPiece = botPieces[index]; // Get piece of that random index

        /* Update References */
        currentlyDragging = chessPieces[randomPiece.currentX, randomPiece.currentY];

        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y); // Get a list of available moves of the piece that is currently dragging.

        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves,
            castlingWhiteKingSide, castlingWhiteQueenSide, castlingBlackKingSide, castlingBlackQueenSide, possibleEnPassantTarget); // Get what special move is possible and add the special move to available moves.

        PreventCheck(ref chessPieces, currentlyDragging, ref availableMoves); // Check if our king will be in danger if we move our piece, if so, remove that move as an possible option.

        /* Choose a random move */
        if (!availableMoves.Any())
        {
            Debug.Log("RandomBot: No moves detected.");
            return;
        }
        index = rnd.Next(availableMoves.Count);
        Vector2Int randomMove = availableMoves[index];

        /* Move the piece */
        Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

        bool validMove = MoveTo(currentlyDragging, randomMove.x, randomMove.y);
        if (!validMove)
        {
            // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
            // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
            currentlyDragging.SetPositionWithLimits(transform.TransformPoint(GetTileCenter(previousPosition.x, previousPosition.y)), transform.rotation.eulerAngles, tileSize / 4, false);
        }

        /* Reset References */
        currentlyDragging = null;
        availableMoves.Clear();
    }
    private void AggressiveRandomMove(bool turn)
    {
        int targetTeam = turn ? 0 : 1;
        List<Vector2Int> pieceMoves = new List<Vector2Int>();
        List<ChessPiece> botPieces = new List<ChessPiece>();
        List<ChessPiece> aggressiveBotPieces = new List<ChessPiece>();

        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].team == targetTeam)
                    {
                        pieceMoves = chessPieces[x, y].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y); // Get a list of available moves of the piece that is currently dragging.

                        specialMove = chessPieces[x, y].GetSpecialMoves(ref chessPieces, ref moveList, ref pieceMoves,
                            castlingWhiteKingSide, castlingWhiteQueenSide, castlingBlackKingSide, castlingBlackQueenSide, possibleEnPassantTarget); // Get what special move is possible and add the special move to available moves.

                        PreventCheck(ref chessPieces, chessPieces[x, y], ref pieceMoves); // Check if our king will be in danger if we move our piece, if so, remove that move as an possible option.

                        if (pieceMoves.Any())
                        {
                            botPieces.Add(chessPieces[x, y]);

                            foreach (Vector2Int move in pieceMoves)
                                if (chessPieces[move.x, move.y] != null)
                                    if (chessPieces[move.x, move.y].team != targetTeam) // if the piece can attack, we add it as aggressive piece
                                    {
                                        aggressiveBotPieces.Add(chessPieces[x, y]);
                                        break;
                                    }
                        }
                    }

        /* Choose a random piece */
        int index; ChessPiece randomPiece;
        System.Random rnd = new System.Random(); // Random object
        if (!aggressiveBotPieces.Any())
        {
            //Debug.Log("RandomBot: No aggressive pieces with available moves.");
            if (!botPieces.Any())
            {
                Debug.Log("RandomBot: No pieces with available moves.");
                return;
            }
            else
            {
                index = rnd.Next(botPieces.Count); // Get random index
                randomPiece = botPieces[index]; // Get piece of that random index
            }
        }
        else
        {
            index = rnd.Next(aggressiveBotPieces.Count); // Get random index
            randomPiece = aggressiveBotPieces[index]; // Get piece of that random index
        }

        /* Update References */
        currentlyDragging = chessPieces[randomPiece.currentX, randomPiece.currentY];

        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y); // Get a list of available moves of the piece that is currently dragging.

        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves,
            castlingWhiteKingSide, castlingWhiteQueenSide, castlingBlackKingSide, castlingBlackQueenSide, possibleEnPassantTarget); // Get what special move is possible and add the special move to available moves.

        PreventCheck(ref chessPieces, currentlyDragging, ref availableMoves); // Check if our king will be in danger if we move our piece, if so, remove that move as an possible option.

        /* Choose a random move */
        if (!availableMoves.Any())
        {
            Debug.Log("RandomBot: No moves detected.");
            return;
        }
        Vector2Int randomMove = new Vector2Int();
        if (!aggressiveBotPieces.Any())
        {
            index = rnd.Next(availableMoves.Count);
            randomMove = availableMoves[index];
        }
        else
        {
            foreach (Vector2Int move in availableMoves)
                if (chessPieces[move.x, move.y] != null)
                    if (chessPieces[move.x, move.y].team != targetTeam) // if the piece can attack, we add it as aggressive piece
                    {
                        randomMove = move;
                        break;
                    }
        }

        /* Move the piece */
        Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

        bool validMove = MoveTo(currentlyDragging, randomMove.x, randomMove.y);
        if (!validMove)
        {
            // Convertimos la posici�n local a posici�n global teniendo en cuenta la transformaci�n del tablero
            // Se debe aplicar al final, despues de haber calculado todas las transformaciones locales
            currentlyDragging.SetPositionWithLimits(transform.TransformPoint(GetTileCenter(previousPosition.x, previousPosition.y)), transform.rotation.eulerAngles, tileSize / 4, false);
        }

        /* Reset References */
        currentlyDragging = null;
        availableMoves.Clear();
    }

}
