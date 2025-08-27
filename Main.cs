using Godot;
using System;
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
	public void GetTile(ref int x, ref int y) 
	{
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
				DeepFirst(0, 0, true, 2);
            }
        }
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
