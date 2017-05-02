﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  NodeManager serves as an intermediate between individual Nodes and 
  the StoryDialogueEditor.
  
  This allows the organization of Action<Node> to be removed from the 
  StoryDialogueEditor itself, without cloning a copy of the main editor
  into every single Node object.
*/
public static class NodeManager {
	
	public static StoryDialogueEditor mainEditor;
	
	// all Nodes use these styles
	public static GUIStyle nodeDefault;
	public static GUIStyle nodeSelected;
	
	// defines Node dimensions
	public const int NODE_WIDTH = 200;
	public const int NODE_HEIGHT = 37;
	
	/*
	  DrawNodes() draws all the mainEditor.nodes in the StoryDialogueEditor window.
	*/
	public static void DrawNodes() {
		if (mainEditor.nodes != null) {
			for (int i = 0; i < mainEditor.nodes.Count; i++) {
				mainEditor.nodes[i].Draw();
			}
		}
	}
	
	/*
	  ProcessEvents() goes through all the mainEditor.nodes and processes their events.
	*/
	public static void ProcessEvents(Event e) {
		if (mainEditor.nodes != null) {
			// processed backwards because mainEditor.nodes on the top are rendered on top
			for (int i = mainEditor.nodes.Count - 1; i >= 0; i--) {
				mainEditor.nodes[i].ProcessEvent(e);
			}
		}
	}
	
	/*
	  RemoveNode() removes the given Node from the global list of mainEditor.nodes, and 
	  destroys any connections it was part of.
	*/
	public static void RemoveNode(Node node) {
		Undo.RecordObject(mainEditor, "removing node and associated connections...");
		
		if (mainEditor.connections != null) { 
			// build the list of Connections to remove
			List<Connection> connectionsToRemove = new List<Connection>();
			for (int i = 0; i < mainEditor.connections.Count; i++) {
				if (mainEditor.connections[i].inPoint == node.inPoint || mainEditor.connections[i].outPoint == node.outPoint) {
					connectionsToRemove.Add(mainEditor.connections[i]);
				}
			}
			
			// remove all the connections from the global list of connections.
			for (int i = 0; i < connectionsToRemove.Count; i++) {
				mainEditor.connections.Remove(connectionsToRemove[i]);
			}
			
			// free the reference for GC
			connectionsToRemove = null;
		}
		
		// remove the node from the global node list and the SelectionManager
		mainEditor.nodes.Remove(node);
		SelectionManager.Deselect(node);
		
		Undo.FlushUndoRecordObjects();
	}
	
	/*
	  AddNoteAt() creates a new Node at the given mouse position.
	*/
	public static void AddNodeAt(Vector2 nodePosition) {
		Undo.RecordObject(mainEditor, "adding node at...");
		
		if (mainEditor.nodes == null) {
			mainEditor.nodes = new List<Node>();
		}
		
		// add node as close to center as possible while staying on grid
		nodePosition.x -= (NODE_WIDTH/2) - (NODE_WIDTH/2) % StoryDialogueEditor.GRID_SIZE;
		nodePosition.y -= (NODE_HEIGHT/2) - (NODE_HEIGHT/2) % StoryDialogueEditor.GRID_SIZE;
		Node newNode = ScriptableObject.CreateInstance<Node>();
		newNode.Init(
			nodePosition, NODE_WIDTH, NODE_HEIGHT, 
			nodeDefault, nodeSelected, 
			RemoveNode);
		
		mainEditor.nodes.Add(newNode);
		
		Undo.FlushUndoRecordObjects();
	}
}
