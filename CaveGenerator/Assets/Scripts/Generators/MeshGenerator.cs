using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public class MeshGenerator : MonoBehaviour
{
	#region Variables

	private SquareGrid _squareGrid;
	public MeshFilter walls;
	public MeshFilter cave;
	public bool is2DMode;
	private const int TileAmount = 10;
	
	private List<Vector3> _vertices;
	private List<int> _triangles;
	private readonly Dictionary<int, List<Triangle>> _triangleDictionary = new Dictionary<int, List<Triangle>>();
	private readonly List<List<int>> _outlines = new List<List<int>>();
	private readonly HashSet<int> _checkedVertices = new HashSet<int>();
	
	#endregion

	#region Private Methods

	private void CreateWallMesh()
	{
		CalculateMeshOutlines();

		var wallVertices = new List<Vector3>();
		var wallTriangles = new List<int>();
		var wallMesh = new Mesh();
		var wallHeight = 5;

		foreach (var outline in _outlines)
		{
			for (int i = 0; i < outline.Count - 1; i++)
			{
				var startIndex = wallVertices.Count;
				wallVertices.Add(_vertices[outline[i]]); // left
				wallVertices.Add(_vertices[outline[i + 1]]); // right
				wallVertices.Add(_vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add(_vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}

		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		walls.mesh = wallMesh;

		var wallCollider = walls.gameObject.AddComponent<MeshCollider>();
		wallCollider.sharedMesh = wallMesh;
	}


	private void TriangulateSquare(Square square)
	{
		switch (square.Configuration)
		{
			case 0:
				break;

			// 1 points:
			case 1:
				MeshFromPoints(square.CenterLeft, square.CenterBottom, square.BottomLeft);
				break;
			case 2:
				MeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
				break;
			case 4:
				MeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
				break;
			case 8:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
				break;

			// 2 points:
			case 3:
				MeshFromPoints(square.CenterRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
				break;
			case 6:
				MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.CenterBottom);
				break;
			case 9:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
				break;
			case 12:
				MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterLeft);
				break;
			case 5:
				MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight, square.CenterBottom,
					square.BottomLeft, square.CenterLeft);
				break;
			case 10:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight,
					square.CenterBottom, square.CenterLeft);
				break;

			// 3 point:
			case 7:
				MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.BottomLeft,
					square.CenterLeft);
				break;
			case 11:
				MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight,
					square.BottomLeft);
				break;
			case 13:
				MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom,
					square.BottomLeft);
				break;
			case 14:
				MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CenterBottom,
					square.CenterLeft);
				break;

			// 4 point:
			case 15:
				MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);
				_checkedVertices.Add(square.TopLeft.VertexIndex);
				_checkedVertices.Add(square.TopRight.VertexIndex);
				_checkedVertices.Add(square.BottomRight.VertexIndex);
				_checkedVertices.Add(square.BottomLeft.VertexIndex);
				break;
		}
	}

	private void MeshFromPoints(params Node[] points)
	{
		AssignVertices(points);

		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5)
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);
	}

	private void AssignVertices(Node[] points)
	{
		foreach (var point in points)
		{
			if (point.VertexIndex != -1) 
				continue;
			point.VertexIndex = _vertices.Count;
			_vertices.Add(point.Position);
		}
	}

	private void CreateTriangle(Node a, Node b, Node c)
	{
		_triangles.Add(a.VertexIndex);
		_triangles.Add(b.VertexIndex);
		_triangles.Add(c.VertexIndex);

		Triangle triangle = new Triangle(a.VertexIndex, b.VertexIndex, c.VertexIndex);
		AddTriangleToDictionary(triangle.VertexIndexA, triangle);
		AddTriangleToDictionary(triangle.VertexIndexB, triangle);
		AddTriangleToDictionary(triangle.VertexIndexC, triangle);
	}

	private void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
	{
		if (_triangleDictionary.ContainsKey(vertexIndexKey))
		{
			_triangleDictionary[vertexIndexKey].Add(triangle);
		}
		else
		{
			var triangleList = new List<Triangle>{triangle};
			_triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}

	private void CalculateMeshOutlines()
	{

		for (int vertexIndex = 0; vertexIndex < _vertices.Count; vertexIndex++)
		{
			if (_checkedVertices.Contains(vertexIndex)) 
				continue;
			var newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
			if (newOutlineVertex == -1) 
				continue;
			_checkedVertices.Add(vertexIndex);

			var newOutline = new List<int> {vertexIndex};
			_outlines.Add(newOutline);
			FollowOutline(newOutlineVertex, _outlines.Count - 1);
			_outlines[_outlines.Count - 1].Add(vertexIndex);
		}
	}

	private void FollowOutline(int vertexIndex, int outlineIndex)
	{
		_outlines[outlineIndex].Add(vertexIndex);
		_checkedVertices.Add(vertexIndex);
		var nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1)
		{
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}

	private int GetConnectedOutlineVertex(int vertexIndex)
	{
		var trianglesContainingVertex = _triangleDictionary[vertexIndex];

		foreach (var triangle in trianglesContainingVertex)
		{
			for (int j = 0; j < 3; j++)
			{
				var vertexB = triangle[j];
				if (vertexB != vertexIndex && !_checkedVertices.Contains(vertexB))
				{
					if (IsOutlineEdge(vertexIndex, vertexB))
					{
						return vertexB;
					}
				}
			}
		}
		return -1;
	}

	private bool IsOutlineEdge(int vertexA, int vertexB)
	{
		var trianglesContainingVertexA = _triangleDictionary[vertexA];
		var sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++)
		{
			if (!trianglesContainingVertexA[i].Contains(vertexB)) 
				continue;
			sharedTriangleCount++;
			if (sharedTriangleCount > 1)
			{
				break;
			}
		}
		return sharedTriangleCount == 1;
	}

	#endregion
	
	public void GenerateMesh(int[,] map, float squareSize)
	{
		_triangleDictionary.Clear();
		_outlines.Clear();
		_checkedVertices.Clear();
		_squareGrid = new SquareGrid(map, squareSize);
		_vertices = new List<Vector3>();
		_triangles = new List<int>();

		for (int x = 0; x < _squareGrid.Squares.GetLength(0); x++)
		{
			for (int y = 0; y < _squareGrid.Squares.GetLength(1); y++)
			{
				TriangulateSquare(_squareGrid.Squares[x, y]);
			}
		}

		var mesh = new Mesh();
		cave.mesh = mesh;
		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		mesh.RecalculateNormals();

		var uvs = new Vector2[_vertices.Count];

		for (int i = 0; i < _vertices.Count; i++)
		{
			var percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize,
				_vertices[i].x) * TileAmount;
			var percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize,
				_vertices[i].z) * TileAmount;

			uvs[i] = new Vector2(percentX, percentY);
		}

		mesh.uv = uvs;
		
		if (is2DMode)
		{
			RemoveOld2DColliders();
			Generate2DColliders();
			return;
		}
		
		CreateWallMesh();
	}

	private void Generate2DColliders()
	{
		CalculateMeshOutlines();

		foreach (var outline in _outlines)
		{
			var edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
			var edgePoints = new Vector2[outline.Count];

			for (int i = 0; i < outline.Count; i++)
			{
				edgePoints[i] = new Vector2(_vertices[outline[i]].x, _vertices[outline[i]].z);
			}

			edgeCollider.points = edgePoints;
		}
	}

	private void RemoveOld2DColliders()
	{
		var currColliders = gameObject.GetComponents<EdgeCollider2D>();
		foreach (var edgeCollider2D in currColliders)
		{
			Destroy(edgeCollider2D);
		}
	}

	private readonly struct Triangle
	{
		public readonly int VertexIndexA;
		public readonly int VertexIndexB;
		public readonly int VertexIndexC;
		private readonly int[] _vertices;

		public Triangle(int a, int b, int c)
		{
			VertexIndexA = a;
			VertexIndexB = b;
			VertexIndexC = c;

			_vertices = new int[3];
			_vertices[0] = a;
			_vertices[1] = b;
			_vertices[2] = c;
		}

		public int this[int i] => _vertices[i];
		
		public bool Contains(int vertexIndex)
		{
			return vertexIndex == VertexIndexA || vertexIndex == VertexIndexB || vertexIndex == VertexIndexC;
		}
	}
}