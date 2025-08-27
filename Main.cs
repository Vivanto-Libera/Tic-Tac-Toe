using Godot;
using System;

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
				tiles[i, j].setState(Tile.State.None);
			}
		}
		GetNode<Button>("First").Show();
		GetNode<Button>("Second").Show();
	}
}
