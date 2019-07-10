using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class qwirkleScript : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio Audio;

    static System.Random rnd = new System.Random();

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	Tile[][] board;
	Tile[] sideboard;
	int selected = 0;

	public GameObject[] tiles;
	public GameObject[] available;
	public Material[] tileMats;
	public Material emptyMat;
	public Material blackMat;
	public Material greenMat;
	public KMSelectable[] sideBtns;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		sideBtns[0].OnInteract += delegate () { PressSide(0); return false; };
		sideBtns[1].OnInteract += delegate () { PressSide(1); return false; };
		sideBtns[2].OnInteract += delegate () { PressSide(2); return false; };
		sideBtns[3].OnInteract += delegate () { PressSide(3); return false; };
	}

	void PressSide(int btn)
	{
		available[selected].transform.Find("selected").GetComponentInChildren<Renderer>().material = blackMat;
		selected = btn;
		available[selected].transform.Find("selected").GetComponentInChildren<Renderer>().material = greenMat;
	}

	void Start () 
	{
		GenerateBoard();
		GenerateAvailableTiles();
	}

	void GenerateBoard()
	{
		board = new Tile[][] { 
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) },
								new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty) }
							 };

		List<int> priority = Enumerable.Range(0, 48).ToList().OrderBy(x => rnd.Next()).ToList();
		List<int> order = new List<int>();
		bool firstTile = true;		

		int cnt = 0;

		while(priority.Count() != 0)
		{
			int row = priority[0] / 7;
			int column = priority[0] % 7;

			if(cnt == priority.Count()) break;

			if(!CheckValidTile(row, column) && !firstTile)
			{
				cnt++;
				priority.Add(priority.ElementAt(0));
				priority.RemoveAt(0);
				continue;
			}

			cnt = 0;

			firstTile = false;

			List<Tile> possibilities = GetPossibilities(row, column, true);

			if(possibilities.Count() != 0)
			{
				board[row][column] = possibilities.OrderBy(x => rnd.Next()).ElementAt(0);
				order.Add(priority.ElementAt(0));
				tiles[priority.ElementAt(0)].transform.GetComponentInChildren<Renderer>().material = tileMats[board[row][column].color * 6 + board[row][column].shape];
			}
			priority.RemoveAt(0);
		}

		for(int i = order.Count() - 7; i < order.Count; i++)
		{
			int row = order[i] / 7;
			int column = order[i] % 7;
			board[row][column] = new Tile(Tile.empty);
			tiles[order.ElementAt(i)].transform.GetComponentInChildren<Renderer>().material = emptyMat;
		}
	}

	bool CheckValidTile(int row, int column)
	{
		if(row != 0 && !board[row - 1][column].IsEmpty())
			return true;

		if(column != 0 && !board[row][column - 1].IsEmpty())
			return true;

		if(row != 6 && !board[row + 1][column].IsEmpty())
			return true;

		if(column != 6 && !board[row][column + 1].IsEmpty())
			return true;

		return false;
	}

	List<Tile> GetPossibilities(int row, int column, bool boring)
	{
		List<Tile> ret = new List<Tile>(); 

		bool[][] down = GetColumnPossibilities(row, column, true);
		bool[][] up = GetColumnPossibilities(row, column, false);
		bool[][] right = GetRowPossibilities(row, column, true);
		bool[][] left = GetRowPossibilities(row, column, false);

		for(int i = 0; i < 6; i++)
			for(int j = 0; j < 6; j++)
				if(down[i][j] && up[i][j] && right[i][j] && left[i][j] && (!boring || !BoringBoard(row, column)) && CheckConnectors(row, column, i, j))
					ret.Add(new Tile(i, j));

		return ret;
	}

	bool BoringBoard(int row, int column)
	{
		List<Tile> adj = new List<Tile>();

		if(row != 0 && !board[row - 1][column].IsEmpty())
			adj.Add(board[row - 1][column]);		
		if(row != 6 && !board[row + 1][column].IsEmpty())
			adj.Add(board[row + 1][column]);
		if(column != 0 && !board[row][column - 1].IsEmpty())
			adj.Add(board[row][column - 1]);
		if(column != 6 && !board[row][column + 1].IsEmpty())
			adj.Add(board[row][column + 1]);	

		if(adj.Count() > 1)
			return true;

		return false;
	}

	bool CheckConnectors(int row, int column, int color, int shape)
	{
		List<int> connectColor = new List<int>();
		List<int> connectShape = new List<int>();

		for(int i = 0; i < 7; i++)
		{
			int nextColor, nextShape;

			if(row == i)
			{
				nextColor = color;
				nextShape = shape;
			}
			else
			{
				if(board[i][column].IsEmpty())
				{
					connectColor = new List<int>();
					connectShape = new List<int>();
					continue;
				}

				nextColor = board[i][column].color;
				nextShape = board[i][column].shape;
			}

			if(connectColor.Count() == 0 && connectShape.Count() == 0)
			{
				connectColor.Add(nextColor);
				connectShape.Add(nextShape);
				continue;
			}

			if(connectColor.Exists(x => x == nextColor) && connectShape.Exists(x => x == nextShape))
				return false;
			else if(!connectColor.All(x => x == nextColor) && !connectShape.All(x => x == nextShape))
				return false;

			connectColor.Add(nextColor);
			connectShape.Add(nextShape);
		}

		connectColor = new List<int>();
		connectShape = new List<int>();

		for(int i = 0; i < 7; i++)
		{
			int nextColor, nextShape;

			if(column == i)
			{
				nextColor = color;
				nextShape = shape;
			}
			else
			{
				if(board[row][i].IsEmpty())
				{
					connectColor = new List<int>();
					connectShape = new List<int>();
					continue;
				}

				nextColor = board[row][i].color;
				nextShape = board[row][i].shape;
			}

			if(connectColor.Count() == 0 && connectShape.Count() == 0)
			{
				connectColor.Add(nextColor);
				connectShape.Add(nextShape);
				continue;
			}

			if(connectColor.Exists(x => x == nextColor) && connectShape.Exists(x => x == nextShape))
				return false;
			else if(!connectColor.All(x => x == nextColor) && !connectShape.All(x => x == nextShape))
				return false;

			connectColor.Add(nextColor);
			connectShape.Add(nextShape);
		}

		return true;
	}

	bool[][] GetColumnPossibilities(int row, int column, bool direction)
	{
		bool[][] ret = new bool[][] {
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
									};

		for(int i = (direction ? row + 1 : row - 1); i >= 0 && i < 7; i += direction ? 1 : -1)
		{
			if(board[i][column].IsEmpty())
				break;

			for(int j = 0; j < 6; j++)
				for(int k = 0; k < 6; k++)
					if((j != board[i][column].color && k != board[i][column].shape) || (j == board[i][column].color && k == board[i][column].shape))
						ret[j][k] = false;
		}

		return ret;
	}

	bool[][] GetRowPossibilities(int row, int column, bool direction)
	{
		bool[][] ret = new bool[][] {
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
										new bool[] { true, true, true, true, true, true},
									};

		for(int i = (direction ? column + 1 : column - 1); i >= 0 && i < 7; i += direction ? 1 : -1)
		{
			if(board[row][i].IsEmpty())
				break;

			for(int j = 0; j < 6; j++)
				for(int k = 0; k < 6; k++)
					if((j != board[row][i].color && k != board[row][i].shape) || (j == board[row][i].color && k == board[row][i].shape))
						ret[j][k] = false;
		}

		return ret;
	}

	void GenerateAvailableTiles()
	{
		sideboard = new Tile[] { new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty), new Tile(Tile.empty)};
		List<int> priority = Enumerable.Range(0, 48).ToList().OrderBy(x => rnd.Next()).ToList();
		List<Tile> stack = new List<Tile>();
		
		while(stack.Count() != 1)
		{
			int row = priority[0] / 7;
			int column = priority[0] % 7;

			if(!board[row][column].IsEmpty())
			{
				priority.RemoveAt(0);
				continue;
			}

			List<Tile> possibilities = GetPossibilities(row, column, false).OrderBy(x => rnd.Next()).ToList();

			if(possibilities.Count() == 0)
			{
				priority.RemoveAt(0);
				continue;
			}

			stack.Add(possibilities.ElementAt(0));
		}

		List<int> colors = Enumerable.Range(0, 5).ToList().OrderBy(x => rnd.Next()).ToList();
		List<int> shapes = Enumerable.Range(0, 5).ToList().OrderBy(x => rnd.Next()).ToList();

		int colorIndex = 0;
		int shapeIndex = 0;

		while(stack.Count() != 4)
		{
			while(stack.Exists(x => x.color == colors.ElementAt(colorIndex))) colorIndex++;
			while(stack.Exists(x => x.shape == shapes.ElementAt(shapeIndex))) shapeIndex++;

			stack.Add(new Tile(colors.ElementAt(colorIndex), shapes.ElementAt(shapeIndex)));
		}

		stack = stack.OrderBy(x => rnd.Next()).ToList();

		for(int i = 0; i < stack.Count(); i++)
		{
			sideboard[i] = stack.ElementAt(i);
			available[i].transform.GetComponentInChildren<Renderer>().material = tileMats[sideboard[i].color * 6 + sideboard[i].shape];
		}
	}
}
