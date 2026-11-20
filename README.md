# vacac_takehome_JoshEyres
Take home task for Vacac graduate software engineer

### Project Overview
This project is a prototype implementation of a modular 3D factory simulation featuring port-snapped placement, continuous conveyor transport (including incline/decline), deposit boxes with counters, queued item merging, and type-based splitting.

* **GitHub Repository:** https://github.com/JMEyres/vacac_takehome

### Development Tools & Environment
* **Engine:** Unity 6.6 (`6000.6.9f1`)
* **IDE & Language:** C#, Visual Studio Code
* **Version Control:** GitHub / GitFork

### Setup & Running Instructions
* **Running the Standalone Build:** Extract the `.zip` archive and launch the `.exe` directly.
* **Opening in the Unity Editor (Important Note):**
  * To keep the repository within GitHub's 100 MB per-file limit without relying on Git LFS, the `Assets/VACAC/` directory is excluded via `.gitignore`.
  * **To open the project in Unity:** Drop the provided `VACAC` package back into `Conveyor_Project/Assets/` and Unity should automatically relink all references.
 
### Controls & Gameplay Loop
* **Camera Movement:** Hold **Right-Click** + **WASD** to move, **Mouse** to look around, and **Q / E** to adjust height.
* **Select Placeable:** Click the corresponding UI button in the bottom toolbar (or press **1–4** as shortcuts for belt variants).
* **Rotate Placeable:** **Mouse Scroll Wheel** up/down (allows angles for curved belt paths).
* **Place / Cancel:** **Left-Click** to place, **Right-Click** to cancel active placement.
* **Quit Application:** **Esc**

**Basic Workflow:** Connect conveyors from a **Can/Box Spawner** through **Mergers** and **Smart Splitters** into **Deposit Boxes** at the end to increment the HUD counters.  
*(Note: Connections snap exit-to-entry or entry-to-exit, and spawners/splitters expect a level conveyor segment before transitioning onto inclines or declines. In this prototype, port direction indicators are only visible with Unity Gizmos enabled, if an object doesn't snap on approach, rotate it 180 degrees with the scroll wheel to align the ports.)*


### System Architecture & Design Decisions
* **Modular Placement System (`Placeable` & `Port`):** Each placeable object uses a unified `Placeable` component holding its ground attachment point alongside lists of entry and exit port sockets to handle transform snapping and item handoffs. Because angled snapping is supported, straight belts can also be chained into curved turns, and new machinery can be added purely via prefab setup without modifying the underlying placement code.

* **Conveyor & Routing Movement:** Given the 48-hour turnaround and project scope, I deliberately chose deterministic entry-to-exit movement paired with `Queue`-buffered mergers and type-filtering splitters rather than complex spline meshes or rigidbody physics. This keeps item routing predictable, eliminates physics jitter across inclines and angled turns, and scales more performantly with higher item counts.

### Future Improvements
With more development time, I would expand the prototype with:
* **Visual Port Indicators & Auto-Orientation:** World-space directional arrows on ghost previews (or automatic port-matching rotation) so entry/exit alignment is immediately clear in builds without Unity Gizmos.
* **Direct Incline-to-Machine Snapping:** Adjusting item spawn offsets and incline belt triggers so spawners and splitters can feed directly onto sloped conveyors without requiring a flat intermediate belt.
* **Bulldoze / Delete Tool:** Runtime removal and re-routing of placed machines and belts.
* **Object Pooling:** Pooling spawned cans and boxes to eliminate instantiation/destruction overhead over long sessions.
* **Backpressure / Buffering:** Allowing belts or machines to stall item flow rather than running continuously.
* **Save / Load System:** Allow for saving/loading of factory layouts rather than rebuilding each time.