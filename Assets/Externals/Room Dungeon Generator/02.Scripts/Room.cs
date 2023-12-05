using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;

namespace ooparts.dungen
{
	public class Room : MonoBehaviour
	{
		public Corridor CorridorPrefab;
		public IntVector2 Size;
		public IntVector2 Coordinates;
		public int Num;
		public GameObject torchPrefab;

		private GameObject _tilesObject;
		private GameObject _roofObject;
		private GameObject _wallsObject;
		private GameObject _doorsObject;
		public Tile[] TilePrefab;
		private Tile[,] _tiles;
		public Wall[] walls;
		public RoomSetting Setting;

		public Dictionary<Room, Corridor> RoomCorridor = new Dictionary<Room, Corridor>();

		private Map _map;

		public GameObject PlayerPrefab;

		public GameObject DoorPrefab;

		public bool hasPlayer;
		public bool bossRoom;

		public void Init(Map map)
		{
			_map = map;
		}

		public IEnumerator Generate()
		{
			// Create parent object
			_tilesObject = new GameObject("Tiles");
			_tilesObject.transform.parent = transform;
			_tilesObject.transform.localPosition = Vector3.zero;

            _roofObject = new GameObject("Roof");
            _roofObject.transform.parent = transform;
            _roofObject.transform.localPosition = Vector3.zero;

            _tiles = new Tile[Size.x, Size.z];

            Tile randomItem = TilePrefab.Length > 0 ? TilePrefab[Random.Range(0, TilePrefab.Length)] : null;

            for (int x = 0; x < Size.x; x++)
			{
				for (int z = 0; z < Size.z; z++)
				{
                    _tiles[x, z] = CreateTile(new IntVector2((Coordinates.x + x), Coordinates.z + z), randomItem);
				}
			}
			yield return null;

            NavMeshSurface navMesh = _tilesObject.gameObject.AddComponent<NavMeshSurface>();
			navMesh.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
			navMesh.collectObjects = CollectObjects.Children;
        }

		private Tile CreateTile(IntVector2 coordinates, Tile tileSelected)
		{
			if (_map.GetTileType(coordinates) == TileType.Empty)
			{
				_map.SetTileType(coordinates, TileType.Room);
			}
			else
			{
				Debug.LogError("Tile Conflict!");
            }

            Tile newTile = Instantiate(tileSelected);
			newTile.Coordinates = coordinates;
			newTile.name = "Tile " + coordinates.x + ", " + coordinates.z;
			newTile.transform.parent = _tilesObject.transform;
			newTile.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Coordinates.z - Size.z * 0.5f + 0.5f);

            Renderer rendTile = newTile.GetComponentInChildren<Renderer>();
            rendTile.material.SetFloat("_Metallic", 0.0f);
            rendTile.material.SetFloat("_Smoothness", 0.0f);

            Tile roof = Instantiate(tileSelected);
            roof.Coordinates = coordinates;
            roof.name = "Roof " + coordinates.x + ", " + coordinates.z;
            roof.transform.parent = _roofObject.transform;
            roof.transform.localPosition = RoomMapManager.TileSize * new Vector3(coordinates.x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, coordinates.z - Coordinates.z - Size.z * 0.5f + 0.5f);
			roof.transform.GetChild(0).Rotate(new Vector3(180, 0, 0));
			roof.transform.position = new Vector3(roof.transform.position.x, 4.1f, roof.transform.position.z);

            Renderer rendRoof = roof.GetComponentInChildren<Renderer>();
            rendRoof.material.SetFloat("_Metallic", 0.0f);
            rendRoof.material.SetFloat("_Smoothness", 0.0f);

            return newTile;
		}

		public Corridor CreateCorridor(Room otherRoom)
		{
			// Don't create if already connected
			if (RoomCorridor.ContainsKey(otherRoom))
			{
				return RoomCorridor[otherRoom];
			}

			Corridor newCorridor = Instantiate(CorridorPrefab);
			newCorridor.name = "Corridor (" + otherRoom.Num + ", " + Num + ")";
			newCorridor.transform.parent = transform.parent;
			newCorridor.Coordinates = new IntVector2(Coordinates.x + Size.x / 2, otherRoom.Coordinates.z + otherRoom.Size.z / 2);
			newCorridor.transform.localPosition = new Vector3(newCorridor.Coordinates.x - _map.MapSize.x / 2, 0, newCorridor.Coordinates.z - _map.MapSize.z / 2);
			newCorridor.Rooms[0] = otherRoom;
			newCorridor.Rooms[1] = this;
			newCorridor.Length = Vector3.Distance(otherRoom.transform.localPosition, transform.localPosition);
			newCorridor.Init(_map);
			otherRoom.RoomCorridor.Add(this, newCorridor);
			RoomCorridor.Add(otherRoom, newCorridor);

			return newCorridor;
		}

		public IEnumerator CreateWalls()
		{
			_wallsObject = new GameObject("Walls");
			_wallsObject.transform.parent = transform;
			_wallsObject.transform.localPosition = Vector3.zero;

            _doorsObject = new GameObject("Doors");
            _doorsObject.transform.parent = transform;
            _doorsObject.transform.localPosition = Vector3.zero;

            IntVector2 leftBottom = new IntVector2(Coordinates.x - 1, Coordinates.z - 1);
			IntVector2 rightTop = new IntVector2(Coordinates.x + Size.x, Coordinates.z + Size.z);
			for (int x = leftBottom.x; x <= rightTop.x; x++)
			{
				for (int z = leftBottom.z; z <= rightTop.z; z++)
				{
                    // If it's center or corner or not wall
                    if ((x != leftBottom.x && x != rightTop.x && z != leftBottom.z && z != rightTop.z) ||
						((x == leftBottom.x || x == rightTop.x) && (z == leftBottom.z || z == rightTop.z)))
					{
						continue;
					}
					Quaternion rotation = Quaternion.identity;
					if (x == leftBottom.x)
					{
						rotation = MapDirection.West.ToRotation();
					}
					else if (x == rightTop.x)
					{
						rotation = MapDirection.East.ToRotation();
					}
					else if (z == leftBottom.z)
					{
						rotation = MapDirection.South.ToRotation();
					}
					else if (z == rightTop.z)
					{
						rotation = MapDirection.North.ToRotation();
					}
					else
					{
						Debug.LogError("Wall is not on appropriate location!!");
					}

					Transform wallParent;
					GameObject wallPrefab;

					bool isDoor = false;

					if (_map.GetTileType(new IntVector2(x, z)) != TileType.Wall)
					{
						wallParent = _doorsObject.transform;
						wallPrefab = DoorPrefab;
						isDoor = true;
					}
					else
					{
						wallParent = _wallsObject.transform;
						wallPrefab = ChooseWall();
                    }

                    GameObject newWall = Instantiate(wallPrefab);
					newWall.name = "Wall (" + x + ", " + z + ")";
                    newWall.transform.parent = wallParent;
                    newWall.transform.localPosition = RoomMapManager.TileSize * new Vector3(x - Coordinates.x - Size.x * 0.5f + 0.5f, 0f, z - Coordinates.z - Size.z * 0.5f + 0.5f);
					newWall.transform.localRotation = rotation;
					newWall.transform.localScale *= RoomMapManager.TileSize;

                    Renderer rend = newWall.GetComponentInChildren<Renderer>();
                    rend.material.SetFloat("_Metallic", 0.0f);
                    rend.material.SetFloat("_Smoothness", 0.0f);

					newWall.SetActive(!isDoor);
                }
			}
			yield return null;
		}

        private GameObject ChooseWall()
        {
            int totalChance = 0;

            foreach (Wall wall in walls)
            {
                totalChance += wall.chance;
            }

            int randomValue = Random.Range(0, totalChance);

            foreach (Wall wall in walls)
            {
                if (randomValue < wall.chance)
                {
                    return wall.obj;
                }

                randomValue -= wall.chance;
            }

            return walls[walls.Length - 1].obj;
        }

        public IEnumerator CreatePlayer()
		{
			GameObject player = Instantiate((PlayerPrefab));
			player.name = "Player";
			player.transform.parent = transform.parent;
			player.transform.localPosition = transform.localPosition;
			hasPlayer = true;
			yield return null;
		}
	}
}

[System.Serializable]
public class Wall
{
	[Range(0, 100)]public int chance;
	public GameObject obj;
}