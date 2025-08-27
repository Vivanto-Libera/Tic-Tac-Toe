using Godot;
using System;

public partial class Tile : Button
{
	[Export]
	public int row;
	[Export] public int column;

	[Signal]
	public delegate void PointEventHandler(int row, int column);

	public enum State
	{
		None,
		O,
		X,
	}
	private State state = State.None;
	public void setState(State newState)
	{
		state = newState;
		if(state == State.None) 
		{
			Text = "";
			return;
		}
		Text = state.ToString();
		SetDeferred(PropertyName.Disabled, true);
	}
	public State getState() 
	{
		return state;
	}

	public void OnPressed() 
	{
		EmitSignal(SignalName.Point, row, column);
	}
}
