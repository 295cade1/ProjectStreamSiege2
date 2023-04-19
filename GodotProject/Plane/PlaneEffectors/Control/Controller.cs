using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Controller : Node
{
   public enum ControlType
	{
		Pitch,
		Roll,
		Yaw,
		Power
	}
   private List<Node> getAllChildren(Node curNode) {
		List<Node> children = new List<Node>(curNode.GetChildren());
		List<Node> returnArray = new List<Node>();
		foreach (Node child in children) {
			returnArray.AddRange(getAllChildren(child));
		}
		returnArray.AddRange(children);
		return returnArray;
	}

   public override void _Ready(){
		initSubscribers();
	}
	public override void _Process(double delta){
		updateValues(delta);
	}

   protected abstract void updateValues(double delta);

	private void initSubscribers() {
		foreach (Node child in getAllChildren(GetNode(".."))) {
			if (child is ControlSubscriber) {
				((ControlSubscriber)child).setController(this);
			}
		}
	}
   public abstract float getControlValue(Controller.ControlType type);
}
