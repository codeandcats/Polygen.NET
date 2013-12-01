using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polygen.UI
{
	public class UndoManager<State>
	{
		private Stack<KeyValuePair<State, string>> undoStack = new Stack<KeyValuePair<State, string>>();
		private Stack<KeyValuePair<State, string>> redoStack = new Stack<KeyValuePair<State, string>>();

		private State currentState = default(State);
		private bool initalised = false;

		public void MadeChanges(State newState, string changeDescription)
		{
			undoStack.Push(new KeyValuePair<State, string>(currentState, changeDescription));
			redoStack.Clear();
			currentState = newState;
		}

		public bool CanUndo()
		{
			return initalised && (undoStack.Count > 0);
		}

		public bool CanRedo()
		{
			return initalised && (redoStack.Count > 0);
		}

		public string GetUndoChangeDescription()
		{
			if (!CanUndo())
				return "";
			else
				return undoStack.Peek().Value;
		}

		public string GetRedoChangeDescription()
		{
			if (!CanRedo())
				return "";
			else
				return redoStack.Peek().Value;
		}

		public State Undo()
		{
			if (!CanUndo())
				throw new Exception("Nothing to undo");

			// Pop our new state off our undo stack
			var newState = undoStack.Pop();

			// Add our current state to the redo stack
			redoStack.Push(new KeyValuePair<State, string>(currentState, newState.Value));

			// Set our current state to the new state (from the undo stack)
			currentState = newState.Key;

			return currentState;
		}

		public State Redo()
		{
			if (!CanRedo())
				throw new Exception("Nothing to redo");

			// Pop our new state off our redo stack
			var newState = redoStack.Pop();

			// Add our current state to the undo stack
			undoStack.Push(new KeyValuePair<State, string>(currentState, newState.Value));

			// Set our current state to the new state (from the redo stack)
			currentState = newState.Key;

			return currentState;
		}

		public void Reset()
		{
			currentState = default(State);
			undoStack.Clear();
			redoStack.Clear();
			initalised = false;
		}

		public void Init(State currentState)
		{
			this.currentState = currentState;
			initalised = true;
		}
	}
}
