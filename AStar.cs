using UnityEngine;
using System;
using System.Collections.Generic;

//This is a custom implementation of a default .net class
public class Tuple<T1, T2>
{
	public T1 Item1 { get; private set; }
	public T2 Item2 { get; private set; }
	internal Tuple(T1 first, T2 second)
	{
		Item1 = first;
		Item2 = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}

// A* needs only a WeightedGraph and a location type L, and does *not*
// have to be a grid. However, in the example code I am using a grid.
public interface WeightedGraph<L>
{
	int Cost(Location a, Location b);
	IEnumerable<Location> Neighbors(Location id);
}


public struct Location
{
	// Implementation notes: I am using the default Equals but it can
	// be slow. You'll probably want to override both Equals and
	// GetHashCode in a real project.
	
	public readonly int x, y;
	public Location(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public class EncounterMap:WeightedGraph<Location>
{
	// Implementation notes: I made the fields public for convenience,
	// but in a real project you'll probably want to follow standard
	// style and make them private.
	
	public static readonly Location[] DIRS = new []
	{
		new Location(1, 0),
		new Location(0, -1),
		new Location(-1, 0),
		new Location(0, 1)
	};
	
	//public int width, height;
	public HashSet<Location> walls = new HashSet<Location>();
	//public HashSet<Location> forests = new HashSet<Location>();
	//List<Location> locs=new List<Location>();
	Dictionary<Vector2,EncounterRoom> map;
	
	public EncounterMap (Dictionary<Vector2,EncounterRoom> realMap)
	{
		//this.width = width;
		//this.height = height;
		foreach (Vector2 roomCoord in realMap.Keys) 
		{
			if (realMap[roomCoord].isWall) {walls.Add(new Location((int)roomCoord.x,(int)roomCoord.y));}
			//locs.Add(new Location((int)roomCoord.x,(int)roomCoord.y));
		}
		map=realMap;	
	}
	
	public bool InBounds(Location id)
	{
		return map.ContainsKey(new Vector2(id.x,id.y));//locs.Contains(id);
		/*
		return 0 <= id.x && id.x < width
			&& 0 <= id.y && id.y < height;*/
	}
	
	public bool Passable(Location id)
	{
		return !walls.Contains(id); //|| map[new Vector2(id.x,id.y)].hasEnemies);
	}
	
	public int Cost(Location a, Location b)
	{
		return map[new Vector2(b.x,b.y)].barricadeInRoom!=null ? 2 : 1;
	}
	
	public IEnumerable<Location> Neighbors(Location id)
	{
		foreach (var dir in DIRS) {
			Location next = new Location(id.x + dir.x, id.y + dir.y);
			if (InBounds(next) && Passable(next)) {
				yield return next;
			}
		}
	}
}

public class SquareGrid : WeightedGraph<Location>
{
	// Implementation notes: I made the fields public for convenience,
	// but in a real project you'll probably want to follow standard
	// style and make them private.
	
	public static readonly Location[] DIRS = new []
	{
		new Location(1, 0),
		new Location(0, -1),
		new Location(-1, 0),
		new Location(0, 1)
	};
	
	public int width, height;
	public HashSet<Location> walls = new HashSet<Location>();
	public HashSet<Location> forests = new HashSet<Location>();
	
	public SquareGrid(int width, int height)
	{
		this.width = width;
		this.height = height;
	}
	
	public bool InBounds(Location id)
	{
		return 0 <= id.x && id.x < width
			&& 0 <= id.y && id.y < height;
	}
	
	public bool Passable(Location id)
	{
		return !walls.Contains(id);
	}
	
	public int Cost(Location a, Location b)
	{
		return forests.Contains(b) ? 5 : 1;
	}
	
	public IEnumerable<Location> Neighbors(Location id)
	{
		foreach (var dir in DIRS) {
			Location next = new Location(id.x + dir.x, id.y + dir.y);
			if (InBounds(next) && Passable(next)) {
				yield return next;
			}
		}
	}
}


public class PriorityQueue<T>
{
	// I'm using an unsorted array for this example, but ideally this
	// would be a binary heap. Find a binary heap class:
	// * https://bitbucket.org/BlueRaja/high-speed-priority-queue-for-c/wiki/Home
	// * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
	// * http://xfleury.github.io/graphsearch.html
	// * http://stackoverflow.com/questions/102398/priority-queue-in-net
	
	private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();
	
	public int Count
	{
		get { return elements.Count; }
	}
	
	public void Enqueue(T item, int priority)
	{
		elements.Add(new Tuple<T,int>(item,priority));//Tuple.Create(item, priority));
	}
	
	public T Dequeue()
	{
		int bestIndex = 0;
		
		for (int i = 0; i < elements.Count; i++) {
			if (elements[i].Item2 < elements[bestIndex].Item2) {
				bestIndex = i;
			}
		}
		
		T bestItem = elements[bestIndex].Item1;
		elements.RemoveAt(bestIndex);
		return bestItem;
	}
}


public class AStarSearch
{
	public Dictionary<Location, Location> cameFrom
		= new Dictionary<Location, Location>();
	public Dictionary<Location, int> costSoFar
		= new Dictionary<Location, int>();
		public List<Vector2> path=new List<Vector2>();
	
	// Note: a generic version of A* would abstract over Location and
	// also Heuristic
	static public int Heuristic(Location a, Location b)
	{
		return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
	}
	
	public AStarSearch(WeightedGraph<Location> graph, Location start, Location goal)
	{
		int iterationCount=0;
		var frontier = new PriorityQueue<Location>();
		frontier.Enqueue(start, 0);
		
		cameFrom[start] = start;
		costSoFar[start] = 0;
		int bestPriority=int.MaxValue;
		Location bestSecondLocation=start;
		bool goalReached=false;
		
		while (frontier.Count > 0)
		{
			var current = frontier.Dequeue();
			
			if (current.Equals(goal))
			{
				goalReached=true;
				break;
			}
			//int minCost=int.MaxValue;
			//Location minLocation=new Location(0,0);
			foreach (var next in graph.Neighbors(current))
			{
				int newCost = costSoFar[current]
				+ graph.Cost(current, next);
				int priority = newCost + Heuristic(next, goal);
				if (!costSoFar.ContainsKey(next)
				    || newCost < costSoFar[next])
				{
					costSoFar[next] = newCost;
					frontier.Enqueue(next, priority);
					cameFrom[next] = current;
				}
				if (current.x==start.x && current.y==start.y)
				{
					if (priority<bestPriority)
					{
						bestPriority=priority;
						bestSecondLocation=next;
					}
				}
				iterationCount++;
				/*
				if (minCost>costSoFar[next]) 
				{
					minCost=costSoFar[next];
					minLocation=next;
				}*/
			}
			//path.Add(minLocation);
		}
		//GameManager.DebugPrint("camefrom has "+cameFrom.Count);
		//GameManager.DebugPrint("costsofar has "+costSoFar.Count);
		if (goalReached)path=ReconstructPath(start,goal);
		else path=ReconstructPath(start,bestSecondLocation);
		//GameManager.DebugPrint("path has "+path.Count);
		/*
		var cursor=goal;
		while (cursor.x!=start.x && cursor.y!=start.y)
		{
			int minCost=int.MaxValue;
			Location minLocation=new Location(0,0);
			foreach (var next in graph.Neighbors(cursor))
			{
				if (costSoFar.ContainsKey(next)) 
				{
					if (minCost>costSoFar[next]) 
					{
						minCost=costSoFar[next];
						minLocation=next;
					}
				}
			}
			path.Add(minLocation);
			cursor=minLocation;
		}*/
		//GameManager.DebugPrint("A* done. Iteration count:"+iterationCount);
	}
	
	List<Vector2> ReconstructPath(Location start,Location goal) 
	{
		List<Vector2> rpath=new List<Vector2>();
		//this accomodates for unsolvable paths
		Location current = goal;
		rpath.Add(new Vector2(current.x,current.y));
		while (!(current.x == start.x && current.y == start.y)) 
		{
			current = cameFrom[current];
			rpath.Add(new Vector2(current.x,current.y));
		}
		rpath.Reverse();
		//std::reverse(path.begin(), path.end());
		return rpath;
	}
}



public class Test
{
	static void DrawGrid(SquareGrid grid, AStarSearch astar) {
		// Print out the cameFrom array
		for (var y = 0; y < 10; y++)
		{
			for (var x = 0; x < 10; x++)
			{
				Location id = new Location(x, y);
				Location ptr = id;
				if (!astar.cameFrom.TryGetValue(id, out ptr))
				{
					ptr = id;
				}
				if (grid.walls.Contains(id)) { Console.Write("##"); }
				else if (ptr.x == x+1) { Console.Write("\u2192 "); }
				else if (ptr.x == x-1) { Console.Write("\u2190 "); }
				else if (ptr.y == y+1) { Console.Write("\u2193 "); }
				else if (ptr.y == y-1) { Console.Write("\u2191 "); }
				else { Console.Write("* "); }
			}
			Console.WriteLine();
		}
	}
	
	public static void PrintPath(AStarSearch search, Location endPoint)
	{
		foreach (Vector2 locCoords in search.path)
		{
			GameManager.DebugPrint(" to "+locCoords);//new Vector2(path[loc].x,path[loc].y));//+" to "+new Vector2(loc.x,loc.y));
		}
		/*
		Location cursor=endPoint;
		while (cursor.x!=path[cursor].x && cursor.y!=path[cursor].y)
		{
			GameManager.DebugPrint(new Vector2(path[cursor].x,path[cursor].y)+" to "+new Vector2(cursor.x,cursor.y));
			cursor=path[cursor];
		}*/
		/*
		Location cursor=endPoint;
		List<Location> pathNodes=new List<Location>();
		while (cursor.x!=path[cursor].x && cursor.y!=path[cursor].y)
		{
			fuck.costSoFar[]
		}
		
		foreach (Location loc in path.Keys) 
		{
			GameManager.DebugPrint(new Vector2(path[loc].x,path[loc].y)+" to "+new Vector2(loc.x,loc.y));
		}*/
	}
	
	static void Main()
	{
		// Make "diagram 4" from main article
		var grid = new SquareGrid(10, 10);
		for (var x = 1; x < 4; x++)
		{
			for (var y = 7; y < 9; y++)
			{
				grid.walls.Add(new Location(x, y));
			}
		}
		grid.forests = new HashSet<Location>
		{
			new Location(3, 4), new Location(3, 5),
			new Location(4, 1), new Location(4, 2),
			new Location(4, 3), new Location(4, 4),
			new Location(4, 5), new Location(4, 6),
			new Location(4, 7), new Location(4, 8),
			new Location(5, 1), new Location(5, 2),
			new Location(5, 3), new Location(5, 4),
			new Location(5, 5), new Location(5, 6),
			new Location(5, 7), new Location(5, 8),
			new Location(6, 2), new Location(6, 3),
			new Location(6, 4), new Location(6, 5),
			new Location(6, 6), new Location(6, 7),
			new Location(7, 3), new Location(7, 4),
			new Location(7, 5)
		};
		
		// Run A*
		var astar = new AStarSearch(grid, new Location(1, 4),
		                            new Location(8, 5));
		
		DrawGrid(grid, astar);
	}
}
