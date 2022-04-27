using LiminalDungeonGeneration;
using MeshBuilderLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiminalDungeonGenerator : MonoBehaviour
{
    // Constants
    public const float CONNECTION_LENGTH = 0.2f;
    public const float CONNECTION_WIDTH = 1.2f;
    public const float CONNECTION_HEIGHT = 2f;

    public const float EXIT_POINT_CHANCE_PER_WALL = 0.35f;
    public const float MIN_WALL_LENGTH_FOR_EXIT_POINT = 2.5f;

    public const string COLLISION_LAYER = "PlayerCollision";

    private const int MAX_FAILED_ATTEMPTS = 40;
    private const int MAX_MODULES = 50;

    public const float FLOOR_TEXTURE_SCALING = 0.1f;
    public const float WALL_TEXTURE_SCALING = 0.2f;

    // Map objects
    private GameObject DungeonObject;
    public PlayerController Player;
    public List<DungeonModule> Modules = new List<DungeonModule>();
    public List<Gate> Gates = new List<Gate>();

    // Algorithm flow data
    enum AlgorithmState
    {
        Start,
        AddModule,
        ChoseExitPoints,
        AlignModule,
        RotateModule,
        ApplyModule,
        BuildConnection,
        DestroyModule,
        Finalize,
        Done
    }
    private AlgorithmState State;
    public bool AutoRunAlgorithm;
    public int UnsuccessfulAttempts = 0;
    public List<ExitPoint> UnconnectedExitPoints = new List<ExitPoint>();
    private DungeonModule TempModule;
    private ExitPoint TempModuleExitPoint;
    private ExitPoint TempDungeonExitPoint;

    // Population tables
    private enum ModuleType
    {
        DefaultRoom,
        Corridor,
        Hall
    }
    private Dictionary<ModuleType, int> ModuleTable = new Dictionary<ModuleType, int>()
    {
        { ModuleType.DefaultRoom, 100 },
        { ModuleType.Corridor, 100 },
        { ModuleType.Hall, 25 },
    };
    private ModuleType NextModuleType;

    void Start()
    {
        Player.GetComponent<PlayerController>().enabled = false;
        State = AlgorithmState.Start;
    }

    void Update()
    {
        if (AutoRunAlgorithm && State != AlgorithmState.Done) PerformNextAlgorithmStep();

        if (!AutoRunAlgorithm && Input.GetKeyDown(KeyCode.Space))
        {
            PerformNextAlgorithmStep();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            AutoRunAlgorithm = !AutoRunAlgorithm;
        }
    }

    private void PerformNextAlgorithmStep()
    {
        if(State == AlgorithmState.Start)
        {
            AddStartRoom();
            State = AlgorithmState.AddModule;
        }

        else if(State == AlgorithmState.AddModule)
        {
            TempModule = AddRandomModule();
            State = AlgorithmState.ChoseExitPoints;
        }

        else if (State == AlgorithmState.ChoseExitPoints)
        {
            ChoseConnectionExitPoints(TempModule);
            State = AlgorithmState.AlignModule;
        }

        else if (State == AlgorithmState.AlignModule)
        {
            AlignModule(TempModule, TempModuleExitPoint, TempDungeonExitPoint);
            State = AlgorithmState.RotateModule;
        }

        else if (State == AlgorithmState.RotateModule)
        {
            RotateModule(TempModule, TempModuleExitPoint, TempDungeonExitPoint);
            if(DoesModuleCollideWithDungeon(TempModule))
            {
                // Module collides with dungeon - destroying module
                State = AlgorithmState.DestroyModule;
            }
            else
            {
                // Module does not collide with dungeon - applying module
                State = AlgorithmState.ApplyModule;
            }
        }

        else if (State == AlgorithmState.ApplyModule)
        {
            ApplyModule(TempModule, TempModuleExitPoint, TempDungeonExitPoint);
            State = AlgorithmState.BuildConnection;
        }

        else if(State == AlgorithmState.BuildConnection)
        {
            BuildConnection(TempModule, TempModuleExitPoint, TempDungeonExitPoint);
            if (UnconnectedExitPoints.Count == 0 || Modules.Count >= MAX_MODULES) State = AlgorithmState.Finalize; // Stop when there are no more unconnected exit points
            else State = AlgorithmState.AddModule;
        }

        else if (State == AlgorithmState.DestroyModule)
        {
            DestroyModule(TempModule);
            if (UnsuccessfulAttempts > MAX_FAILED_ATTEMPTS) State = AlgorithmState.Finalize; // Stop when too many attempts to add a module failed
            else State = AlgorithmState.AddModule;
        }

        else if(State == AlgorithmState.Finalize)
        {
            FinalizeDungeon();
            State = AlgorithmState.Done;
        }

    }

    #region Algorithm steps

    private void AddStartRoom()
    {
        DungeonObject = new GameObject("Map");

        DungeonModule startModule = GetRandomModule(ModuleType.DefaultRoom);
        Modules.Add(startModule);
        startModule.transform.SetParent(DungeonObject.transform);
        UnconnectedExitPoints.AddRange(startModule.ExitPoints);
        NextModuleType = HelperFunctions.GetWeightedRandomElement<ModuleType>(ModuleTable);

        // Debug exit points
        foreach (ExitPoint exitPoint in UnconnectedExitPoints) DebugExitPoint(exitPoint, 2f, Color.green, 5f);
    }

    private DungeonModule AddRandomModule()
    {
        DungeonModule newModule = GetRandomModule(NextModuleType);
        newModule.transform.position += new Vector3(20, 0, 20);

        // Debug exit points
        foreach (ExitPoint exitPoint in newModule.ExitPoints) DebugExitPoint(exitPoint, 2f, Color.green, 2f);
        // Debug available points
        foreach (ExitPoint exitPoint in UnconnectedExitPoints) DebugExitPoint(exitPoint, 2f, Color.green, 2f);

        return newModule;
    }

    private void ChoseConnectionExitPoints(DungeonModule module)
    {
        // Take a random exit point on the module
        TempModuleExitPoint = module.ExitPoints[Random.Range(0, module.ExitPoints.Count)];
        DebugExitPoint(TempModuleExitPoint, 2f, Color.red, 2f);

        // Take a random unconnecte exit point of the dungeon
        TempDungeonExitPoint = UnconnectedExitPoints[Random.Range(0, UnconnectedExitPoints.Count)];
        DebugExitPoint(TempDungeonExitPoint, 2f, Color.red, 2f);
    }

    private void AlignModule(DungeonModule module, ExitPoint moduleExitPoint, ExitPoint dungeonExitPoint)
    {
        // Align the module so that the exit points overlap
        Vector3 exitPointOffset = dungeonExitPoint.GetForwardPosition(CONNECTION_LENGTH) - moduleExitPoint.GetWorldPosition();
        module.transform.position += exitPointOffset;
    }

    private void RotateModule(DungeonModule module, ExitPoint moduleExitPoint, ExitPoint dungeonExitPoint)
    {
        // Rotate the module to the point that the exit points face each other (angle difference = 180);
        float angleDifference = moduleExitPoint.GetWorldDirection() - dungeonExitPoint.GetWorldDirection();
        float rotationAngle = 180 - angleDifference;
        module.transform.RotateAround(dungeonExitPoint.GetForwardPosition(CONNECTION_LENGTH), Vector3.up, rotationAngle);
    }

    private void ApplyModule(DungeonModule module, ExitPoint moduleExitPoint, ExitPoint dungeonExitPoint)
    {
        Modules.Add(module);
        module.transform.SetParent(DungeonObject.transform);
        UnconnectedExitPoints.AddRange(module.ExitPoints);
        UnconnectedExitPoints.Remove(moduleExitPoint);
        UnconnectedExitPoints.Remove(dungeonExitPoint);
        UnsuccessfulAttempts = 0;
        NextModuleType = HelperFunctions.GetWeightedRandomElement<ModuleType>(ModuleTable);
    }

    private void BuildConnection(DungeonModule module, ExitPoint moduleExitPoint, ExitPoint dungeonExitPoint)
    {
        // Carve holes in walls in both modules
        module.OpenExitPoint(moduleExitPoint);
        dungeonExitPoint.Module.OpenExitPoint(dungeonExitPoint);

        // Create a connection passage between the modules
        ExitPoint ex1 = moduleExitPoint;
        ExitPoint ex2 = dungeonExitPoint;
        Vector3 bl1 = ex1.GetOffsetPosition(- CONNECTION_WIDTH / 2, 0f);
        Vector3 br1 = ex1.GetOffsetPosition(CONNECTION_WIDTH / 2, 0f);
        Vector3 tl1 = ex1.GetOffsetPosition(-CONNECTION_WIDTH / 2, CONNECTION_HEIGHT);
        Vector3 tr1 = ex1.GetOffsetPosition(CONNECTION_WIDTH / 2, CONNECTION_HEIGHT);
        Vector3 bl2 = ex2.GetOffsetPosition(-CONNECTION_WIDTH / 2, 0f);
        Vector3 br2 = ex2.GetOffsetPosition(CONNECTION_WIDTH / 2, 0f);
        Vector3 tl2 = ex2.GetOffsetPosition(-CONNECTION_WIDTH / 2, CONNECTION_HEIGHT);
        Vector3 tr2 = ex2.GetOffsetPosition(CONNECTION_WIDTH / 2, CONNECTION_HEIGHT);

        MeshBuilder gateBuilder = new MeshBuilder("gate_" + Modules.Count, COLLISION_LAYER);
        int gateSubmeshIndex = gateBuilder.AddNewSubmesh(MaterialHandler.Instance.WoodSiding);
        
        gateBuilder.BuildPlane(gateSubmeshIndex, bl1, br1, bl2, br2, Vector2.zero, Vector2.one); // Floor
        gateBuilder.BuildPlane(gateSubmeshIndex, bl1, br2, tr2, tl1, Vector2.zero, Vector2.one); // Right wall
        gateBuilder.BuildPlane(gateSubmeshIndex, bl2, br1, tr1, tl2, Vector2.zero, Vector2.one); // Left wall
        gateBuilder.BuildPlane(gateSubmeshIndex, tr1, tl1, tr2, tl2, Vector2.zero, Vector2.one); // Ceiling

        GameObject gateObject = gateBuilder.ApplyMesh(addCollider: true);
        Gate gate = gateObject.AddComponent<Gate>();
        gate.Init(dungeonExitPoint, moduleExitPoint);
        Gates.Add(gate);
        gate.transform.SetParent(DungeonObject.transform);
    }

    private void DestroyModule(DungeonModule module)
    {
        Destroy(module.gameObject);
        UnsuccessfulAttempts++;
    }

    private void FinalizeDungeon()
    {
        // Add a huge ceiling over everything to fix lighting bug
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(0f, 30f, 0f);
        plane.transform.localScale = new Vector3(100, 0f, 100);

        Player.GetComponent<PlayerController>().enabled = true;
    }

    #endregion

    private DungeonModule GetRandomModule(ModuleType type)
    {
        if(type == ModuleType.DefaultRoom) return DefaultRoomGenerator.GenerateRandomRoom();
        if(type == ModuleType.Corridor) return CorridorGenerator.GenerateRandomCorridor();
        if(type == ModuleType.Hall) return HallGenerator.GenerateRandomHall();
        throw new System.Exception("moduletype not mapped");
    }

    private bool DoesModuleCollideWithDungeon(DungeonModule module) // TODO: Consider module heights
    {
        foreach (DungeonModule dungeonModule in Modules)
        {
            // If the rooms are vertically apart, they can't collide
            if (module.transform.position.y > dungeonModule.transform.position.y + dungeonModule.Height ||
               dungeonModule.transform.position.y > module.transform.position.y + module.Height) continue;

            // Check collision between all walls of the dungeon with all walls of the module
            for (int i = 0; i < dungeonModule.Bounds.Points.Count; i++)
            {
                Vector2 pointP1 = dungeonModule.Bounds.GetTransformedPoint(i, dungeonModule.transform);
                Vector2 nextPointP1 = i < dungeonModule.Bounds.Points.Count - 1 ? dungeonModule.Bounds.GetTransformedPoint(i + 1, dungeonModule.transform) : dungeonModule.Bounds.GetTransformedPoint(0, dungeonModule.transform);
                Debug.DrawLine(new Vector3(pointP1.x, 0f, pointP1.y), new Vector3(nextPointP1.x, 0f, nextPointP1.y), Color.blue, 2f);

                for (int j = 0; j < module.Bounds.Points.Count; j++)
                {
                    Vector2 pointP2 = module.Bounds.GetTransformedPoint(j, module.transform);
                    Vector2 nextPointP2 = j < module.Bounds.Points.Count - 1 ? module.Bounds.GetTransformedPoint(j + 1, module.transform) : module.Bounds.GetTransformedPoint(0, module.transform);
                    Debug.DrawLine(new Vector3(pointP2.x, 0f, pointP2.y), new Vector3(nextPointP2.x, 0f, nextPointP2.y), Color.yellow, 2f);

                    if (HelperFunctions.DoLineSegmentsIntersect(pointP1, nextPointP1, pointP2, nextPointP2)) return true;
                }
            }
        }
        return false;
    }
    private void DebugExitPoint(ExitPoint exitPoint, float length, Color color, float duration)
    {

        Debug.DrawLine(exitPoint.GetWorldPosition(), exitPoint.GetForwardPosition(length), color, duration);
    }
}
