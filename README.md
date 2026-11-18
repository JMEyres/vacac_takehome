# vacac_takehome
Take home task for Vacac graduate software engineer

### Asset Exclusion Note
To keep the repository within GitHub's 100 MB per-file limit without relying on Git LFS (several of the provided source `.fbx` models are ~240 MB each) and to maintain reliable push speeds, the `Assets/VACAC/` directory is excluded via `.gitignore`.

* **To run immediately:** Use the compiled executable in the submitted release zip.
* **To open in the Unity Editor:** Drop the provided `VACAC` asset folder back into `Conveyor_Project/Assets/` and Unity will automatically relink all prefab mesh/material references.