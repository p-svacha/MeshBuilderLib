using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeshBuilderLib
{
    public class MeshBuilder
    {
        /// <summary>
        /// The GameObject the MeshBuilder is working on.
        /// </summary>
        private GameObject MeshObject;

        /// <summary>
        /// Vertex data of the mesh. Contains position and uv for all vertices. Vertex data is shared across submeshes.
        /// </summary>
        private List<MeshVertex> Vertices = new List<MeshVertex>();

        /// <summary>
        /// Triangle data of the mesh. The index of the outer list represents the index of the submesh that the triangles in the inner list are part of.
        /// </summary>
        private List<List<MeshTriangle>> Triangles = new List<List<MeshTriangle>>();

        /// <summary>
        /// The index of the submesh that new triangles are added to (if not specified otherwise).
        /// </summary>
        private int CurrentSubmesh = -1;

        /// <summary>
        /// Material data for the mesh. The index in the list represents the index of the submesh that the material is applied to.
        /// </summary>
        private List<Material> Materials = new List<Material>();

        #region Instancing

        /// <summary>
        /// Create a MeshBuilder for a new GameObject. Does not creat any submesh.
        /// </summary>
        public MeshBuilder(string name, string layer, Transform parent = null)
        {
            MeshObject = new GameObject(name);
            MeshObject.layer = LayerMask.NameToLayer(layer);
            if (parent != null) MeshObject.transform.SetParent(parent);
            MeshObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = MeshObject.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }

        /// <summary>
        /// Create a MeshBuilder for an existing object. Used to add/modify a mesh. Material is given for the first submesh. Does not creat any submesh.
        /// </summary>
        public MeshBuilder(GameObject meshObject)
        {
            MeshObject = meshObject;
        }

        #endregion

        #region Applying

        /// <summary>
        /// Applies all mesh data to the MeshFilter of and all material data to the MeshRenderer of the MeshObject GameObject.
        /// If addCollider is set, a MeshCollider will be updated/added to the object.
        /// If applyInEditor is set, then the shared attributes of the mesh will be set, meaning it will show up in the editor.
        /// </summary>
        public GameObject ApplyMesh(bool addCollider = false, bool applyInEditor = false)
        {
            // Set index values for all vertices
            for (int i = 0; i < Vertices.Count; i++) Vertices[i].Id = i;

            MeshFilter meshFilter = MeshObject.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = MeshObject.GetComponent<MeshRenderer>();

            Mesh targetMesh = applyInEditor ? meshFilter.sharedMesh : meshFilter.mesh;
            targetMesh.Clear();
            targetMesh.SetVertices(Vertices.Select(x => x.Position).ToArray()); // Set the vertices
            targetMesh.SetUVs(0, Vertices.Select(x => x.UV).ToArray()); // Set the UV's
            targetMesh.SetUVs(1, Vertices.Select(x => x.UV2).ToArray()); // Set the UV's
            targetMesh.subMeshCount = Triangles.Count; // Set the submesh count
            for (int i = 0; i < Triangles.Count; i++) // Set the triangles for each submesh
            {
                List<int> triangles = new List<int>();
                foreach (MeshTriangle triangle in Triangles[i])
                {
                    triangles.Add(triangle.Vertex1.Id);
                    triangles.Add(triangle.Vertex2.Id);
                    triangles.Add(triangle.Vertex3.Id);
                }
                targetMesh.SetTriangles(triangles, i);
            }
            targetMesh.Optimize();
            targetMesh.RecalculateNormals();

            // Set the material for each submesh
            if (applyInEditor) meshRenderer.sharedMaterials = Materials.ToArray(); 
            else meshRenderer.materials = Materials.ToArray();

            // Collider
            if(addCollider)
            {
                MeshCollider meshCollider = MeshObject.GetComponent<MeshCollider>();
                if (meshCollider != null) GameObject.Destroy(meshCollider);
                MeshObject.AddComponent<MeshCollider>();
            }

            return MeshObject;
        }

        #endregion

        #region Basic Building

        /// <summary>
        /// Adds a new submesh to the mesh with the specified Material. Returns the index of the new submesh.
        /// </summary>
        public int AddNewSubmesh(Material material)
        {
            Triangles.Add(new List<MeshTriangle>());
            CurrentSubmesh++;
            Materials.Add(material);
            return CurrentSubmesh;
        }

        /// <summary>
        /// Add a new vertex given its position and uv(s) data to the mesh.
        /// </summary>
        public MeshVertex AddVertex(Vector3 position, Vector2 uv, Vector2? uv2 = null)
        {
            MeshVertex vertex = new MeshVertex(position, uv, uv2);
            Vertices.Add(vertex);
            return vertex;
        }

        /// <summary>
        /// Add a MeshVertex to the mesh.
        /// </summary>
        public void AddVertex(MeshVertex meshVertex)
        {
            Vertices.Add(meshVertex);
        }

        /// <summary>
        /// Remove a specific vertex from the mesh.
        /// </summary>
        public void RemoveVertex(MeshVertex meshVertex)
        {
            Vertices.Remove(meshVertex);
        }

        /// <summary>
        /// Add a triangle to the mesh given the submesh index and reference of the 3 MeshVertices.
        /// </summary>
        public MeshTriangle AddTriangle(int submeshIndex, MeshVertex vertex1, MeshVertex vertex2, MeshVertex vertex3)
        {
            MeshTriangle triangle = new MeshTriangle(submeshIndex, vertex1, vertex2, vertex3);
            Triangles[submeshIndex].Add(triangle);
            return triangle;
        }

        /// <summary>
        /// Removes a triangle from a submesh. Does not remove the associated vertices.
        /// </summary>
        public void RemoveTriangle(MeshTriangle triangle)
        {
            Triangles[triangle.SubmeshIndex].Remove(triangle);
        }

        /// <summary>
        /// Adds triangles for a plane to a submesh. Order of vertices must be clockwise. Returns the created triangles a list.
        /// </summary>
        public List<MeshTriangle> AddPlane(int submeshIndex, MeshVertex v1, MeshVertex v2, MeshVertex v3, MeshVertex v4)
        {
            MeshTriangle t1 = AddTriangle(submeshIndex, v1, v3, v2);
            MeshTriangle t2 = AddTriangle(submeshIndex, v1, v4, v3);
            return new List<MeshTriangle>() { t1, t2 };
        }

        /// <summary>
        /// Removes all triangles and vertices of a plane from a submesh.
        /// </summary>
        public void RemovePlane(int submeshIndex, MeshPlane plane)
        {
            Vertices.Remove(plane.Vertex1);
            Vertices.Remove(plane.Vertex2);
            Vertices.Remove(plane.Vertex3);
            Vertices.Remove(plane.Vertex4);
            RemoveTriangle(plane.Triangle1);
            RemoveTriangle(plane.Triangle2);
        }

        /// <summary>
        /// Adds all MeshVertices and MeshTriangles to build a plane. Returns a MeshPlane containing all data.
        /// UV from first to second vector is uv-y-axis
        /// </summary>
        public MeshPlane BuildPlane(int submeshIndex, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector2 uvStart, Vector2 uvEnd)
        {
            MeshVertex mv1 = AddVertex(v1, uvStart);
            MeshVertex mv2 = AddVertex(v2, new Vector2(uvStart.x, uvEnd.y));
            MeshVertex mv3 = AddVertex(v3, uvEnd);
            MeshVertex mv4 = AddVertex(v4, new Vector2(uvEnd.x, uvStart.y));

            MeshTriangle tri1 = AddTriangle(submeshIndex, mv1, mv3, mv2);
            MeshTriangle tri2 = AddTriangle(submeshIndex, mv1, mv4, mv3);

            return new MeshPlane(mv1, mv2, mv3, mv4, tri1, tri2);
        }

        /// <summary>
        /// Create a MeshPlane out of 4 MeshVertices that already contain all UV data.
        /// </summary>
        public MeshPlane BuildPlane(int submeshIndex, MeshVertex mv1, MeshVertex mv2, MeshVertex mv3, MeshVertex mv4)
        {
            MeshTriangle tri1 = AddTriangle(submeshIndex, mv1, mv3, mv2);
            MeshTriangle tri2 = AddTriangle(submeshIndex, mv1, mv4, mv3);

            return new MeshPlane(mv1, mv2, mv3, mv4, tri1, tri2);
        }

        /// <summary>
        /// Builds a flat circle (y-axis) on the specified submesh and returns created vertices and triangles as a MeshElement.
        /// </summary>
        public MeshElement BuildCircle(int submeshIndex, Vector3 centerPosition, float radius, int nEdges, bool flipFaceDirection = false)
        {
            List<MeshVertex> vertices = new List<MeshVertex>();
            List<MeshTriangle> triangles = new List<MeshTriangle>();

            MeshVertex centerPoint = AddVertex(Vector3.zero, Vector2.zero);
            vertices.Add(centerPoint);
            MeshVertex firstOuterPoint = null, prevOuterPoint = null, currentOuterPoint = null;

            float angleStep = 360f / nEdges;
            for(int i = 0; i < nEdges; i++)
            {
                float angle = i * angleStep;
                float x = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                float y = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                prevOuterPoint = currentOuterPoint;
                currentOuterPoint = AddVertex(new Vector3(x, 0f, y), new Vector2(x, y));
                vertices.Add(currentOuterPoint);

                if (i == 0) firstOuterPoint = currentOuterPoint;
                else triangles.Add(flipFaceDirection ? AddTriangle(submeshIndex, centerPoint, currentOuterPoint, prevOuterPoint) : AddTriangle(submeshIndex, centerPoint, prevOuterPoint, currentOuterPoint));

                if (i == nEdges - 1) triangles.Add(flipFaceDirection ? AddTriangle(submeshIndex, centerPoint, firstOuterPoint, currentOuterPoint) : AddTriangle(submeshIndex, centerPoint, currentOuterPoint, firstOuterPoint));
            }

            foreach (MeshVertex v in vertices) v.Position += centerPosition;

            return new MeshElement(submeshIndex, vertices, triangles);
        }

        /// <summary>
        /// Adds all MeshVertices and MeshTriangles to build a sphere on the specified submesh and returns them as a MeshElement.
        /// nRows and nCols define the level of detail of the sphere.
        /// </summary>
        public MeshElement BuildSphere(int submeshIndex, Vector3 centerPosition, float width, float height, int nRows, int nCols)
        {
            List<MeshVertex> vertices = new List<MeshVertex>();
            int numVertices = (nRows + 1) * (nCols + 1);
            for (int i = 0; i < numVertices; i++)
            {
                float x = i % (nCols + 1);
                float y = i / (nCols + 1);
                float x_pos = x / nCols;
                float y_pos = y / nRows;
                Vector3 vertexPos = new Vector3(x_pos, y_pos, 0);

                float u = x / nCols;
                float v = y / nRows;
                Vector2 vertexUv = new Vector2(u, v);

                vertices.Add(AddVertex(vertexPos, vertexUv));
            }

            List<MeshTriangle> triangles = new List<MeshTriangle>();
            int numTriangles = 2 * nRows * nCols;
            for (int i = 0; i < numTriangles; i++)
            {
                int[] triIndex = new int[3];
                if (i % 2 == 0)
                {
                    triIndex[0] = i / 2 + i / (2 * nCols);
                    triIndex[1] = triIndex[0] + 1;
                    triIndex[2] = triIndex[0] + (nCols + 1);
                }
                else
                {
                    triIndex[0] = (i + 1) / 2 + i / (2 * nCols);
                    triIndex[1] = triIndex[0] + (nCols + 1);
                    triIndex[2] = triIndex[1] - 1;

                }
                triangles.Add(AddTriangle(submeshIndex, vertices[triIndex[0]], vertices[triIndex[2]], vertices[triIndex[1]]));
            }

            for (int i = 0; i < numVertices; i++)
            {
                Vector3 spherePos;
                spherePos.x = width * Mathf.Cos(vertices[i].Position.x * 2 * Mathf.PI) * Mathf.Cos(vertices[i].Position.y * Mathf.PI - Mathf.PI / 2);
                spherePos.y = height * Mathf.Sin(vertices[i].Position.y * Mathf.PI - Mathf.PI / 2);
                spherePos.z = width * Mathf.Sin(vertices[i].Position.x * 2 * Mathf.PI) * Mathf.Cos(vertices[i].Position.y * Mathf.PI - Mathf.PI / 2);

                vertices[i].Position = spherePos;
            }

            foreach (MeshVertex v in vertices) v.Position += centerPosition;

            return new MeshElement(submeshIndex, vertices, triangles);
        }

        /// <summary>
        /// Adds all MeshVertices and MeshTriangles to build a regular cylinder on the specified submesh and returns them as a MeshElement.
        /// </summary>
        public MeshElement BuildCylinder(int submeshIndex, Vector3 bottomCenterPosition, float radius, float height, int nEdges)
        {
            List<MeshVertex> vertices = new List<MeshVertex>();
            List<MeshTriangle> triangles = new List<MeshTriangle>();

            // Top and Bottom flat circle
            MeshElement botCircle = BuildCircle(submeshIndex, bottomCenterPosition, radius, nEdges, flipFaceDirection: true);
            MeshElement topCircle = BuildCircle(submeshIndex, bottomCenterPosition + new Vector3(0f, height, 0f), radius, nEdges);

            vertices.AddRange(botCircle.Vertices);
            vertices.AddRange(topCircle.Vertices);
            triangles.AddRange(botCircle.Triangles);
            triangles.AddRange(topCircle.Triangles);

            // Walls
            Vector3 firstBotPoint = Vector3.zero, firstTopPoint = Vector3.zero, prevBotPoint = Vector3.zero, prevTopPoint = Vector3.zero, currentBotPoint = Vector3.zero, currentTopPoint = Vector3.zero;

            float angleStep = 360f / nEdges;
            for (int i = 0; i < nEdges; i++)
            {
                float angle = i * angleStep;
                float x = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                float y = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                prevBotPoint = currentBotPoint;
                prevTopPoint = currentTopPoint;

                currentBotPoint = bottomCenterPosition + new Vector3(x, 0f, y);
                currentTopPoint = bottomCenterPosition + new Vector3(x, height, y);

                if (i == 0)
                {
                    firstBotPoint = currentBotPoint;
                    firstTopPoint = currentTopPoint;
                }
                else
                {
                    MeshPlane wall = BuildPlane(submeshIndex, prevBotPoint, prevTopPoint, currentTopPoint, currentBotPoint, new Vector2(0f, 0f), new Vector2(1f, height));
                    vertices.Add(wall.Vertex1); vertices.Add(wall.Vertex2); vertices.Add(wall.Vertex3); vertices.Add(wall.Vertex4);
                    triangles.Add(wall.Triangle1); triangles.Add(wall.Triangle2);
                }

                if (i == nEdges - 1) 
                {
                    MeshPlane wall = BuildPlane(submeshIndex, currentBotPoint, currentTopPoint, firstTopPoint, firstBotPoint, new Vector2(0f, 0f), new Vector2(1f, height));
                    vertices.Add(wall.Vertex1); vertices.Add(wall.Vertex2); vertices.Add(wall.Vertex3); vertices.Add(wall.Vertex4);
                    triangles.Add(wall.Triangle1); triangles.Add(wall.Triangle2);
                }
            }

            return new MeshElement(submeshIndex, vertices, triangles);
        }

        /// <summary>
        /// Builds a cylinder consisting of different segments that can have their own radius and height and adds it to the specified submesh.
        /// Returns all created vertices and triangles as a MeshElement.
        /// </summary>
        public MeshElement BuildSegmentedCylinder(int submeshIndex, Vector3 bottomCenterPosition, List<float> stepRadii, List<float> stepHeights, int nEdges)
        {
            List<MeshVertex> vertices = new List<MeshVertex>();
            List<MeshTriangle> triangles = new List<MeshTriangle>();

            // Top and Bottom flat circle
            MeshElement botCircle = BuildCircle(submeshIndex, bottomCenterPosition, stepRadii[0], nEdges, flipFaceDirection: true);
            MeshElement topCircle = BuildCircle(submeshIndex, bottomCenterPosition + new Vector3(0f, stepHeights.Sum(), 0f), stepRadii.Last(), nEdges);

            vertices.AddRange(botCircle.Vertices);
            vertices.AddRange(topCircle.Vertices);
            triangles.AddRange(botCircle.Triangles);
            triangles.AddRange(topCircle.Triangles);

            // Segment walls
            int nSegments = stepHeights.Count;
            float angleStep = 360f / nEdges;
            float currentHeight = 0f;
            for (int s = 0; s < nSegments; s++)
            {
                Vector3 firstBotPoint = Vector3.zero, firstTopPoint = Vector3.zero, prevBotPoint = Vector3.zero, prevTopPoint = Vector3.zero, currentBotPoint = Vector3.zero, currentTopPoint = Vector3.zero;
                for (int i = 0; i < nEdges; i++)
                {
                    float angle = i * angleStep;
                    float botX = stepRadii[s] * Mathf.Sin(Mathf.Deg2Rad * angle);
                    float botY = stepRadii[s] * Mathf.Cos(Mathf.Deg2Rad * angle);
                    float topX = stepRadii[s + 1] * Mathf.Sin(Mathf.Deg2Rad * angle);
                    float topY = stepRadii[s + 1] * Mathf.Cos(Mathf.Deg2Rad * angle);
                    prevBotPoint = currentBotPoint;
                    prevTopPoint = currentTopPoint;

                    currentBotPoint = bottomCenterPosition + new Vector3(botX, currentHeight, botY);
                    currentTopPoint = bottomCenterPosition + new Vector3(topX, currentHeight + stepHeights[s], topY);

                    if (i == 0)
                    {
                        firstBotPoint = currentBotPoint;
                        firstTopPoint = currentTopPoint;
                    }
                    else
                    {
                        MeshPlane wall = BuildPlane(submeshIndex, prevBotPoint, prevTopPoint, currentTopPoint, currentBotPoint, new Vector2(0f, currentHeight), new Vector2(1f, currentHeight + stepHeights[s]));
                        vertices.Add(wall.Vertex1); vertices.Add(wall.Vertex2); vertices.Add(wall.Vertex3); vertices.Add(wall.Vertex4);
                        triangles.Add(wall.Triangle1); triangles.Add(wall.Triangle2);
                    }

                    if (i == nEdges - 1)
                    {
                        MeshPlane wall = BuildPlane(submeshIndex, currentBotPoint, currentTopPoint, firstTopPoint, firstBotPoint, new Vector2(0f, currentHeight), new Vector2(1f, currentHeight + stepHeights[s]));
                        vertices.Add(wall.Vertex1); vertices.Add(wall.Vertex2); vertices.Add(wall.Vertex3); vertices.Add(wall.Vertex4);
                        triangles.Add(wall.Triangle1); triangles.Add(wall.Triangle2);
                    }
                }

                currentHeight += stepHeights[s];
            }

            return new MeshElement(submeshIndex, vertices, triangles);
        }

        /// <summary>
        /// Builds and returns a 2-dimensional polygon, given it's layout and altitude. MeshPolygons are always flat on the y-axis.
        /// </summary>
        public MeshElement BuildPolygon(int submeshIndex, Polygon polygon, float altitude, float uvScaling = 1f, bool flipFaceDirection = false)
        {
            List<MeshVertex> vertices = new List<MeshVertex>();
            List<MeshTriangle> triangles = new List<MeshTriangle>();
            List<Vector2> uvs = polygon.GetUVs(uvScaling, flipFaceDirection);
            for (int i = 0; i < polygon.NumPoints; i++)
            {
                vertices.Add(AddVertex(new Vector3(polygon.Points[i].x, altitude, polygon.Points[i].y), uvs[i]));
            }
            int[] vIds = PolygonTriangulator.Triangulate(polygon, flipFaceDirection);
            for (int i = 0; i < vIds.Length; i += 3)
            {
                triangles.Add(AddTriangle(submeshIndex, vertices[vIds[i]], vertices[vIds[i + 1]], vertices[vIds[i + 2]]));
            }
            return new MeshElement(submeshIndex, vertices, triangles);
        }

        /// <summary>
        /// Builds and returns a room given its ground plan and height.
        /// </summary>
        public MeshRoom BuildRoom(Polygon groundPlan, float height, Material floorMat, Material wallMat, Material ceilMat, float floorCeilUvScaling = 1f, float wallUvScaling = 1f)
        {
            // Floor
            int floorSubmeshIndex = AddNewSubmesh(floorMat);
            MeshElement floor = BuildPolygon(floorSubmeshIndex, groundPlan, 0f, uvScaling: floorCeilUvScaling);

            // Ceiling
            int ceilingSubmeshIndex = AddNewSubmesh(wallMat);
            MeshElement ceiling = BuildPolygon(ceilingSubmeshIndex, groundPlan, height, uvScaling: floorCeilUvScaling, flipFaceDirection: true);

            // Walls and exit points
            int wallSubmeshIndex = AddNewSubmesh(ceilMat);
            List<MeshPlane> walls = new List<MeshPlane>();
            float uvStart = 0f;
            float uvEnd = 0f;
            for (int i = 0; i < groundPlan.Points.Count; i++)
            {
                Vector2 point = groundPlan.Points[i];
                Vector2 nextPoint = i < groundPlan.Points.Count - 1 ? groundPlan.Points[i + 1] : groundPlan.Points[0];

                uvEnd -= Vector2.Distance(point, nextPoint);

                walls.Add(BuildPlane(wallSubmeshIndex,
                    AddVertex(new Vector3(point.x, 0, point.y), new Vector2(uvStart * wallUvScaling, 0)),
                    AddVertex(new Vector3(point.x, height, point.y), new Vector2(uvStart * wallUvScaling, height * wallUvScaling)),
                    AddVertex(new Vector3(nextPoint.x, height, nextPoint.y), new Vector2(uvEnd * wallUvScaling, height * wallUvScaling)),
                    AddVertex(new Vector3(nextPoint.x, 0, nextPoint.y), new Vector2(uvEnd * wallUvScaling, 0))
                    ));

                uvStart = uvEnd;
            }

            return new MeshRoom(floor, ceiling, walls, floorSubmeshIndex, ceilingSubmeshIndex, wallSubmeshIndex);
        }

        #endregion

        #region Complex Building

        /// <summary>
        /// Carves a hole into a plane. Only works correctly for rectangular planes at the moment. The hole position is the center.
        /// </summary>
        public void CarveHoleInPlane(int submeshIndex, MeshPlane plane, Vector2 holePosition, Vector2 holeDimensions)
        {
            // Remove the wall that contains the hole
            RemovePlane(submeshIndex, plane);

            // Add new vertices on the sides of the hole
            Vector3 planeVectorX = plane.Vertex4.Position - plane.Vertex1.Position;
            float planeLengthX = planeVectorX.magnitude;
            float relHoleWidth = holeDimensions.x / planeLengthX;
            float relativeHolePositionX = holePosition.x / planeLengthX;
            float xStart = relativeHolePositionX - relHoleWidth / 2;
            float xEnd = relativeHolePositionX + relHoleWidth / 2;

            Vector3 planeVectorY = plane.Vertex2.Position - plane.Vertex1.Position;
            float planeLengthY = planeVectorY.magnitude;
            float relHoleHeight = holeDimensions.y / planeLengthY;
            float relativeHolePositionY = holePosition.y / planeLengthY;
            float yStart = relativeHolePositionY - relHoleHeight / 2;
            float yEnd = relativeHolePositionY + relHoleHeight / 2;

            float uvVectorX = plane.Vertex4.UV.x - plane.Vertex1.UV.x;
            float uvStartX = plane.Vertex1.UV.x + xStart * uvVectorX;
            float uvEndX = plane.Vertex1.UV.x + xEnd * uvVectorX;

            float uvVectorY = plane.Vertex2.UV.y - plane.Vertex1.UV.y;
            float uvStartY = plane.Vertex1.UV.y + yStart * uvVectorY;
            float uvEndY = plane.Vertex1.UV.y + yEnd * uvVectorY;

            Vector3 pv1 = plane.Vertex1.Position;
            Vector3 pv2 = plane.Vertex2.Position;
            Vector3 pv3 = plane.Vertex3.Position;
            Vector3 pv4 = plane.Vertex4.Position;

            Vector3 sb1 = plane.Vertex1.Position + xStart * planeVectorX;
            Vector3 st1 = plane.Vertex2.Position + xStart * planeVectorX;
            Vector3 st2 = plane.Vertex2.Position + xEnd * planeVectorX;
            Vector3 sb2 = plane.Vertex1.Position + xEnd * planeVectorX;

            Vector3 hb1 = plane.Vertex1.Position + xStart * planeVectorX + yStart * planeVectorY;
            Vector3 ht1 = plane.Vertex1.Position + xStart * planeVectorX + yEnd * planeVectorY;
            Vector3 ht2 = plane.Vertex1.Position + xEnd * planeVectorX + yEnd * planeVectorY;
            Vector3 hb2 = plane.Vertex1.Position + xEnd * planeVectorX + yStart * planeVectorY;

            BuildPlane(submeshIndex, pv1, pv2, st1, sb1, plane.Vertex1.UV, new Vector2(uvStartX, plane.Vertex2.UV.y));
            BuildPlane(submeshIndex, sb2, st2, pv3, pv4, new Vector2(uvEndX, plane.Vertex1.UV.y), plane.Vertex3.UV);

            if (holePosition.y + holeDimensions.y / 2 < planeLengthY) // Add a plane above the hole if one is needed
                BuildPlane(submeshIndex, ht1, st1, st2, ht2, new Vector2(uvStartX, uvEndY), new Vector2(uvEndX, plane.Vertex3.UV.y));

            // Add vertices below the hole
            if (holePosition.y - holeDimensions.y / 2 > 0) // Add a plane below the hole if one is needed
                BuildPlane(submeshIndex, sb1, hb1, hb2, sb2, new Vector2(uvStartX, plane.Vertex1.UV.y), new Vector2(uvEndX, uvStartY));

            ApplyMesh(addCollider: true);
        }

        #endregion

    }
}
