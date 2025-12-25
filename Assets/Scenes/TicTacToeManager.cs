using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    // Game board state: 0 = empty, 1 = X, 2 = O
    private int[,] board = new int[3, 3];
    private int currentPlayer = 1; // 1 = X, 2 = O
    private bool gameOver = false;

    public GameObject xPrefab;
    public GameObject oPrefab;
    public Transform gameBoardTransform;
    public Camera gameCamera;

    void Start()
    {
        Debug.Log("TicTacToe Game Started - Click on a cell to play");
        
        // Find camera if not assigned
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
            if (gameCamera == null)
            {
                Debug.LogError("No camera found! Please assign a camera or tag your camera as MainCamera");
            }
        }
    }

    void Update()
    {
        if (gameOver || gameCamera == null) return;

        // Handle mouse click for cell selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if we hit a cell
                if (hit.collider.gameObject.name.StartsWith("Cell_"))
                {
                    HandleCellClick(hit.collider.gameObject);
                }
            }
        }
    }

    void HandleCellClick(GameObject cell)
    {
        // Parse cell coordinates from name (Cell_row_col)
        string[] parts = cell.name.Split('_');
        if (parts.Length != 3) return;

        int row = int.Parse(parts[1]);
        int col = int.Parse(parts[2]);

        // Check if cell is already occupied
        if (board[row, col] != 0)
        {
            Debug.Log("Cell already occupied!");
            return;
        }

        // Place piece
        board[row, col] = currentPlayer;
        PlacePiece(row, col, cell.transform.position);

        // Check for win
        if (CheckWin(currentPlayer))
        {
            gameOver = true;
            string winner = currentPlayer == 1 ? "X" : "O";
            Debug.Log($"Player {winner} wins!");
            return;
        }

        // Check for draw
        if (CheckDraw())
        {
            gameOver = true;
            Debug.Log("It's a draw!");
            return;
        }

        // Switch player
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        string nextPlayer = currentPlayer == 1 ? "X" : "O";
        Debug.Log($"Player {nextPlayer}'s turn");
    }

    void PlacePiece(int row, int col, Vector3 cellPosition)
    {
        GameObject prefab = currentPlayer == 1 ? xPrefab : oPrefab;
        
        if (prefab != null)
        {
            Vector3 piecePosition = new Vector3(cellPosition.x, cellPosition.y + 0.6f, cellPosition.z);
            GameObject piece = Instantiate(prefab, piecePosition, Quaternion.identity);
            piece.transform.parent = gameBoardTransform;
            piece.name = $"Piece_{row}_{col}";
        }
        else
        {
            Debug.LogWarning("Prefabs not assigned! Please assign X and O prefabs in the inspector.");
        }
    }

    bool CheckWin(int player)
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] == player && board[row, 1] == player && board[row, 2] == player)
                return true;
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] == player && board[1, col] == player && board[2, col] == player)
                return true;
        }

        // Check diagonals
        if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            return true;

        if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            return true;

        return false;
    }

    bool CheckDraw()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == 0)
                    return false;
            }
        }
        return true;
    }

    public void ResetGame()
    {
        // Clear board
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                board[row, col] = 0;
            }
        }

        // Remove all pieces
        foreach (Transform child in gameBoardTransform)
        {
            if (child.name.StartsWith("Piece_"))
            {
                Destroy(child.gameObject);
            }
        }

        // Reset state
        currentPlayer = 1;
        gameOver = false;
        Debug.Log("Game reset - Player X's turn");
    }
}
