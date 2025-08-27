using Godot;
using System;

public partial class Tile : Button
{
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
		Text = state.ToString();
		SetDeferred(PropertyName.Disabled, true);
	}
	public State getState() 
	{
		return state;
	}
}
