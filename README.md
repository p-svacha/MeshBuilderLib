# About
This project contains a Unity library aiming to assist in procedural mesh generation. It's aim is to automate processes that have to be repeated often when dealing with (procedural) mesh generation in unity. It adds functionalities to initialize GameObjects for mesh generation, adding basic vertex, triangles and primitive shapes to meshes, and applying the mesh to GameObjects. Also includes support for submeshes and material and uv handling.

# How to Use
All the needed files are in Assets/Scripts/MeshBuilderLib. To use the library simply copy that folder and import it by "using MeshBuilderLib".
To use the functionalities create a new MeshBuilder object. You can then manipulate the mesh by using the several functions of the meshbuilder. When you're done, use the ApplyMesh function to update the mesh in the scene.

The repo also includes some example projects that use the library. 

# Editor example projects
You'll find 2 generators that create meshes in-editor using the library. They are added to the default scene. Simply press the "Generate" button on their GameObjects to try them out.

# Runtime example project - Liminial Dungeon Generator
The repo also includes a bigger test project that uses the library - the LiminalDungeonGenerator. This generator showcases how the library can also be used for runtime mesh generation.
It can generate a whole simple dungeon, consisting of simple rooms and corridors using the MeshBuilder library and a basic dungeon generator algorithm to create and connect modules/rooms.

Here's a screenshot of how that looks.

<br/><img src="Screenshots/Liminal1.png" width="400" /><br/>