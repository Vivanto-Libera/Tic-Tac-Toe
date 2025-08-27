using Godot;
using System;
using System.Collections.Generic;
using static Tile.State;

public partial class Main : Node
{
	public Tile[,] tiles = new Tile[3, 3];
	public Main() 
	{
		for(int i = 0;i < 3; i++) 
		{
			string row = "";
			switch (i) 
			{
				case 0:
					row = "A";
					break;
				case 1:
					row = "B";
					break;
				case 2:
					row = "C";
					break;
			}
			for(int j = 0; j < 3; j++) 
			{
				string column = j.ToString();
				string tileName = row + column;
				tiles[i, j] = GetNode<Tile>(tileName);
			}
		}
	}
	public void GameStart() 
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				tiles[i, j].setState(None);
			}
		}
		GetNode<Button>("First").Show();
		GetNode<Button>("Second").Show();
	}
	public void OrderSelected() 
	{
        GetNode<Button>("First").Hide();
        GetNode<Button>("Second").Hide();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
				tiles[i, j].SetDeferred(Tile.PropertyName.Disabled, false);
            }
        }
    }
	public void OnFirstPressed() 
	{
		OrderSelected();
    }
	public void OnSecondPressed() 
	{
		OrderSelected();
    }

	public void AIMove() 
	{
		
	}
	
	public void JudgeWin() 
	{

	}
}

public class AlphaBeta 
{
	private enum WhoWin 
	{
		OWin,
		XWin,
		Draw,
		NotEnd,
	}
	Tile.State[,] board = new Tile.State[3, 3];
	public struct Point 
	{
		int x;
		int y;
		public Point(int x1, int y1) 
		{
			x = x1;
			y = y1;
		}
	}
	List<Point> points0;
	List<Point> points1;
	List<Point> pointsm1;

	public AlphaBeta(Tile[,] theBoard) 
	{
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
				board[i, j] = theBoard[i, j].getState();
            }
        }
    }
	public Point GetTile() 
	{
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
				int theValue = DeepFirst(i, j, true, 2);
				if (theValue == 1)
				{
					points1.Add(new Point(i, j));
				}
				else if (theValue == 0 && points1.Count == 0) 
				{
					points0.Add(new Point(i, j));
				}
				else if(points0.Count == 0) 
				{
					pointsm1.Add(new Point(i, j));
                }
            }
        }
		if(points1.Count != 0) 
		{
			return points1[GD.RandRange(0, points1.Count - 1)];
		}
        if (points0.Count != 0)
        {
            return points0[GD.RandRange(0, points0.Count - 1)];
        }
        return pointsm1[GD.RandRange(0, pointsm1.Count - 1)];
    }
	private int DeepFirst(int x, int y, bool isMax, int parent) 
	{
		if (board[x,y] != None) 
		{
			return parent;
		}
		int curValue = isMax ? -2 : 2;
		board[x, y] = isMax ? O : X;
		WhoWin whoWin = JudgeWhoWin();
		if(whoWin == WhoWin.Draw) 
		{
			return 0;
		}
		if(whoWin == WhoWin.OWin)
		{
			return 1;
		}
		else if(whoWin == WhoWin.XWin)
		{
			return -1;
		}
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int newValue = DeepFirst(i, j, !isMax, curValue);
				if (isMax) 
				{
					if(newValue > curValue) 
					{
						curValue = newValue;
					}
					if(curValue > parent) 
					{
						return parent;
					}
				}
				else 
				{
                    if (newValue < curValue)
                    {
                        curValue = newValue;
                    }
                    if (curValue < parent)
                    {
                        return parent;
                    }
                }
            }
        }
		board[x, y] = None;
		return curValue;
    }
	private WhoWin JudgeWhoWin() 
	{
		if (board[0, 0] != None) 
		{
			if ((board[0, 0] == board[0, 1] && board[0, 0] == board[0, 2])
				|| (board[0, 0] == board[1, 0] && board[0, 0] == board[2, 0])) 
			{
				return StateToWhoWin(board[0, 0]);
			}
		}
        if (board[2, 2] != None)
        {
            if ((board[2, 2] == board[2, 1] && board[2, 2] == board[2, 0])
                || (board[2, 2] == board[1, 2] && board[2, 2] == board[0, 2]))
            {
                return StateToWhoWin(board[2, 2]);
            }
        }
		if (board[1, 1] != None) 
		{
            if ((board[1, 1] == board[1, 0] && board[1, 1] == board[1, 2])
				|| (board[1, 1] == board[0, 1] && board[1, 1] == board[2, 1])
                || (board[1, 1] == board[0, 0] && board[1, 1] == board[2, 2])
                || (board[1, 1] == board[2, 0] && board[1, 1] == board[2, 0]))
            {
                return StateToWhoWin(board[1, 1]);
            }
        }
		for(int i=0;i<3;i++)
		{
			for(int j = 0; j < 3; j++) 
			{
				if (board[i, j] == None) 
				{
					return WhoWin.NotEnd;
				}
			}
		}
		return WhoWin.Draw;
    }
	private WhoWin StateToWhoWin(Tile.State who) 
	{
        if (who == O)
        {
            return WhoWin.OWin;
        }
        else
        {
            return WhoWin.XWin;
        }
    }
}
