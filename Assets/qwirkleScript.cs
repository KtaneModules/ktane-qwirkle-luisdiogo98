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
	public GameObject[] tiles;
	public Material[] tileMats;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		//btn.OnInteract += delegate () { PressButton(); return false; };
	}

	void Start () 
	{
		GenerateBoard();
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

			List<Tile> possibilities = GetPossibilities(row, column);

			if(possibilities.Count() != 0)
			{
				board[row][column] = possibilities.OrderBy(x => rnd.Next()).ElementAt(0);
				order.Add(priority.ElementAt(0));
				tiles[priority.ElementAt(0)].transform.GetComponentInChildren<Renderer>().material = tileMats[board[row][column].color * 6 + board[row][column].shape];
			}
			priority.RemoveAt(0);
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

	List<Tile> GetPossibilities(int row, int column)
	{
		List<Tile> ret = new List<Tile>(); 

		if(IncompatibleTile(row, column))
			return ret;

		bool[][] down = GetColumnPossibilities(row, column, true);
		bool[][] up = GetColumnPossibilities(row, column, false);
		bool[][] right = GetRowPossibilities(row, column, true);
		bool[][] left = GetRowPossibilities(row, column, false);

		for(int i = 0; i < 6; i++)
			for(int j = 0; j < 6; j++)
				if(down[i][j] && up[i][j] && right[i][j] && left[i][j])
					ret.Add(new Tile(i, j));

		return ret;
	}

	bool IncompatibleTile(int row, int column)
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
}
