using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Tile.State;

public partial class Main : Node
{
	[Signal]
	public delegate void GameOverEventHandler(int result);
	[Signal]
	public delegate void GameResetEventHandler();

	public Tile[,] tiles = new Tile[3, 3];

	public override void _Ready()
	{
		for (int i = 0; i < 3; i++)
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
			for (int j = 0; j < 3; j++)
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
		AIMove();
	}

	public void AIMove() 
	{
		AlphaBeta.Point point = new AlphaBeta(tiles).GetTile();
		tiles[point.x, point.y].setState(O);
		JudgeWin(false);
	}

	public void JudgeWin(bool isPlayerMove) 
	{
		AlphaBeta.WhoWin who = new AlphaBeta(tiles).JudgeWhoWin();
		if(who != AlphaBeta.WhoWin.NotEnd) 
		{
			EmitSignal(SignalName.GameOver,(int)who);
			return;
		}
		if (isPlayerMove)
		{
			AIMove();
		}
	}

	public void OnTilePressed(int row, int column) 
	{
		tiles[row, column].setState(X);
		JudgeWin(true);
	}
	
	public async void OnGameOver(int result) 
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				tiles[i, j].SetDeferred(Tile.PropertyName.Disabled, true);
			}
		}
		Label message = GetNode<Label>("Message");
		if(result == (int)AlphaBeta.WhoWin.OWin) 
		{
			message.Text = "你输了";
		}
		else if (result == (int)AlphaBeta.WhoWin.XWin)
		{
			message.Text = "你赢了";
		}
		else 
		{
			message.Text = "平局";
		}
		message.Show();
		await ToSignal(GetTree().CreateTimer(3), Timer.SignalName.Timeout);
		message.Hide();
		EmitSignal(SignalName.GameReset);
	}

	public void OnGameReset() 
	{
		GameStart();
	}
}

public class AlphaBeta 
{
	public enum WhoWin 
	{
		OWin,
		XWin,
		Draw,
		NotEnd,
	}
	
	public Tile.State[,] board = new Tile.State[3, 3];
	public struct Point 
	{
		public int x;
		public int y;
		public Point(int x1, int y1) 
		{
			x = x1;
			y = y1;
		}
	}
	public struct Action
	{
		public Point point;
		public int v;
		public Action(Point aPoint, int aV) 
		{
			point = aPoint;
			v = aV;
		}
	}

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
		Point point = MaxValue(-2,2).point;
		return point;
	}
	private Action MaxValue(int alpha, int beta) 
	{
		WhoWin who = JudgeWhoWin();
		if (who == WhoWin.XWin)
		{
			return new Action(new Point(0, 0), -1);
		}
		if(who == WhoWin.Draw) 
		{
			return new Action(new Point(0, 0), 0);
		}
		int v = -2;
		Action newAction = new Action(new Point(0, 0), v);
		for (int i = 0; i < 3; i++) 
		{
			for(int j = 0; j < 3; j++) 
			{
				if(board[i, j] == None) 
				{
					board[i, j] = O;
					int newV = MinValue(alpha, beta).v;
					if(newV >= beta) 
					{
						board[i, j] = None;
						return new Action(new Point(i, j), newV);
					}
					if(newV > v) 
					{
						v = newV;
						newAction = new Action(new Point(i, j), v);
					}
					alpha = v > alpha ? v : alpha;
					board[i, j] = None;
				}
			}
		}
		return newAction;
	}
	private Action MinValue(int alpha, int beta)
	{
		WhoWin who = JudgeWhoWin();
		if (who == WhoWin.OWin)
		{
			return new Action(new Point(0, 0), 1);
		}
		if (who == WhoWin.Draw)
		{
			return new Action(new Point(0, 0), 0);
		}
		int v = 2;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				if (board[i, j] == None)
				{
					board[i, j] = X;
					int newV = MaxValue(alpha, beta).v;
					if (newV <= alpha)
					{
						board[i, j] = None;
						return new Action(new Point(0, 0), newV);
					}
					v = newV < v ? newV : v;
					beta = v < beta ? v : beta;
					board[i, j] = None;
				}
			}
		}
		return new Action(new Point(0, 0), v);
	}

	public WhoWin JudgeWhoWin() 
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
				|| (board[1, 1] == board[2, 0] && board[1, 1] == board[0, 2]))
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
