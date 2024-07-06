using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { set; get; }

    public GameObject highlightPrefab;
    public GameObject captureHighlightPrefab; // New prefab for capturing highlight
    private List<GameObject> highlights;
    private List<GameObject> captureHighlights; // List for capture highlights

    private Camera mainCamera;
    private GameObject selectedPiece;
    private bool[,] allowedMoves;
    public float floatHeight = 1.5f;
    public float moveSpeed = 5f;
    public LayerMask boardMask;
    public LayerMask pieceMask; // New LayerMask for pieces

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
        captureHighlights = new List<GameObject>(); // Initialize capture highlights list
        mainCamera = Camera.main;
    }

    private GameObject GetHighLightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    private GameObject GetCaptureHighlightObject() // capture highlight
    {
        GameObject go = captureHighlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(captureHighlightPrefab);
            captureHighlights.Add(go);
        }

        return go;
    }

    public void HighLightAllowedMoves(bool[,] moves)
    {
        allowedMoves = moves;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    Vector3 position = new Vector3(i + 0.5f, 0.0001f, j + 0.5f);
                    if (IsCaptureMove(position))
                    {
                        GameObject go = GetCaptureHighlightObject();
                        go.SetActive(true);
                        go.transform.position = position;
                    }
                    else
                    {
                        GameObject go = GetHighLightObject();
                        go.SetActive(true);
                        go.transform.position = position;
                    }
                }
            }
        }
    }

    private bool IsCaptureMove(Vector3 position) // New method to check if a move is a capture move
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f, pieceMask);
        return colliders.Length > 0;
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
            go.SetActive(false);
        foreach (GameObject go in captureHighlights) // Hide capture highlights as well
            go.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, pieceMask) && hit.collider.CompareTag("ChessPiece"))
            {
                selectedPiece = hit.collider.gameObject;
                bool[,] moves = GetAllowedMovesForPiece(selectedPiece);
                HighLightAllowedMoves(moves);
            }
            else if (selectedPiece != null)
            {
                Vector3 targetPosition = GetMouseWorldPosition();
                if (targetPosition != Vector3.zero)
                {
                    int x = Mathf.RoundToInt(targetPosition.x);
                    int z = Mathf.RoundToInt(targetPosition.z);

                    if (allowedMoves[x, z])
                    {
                        StartCoroutine(MovePiece(selectedPiece, new Vector3(x, selectedPiece.transform.position.y, z)));
                    }
                }
                selectedPiece = null;
                HideHighlights();
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, boardMask))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private IEnumerator MovePiece(GameObject piece, Vector3 targetPosition)
    {
        Vector3 startPosition = piece.transform.position;
        float elapsedTime = 0;
        float journeyTime = Vector3.Distance(startPosition, targetPosition) / moveSpeed;

        while (elapsedTime < journeyTime)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / journeyTime)) + Vector3.up * Mathf.Sin((elapsedTime / journeyTime) * Mathf.PI) * floatHeight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = targetPosition;
    }

    private bool[,] GetAllowedMovesForPiece(GameObject piece)
    {
        return new bool[8, 8];
    }
}
